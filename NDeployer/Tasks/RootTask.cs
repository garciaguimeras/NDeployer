using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Tasks;
using NDeployer.Util;

namespace NDeployer.Tasks
{
	class RootTask : GeneratorTask
	{

		string[] args;

		public RootTask(TaskDef rootNode, string[] args) : base(rootNode)
		{
			this.args = args;
			ContextStrategy = ContextStrategy.KEEP;
		}

		public override bool IsValidTaskDef()
		{
			if (RootNode == null)
			{
				environment.AddToErrorList("Invalid build file. Can't find document root");
				return false;
			}
			return true;
		}

		public void LoadArguments()
		{
			environment.AddProperty("ARGS.LENGTH", args.Length.ToString());
			for (int i = 0; i < args.Length; i++)
			{
				string argName = string.Format("ARGS.{0}", (i + 1).ToString());
				environment.AddProperty(argName, args[i]);
			}
		}

		public void LoadMetaAttributes()
		{
			LoadMetaAttributes(RootNode.Children);
		}

		public override void ExecuteGenerator()
		{
			LoadMetaAttributes(RootNode.Children);
			LoadImports(RootNode.Children);
			LoadArguments();
			LoadProperties(RootNode.Children);
			ExecuteContext(RootNode.Children);
		}

	}
}

