using System;

namespace NDeployer.Script
{
	abstract class ScriptFile
	{
		public abstract TaskDef Parse(string filename);
	}
}

