using NDeployer.Lang;

namespace NDeployer.Script
{
	abstract class ScriptFile
	{
		public abstract ModuleInfo Parse(string filename);
	}
}

