using System;
using System.IO;
using System.Linq;

using NDeployer.Script;
using NDeployer.Tasks;

namespace NDeployer.Util
{
	static class ScriptLoader
	{

		public static void Load(string filename)
		{
			Load(filename, false, new string[] { });
		}

		public static void Load(string filename, bool executeScript, params string[] argList)
		{
			Environment environment = Environment.GetEnvironment();

			if (!File.Exists(filename))
			{
				environment.AddToErrorList("Script file not found {0}", filename);
				return;
			}

			ScriptFile scriptFile = ScriptFactory.GetScriptForFilename(filename);
			if (scriptFile == null)
			{
				environment.AddToErrorList("Invalid file type or extension {0}", filename);
				return;
			}

			TaskDef rootTaskDef = scriptFile.Parse(filename);
			if (rootTaskDef == null)
			{
				environment.AddToErrorList("Could not parse file {0}", filename);
				return;
			}

			RootTask rootTask = new RootTask(rootTaskDef, argList);
			bool isValid = rootTask.IsValidTaskDef();
			if (!isValid)
				return;

			if (executeScript)
				rootTask.Execute();
			else
				rootTask.Load();
		}

	}
}

