namespace XPKTool;

public class XPKArchive
{
	// XPKArchive class
	// Responsible for unpacking a .xpk file
	public string Name;
	public string FilePath;
	public uint TotalFiles;
	public uint FileDataSizeOffset;
	public uint FileDataSize;
	public FileStream fs;
	public BinaryReader br;
	public List<XPKFile> Files = [];

	public XPKArchive(string filePath)
	{
		Name = Path.GetFileNameWithoutExtension(filePath);
		FilePath = filePath;
		fs = new FileStream(filePath, FileMode.Open);
		br = new BinaryReader(fs);

		TotalFiles = br.ReadUInt32();
		for (var i = 0; i < TotalFiles; i++)
		{
			Files.Add(new XPKFile());
		}
		ReadStringsOffsetTable();
		ReadStringsTable();
		ReadFileSizesTable();
		ReadHashTable();
		ReadFileDataOffsetsTable();
		ExtractAll();
		br.Close();
		fs.Close();
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine(string.Concat("[+] File ", Name, ".xpk was successfully unpacked, ", TotalFiles, " files were extracted."));
		Utils.ExitProgram();
	}

	/// <summary>
	/// Extracts all files from the XPK archive to the output directory.
	/// </summary>
	public void ExtractAll()
	{
		foreach (var file in Files)
		{
			var outputDir = Path.Combine(Name, Path.GetDirectoryName(
				file.Name.Replace('\\', Path.DirectorySeparatorChar)) ?? string.Empty);
			var outputFile = Path.Combine(Name, file.Name.Replace('\\', Path.DirectorySeparatorChar));

			// Create directory if it doesn't exist
			if (!Directory.Exists(outputDir))
			{
				Directory.CreateDirectory(outputDir);
			}

			// Extract the file
			File.WriteAllBytes(outputFile, GetFileData(file.DataOffset, (int)file.Size));
		}
	}

	/// <summary>
	/// Reads a file's data from the archive at the specified offset and size.
	/// </summary>
	/// <param name="offset">The offset in the archive where the file data starts.</param>
	/// <param name="size">The size of the file data in bytes.</param>
	/// <returns>The file data as a byte array.</returns>
	public byte[] GetFileData(uint offset, int size)
	{
		br.BaseStream.Position = offset;
		var numArray = new byte[size];
		br.Read(numArray, 0, size);
		return numArray;
	}

	/// <summary>
	/// Reads the table of file data offsets from the archive and stores them in the file list.
	/// </summary>
	public void ReadFileDataOffsetsTable()
	{
		for (var i = 0; i < TotalFiles; i++)
		{
			Files[i].DataOffset = br.ReadUInt32();
		}
	}

	/// <summary>
	/// Reads the table of file sizes from the archive and stores them in the file list.
	/// </summary>
	public void ReadFileSizesTable()
	{
		for (var i = 0; i < TotalFiles; i++)
		{
			Files[i].Size = br.ReadUInt32();
		}
	}

	/// <summary>
	/// Reads the hash table (creation dates) from the archive and stores them in the file list.
	/// </summary>
	public void ReadHashTable()
	{
		for (var i = 0; i < TotalFiles; i++)
		{
			Files[i].CreationDate = br.ReadUInt32();
		}
	}

	/// <summary>
	/// Reads the table of string offsets for file names from the archive and stores them in the file list.
	/// </summary>
	public void ReadStringsOffsetTable()
	{
		for (var i = 0; i < TotalFiles; i++)
		{
			Files[i].NameOffset = br.ReadUInt32();
		}
		FileDataSizeOffset = br.ReadUInt32();
	}

	/// <summary>
	/// Reads the file names from the archive using the previously read string offsets.
	/// </summary>
	public void ReadStringsTable()
	{
		for (var i = 0; i < TotalFiles; i++)
		{
			Files[i].Name = Utils.ReadNullTerminatedString(br);
		}
		FileDataSize = br.ReadUInt32();
	}
}
