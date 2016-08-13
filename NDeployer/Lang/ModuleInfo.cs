using System;
using NDeployer.Script;
using NDeployer.Tasks;

namespace NDeployer.Lang
{
	class ModuleInfo
	{

		string name;
		TaskDef rootTask;

		public string Name { get { return name; } }
		public TaskDef RootTask { get { return rootTask; } }

		public ModuleInfo(string name, TaskDef rootTask)
		{
			this.name = name;
			this.rootTask = rootTask;
		}

	}
}

