using System;
using System.Collections.Generic;
using System.IO;

namespace tModReader
{
	public class Reader
	{
/*
		private const string InputDirectory = "Input";
*/

/*
		private const string OutputDirectory = "Output";
*/

/*
		private const string LogDirectory = "Logs";
*/

		private static readonly string LogFile = "Logs" + Path.DirectorySeparatorChar + "Logs.txt";

		public static void Main(string[] args)
		{
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("使用方法：把.tmod文件放入Input文件夹里，经过拆解之后可以在OutPut文件夹里找到源码！");
            Console.WriteLine("注意！有些.tmod文件选择了加密（先编译在整合）的情况下，本程序虽然可以反出资源文件但是源码已经编译在Windows.dll里面，需要用反编译器来查看源码。\n");
			Directory.CreateDirectory("Input");
			string[] files = Directory.GetFiles("Input", "*.tmod", SearchOption.TopDirectoryOnly);
			Directory.CreateDirectory("Output");
			Directory.CreateDirectory("Logs");
			File.WriteAllText(LogFile, "");
			string[] array = files;
            Console.WriteLine("目前搜索到的tmodloader文件：");
		    var n = 1;
		    foreach (string path in array)
		    {
		        Console.WriteLine(n.ToString() + "." + path);
		        n++;
		    }
            Console.WriteLine("请按下任意键开始拆解：");
            Console.ReadKey();
            Console.ForegroundColor=ConsoleColor.White;
		    foreach (string path in array)
			{
                Console.WriteLine("正在拆解： " + path);
			    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			    File.AppendAllText(LogFile, "Unpacking " + fileNameWithoutExtension + Environment.NewLine);
			    string text = "Output" + Path.DirectorySeparatorChar.ToString() + fileNameWithoutExtension + Path.DirectorySeparatorChar.ToString();
			    Directory.CreateDirectory(text);
			    ClearDirectory(text);
			    TmodFile tmodFile = new TmodFile(path);
			    tmodFile.Read();
			    string text2 = "Found files ";
			    foreach (KeyValuePair<string, byte[]> current in tmodFile)
			    {
			        text2 = text2 + current.Key + ", ";
			    }
			    text2 = text2.Substring(0, text2.Length - 2) + " in " + fileNameWithoutExtension + Environment.NewLine;
			    File.AppendAllText(LogFile, text2);
			    //BuildProperties buildProperties = BuildProperties.ReadModFile(tmodFile);
			    //bool hideCode = buildProperties.HideCode;

			    bool flag = tmodFile.HasFile("Windows.dll");
			    if (flag)
			    {
			        File.AppendAllText(LogFile, "Extracting Windows.dll for " + fileNameWithoutExtension + Environment.NewLine);
			        File.WriteAllBytes(text + "Windows.dll", tmodFile.GetFile("Windows.dll"));
			    }
			    bool flag2 = tmodFile.HasFile("Mono.dll");
			    if (flag2)
			    {
			        File.AppendAllText(LogFile, "Extracting Other.dll for " + fileNameWithoutExtension + Environment.NewLine);
			        File.WriteAllBytes(text + "Mono.dll", tmodFile.GetFile("Mono.dll"));
			    }
			    bool flag3 = tmodFile.HasFile("All.dll");
			    if (flag3)
			    {
			        File.AppendAllText(LogFile, "Extracting All.dll for " + fileNameWithoutExtension + Environment.NewLine);
			        File.WriteAllBytes(text + "All.dll", tmodFile.GetFile("All.dll"));
			    }
				
			    //bool hideResources = buildProperties.hideResources;
			    //if (hideResources)
			    //{
			    //File.AppendAllText(Reader.logFile, "The modder has chosen to hide resources (ie. images) from tModReader.");
			    //	File.AppendAllText(Reader.logFile, Environment.NewLine);
			    //}
			    //bool flag4 = !buildProperties.includeSource || buildProperties.hideCode;
			    //if (flag4)
			    //{
			    //	File.AppendAllText(Reader.logFile, "The modder has chosen to hide their code source from tModReader.");
			    //	File.AppendAllText(Reader.logFile, Environment.NewLine);
			    //}
			    try
			    {
			        foreach (KeyValuePair<string, byte[]> current2 in tmodFile)
			        {
			            string text3 = text + current2.Key;
			            //bool flag5 = text3.Substring(text3.Length - 3) == ".cs";
			            //bool flag6 = text3.Substring(text3.Length - 4) == ".dll";
			            //if (!flag6)
			            //{
			                //bool flag7 = !flag5 && buildProperties.hideResources;
			                //if (!flag7)
			                //{
			                //bool flag8 = flag5 && buildProperties.hideCode;
			                //if (!flag8)
			                //{
			                string directoryName = Path.GetDirectoryName(text3);
			                bool flag9 = !string.IsNullOrEmpty(directoryName);
			                if (flag9)
			                {
			                    Directory.CreateDirectory(directoryName);
			                }
			                File.WriteAllBytes(text3, current2.Value);
			                //}
			                //}
			            //}
			        }
			    }
			    catch (Exception ex)
			    {
			        File.AppendAllText(LogFile, ex.Message + Environment.NewLine + ex.StackTrace);
			    }
			}
            Console.WriteLine("\n拆解完成，请按任意键退出！");
		    Console.ReadKey();
		}

		private static void ClearDirectory(string directory)
		{
			string[] directories = Directory.GetDirectories(directory, ".", SearchOption.TopDirectoryOnly);
			foreach (string text in directories)
			{
			    ClearDirectory(text);
			    Directory.Delete(text);
			}
			string[] files = Directory.GetFiles(directory, ".", SearchOption.TopDirectoryOnly);
			foreach (string path in files)
			{
			    File.Delete(path);
			}
		}
	}
}
