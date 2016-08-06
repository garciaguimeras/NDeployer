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
		public List<TaskDef> Children { get; set; }

		public TaskDef()
		{
			Name = "";
			Attributes = new Dictionary<string, string>();
			Children = new List<TaskDef>();
		}

		public static TaskDef Create(string name)
		{
			return new TaskDef { Name = name };
		}

		public string AttributeByName(string name)
		{
			if (Attributes.ContainsKey(name))
				return Attributes[name];
			return null;
		}

		public List<TaskDef> ChildrenByName(string name)
		{
			List<TaskDef> filtered = new List<TaskDef>();
			foreach (TaskDef child in Children)
			{
				if (child.Name.Equals(name))
					filtered.Add(child);
			}
			return filtered;
		}

	}
}

