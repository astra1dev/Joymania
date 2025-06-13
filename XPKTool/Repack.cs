using System.Text;

namespace XPKTool;

public class XPKCreator
{
	// XPKCreator class
	// Responsible for creating a .xpk file from a folder
	public string[] AllFiles;
	public uint TotalFiles;
	public string FolderName;
	public string FolderDirectory;
	public Offsets OffsetsUtil;
	public List<XPKFile> Files = [];
	public FileStream fs;
	public BinaryWriter bw;

	public XPKCreator(string folderDirectory)
	{
		if (!Directory.Exists(folderDirectory))
		{
			Utils.ErrorAndExit("[!] Folder does not exist! Drag and drop a valid folder on the program.");
		}
		AllFiles = Directory.GetFiles(folderDirectory, "*.*", SearchOption.AllDirectories);
		TotalFiles = (uint)AllFiles.Length;
		FolderName = Path.GetFileName(folderDirectory);
		FolderDirectory = folderDirectory;
		var archivePath = Path.Combine(FolderName, ".xpk");
		if (TotalFiles == 0)
		{
			Utils.ErrorAndExit("[!] No files were found in the specified directory!");
		}
		else
		{
			// if there is already a .xpk file with the same name, create a backup of it
			if (File.Exists(archivePath))
			{
				File.Copy(archivePath, archivePath.Replace(".xpk", ".xpk.bak"), true);
				Console.WriteLine(string.Concat("[-] Creating file backup to ", FolderName, ".xpk.bak"));
			}
			Console.WriteLine(string.Concat("[-] Packing ", TotalFiles, " files from folder: (", FolderName, ") to file ", FolderName, ".xpk"));
			OffsetsUtil = new Offsets(TotalFiles);
			fs = new FileStream(archivePath, FileMode.Create);
			bw = new BinaryWriter(fs);
			for (var i = 0; i < TotalFiles; i++)
			{
				Files.Add(new XPKFile());
			}
			var length = 0;
			uint num = 0;
			uint num1 = 0;
			var allFiles = AllFiles;
			foreach (var str1 in allFiles)
			{
				FileInfo fileInfo = new(str1);
				var relativePath = Path.GetRelativePath(folderDirectory, str1);
				Files[length].Name = relativePath;
				Files[length].Size = (uint)fileInfo.Length;
				var item = Files[length];
				var creationTime = fileInfo.CreationTime;
				item.CreationDate = Convert.ToUInt32(Utils.ConvertToTimestamp(creationTime.ToUniversalTime()));
				Files[length].NameOffset = num1;
				num1 = Offsets.CalcNextStringOffset(relativePath.Length, (int)num1);
				num += (uint)fileInfo.Length;
				length++;
			}
			OffsetsUtil.FILENAME_STRINGS_BLOCK_SIZE = num1;
			OffsetsUtil.FILEDATA_BLOCK_SIZE = num;
			WriteHeader();
			WriteFilenameStringsOffsetsTable();
			WriteFilenameStringsTable();
			WriteFileSizesTable();
			WriteHashTable();
			WriteFileOffsetsTable();
			WriteFilesData();
			bw.Close();
			fs.Close();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(string.Concat("[+] File ", FolderName, ".xpk successfully created!"));
			Utils.ExitProgram();
		}
	}

	/// <summary>
	/// Reads all bytes from the specified file.
	/// </summary>
	/// <param name="filePath">The path to the file.</param>
	/// <returns>The file data as a byte array.</returns>
	public static byte[] GetFileDataBytes(string filePath)
	{
		return File.ReadAllBytes(filePath);
	}

	/// <summary>
	/// Writes the offsets of each file name string to the archive.
	/// </summary>
	public void WriteFilenameStringsOffsetsTable()
	{
		for (var i = 0; i < TotalFiles; i++)
		{
			bw.Write(Files[i].NameOffset);
		}
		bw.Write(OffsetsUtil.FILENAME_STRINGS_BLOCK_SIZE);
	}

	/// <summary>
	/// Writes all file name strings to the archive, followed by a null terminator.
	/// </summary>
	public void WriteFilenameStringsTable()
	{
		for (var i = 0; i < TotalFiles; i++)
		{
			bw.Write(Encoding.Default.GetBytes(Files[i].Name));
			bw.Write((byte)0);
		}
		bw.Write(OffsetsUtil.FILEDATA_BLOCK_SIZE);
	}

	/// <summary>
	/// Writes the offsets for each file's data block to the archive.
	/// </summary>
	public void WriteFileOffsetsTable()
	{
		var PADDINGTOFILEDATA = OffsetsUtil.PADDING_TO_FILEDATA;
		for (var i = 0; i < TotalFiles; i++)
		{
			bw.Write(PADDINGTOFILEDATA);
			PADDINGTOFILEDATA = Offsets.CalcNextFileDataOffset((int)Files[i].Size, (int)PADDINGTOFILEDATA);
		}
	}

	/// <summary>
	/// Writes the raw data of all files to the archive.
	/// </summary>
	public void WriteFilesData()
	{
		var allFiles = AllFiles;
		foreach (var str in allFiles)
		{
			bw.Write(GetFileDataBytes(str));
		}
	}

	/// <summary>
	/// Writes the sizes of all files to the archive.
	/// </summary>
	public void WriteFileSizesTable()
	{
		for (var i = 0; i < TotalFiles; i++)
		{
			bw.Write(Files[i].Size);
		}
	}

	/// <summary>
	/// Writes the creation dates (hash table) of all files to the archive.
	/// </summary>
	public void WriteHashTable()
	{
		for (var i = 0; i < TotalFiles; i++)
		{
			bw.Write(Files[i].CreationDate);
		}
	}

	/// <summary>
	/// Writes the archive header, including the total number of files.
	/// </summary>
	public void WriteHeader()
	{
		bw.Write(TotalFiles);
	}
}
