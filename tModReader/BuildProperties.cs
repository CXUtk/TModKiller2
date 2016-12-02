using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace tModReader
{
	internal class BuildProperties
	{
		internal string[] DllReferences = new string[0];

		internal string[] ModReferences = new string[0];

		internal string[] BuildIgnores = new string[0];

		internal string Author = "";

		internal Version Version = new Version(1, 0);

		internal string DisplayName = "";

		internal bool NoCompile;

		internal bool HideCode = true;

		internal bool HideResources = true;

		internal bool IncludeSource;

		internal bool IncludePdb;

		internal bool EditAndContinue;

		internal int LanguageVersion = 4;

		internal string Homepage = "";

		internal string Description = "";

        internal static BuildProperties ReadBuildFile(string modDir)
        {
            string path = modDir + Path.DirectorySeparatorChar.ToString() + "build.txt";
            string path2 = modDir + Path.DirectorySeparatorChar.ToString() + "description.txt";
            BuildProperties buildProperties = new BuildProperties();
            bool flag = !File.Exists(path);
            BuildProperties result;
            if (flag)
            {
                result = buildProperties;
            }
            else
            {
                bool flag2 = File.Exists(path2);
                if (flag2)
                {
                    buildProperties.Description = File.ReadAllText(path2);
                }
                string[] array = File.ReadAllLines(path);
                string[] array2 = array;
                foreach (string text in array2)
                {
                    bool flag3 = text.Length == 0;
                    if (!flag3)
                    {
                        int num = text.IndexOf('=');
                        string text2 = text.Substring(0, num).Trim();
                        string text3 = text.Substring(num + 1).Trim();
                        bool flag4 = text3.Length == 0;
                        if (!flag4)
                        {
                            string text4 = text2;
                            uint num2 = 0;
                            if (text4 == "includePDB")
                            {
                                buildProperties.IncludePdb = (text3.ToLower() == "true");
                            }

                            else if (text4 == "dllReferences")
                            {
                                string[] array3 = text3.Split(new char[]
                                {
                                    ','
                                });
                                for (int j = 0; j < array3.Length; j++)
                                {
                                    string text5 = array3[j].Trim();
                                    bool flag5 = text5.Length > 0;
                                    if (flag5)
                                    {
                                        array3[j] = text5;
                                    }
                                }
                                buildProperties.DllReferences = array3;
                            }

                            else if (text4 == "noCompile")
                            {
                                buildProperties.NoCompile = (text3.ToLower() == "true");
                            }
                            else if (num2 != 1181855383u)
                            {
                                if (num2 != 1310244050u)
                                {
                                    if (num2 == 1333443158u)
                                    {
                                        if (text4 == "author")
                                        {
                                            buildProperties.Author = text3;
                                        }
                                    }
                                }
                                else if (text4 == "includeSource")
                                {
                                    buildProperties.IncludeSource = (text3.ToLower() == "true");
                                }
                            }
                            else if (text4 == "version")
                            {
                                buildProperties.Version = new Version(text3);
                            }


                            if (text4 == "hideResources")
                            {
                                buildProperties.HideResources = (text3.ToLower() != "false");
                            }


                            else if (text4 == "hideCode")
                            {
                                buildProperties.HideCode = (text3.ToLower() != "false");
                            }

                            else if (text4 == "buildIgnore")
                            {
                                string[] array4 = text3.Split(new char[]
                                {
                                    ','
                                });
                                for (int k = 0; k < array4.Length; k++)
                                {
                                    string text6 = array4[k].Trim();
                                    bool flag6 = text6.Length > 0;
                                    if (flag6)
                                    {
                                        array4[k] = text6;
                                    }
                                }
                                buildProperties.BuildIgnores = array4;
                            }


                            if (text4 == "displayName")
                            {
                                buildProperties.DisplayName = text3;
                            }
                            else if (text4 == "homepage")
                            {
                                buildProperties.Homepage = text3;



                                if (text4 == "modReferences")
                                {
                                    string[] array5 = text3.Split(new char[]
                                    {
                                        ','
                                    });
                                    for (int l = 0; l < array5.Length; l++)
                                    {
                                        string text7 = array5[l].Trim();
                                        bool flag7 = text7.Length > 0;
                                        if (flag7)
                                        {
                                            array5[l] = text7;
                                        }
                                    }
                                    buildProperties.ModReferences = array5;
                                }


                                else if (text4 == "languageVersion")
                                {
                                    bool flag8 = !int.TryParse(text3, out buildProperties.LanguageVersion);
                                    if (flag8)
                                    {
                                        throw new Exception("languageVersion not an int: " + text3);
                                    }
                                    bool flag9 = buildProperties.LanguageVersion < 4 || buildProperties.LanguageVersion > 6;
                                    if (flag9)
                                    {
                                        throw new Exception("languageVersion (" + buildProperties.LanguageVersion + ") must be between 4 and 6");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            result = buildProperties;
            return result;
        }

		internal byte[] ToBytes()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					bool flag = DllReferences.Length != 0;
					if (flag)
					{
						binaryWriter.Write("dllReferences");
						string[] array = DllReferences;
						foreach (string value in array)
						{
						    binaryWriter.Write(value);
						}
						binaryWriter.Write("");
					}
					bool flag2 = ModReferences.Length != 0;
					if (flag2)
					{
						binaryWriter.Write("modReferences");
						string[] array2 = ModReferences;
						foreach (string value2 in array2)
						{
						    binaryWriter.Write(value2);
						}
						binaryWriter.Write("");
					}
					bool flag3 = Author.Length > 0;
					if (flag3)
					{
						binaryWriter.Write("author");
						binaryWriter.Write(Author);
					}
					binaryWriter.Write("version");
					binaryWriter.Write(Version.ToString());
					bool flag4 = DisplayName.Length > 0;
					if (flag4)
					{
						binaryWriter.Write("displayName");
						binaryWriter.Write(DisplayName);
					}
					bool flag5 = Homepage.Length > 0;
					if (flag5)
					{
						binaryWriter.Write("homepage");
						binaryWriter.Write(Homepage);
					}
					bool flag6 = Description.Length > 0;
					if (flag6)
					{
						binaryWriter.Write("description");
						binaryWriter.Write(Description);
					}
					bool flag7 = NoCompile;
					if (flag7)
					{
						binaryWriter.Write("noCompile");
					}
					bool flag8 = !HideCode;
					if (flag8)
					{
						binaryWriter.Write("!hideCode");
					}
					bool flag9 = !HideResources;
					if (flag9)
					{
						binaryWriter.Write("!hideResources");
					}
					bool flag10 = IncludeSource;
					if (flag10)
					{
						binaryWriter.Write("includeSource");
					}
					bool flag11 = IncludePdb;
					if (flag11)
					{
						binaryWriter.Write("includePDB");
					}
					bool flag12 = EditAndContinue;
					if (flag12)
					{
						binaryWriter.Write("editAndContinue");
					}
					binaryWriter.Write("");
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		internal static BuildProperties ReadModFile(TmodFile modFile)
		{
			BuildProperties buildProperties = new BuildProperties();
			byte[] file = modFile.GetFile("Info");
			bool flag = file.Length == 0;
			BuildProperties result;
			if (flag)
			{
				result = buildProperties;
			}
			else
			{
				using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(file)))
				{
					string text = binaryReader.ReadString();
					while (text.Length > 0)
					{
						bool flag2 = text == "dllReferences";
						if (flag2)
						{
							List<string> list = new List<string>();
							string text2 = binaryReader.ReadString();
							while (text2.Length > 0)
							{
								list.Add(text2);
								text2 = binaryReader.ReadString();
							}
							buildProperties.DllReferences = list.ToArray();
						}
						bool flag3 = text == "modReferences";
						if (flag3)
						{
							List<string> list2 = new List<string>();
							string text3 = binaryReader.ReadString();
							while (text3.Length > 0)
							{
								list2.Add(text3);
								text3 = binaryReader.ReadString();
							}
							buildProperties.ModReferences = list2.ToArray();
						}
						bool flag4 = text == "author";
						if (flag4)
						{
							buildProperties.Author = binaryReader.ReadString();
						}
						bool flag5 = text == "version";
						if (flag5)
						{
							buildProperties.Version = new Version(binaryReader.ReadString());
						}
						bool flag6 = text == "displayName";
						if (flag6)
						{
							buildProperties.DisplayName = binaryReader.ReadString();
						}
						bool flag7 = text == "homepage";
						if (flag7)
						{
							buildProperties.Homepage = binaryReader.ReadString();
						}
						bool flag8 = text == "description";
						if (flag8)
						{
							buildProperties.Description = binaryReader.ReadString();
						}
						bool flag9 = text == "noCompile";
						if (flag9)
						{
							buildProperties.NoCompile = true;
						}
						bool flag10 = text == "!hideCode";
						if (flag10)
						{
							buildProperties.HideCode = false;
						}
						bool flag11 = text == "!hideResources";
						if (flag11)
						{
							buildProperties.HideResources = false;
						}
						bool flag12 = text == "includeSource";
						if (flag12)
						{
							buildProperties.IncludeSource = true;
						}
						bool flag13 = text == "includePDB";
						if (flag13)
						{
							buildProperties.IncludePdb = true;
						}
						bool flag14 = text == "editAndContinue";
						if (flag14)
						{
							buildProperties.EditAndContinue = true;
						}
						text = binaryReader.ReadString();
					}
				}
				result = buildProperties;
			}
			return result;
		}

		internal bool IgnoreFile(string resource)
		{
			return BuildIgnores.Any((string fileMask) => FitsMask(resource, fileMask));
		}

		private bool FitsMask(string fileName, string fileMask)
		{
			string pattern = "^" + Regex.Escape(fileMask.Replace(".", "__DOT__").Replace("*", "__STAR__").Replace("?", "__QM__")).Replace("__DOT__", "[.]").Replace("__STAR__", ".*").Replace("__QM__", ".") + "$";
			return new Regex(pattern, RegexOptions.IgnoreCase).IsMatch(fileName);
		}
	}
}
