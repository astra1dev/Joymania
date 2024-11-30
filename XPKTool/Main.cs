namespace XPKTool
{
	internal class Program
	{
		private static void Main(string[] args)
		{
            _ = new Program();
            Console.WriteLine(@"
____  _____________ ____  __  ___________            __   
\   \/  /\______   \    |/ _| \__    ___/___   ____ |  |  
 \     /  |     ___/      <     |    | /  _ \ /  _ \|  |  
 /     \  |    |   |    |  \    |    |(  <_> |  <_> )  |__
/___/\__\ |____|   |____|___\   |____| \____/ \____/|____/");
			Console.WriteLine("v1.0              created by Meth0d & improved by Astral\n");
			// if an argument is specified
			if (args.Length != 0)
			{
				// if the argument is a folder, create a .xpk file from it
				if (!File.Exists(args[0])) { _ = new XPKCreator(args[0]); }
				// if the argument is a .xpk file, extract it
				else if (Path.GetExtension(args[0]) == ".xpk") { _ = new XPKArchive(args[0]); }
				// if the file is not a .xpk file, exit
				else {Utils.ErrorAndExit("[!] Please provide a .XPK file to unpack or a folder to repack.");}
			}
			// if the executable is ran without any arguments
			else
			{
				Console.WriteLine("[-] Packer/Unpacker for XPK Game Files");
				Console.WriteLine("[-] Compatible with: Santa Claus In Trouble (1 & 2) and Rosso Rabbit In Trouble");
				Console.WriteLine("\nUsage:");
				Console.WriteLine("- To unpack: Drop a .XPK FILE on the executable or run 'XPKTool.exe FILE.xpk'.");
				Console.WriteLine("- To repack: Drop a FOLDER on the executable or run 'XPKTool.exe FOLDER'.\n");
				Utils.ExitProgram();
			}
		}
	}
}
