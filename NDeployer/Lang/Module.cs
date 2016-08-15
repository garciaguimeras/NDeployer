using System;
using System.Collections.Generic;

namespace NDeployer.Lang
{
	class Module
	{
		string name;
		List<Module> dependencies;
		Context context;

		public string Name { get { return name; } }
		public List<Module> Dependencies { get { return dependencies; } }
		public Context Context { get { return context; } }	}
}

