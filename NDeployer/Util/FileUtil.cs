using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Ionic.Zip;

namespace NDeployer.Util
{
	static class FileUtil
	{
		public static bool DeleteDirectoryRecursively(string dirName)
		{
			if (!Directory.Exists(dirName))
				return false;

			foreach (string filename in Directory.GetFiles(dirName))
				File.Delete(filename);

			foreach (string directory in Directory.GetDirectories(dirName))
				DeleteDirectoryRecursively(directory);

			Directory.Delete(dirName);

			return true;
		}

		public static IEnumerable<string> ReadDirectoryRecursively(string path)
		{
			List<string> result = new List<string>();

			string[] files = Directory.GetFiles(path);
			foreach (string f in files)
				result.Add(f);

			string[] dirs = Directory.GetDirectories(path);
			foreach (string dir in dirs)
			{
				IEnumerable<string> childResult = ReadDirectoryRecursively(dir);
				result.AddRange(childResult);
			}

			return result;
		}

		public static string GetRelativePath(string filename, string basePath)
		{
			string dirName = Path.GetDirectoryName(filename);
			string relativePath = ".";
			if (!dirName.Equals(basePath))
				relativePath = dirName.Substring(basePath.Length + 1); // path + /
			return relativePath;
		}

		public static string FixDirectorySeparator(string str)
		{
			str = str.Replace("/", Path.DirectorySeparatorChar.ToString());
			str = str.Replace("\\", Path.DirectorySeparatorChar.ToString());
			return str;
		}

	}
}

