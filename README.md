# Radiant-Mythology-3
Scripts use Encoding: `[CP20932] EUC-JP Double-Byte Edition of Windows`

# Hacker Notes

## General Process
1. Use `Namco file project.exe` to unpack `namco.bdi` into `.arc` files.
2. Use [arc_extractor.py](arc_extractor.py) (or in conjuction with [arc_extractor.bat](arc_extractor.bat)) to extract as much as possible from each `.arc` in the previous step.
3. When modifying a file, use `EZBIND Editor.exe` to EXTRACT files in the `.arc` file. Swap the files, then IMPORT all the files back in to get a new `.arc` file.
4. Use `Namco file project.exe` to rebuild a new `namco.bdi` file.
5. Use `UDMGen` or `Apache3` to replace the `namco.bdi` file.  Test the newly modify ISO, and if it works, remember to create a patch and share it!

## Reference
1. `mevXX_XXX.scr` appears to be where the story script is.  For example, the very first scene dialog should be in `mev00_020.scr` found in the `21.arc` file.
2. The `tow-tools` from `delguoqing` are Python2 scripts.  Use ChatGPT to convert to Python3 for convieience.

# Credits
 - `Omarrrio` for the Initial tools!
 - `Stormyu` ASM investigations and game file analysis that helped make the above tools.
 - `delguoqing` for the Python2 scripts
 
