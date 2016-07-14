using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using NDeployer.Tasks;
using NDeployer.Util;

namespace NDeployer.Tasks
{
	class RootTask : Task
	{

		string filename;

		public RootTask(string filename) : base("xml")
		{
			this.filename = filename;
		}

		private void ReadProperties(XElement root)
		{
			foreach (XElement element in root.Elements("property"))
			{
				TaskFactory.CreateTaskForTag("property").ProcessXml(element);
			}
		}

		private XElement GetDocumentRoot()
		{
			XDocument xDoc = XDocument.Load(filename);
			IEnumerable<XElement> elements = xDoc.Elements("xml");
			if (elements.Count() != 1)
				return null;
			return elements.First();
		}

		public override bool ProcessXml(XElement rootNode)
		{
			if (rootNode == null)
			{
				environment.Pipe.AddToErrorPipe("Invalid build file. Can't find document root");
				return false;
			}

			ReadProperties(rootNode);
			bool result = PropertyEvaluator.EvalAllProperties();
			if (!result)
			{
				environment.Pipe.AddToErrorPipe("Error evaluating properties. Execution suspended.");
				return false;
			}

			return true;
		}

		public override void Execute()
		{
			XElement root = GetDocumentRoot();
			ProcessXml(root);

			if (environment.Pipe.Error.Count() > 0)
			{
				environment.Pipe.PrintErrorPipe();
				return;
			}

			IEnumerable<XElement> elements = root.Elements().Where(e => !e.Name.ToString().Equals("property"));
			ExecuteContext(elements);
		}

	}
}

