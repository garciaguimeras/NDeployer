using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace NDeployer.Tasks
{
	class ClearPipeTask : Task
	{
		
		public ClearPipeTask(string name) : base(name)
		{}

		public override bool ProcessXml(XElement rootNode)
		{
			return true;
		}

		public override void Execute()
		{
			environment.ClearPipe();
		}

	}
}