﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NDeployer.Script;
using NDeployer.Util;
using System.Xml.Xsl.Runtime;

namespace NDeployer.Tasks
{
	class PrintTask : Task
	{

		string text;

		public PrintTask(string name) : base(name)
		{
			text = null;
		}

		public override bool ProcessTaskDef(TaskDef rootNode)
		{
			text = GetAttribute(rootNode, "text");
			if (text == null)
			{
				AddAttributeNotFoundError("text");
				return false;
			}
			return true;
		}

		private List<string> ExtractReferences(string txt)
		{
			List<string> references = new List<string>();

			bool refFound = true;
			while (refFound)
			{
				int iPos = txt.IndexOf("[[");
				if (iPos == -1)
				{
					refFound = false;
					continue;
				}
				txt = iPos + 2 < txt.Length ? txt.Substring(iPos + 2) : "";
				int fPos = txt.IndexOf("]]");
				if (fPos == -1)
				{
					refFound = false;
					continue;
				}
				string rf = txt.Substring(0, fPos);
				txt = fPos + 2 < txt.Length ? txt.Substring(fPos + 2) : "";
				references.Add(rf);
			}

			return references;
		}

		public Dictionary<string, string> EvaluateReferences(List<string> references, Dictionary<string, string> data)
		{
			Dictionary<string, string> eval = new Dictionary<string, string>();

			foreach (string rf in references)
			{
				if (!data.Keys.Contains(rf))
					return null;
				eval.Add(rf, data[rf]);
			}

			return eval;
		}

		public override void Execute()
		{
			text = PropertyEvaluator.EvalValue(text);
			if (text == null)
				return;

			List<string> references = ExtractReferences(text);
			if (references.Count == 0)
			{
				Console.WriteLine(text);
				return;
			}

			IEnumerable<Dictionary<string, string>> input = environment.Pipe.FilterStandardPipe("include", "exclude");
			foreach (Dictionary<string, string> data in input)
			{
				Dictionary<string, string> eval = EvaluateReferences(references, data);
				if (eval == null)
					continue;

				string txt = text;
				foreach (string rf in eval.Keys)
				{
					txt = txt.Replace("[[" + rf + "]]", eval[rf]);
				}

				Console.WriteLine(txt);
			}
		}

	}
}