# CMD++
![](https://img.shields.io/github/downloads/Abbin44/CMD-Plus/total?color=orange&label=Downloads)
![](https://img.shields.io/github/stars/Abbin44/CMD-Plus?color=orange&label=Stars)
![](https://img.shields.io/github/forks/Abbin44/CMD-Plus?color=orange&label=Forks)
![](https://img.shields.io/endpoint?url=https://raw.githubusercontent.com/Abbin44/CMD-Plus/master/Shields/lines.json)

A custom shell that allows you to run a bunch of different commands, including everything from CMD/Batch. There are plans to have at least some support for linux in the future. Currently there are some missing references that needs to be rewritten to make it work. 

I am grateful for any feature suggestions or bug reports that you leave. You can do so in the issues tab!
![alt text](https://github.com/Abbin44/Custom-Shell/blob/master/preview.png?raw=true)

### Check the github wiki page for more in depth info about the commands.

**I Recommend running the shell as administrator since certain commands does not work without admin privilege**

All data that is stored gets stored here `C:\Users\%username%\AppData\Local\CMD++\`
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

## Rail Scripting Language
The rail scripting language is designed to simplify tasks that require many commands to be ran before you can start working on what you need.
Certain design choices may lead to unexpected behavior if the syntax is not followed correctly. The one of the biggest decicions i made was to run the script even it there are syntax errors.
The language is decently complex even at first release so it is highly recommended that you read the github wiki before using it. The documentation can be found [here](https://github.com/Abbin44/CMD-Plus/wiki/Rails)

As of now there are only a few script specific syntaxes that were chosen to minimize work and maximize functionality.
The combination of if statements, labels, gotos and change of variable values lets you check data, create loops, and debug the script. Which i though was a good set of tools for a v1.0 release for the language.

  + If statements
  + Print Statements
  + Labels/Goto Labels
  + Floating Point Variables (Can be used as ints without any extra work)
  
![alt text](https://github.com/Abbin44/Custom-Shell/blob/master/script_preview.png?raw=true)

## Batch Integration
The Windows Command Line or CMD, is fully integrated and all batch commands that can be run in CMD can be run in my shell as well.

## Currently working on
As of 2022/08/27
 * <s>Settings to allow certain values to be saved as defaults.</s> *Done*
 * More features in Rail, specifically loops. *Has gotten more features, but not loops*
 * <s>Reworking the SSH code to be a more session like feature. Possibly changing the SSH lib as well as part of the linux support goal.</s> *Done*

## Upcoming features
All tasks that will appear here will be bigger and more time consuming than other tasks that regularly gets pushed.

- [ ] Fix the calculator
- [ ] Add linux support - This requires both the FTP and SSH libraries to be exchanged with something that can run with mono or dotnet.
