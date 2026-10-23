using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

// Applies the published Pimax profile to Astien's exact v1.0.0 binary.
// Upstream source is not included or reconstructed by this project.
class PatchCalibration {
 static int FileOffset(byte[] b, uint rva) {
  int pe=BitConverter.ToInt32(b,0x3c), sections=BitConverter.ToUInt16(b,pe+6);
  int start=pe+24+BitConverter.ToUInt16(b,pe+20);
  for(int i=0;i<sections;i++) {
   int s=start+40*i; uint va=BitConverter.ToUInt32(b,s+12);
   uint size=Math.Max(BitConverter.ToUInt32(b,s+8),BitConverter.ToUInt32(b,s+16));
   if(rva>=va&&rva<va+size) return checked((int)(BitConverter.ToUInt32(b,s+20)+rva-va));
  }
  throw new InvalidDataException("Method RVA not mapped to a PE section.");
 }
 static void Change(byte[] bytes, MethodDefinition method, string name, int axis, float expected, float value) {
  int hits=0;
  var il=method.Body.Instructions;
  for(int i=4;i<il.Count;i++) {
   var field=il[i].Operand as FieldReference;
   if(il[i].OpCode!=OpCodes.Stsfld||field==null||field.Name!=name) continue;
   for(int a=0;a<3;a++) if(il[i-4+a].OpCode!=OpCodes.Ldc_R4) throw new InvalidDataException("Unexpected vector initializer.");
   if(il[i-1].OpCode!=OpCodes.Newobj) throw new InvalidDataException("Unexpected vector constructor.");
   var instruction=il[i-4+axis];
   if(Math.Abs((float)instruction.Operand-expected)>0.000001f) throw new InvalidDataException("Unexpected original calibration: "+name);
   int body=FileOffset(bytes,(uint)method.RVA);
   int header=(bytes[body]&3)==2?1:(BitConverter.ToUInt16(bytes,body)>>12)*4;
   int opcode=body+header+instruction.Offset;
   if(bytes[opcode]!=0x22) throw new InvalidDataException("Expected ldc.r4.");
   Buffer.BlockCopy(BitConverter.GetBytes(value),0,bytes,opcode+1,4);
   Console.WriteLine(name+"["+axis+"]: "+expected+" -> "+value);
   hits++;
  }
  if(hits!=1) throw new InvalidDataException("Calibration field was not unique: "+name);
 }
 static void Main(string[] args) {
  if(args.Length!=2) throw new ArgumentException("Usage: PatchCalibration original.dll patched.dll");
  byte[] bytes=File.ReadAllBytes(args[0]);
  MethodDefinition initializer=null;
  using(var assembly=AssemblyDefinition.ReadAssembly(args[0])) {
   foreach(var type in assembly.MainModule.Types) if(type.FullName=="UnityVRPlugin.patches.GamePatches")
    foreach(var method in type.Methods) if(method.Name==".cctor") initializer=method;
   if(initializer==null) throw new InvalidDataException("Calibration initializer missing.");
   Change(bytes,initializer,"aimingOffset",0,-40f,-67f);
   Change(bytes,initializer,"aimingOffsetOpenXR",0,0f,-27f);
   Change(bytes,initializer,"pistolLocalPosition",1,0f,-0.04f);
   Change(bytes,initializer,"pistolLocalPosition",2,-0.075f,-0.035f);
  }
  File.WriteAllBytes(args[1],bytes);
 }
}
