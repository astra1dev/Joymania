namespace XPKTool
{
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
			for (int i = 0; i < TotalFiles; i++)
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
			Console.WriteLine(string.Concat(new object[] { "[+] File ", Name, ".xpk was successfully unpacked, ", TotalFiles, " files were extracted." }));
			Utils.ExitProgram();
		}

		public void ExtractAll()
		{
			foreach (XPKFile file in Files)
			{
				Directory.CreateDirectory(string.Concat(Name, "\\", Path.GetDirectoryName(file.Name)));
				File.WriteAllBytes(string.Concat(Name, "\\", file.Name), GetFileData(file.DataOffset, (int)file.Size));
			}
		}

		public byte[] GetFileData(uint offset, int size)
		{
			br.BaseStream.Position = offset;
			byte[] numArray = new byte[size];
			br.Read(numArray, 0, size);
			return numArray;
		}

		public void ReadFileDataOffsetsTable()
		{
			for (int i = 0; i < TotalFiles; i++)
			{
				Files[i].DataOffset = br.ReadUInt32();
			}
		}

		public void ReadFileSizesTable()
		{
			for (int i = 0; i < TotalFiles; i++)
			{
				Files[i].Size = br.ReadUInt32();
			}
		}

		public void ReadHashTable()
		{
			for (int i = 0; i < TotalFiles; i++)
			{
				Files[i].CreationDate = br.ReadUInt32();
			}
		}

		public void ReadStringsOffsetTable()
		{
			for (int i = 0; i < TotalFiles; i++)
			{
				Files[i].NameOffset = br.ReadUInt32();
			}
			FileDataSizeOffset = br.ReadUInt32();
		}

		public void ReadStringsTable()
		{
			for (int i = 0; i < TotalFiles; i++)
			{
				Files[i].Name = Utils.ReadNullTerminatedString(br);
			}
			FileDataSize = br.ReadUInt32();
		}
	}
}
