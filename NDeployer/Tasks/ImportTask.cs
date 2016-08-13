using System.Linq;

using NDeployer.Script;
using NDeployer.Util;
using System.IO;
using System;

namespace NDeployer.Tasks
{
	class ImportTask : Task
	{

		string filename;

		public ImportTask(TaskDef rootNode) : base(rootNode)
		{
			filename = null;
		}

		public override bool IsValidTaskDef()
		{
			filename = GetAttribute(RootNode, "filename");
			if (filename == null)
			{
				AddAttributeNotFoundError("filename");
				return false;
			}
			return true;
		}

		public override void Execute()
		{
			filename = PropertyEvaluator.EvalValue(filename);
			if (filename == null)
			{
				AddErrorEvaluatingAttribute("filename");
				return;
			}

			filename = FileUtil.FixDirectorySeparator(filename);
			string scriptPath = Path.Combine(System.Environment.CurrentDirectory, filename);

			ScriptLoader.Load(scriptPath);
			if (environment.Errors.Count() > 0)
				environment.PrintErrorList();
		}
	}
}

