using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using NDeployer.Util;

namespace NDeployer.Tasks
{
	class FilterTask : Task
	{

		string include;
		string exclude;

		public FilterTask(string name) : base(name)
		{
			include = null;
			exclude = null;
		}

		public override bool ProcessXml(XElement rootNode)
		{
			include = GetAttribute(rootNode, "include");
			exclude = GetAttribute(rootNode, "exclude");
			if (include == null && exclude == null)
				return false;
			return true;
		}

		private bool NeedIncludeFlag(Dictionary<string, string> data)
		{
			foreach (string key in data.Keys) 
			{
				if (WildcardExpressionEvaluator.EvalExpression(include, data[key]))
					return true;
			}
			return false;
		}

		private bool NeedExcludeFlag(Dictionary<string, string> data)
		{
			foreach (string key in data.Keys) 
			{
				if (WildcardExpressionEvaluator.EvalExpression(exclude, data[key]))
					return true;
			}
			return false;
		}

		public override void Execute()
		{
			IEnumerable<Dictionary<string, string>> input = environment.Pipe.Input;
			foreach (Dictionary<string, string> data in input) 
			{
				if (include != null && NeedIncludeFlag(data))
					data.Add("include", "");
				if (exclude != null && NeedExcludeFlag(data))
					data.Add("exclude", "");					
				environment.Pipe.AddToOuputPipe(data);
			}
		}

	}
}

