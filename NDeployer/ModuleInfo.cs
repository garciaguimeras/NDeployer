using System;
using System.Collections.Generic;
using System.Linq;

using NDeployer.Script;
using NDeployer.Tasks;

namespace NDeployer
{
	class ModuleInfo
	{

		string name;
		TaskDef rootTask;
		List<ModuleInfo> dependencies;

		public string Name { get { return name; } }
		public TaskDef RootTask { get { return rootTask; } }
		public IEnumerable<ModuleInfo> Dependencies { get { return dependencies; } }

		public ModuleInfo(string name, TaskDef rootTask)
		{
			this.name = name;
			this.rootTask = rootTask;
			this.dependencies = new List<ModuleInfo>();
		}

		public void AddDependency(ModuleInfo module)
		{
			if (!dependencies.Select(m => m.Name).Contains(module.Name))
			{
				dependencies.Add(module);
			}
		}

	}
}

