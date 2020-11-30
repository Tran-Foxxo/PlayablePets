# Playable Pets

A BepInEx IL2CPP plugin that allows you to play as your pet on both the Steam and Itch.io versions of Among Us! 
Both working for version v2020.11.17 Patch 1.

## <u>**Note: If you end up using this in a video please credit me.**</u>
### You can credit me by saying `Playable Pets was created by Tran-Foxxo on github at https://github.com/Tran-Foxxo/PlayablePets`

## Installation 

1. Download the latest BepInEx IL2CPP x86 build artifact from https://builds.bepis.io/projects/bepinex_be.
2. Go to the Among Us install directory
   * **Steam**: Right click Among Us in Steam.  Go to manage, then browse local files.
   * **Itch**: Go to the `\AmongUs\` directory where `Among Us.exe` is located. (It'll be referred to `\Among Us\` after this.)
3. Extract the .zip into this folder so the folder structure has `\Among Us\BepInEx\`.
4. Download the Unity dependencies for this mod (currently [2020.4.9.zip](https://github.com/HerpDerpinstine/MelonLoader/blob/master/BaseLibs/UnityDependencies/2019.4.9.zip?raw=true)) from [here](https://github.com/HerpDerpinstine/MelonLoader/tree/master/BaseLibs/UnityDependencies) and place them in `\Among Us\BepInEx\unhollowed\base\` (you might need to create this folder).
   * If you've already installed BepInEx delete `\Among Us\BepInEx\unhollowed\assembly-hash.txt` to re-generate the assemblies.
5. Run the game and exit when it's fully loaded, this will create directories / files needed for the mod. (This might take a while.)
6. Download the latest release for your version at https://github.com/Tran-Foxxo/PlayablePets/releases.
7. Put the downloaded `.dll` for your version into `\Among Us\BepInEx\plugins\`.
8. Run the game and use a pet that you own and you'll become the pet!
9. (Optional) Change the configs in `\Among Us\BepInEx\config\tranfox.playablepets.cfg` to your liking.

## Building

1. Follow steps 1-5 of the [installation](https://github.com/Tran-Foxxo/PlayablePets#installation).
2. **Do not remove `Assembly-CSharp-Steam` or `Assembly-CSharp-Itch` from the project references.**
   Re-reference the DLLs in `PlayablePets.csproj` to point to the actual files.  To re-reference `Assembly-CSharp-Steam` and `Assembly-CSharp-Itch` edit `PlayablePets.csproj` with a text editor to manually set their paths.
   * **Note**: You don't need to re-reference the other Unity DLLs / BepInEx DLLs when building for a different version.
3. Change the Solution Configuration depending on which version you are building for.
4. The DLL for your version will be built in it's corresponding folder.

## To-dos 

- [x] Keep the mod up to date. 
- [x] Make a itch.io version. 
- [ ] Move name up a bit more for the taller pets.
- [ ] Maybe a custom sprite for dead pets? (Loaded from external assets)

## Demo

![Gif 1](https://github.com/Tran-Foxxo/PlayablePets/raw/master/gifs/1.gif)
![Gif 2](https://github.com/Tran-Foxxo/PlayablePets/raw/master/gifs/2.gif)
![Gif 3](https://github.com/Tran-Foxxo/PlayablePets/raw/master/gifs/3.gif)