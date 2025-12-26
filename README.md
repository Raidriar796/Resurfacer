# Resurfacer

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that adds batch texture operations.

## ⚠️Warning⚠️
This mod is heavily WIP, it will support more batch operations over time. If you have a specific batch operation you want supported, feel free to request it via an issue.

## How to use
1. Open the `Create New...` menu with a dev tool
2. Spawn the Resurfacer from the `Editor` submenu
3. Provide a slot reference for the target hierarchy
4. Run some batch operations! 

## What does each button do?

- Set Preferred Format
  - Sets the `PreferredFormat` on every texture to the selected format.
  - Textures with `Uncompressed` or `PreferredFormat` enabled will be ignored.
- Force Preferred Format
  - Sets the `PreferredFormat` on every texture and additionally sets `ForceExactVariant`.
  - Textures with `Uncompressed` or `PreferredFormat` enabled will be ignored.
- Unset Force Format
  - Disables `ForceExactVariant` on every texture.
- Unset Preferred Format
  - Disables `PreferredFormat` on every texture.
- Set Mip Map Filter
  - Sets the `MipMapFilter` on every texture to the selected filter.
- Re-Encode
  - Re-encodes every image, generally saving space for cloud storage.

## Requirements
- [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader)

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
2. Place [Resurfacer.dll](https://github.com/Raidriar796/Resurfacer/releases/latest/download/Resurfacer.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Resonite logs. 
