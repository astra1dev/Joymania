namespace XPKTool;

public class Utils
{
	/// <summary>
	/// Converts a DateTime object to a Unix timestamp.
	/// </summary>
	/// <param name="value">The DateTime object.</param>
	/// <returns>The Unix timestamp.</returns>
	public static long ConvertToTimestamp(DateTime value)
	{
		return (value.Ticks - 621355968000000000L) / 10000000;
	}

	/// <summary>
	/// Reads a null-terminated string from a BinaryReader.
	/// </summary>
	/// <param name="br">The BinaryReader to read from.</param>
	/// <returns>The read string.</returns>
	public static string ReadNullTerminatedString(BinaryReader br)
	{
		var str = "";
		while (true)
		{
			var chr = br.ReadChar();
			// If the character is null, break the loop and return the string
			if (chr <= '\0')
			{
				break;
			}
			str = string.Concat(str, chr.ToString());
		}
		return str;
	}

	/// <summary>
	/// Exits the program gracefully, resetting the console color and prompting the user to press Enter.
	/// </summary>
	public static void ExitProgram()
	{
		Console.ResetColor();
		Console.WriteLine("[-] Press Enter to close.");
		Console.ReadLine();
		Environment.Exit(0);
	}

	/// <summary>
	/// Prints an error message in red and exits the program.
	/// </summary>
	/// <param name="text">Text to display in the error message.</param>
	public static void ErrorAndExit(string text)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine(text);
		Console.ResetColor();
		ExitProgram();
	}
}
