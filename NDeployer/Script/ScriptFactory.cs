using System;

namespace NDeployer.Script
{
	static class ScriptFactory
	{

		public static ScriptFile GetScriptForFilename(string filename)
		{
			filename = filename.Trim().ToLower();

			if (filename.EndsWith(".xml"))
				return new XmlFile();

			return null;
		}

	}
}

