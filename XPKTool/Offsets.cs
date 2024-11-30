namespace XPKTool
{
	public class Offsets
	{
		public uint TOTAL_FILES;
		public uint FIXED_BLOCK_SIZE;
		public uint HEADER_SIZE = 4;
		public uint FILENAMES_SIZE = 4;
		public uint FILEDATA_SIZE = 4;
		public uint FILENAME_OFFS_SIZE;
		public uint FILES_SIZES_SIZE;
		public uint HASHTABLE_SIZE;
		public uint FILEDATA_OFFSETS_SIZE;
		public uint FILENAME_STRINGS_BLOCK_SIZE;
		public uint FILEDATA_BLOCK_SIZE;

		public uint PADDING_TO_FILEDATA
		{
			get
			{
				uint HEADERSIZE = HEADER_SIZE + FILENAME_OFFS_SIZE + FILENAMES_SIZE + FILENAME_STRINGS_BLOCK_SIZE + FILEDATA_SIZE + FILES_SIZES_SIZE + HASHTABLE_SIZE + FILEDATA_OFFSETS_SIZE;
				return HEADERSIZE;
			}
		}

		public Offsets(uint totalFiles)
		{
			TOTAL_FILES = totalFiles;
			FIXED_BLOCK_SIZE = totalFiles * 4;
			FILENAME_OFFS_SIZE = FIXED_BLOCK_SIZE;
			FILES_SIZES_SIZE = FIXED_BLOCK_SIZE;
			HASHTABLE_SIZE = FIXED_BLOCK_SIZE;
			FILEDATA_OFFSETS_SIZE = FIXED_BLOCK_SIZE;
		}

		public static uint CalcNextFileDataOffset(int fileSize, int currentTotalSize)
		{
			return (uint)(fileSize + currentTotalSize);
		}

		public static uint CalcNextStringOffset(int stringSize, int currentTotalSize)
		{
			return (uint)(stringSize + 1 + currentTotalSize);
		}
	}
}
