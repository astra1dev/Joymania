<p align="center">
<img src="https://github.com/user-attachments/assets/65e497ae-e8bc-41cc-8833-236629480a5d">
</p>
  
<p align="center">
<b>Modding tool to unpack and repack XPK game files from Joymania games</b>
</p>

Supported games:
- Santa Claus In Trouble
- Santa Claus In Trouble... Again!
- Rosso Rabbit In Trouble.

It can be used to modify the game assets that are packed in the .xpk file.

This project is a fork of [meth0d's XPK Tool](https://github.com/The-Meth0d/XPKTool-SantaClausInTrouble) and offers the following improvements:
- Added support for Linux and MacOS
- Upgrade to .NET Runtime version 8
- Refactored and improved code
- Faster speed and smaller file size

# 💾 Installation
The following operating systems are supported:
- Windows (64-bit, 32-bit, ARM64-based)
- Linux (64-bit, 64-bit with musl libc, ARM-based, ARM64-based)
- macOS (64-bit and ARM64-based (Apple Silicon))

1. Download and install .NET 8 from the [official site](https://dotnet.microsoft.com/download/dotnet/8.0) if you don't already have it installed.
2. Download the file fitting your operating system from the [latest release](../releases/latest)

Alternatively, you can download the `.dll` file from the latest release, which will work on all operating systems using the `dotnet XPKTool.dll` command (.NET 8 required).

<hr>

<b>👷‍♂️ If you don't want to download the pre-compiled EXE, you can build XPK Tool from source by following these steps:</b>
- Download the necessary files with `git clone https://github.com/astra1dev/Joymania.git`
- Run the command `dotnet build` from the XPKTool folder (where `XPKTool.sln` is located)
- The compiled exe will be located here: `src/bin/Debug/net8.0/XPKTool.exe`

# 🛠️ Usage
- To unpack: Drop a .XPK FILE on the executable or run `XPKTool.exe FILE.xpk`.
- To repack: Drop a FOLDER on the executable or run `XPKTool.exe FOLDER`.

# Details
The XPK file is a custom archive format used by the game.

- The first 4 bytes (UINT32) tell how many files are in the archive (177 for Santa Claus In Trouble).
- The next 177*4=708 bytes are the file name offsets of each file in the archive.
- The next 4 bytes is the FileDataSizeOffset
- The next ? bytes are the names of the files in the archive, which are null-terminated strings (ASCII -> readable).
- The next 4 bytes are the FileDataSize, which is the total size of the file data in the archive.
- The next 177*4=708 bytes are the File sizes of each file in the archive.
- The next 177*4=708 bytes are the creation dates of each file in the archive.
- The next 177*4=708 bytes are the file data offsets of each file in the archive.
- The remaining bytes are the file data itself.