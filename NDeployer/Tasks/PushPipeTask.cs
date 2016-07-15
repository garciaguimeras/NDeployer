using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace NDeployer.Tasks
{
	class PushPipeTask : Task
	{
		public PushPipeTask(string name) : base(name)
		{}

		public override bool ProcessXml(XElement rootNode)
		{
			return true;
		}

		public override void Execute()
		{
			environment.PushPipe();
		}

	}
}

