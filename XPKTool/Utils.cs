namespace XPKTool
{
	public class Utils
	{
		public static long ConvertToTimestamp(DateTime value)
		{
			return (value.Ticks - 621355968000000000L) / 10000000;
		}

		public static string ReadNullTerminatedString(BinaryReader br)
		{
			string str = "";
			while (true)
			{
				char chr = br.ReadChar();
				char chr1 = chr;
				if (chr <= '\0')
				{
					break;
				}
				str = string.Concat(str, chr1.ToString());
			}
			return str;
		}

		public static void ExitProgram()
		{
			Console.ResetColor();
			Console.WriteLine("[-] Press Enter to close.");
			Console.ReadLine();
			Environment.Exit(0);
		}

		public static void ErrorAndExit(string text)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(text);
			ExitProgram();
		}
	}
}
