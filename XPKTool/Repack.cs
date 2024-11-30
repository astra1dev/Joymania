using System.Text;

namespace XPKTool
{
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
			string str = string.Concat(FolderName, ".xpk");
			if (TotalFiles == 0)
			{
				Utils.ErrorAndExit("[!] No files were found in the specified directory!");
			}
			else
			{
				// if there is already a .xpk file with the same name, create a backup of it
				if (File.Exists(str))
				{
					File.Copy(str, str.Replace(".xpk", ".xpk.bak"), true);
					Console.WriteLine(string.Concat("[-] Creating file backup to ", FolderName, ".xpk.bak"));
				}
				Console.WriteLine(string.Concat(new object[] { "[-] Packing ", TotalFiles, " files from folder: (", FolderName, ") to file ", FolderName, ".xpk" }));
				OffsetsUtil = new Offsets(TotalFiles);
				fs = new FileStream(str, FileMode.Create);
				bw = new BinaryWriter(fs);
				for (int i = 0; i < TotalFiles; i++)
				{
					Files.Add(new XPKFile());
				}
				int length = 0;
				uint num = 0;
				uint num1 = 0;
				string[] allFiles = AllFiles;
				for (int j = 0; j < allFiles.Length; j++)
				{
					string str1 = allFiles[j];
					FileInfo fileInfo = new(str1);
					string str2 = str1.Replace(string.Concat(folderDirectory, "\\"), "").ToString();
					Files[length].Name = str2;
					Files[length].Size = (uint)fileInfo.Length;
					XPKFile item = Files[length];
					DateTime creationTime = fileInfo.CreationTime;
					item.CreationDate = Convert.ToUInt32(Utils.ConvertToTimestamp(creationTime.ToUniversalTime()));
					Files[length].NameOffset = num1;
					num1 = Offsets.CalcNextStringOffset(str2.Length, (int)num1);
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

		public static byte[] GetFileDataBytes(string filePath)
		{
			return File.ReadAllBytes(filePath);
		}

		public void WriteFilenameStringsOffsetsTable()
		{
			for (int i = 0; i < TotalFiles; i++)
			{
				bw.Write(Files[i].NameOffset);
			}
			bw.Write(OffsetsUtil.FILENAME_STRINGS_BLOCK_SIZE);
		}

		public void WriteFilenameStringsTable()
		{
			for (int i = 0; i < TotalFiles; i++)
			{
				bw.Write(Encoding.Default.GetBytes(Files[i].Name));
				bw.Write((byte)0);
			}
			bw.Write(OffsetsUtil.FILEDATA_BLOCK_SIZE);
		}

		public void WriteFileOffsetsTable()
		{
			uint pADDINGTOFILEDATA = OffsetsUtil.PADDING_TO_FILEDATA;
			for (int i = 0; i < TotalFiles; i++)
			{
				bw.Write(pADDINGTOFILEDATA);
				pADDINGTOFILEDATA = Offsets.CalcNextFileDataOffset((int)Files[i].Size, (int)pADDINGTOFILEDATA);
			}
		}

		public void WriteFilesData()
		{
			string[] allFiles = AllFiles;
			for (int i = 0; i < allFiles.Length; i++)
			{
				string str = allFiles[i];
				bw.Write(GetFileDataBytes(str));
			}
		}

		public void WriteFileSizesTable()
		{
			for (int i = 0; i < TotalFiles; i++)
			{
				bw.Write(Files[i].Size);
			}
		}

		public void WriteHashTable()
		{
			for (int i = 0; i < TotalFiles; i++)
			{
				bw.Write(Files[i].CreationDate);
			}
		}

		public void WriteHeader()
		{
			bw.Write(TotalFiles);
		}
	}
}
