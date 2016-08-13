using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using NDeployer.Lang;

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

		public override ModuleInfo Parse(string filename)
		{
			string moduleName = Path.GetFileNameWithoutExtension(filename);
			XElement root = GetDocumentRoot(filename);
			TaskDef rootTask = XmlFileParser.GetRootTaskDef(root);
			return new ModuleInfo(moduleName, rootTask);
		}
	}
}

