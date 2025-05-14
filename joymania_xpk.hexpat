#include <std/string.pat>
#pragma pattern_limit 10000000

// This hex pattern is designed for the use with ImHex (https://github.com/werwolv/imhex)
// It automatically highlights the different data sections of the xpk archive file formats
// used in SCIT (xmas.xpk), SCIT-AGAIN (xmas.xpk) and RRIT (bb.xpk).

// 4 bytes: Total file count
u32 FileCount @ 0x00;

// 4*FileCount bytes: File name offsets of each file
u32 FileNameOffsets[FileCount] @ 0x04;

// 4 bytes: FileDataSizeOffset
u32 FileDataSizeOffset @ (0x04 + FileCount * 4);

// File Names
struct StringTable {
    char filename[];
};

StringTable FileNames[FileCount] @ (FileCount * 4 + 8);

// 4 bytes: FileDataSize
u32 FileDataSize @ (addressof(FileNames[FileCount-1]) + sizeof(FileNames[FileCount-1]));

// 4*FileCount bytes: File size of each file
u32 FileSizes[FileCount] @ (addressof(FileDataSize) + sizeof(FileDataSize));

// 4*FileCount bytes: File creation dates of each file
// You can use a unix timestamp converter like https://unixtimestamp.com to convert them into their actual dates
u32 FileCreationDates[FileCount] @ (addressof(FileSizes[FileCount-1]) + sizeof(FileSizes[FileCount-1]));

// 4*FileCount bytes: File data offsets of each file
u32 FileDataOffsets[FileCount] @ (addressof(FileCreationDates[FileCount-1]) + sizeof(FileCreationDates[FileCount-1]));

// The remaining bytes (about 95% of the archive) are the file data itself.
struct FileEntry {
    u8 data[FileSizes[i]];
};

// this gives an error: runtimeerror: cannot place variable FileData in heap
//for (u32 i = 0, i < FileCount, i = i + 1) {
//    FileEntry FileData[FileCount] @ FileDataOffsets[i];
//};
