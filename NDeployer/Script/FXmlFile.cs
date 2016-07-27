using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using FlatXml.FXml;

namespace NDeployer.Script
{
	class FXmlFile : ScriptFile
	{

		private List<TaskDef> GetTaskDefs(IEnumerable<FXmlElement> elements)
		{
			List<TaskDef> tasks = new List<TaskDef>();
			foreach (FXmlElement e in elements)
			{
				TaskDef t = new TaskDef();
				t.Name = e.Name;
				foreach (string name in e.FXmlAttributes.Keys)
					t.Attributes.Add(name, e.FXmlAttributes[name]);
				t.TaskDefs = GetTaskDefs(e.Children);

				tasks.Add(t);
			}
			return tasks;
		}

		public override TaskDef Parse(string filename)
		{
			List<string> lines =  new List<string>();

			using (StreamReader reader = new StreamReader(File.OpenRead(filename)))
			{
				while (!reader.EndOfStream)
					lines.Add(reader.ReadLine());
			}

			Preprocessor preprocessor = new Preprocessor();
			string code = preprocessor.FixString(lines);

			TokenParser tokenParser = new TokenParser();
			bool result = tokenParser.Parse(code);
			if (!result)
			{
				Console.WriteLine(tokenParser.Error.Text);
				return null;
			}

			LLParser llParser = new LLParser();
			result = llParser.Parse(tokenParser.Tokens);
			if (!result)
			{
				foreach (Error err in llParser.Errors)
					Console.WriteLine(err.Text);
				return null;
			}

			TaskDef root = new TaskDef();
			root.TaskDefs = GetTaskDefs(llParser.FXmlElements);
			return root;
		}

	}
}

