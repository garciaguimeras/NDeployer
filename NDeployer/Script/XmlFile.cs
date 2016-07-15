using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace NDeployer.Script
{
	class XmlFile : ScriptFile
	{

		private XElement GetDocumentRoot(string filename)
		{
			XDocument xDoc = XDocument.Load(filename);
			IEnumerable<XElement> elements = xDoc.Elements("xml");
			if (elements.Count() != 1)
				return null;
			return elements.First();
		}

		public override TaskDef Parse(string filename)
		{
			XElement root = GetDocumentRoot(filename);
			return XmlFileParser.GetRootTaskDef(root);
		}
	}
}

