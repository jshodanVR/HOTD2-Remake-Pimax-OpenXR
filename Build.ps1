param(
 [Parameter(Mandatory=$true)][string]$GameDirectory,
 [Parameter(Mandatory=$true)][string]$UpstreamArchive,
 [string]$OutputDirectory = (Join-Path $PSScriptRoot 'dist')
)
$ErrorActionPreference = 'Stop'
$version = '1.0.0-beta.1'
$packageName = 'HOTD2-Remake-Pimax-OpenXR-v' + $version
$gameRoot = [IO.Path]::GetFullPath($GameDirectory)
$managed = Join-Path $gameRoot 'THE HOUSE OF THE DEAD 2 Remake_Data\Managed'
$supported = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'docs\SUPPORTED_BUILD.json') -Raw | ConvertFrom-Json
$assembly = Join-Path $managed 'Assembly-CSharp.dll'
if (-not (Test-Path -LiteralPath $assembly -PathType Leaf)) { throw 'Game Managed/Assembly-CSharp.dll not found.' }
if ((Get-FileHash -LiteralPath $assembly -Algorithm SHA256).Hash -ne $supported.gameAssemblySha256) { throw 'Unsupported game assembly. See docs/SUPPORTED_BUILD.json.' }
if (-not (Test-Path -LiteralPath $UpstreamArchive -PathType Leaf)) { throw 'The original Astien FPS_FIX ZIP is required.' }
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
if (-not (Test-Path -LiteralPath $compiler)) { throw 'Windows .NET Framework C# compiler not found.' }
[IO.Directory]::CreateDirectory($OutputDirectory) | Out-Null
$packageRoot = [IO.Path]::GetFullPath((Join-Path $OutputDirectory $packageName))
$archivePath = Join-Path $OutputDirectory ($packageName + '.zip')
if ((Test-Path -LiteralPath $packageRoot) -or (Test-Path -LiteralPath $archivePath)) { throw 'Output already exists. Choose a new OutputDirectory.' }
[IO.Directory]::CreateDirectory($packageRoot) | Out-Null
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [IO.Compression.ZipFile]::OpenRead([IO.Path]::GetFullPath($UpstreamArchive))
try {
 $prefix = $packageRoot.TrimEnd('\') + '\'
 foreach ($entry in $zip.Entries) {
  $relative = $entry.FullName.Replace('/', '\')
  $target = [IO.Path]::GetFullPath((Join-Path $packageRoot $relative))
  if (-not $target.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) { throw ('Unsafe archive entry: ' + $entry.FullName) }
  if ($entry.FullName.EndsWith('/')) { [IO.Directory]::CreateDirectory($target) | Out-Null; continue }
  [IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($target)) | Out-Null
  [IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $target, $false)
 }
} finally { $zip.Dispose() }
$plugins = Join-Path $packageRoot 'BepInEx\plugins'
$core = Join-Path $packageRoot 'BepInEx\core'
$vrDll = Join-Path $plugins 'TheHouseOfTheDead2_VR.dll'
if ((Get-FileHash -LiteralPath $vrDll -Algorithm SHA256).Hash -ne $supported.upstreamModSha256) { throw 'Unsupported Astien VR binary; expected v1.0.0 from the FPS_FIX package.' }
$helper = Join-Path $OutputDirectory ('.build-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($helper) | Out-Null
Copy-Item -LiteralPath (Join-Path $core 'Mono.Cecil.dll') -Destination $helper
$patchTool = Join-Path $helper 'PatchCalibration.exe'
& $compiler '/nologo' ('/out:' + $patchTool) ('/r:' + (Join-Path $core 'Mono.Cecil.dll')) (Join-Path $PSScriptRoot 'tools\PatchCalibration.cs')
if ($LASTEXITCODE -ne 0) { throw 'Calibration tool compilation failed.' }
$patchedVr = Join-Path $helper 'TheHouseOfTheDead2_VR.dll'
& $patchTool $vrDll $patchedVr
if ($LASTEXITCODE -ne 0) { throw 'VR calibration patch failed.' }
Copy-Item -LiteralPath $patchedVr -Destination $vrDll -Force
$references = @(
 ('/r:' + (Join-Path $core 'BepInEx.dll')),
 ('/r:' + (Join-Path $core '0Harmony.dll')),
 ('/r:' + $assembly),
 ('/r:' + (Join-Path $managed 'UnityEngine.dll')),
 ('/r:' + (Join-Path $managed 'UnityEngine.CoreModule.dll')),
 ('/r:' + (Join-Path $managed 'UnityEngine.UI.dll')),
 ('/r:' + (Join-Path $managed 'netstandard.dll')),
 ('/r:' + (Join-Path $managed 'Unity.InputSystem.dll'))
)
foreach ($pluginName in @('HOTD2_PlayerOptions', 'HOTD2_LeftStickWeaponSwitch')) {
 $compileArgs = @('/nologo', '/target:library', '/optimize+', ('/out:' + (Join-Path $plugins ($pluginName + '.dll')))) + $references + @((Join-Path $PSScriptRoot ('src\' + $pluginName + '.cs')))
 & $compiler @compileArgs
 if ($LASTEXITCODE -ne 0) { throw ('Plugin compilation failed: ' + $pluginName) }
}
foreach ($preset in Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'presets') -Filter '*.cfg') {
 Copy-Item -LiteralPath $preset.FullName -Destination (Join-Path $packageRoot ('BepInEx\config\' + $preset.Name)) -Force
}
foreach ($doc in @('README.md', 'README_IT.md', 'CREDITS.md', 'LICENSE', 'THIRD_PARTY_NOTICES.md', 'CHANGELOG.md')) {
 Copy-Item -LiteralPath (Join-Path $PSScriptRoot $doc) -Destination (Join-Path $packageRoot $doc)
}
[IO.Directory]::CreateDirectory((Join-Path $packageRoot 'docs')) | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'docs\SUPPORTED_BUILD.json') -Destination (Join-Path $packageRoot 'docs\SUPPORTED_BUILD.json')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'docs\VALIDATION.md') -Destination (Join-Path $packageRoot 'docs\VALIDATION.md')
$prefix = $packageRoot.TrimEnd('\') + '\'
$files = @(Get-ChildItem -LiteralPath $packageRoot -Recurse -File | Sort-Object FullName | ForEach-Object {
 [PSCustomObject]@{ path=$_.FullName.Substring($prefix.Length).Replace('\','/'); size=$_.Length; sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
})
$manifest = [PSCustomObject]@{ version=$version; upstream='Astien / Astienth - TheHouseOfTheDead2_VR FPS_FIX'; upstreamArchiveSha256=(Get-FileHash -LiteralPath $UpstreamArchive -Algorithm SHA256).Hash.ToLowerInvariant(); profile=$supported.profile; files=$files }
$manifest | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath (Join-Path $packageRoot 'PACKAGE-MANIFEST.json') -Encoding UTF8
[IO.Compression.ZipFile]::CreateFromDirectory($packageRoot, $archivePath, [IO.Compression.CompressionLevel]::Optimal, $false)
$archiveHash = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash.ToLowerInvariant()
($archiveHash + '  ' + [IO.Path]::GetFileName($archivePath)) | Set-Content -LiteralPath (Join-Path $OutputDirectory 'SHA256SUMS.txt') -Encoding ASCII
Write-Output ('PACKAGE: ' + $archivePath)
Write-Output ('SHA256: ' + $archiveHash)
