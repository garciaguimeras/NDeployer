﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Util;

namespace NDeployer.Tasks
{
	class ReadLineTask : Task
	{

		string name;
		string text;

		public ReadLineTask(string name) : base(name)
		{
			name = null;
			text = null;
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			name = GetAttribute(rootNode, "name");
			text = GetAttribute(rootNode, "text");
			if (name == null)
			{
				AddAttributeNotFoundError("name");
				return false;
			}
			if (text == null)
			{
				AddAttributeNotFoundError("text");
				return false;
			}
			return true;
		}

		public override void Execute()
		{
			// Evaluate text property
			text = PropertyEvaluator.EvalValue(text);
			if (text == null)
			{
				environment.Pipe.AddToErrorPipe("Error evaluating attributes. Execution suspended.");
				return;
			}

			Console.Write("{0}: ", text);
			string value = Console.ReadLine();

			environment.AddProperty(name, value);
		}

	}
}
