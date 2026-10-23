# THE HOUSE OF THE DEAD 2 Remake — preset Pimax / OpenXR

**Questo progetto parte dalla mod VR di Astien, autore del repository [Astienth/VR-Mods-Projects](https://github.com/Astienth/VR-Mods-Projects).** La conversione VR originale e i componenti inclusi mantengono i rispettivi crediti. jshodanVR aggiunge calibrazione Pimax, cambio arma con L3 e opzioni per armi e caricatori.

[Scarica v1.0.0-beta.1](https://github.com/jshodanVR/HOTD2-Remake-Pimax-OpenXR/releases/tag/v1.0.0-beta.1) · [English](README.md)

## Preset gia configurato

- Runtime **OpenXR**, modalita OpenXRLoader.
- **Clic dello stick sinistro (L3)** per passare all'arma successiva: un cambio per pressione.
- Inclinazione aggiuntiva delle armi di **27 gradi verso l'alto** rispetto al profilo originale.
- Pistola spostata di **4 cm avanti** e **4 cm in alto**, lungo gli assi locali del supporto dell'arma.
- **Tutte le armi disponibili** durante l'esecuzione del plugin, senza un salvataggio speciale.
- Pistola: **triplo dei colpi base**, ad esempio 6 diventano 18; gli eventuali bonus ordinari vengono aggiunti una sola volta.
- Altre armi: **caricatori doppi**, con i normali bonus inclusi.
- Munizioni finite: i colpi si consumano e bisogna ricaricare.
- Componente FPS_FIX originale incluso e accreditato a CustomForLovemakerz / Lovemakerz.

## Installazione sul gioco base

1. Installa la versione Windows Steam del gioco. La build di riferimento e app Steam **3376690**, build **24017736**, Unity **2022.3.62f3**.
2. Scarica l'asset **HOTD2-Remake-Pimax-OpenXR-v1.0.0-beta.1.zip** dalla release. Il pulsante automatico Source code di GitHub contiene i sorgenti per sviluppatori.
3. Chiudi il gioco. Se hai gia una mod, conserva una copia della cartella BepInEx e delle sue configurazioni.
4. Estrai il contenuto dello ZIP nella cartella che contiene **THE HOUSE OF THE DEAD 2 Remake.exe**, unendo le cartelle BepInEx e Data. Il pacchetto contiene gia la mod completa FPS_FIX di Astien e le sue dipendenze.
5. Configura il runtime OpenXR funzionante per il tuo visore Pimax. Il preset del pacchetto seleziona gia OpenXR.
6. La mod originale emula i controller tramite ViGEmBus. Se manca il driver, trovi l'installer originale in **BepInEx/redist/**; completa personalmente installazione ed eventuale riavvio.
7. Avvia il gioco. Premi lo stick sinistro per scorrere le armi.

La lingua si seleziona dal menu del gioco. Non e incluso alcun salvataggio personale. Le modalita allenamento conservano le limitazioni alle armi previste dal gioco.

## Opzioni

A gioco chiuso puoi modificare **BepInEx/config/local.hotd2.playeroptions.cfg**:

    AllWeapons = true
    InfiniteAmmo = false
    MagazineMultiplier = 2
    PistolBaseMagazineMultiplier = 3

Per ripristinare la capienza normale imposta entrambi i moltiplicatori a 1. Per usare gli sblocchi originali imposta AllWeapons = false. L'associazione L3 si disattiva impostando LeftStickChangesWeapon = false in **BepInEx/config/local.hotd2.leftstickweaponswitch.cfg**. Le modifiche sono lette al successivo avvio.

## Stato della release

Prima release pubblica **beta**. Sono state controllate compilazione, valori della calibrazione, integrita delle dipendenze e composizione dello ZIP. Non e stata completata una prova da installazione pulita per ogni modello Pimax; l'allineamento puo richiedere adattamenti tra controller diversi. Vedi [docs/VALIDATION.md](docs/VALIDATION.md).

## Crediti

La base VR e di **Astien / Astienth**: [progetto originale](https://github.com/Astienth/VR-Mods-Projects). Il fix prestazioni e attribuito a **CustomForLovemakerz / Lovemakerz**, come nel pacchetto originale. Preset e plugin aggiuntivi: **jshodanVR**.

I sorgenti aggiuntivi sono sotto licenza MIT. La distribuzione completa conserva tutte le licenze originali e le condizioni di ridistribuzione gratuita e uso non commerciale di Astien e del fix FPS. Vedi [CREDITS.md](CREDITS.md), [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) e BepInEx/Licenses/ nel pacchetto.
