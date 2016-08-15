using System;
using System.Collections.Generic;

namespace NDeployer.Lang
{
	class Context
	{
		Dictionary<string, MetaAttribute> metaAttributes;
		Dictionary<string, Property> properties;
		Dictionary<string, Function> functions;
		List<Task> tasks;
	
		public Dictionary<string, MetaAttribute> MetaAttributes { get { return metaAttributes; } }
		public Dictionary<string, Property> Properties { get { return properties; } }
		public Dictionary<string, Function> Functions { get { return functions; } }
		public List<Task> Tasks { get { return tasks; } }
	}
}

