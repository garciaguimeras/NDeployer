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
	}
}

