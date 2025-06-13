namespace XPKTool;

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

	/// <summary>
	/// Calculates the padding size up to the file data block based on header and table sizes.
	/// </summary>
	public uint PADDING_TO_FILEDATA
	{
		get
		{
			var HEADERSIZE = HEADER_SIZE + FILENAME_OFFS_SIZE + FILENAMES_SIZE + FILENAME_STRINGS_BLOCK_SIZE + FILEDATA_SIZE + FILES_SIZES_SIZE + HASHTABLE_SIZE + FILEDATA_OFFSETS_SIZE;
			return HEADERSIZE;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Offsets"/> class and calculates block sizes based on the total number of files.
	/// </summary>
	/// <param name="totalFiles">The total number of files in the archive.</param>
	public Offsets(uint totalFiles)
	{
		TOTAL_FILES = totalFiles;
		FIXED_BLOCK_SIZE = totalFiles * 4;
		FILENAME_OFFS_SIZE = FIXED_BLOCK_SIZE;
		FILES_SIZES_SIZE = FIXED_BLOCK_SIZE;
		HASHTABLE_SIZE = FIXED_BLOCK_SIZE;
		FILEDATA_OFFSETS_SIZE = FIXED_BLOCK_SIZE;
	}

	/// <summary>
	/// Calculates the next file data offset based on the current total size and the new file's size.
	/// </summary>
	/// <param name="fileSize">The size of the new file in bytes.</param>
	/// <param name="currentTotalSize">The current total size in bytes.</param>
	/// <returns>The offset for the next file data block.</returns>
	public static uint CalcNextFileDataOffset(int fileSize, int currentTotalSize)
	{
		return (uint)(fileSize + currentTotalSize);
	}

	/// <summary>
	/// Calculates the next string offset based on the current total size and the new string's size.
	/// </summary>
	/// <param name="stringSize">The length of the new string.</param>
	/// <param name="currentTotalSize">The current total size in bytes.</param>
	/// <returns>The offset for the next string in the table.</returns>
	public static uint CalcNextStringOffset(int stringSize, int currentTotalSize)
	{
		return (uint)(stringSize + 1 + currentTotalSize);
	}
}
