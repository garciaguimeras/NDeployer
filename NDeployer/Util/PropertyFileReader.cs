using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NDeployer.Util
{
	static class PropertyFileReader
	{

		public static Dictionary<string, string> Read(string filename)
		{
			Dictionary<string, string> properties = new Dictionary<string, string>();

			using (StreamReader reader = new StreamReader(File.OpenRead(filename)))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine().Trim();

					// Dismiss comments
					if (line.StartsWith("#"))
						continue;

					// Dismiss empty lines
					if (string.IsNullOrEmpty(line))
						continue;

					// A properties file should have ONLY properties
					if (!line.Contains("="))
						return null;

					string name = "";
					string value = "";
					int pos = line.IndexOf("=");
					name = line.Substring(0, pos).Trim();

					if (pos + 1 < line.Length)
						value = line.Substring(pos + 1).Trim();

					// A property name cannot be empty
					if (string.IsNullOrEmpty(name))
						return null;

					// Finally everything seems to be ok, so add the property to the dictionary
					properties.Add(name, value);
				}
			}

			return properties;
		}

	}
}

