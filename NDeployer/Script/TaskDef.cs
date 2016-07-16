using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDeployer.Script
{
	class TaskDef
	{
		public string Name { get; set; }
		public Dictionary<string, string> Attributes { get; set; }
		public List<TaskDef> TaskDefs { get; set; }

		public TaskDef()
		{}

		public string AttributeByName(string name)
		{
			if (Attributes.ContainsKey(name))
				return Attributes[name];
			return null;
		}

		public List<TaskDef> TaskDefsByName(string name)
		{
			List<TaskDef> filtered = new List<TaskDef>();
			foreach (TaskDef child in TaskDefs)
			{
				if (child.Name.Equals(name))
					filtered.Add(child);
			}
			return filtered;
		}

	}
}

