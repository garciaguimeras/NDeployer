using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NDeployer.Script
{
	static class XmlFileParser
	{

		private static Dictionary<string, string> GetAttributes(XElement element)
		{
			Dictionary<string, string> attrs = new Dictionary<string, string>();
			foreach (XAttribute attr in element.Attributes())
			{
				attrs.Add(attr.Name.ToString(), attr.Value);
			}
			return attrs;
		}

		private static TaskDef GetTaskDef(XElement element)
		{
			TaskDef TaskDef = new TaskDef();

			// Set name
			TaskDef.Name = element.Name.ToString();

			// Set attributes
			TaskDef.Attributes = GetAttributes(element);

			// Set value
			if (!string.IsNullOrEmpty(element.Value))
				TaskDef.Attributes.Add("value", element.Value);

			// Set inner TaskDefs
			List<TaskDef> TaskDefs = new List<TaskDef>();
			foreach (XElement child in element.Elements())
			{
				TaskDefs.Add(GetTaskDef(child));
			}
			TaskDef.Children = TaskDefs;

			return TaskDef;
		}

		public static TaskDef GetRootTaskDef(XElement root)
		{
			return GetTaskDef(root);
		}

	}
}

