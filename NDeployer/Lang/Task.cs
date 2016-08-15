using System;
using System.Collections.Generic;

namespace NDeployer.Lang
{
	class Task
	{
		string name;
		Dictionary<string, string> attributes;
		Context context;

		public string Name { get { return name; } }
		public Dictionary<string, string> Attributes { get { return attributes; } }
		public Context Context { get { return context; } }
	}
}

