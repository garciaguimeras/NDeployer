using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{
	class FilterTask : Task
	{

		string include;
		string exclude;

		public FilterTask(TaskDef rootNode) : base(rootNode)
		{
			include = null;
			exclude = null;
		}

		public override bool IsValidTaskDef()
		{
			include = GetAttribute(RootNode, "include");
			exclude = GetAttribute(RootNode, "exclude");
			if (include == null && exclude == null)
			{
				AddOneAttributeMandatoryError("include", "exclude");
				return false;
			}
			return true;
		}

		private bool MatchInclude(Dictionary<string, string> data)
		{
			foreach (string key in data.Keys) 
			{
				if (WildcardExpressionEvaluator.EvalExpression(include, data[key]))
					return true;
			}
			return false;
		}

		private bool MatchExclude(Dictionary<string, string> data)
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
			IEnumerable<Dictionary<string, string>> input = environment.Pipe.Std;
			foreach (Dictionary<string, string> data in input) 
			{
				if (include != null)
				{
					if (MatchInclude(data))
					{
						if (data.ContainsKey("exclude"))
							data.Remove("exclude");
						if (!data.ContainsKey("include"))
							data.Add("include", "");
					} 
					else
					{
						if (!data.ContainsKey("exclude") && !data.ContainsKey("include"))
							data.Add("exclude", "");
					}
				}

				if (exclude != null)
				{
					if (MatchExclude(data))
					{
						if (!data.ContainsKey("exclude"))
							data.Add("exclude", "");
					}
				}
			}
		}

	}
}

