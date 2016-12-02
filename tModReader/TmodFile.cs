using Ionic.Zlib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace tModReader
{
	public class TmodFile : IEnumerable<KeyValuePair<string, byte[]>>
	{
		public readonly string Path;

		private readonly IDictionary<string, byte[]> _files = new Dictionary<string, byte[]>();

		internal bool ValidModBrowserSignature;

		private Exception _readException;

		public Version ModLoaderVersion
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			internal set;
		}

		public Version Version
		{
			get;
			internal set;
		}

		public byte[] Hash
		{
			get;
			private set;
		}

		internal byte[] Signature
		{
			get;
			private set;
		}

		internal TmodFile(string path)
		{
			Signature = new byte[256];
			Path = path;
		}

		public bool HasFile(string fileName)
		{
			return _files.ContainsKey(fileName.Replace('\\', '/'));
		}

		public byte[] GetFile(string fileName)
		{
			byte[] result;
			_files.TryGetValue(fileName.Replace('\\', '/'), out result);
			return result;
		}

		internal void AddFile(string fileName, byte[] data)
		{
			byte[] array = new byte[data.Length];
			data.CopyTo(array, 0);
			_files[fileName.Replace('\\', '/')] = array;
		}

		internal void RemoveFile(string fileName)
		{
			_files.Remove(fileName.Replace('\\', '/'));
		}

		public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator()
		{
			return _files.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal void Read()
		{
			try
			{
				byte[] buffer;
				using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(Path)))
				{
					bool flag = Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) != "TMOD";
					if (flag)
					{
						throw new Exception("Magic Header != \"TMOD\"");
					}
					ModLoaderVersion = new Version(binaryReader.ReadString());
					Hash = binaryReader.ReadBytes(20);
					Signature = binaryReader.ReadBytes(256);
					buffer = binaryReader.ReadBytes(binaryReader.ReadInt32());
					byte[] first = SHA1.Create().ComputeHash(buffer);
					bool flag2 = !first.SequenceEqual(Hash);
					if (flag2)
					{
						throw new Exception("Hash mismatch, data blob has been modified or corrupted");
					}
				}
				using (BinaryReader binaryReader2 = new BinaryReader(new DeflateStream(new MemoryStream(buffer), CompressionMode.Decompress)))
				{
					Name = binaryReader2.ReadString();
					Version = new Version(binaryReader2.ReadString());
					int num = binaryReader2.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						AddFile(binaryReader2.ReadString(), binaryReader2.ReadBytes(binaryReader2.ReadInt32()));
					}
				}
			}
			catch (Exception ex)
			{
				_readException = ex;
			}
		}

		internal Exception ValidMod()
		{
			bool flag = _readException != null;
			Exception result;
			if (flag)
			{
				result = _readException;
			}
			else
			{
				bool flag2 = !HasFile("Info");
				if (flag2)
				{
					result = new Exception("Missing Info file");
				}
				else
				{
				    bool flag3 = !HasFile("All.dll") && (!HasFile("Windows.dll") || !HasFile("Mono.dll"));
				    result = flag3 ? new Exception("Missing All.dll or Windows.dll and Mono.dll") : null;
				}
			}
			return result;
		}
	}
}
