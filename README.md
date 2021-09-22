# CMD++
![](https://img.shields.io/github/downloads/Abbin44/CMD-Plus/total?color=orange&label=Downloads)
![](https://img.shields.io/github/stars/Abbin44/CMD-Plus?color=orange&label=Stars)
![](https://img.shields.io/github/forks/Abbin44/CMD-Plus?color=orange&label=Forks)
![](https://img.shields.io/tokei/lines/github/Abbin44/CMD-Plus?color=orange&label=Total-Lines)

A custom shell that allows you to run a bunch of different commands, including everything from CMD/Batch

I am grateful for any feature suggestions or bug reports that you leave. You can do so in the issues tab!
![alt text](https://github.com/Abbin44/Custom-Shell/blob/master/preview.png?raw=true)

### Check the github wiki page for more in depth info about the commands.

**I Recommend running the shell as administrator since certain commands does not work without admin privilege**

The command history log is saved in a file in `C:\Users\%username%\AppData\Local\CMD++\cmdHistory.log`
### What can the shell do?
+ File Maganement
    + Make and Delete Files and Folders
    + Copy and Move Files
    + Copy Folders
    + Rename Files and Folders
    + Create .zip files
    + Extract .zip files
    + Get the size of a directory
    + List everything in a directory
+ Files
    + Execute .exe files
    + Open files with their standard app
    + Print the contents of a file
    + Edit and save the contents of a file
+ Servers
    + Interact with FPT/FTPS servers
    + Interact with servers using SSH
+ Misc
    + Save your command history
    + Run ANY batch command
    + Execute .bat files
    + List all running processes
    + Kill a process
    + Use the && operator to run multiple commands
    + Dynamic coloring
    
## Wand Editor
The wand editor is the equivilent of the linux command touch. It allows you to open a file, edit it's contents and then either quit without saving, or quit and save. It also allows you to quickly peek at a file which simply prints the content, you cannot edit the text in this mode.

The wand editor also have syntax highlighting for better readability when modifying code. If you open a file that does not contain code, you can easily toggle off the coloring and read everything in plain text.
![alt text](https://github.com/Abbin44/Custom-Shell/blob/master/wand_preview.png?raw=true)

## Batch Integration
The Windows Command Line or CMD, is fully integrated and all batch commands that can be run in CMD can be run in my shell as well.
