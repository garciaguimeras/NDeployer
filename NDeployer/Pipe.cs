using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NDeployer
{

	[Serializable]
    class Pipe
    {

		List<Dictionary<string, string>> std;

		public List<Dictionary<string, string>> Std { get { return std; } }

        public Pipe()
        {
			std = new List<Dictionary<string, string>>();
        }

		public Pipe(List<Dictionary<string, string>> initialStd)
		{
			std = initialStd;
		}

		public Pipe Clone()
		{
			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();
			using (stream)
			{
				formatter.Serialize(stream, this);
				stream.Seek(0, SeekOrigin.Begin);
				return (Pipe)formatter.Deserialize(stream);
			}
		}

		public void AddToStandardPipe(Dictionary<string, string> data)
        {
            std.Add(data);
        }

		public List<Dictionary<string, string>> FilterStandardPipe(string excludeKey)
		{
			List<Dictionary<string, string>> included = new List<Dictionary<string, string>>();
			foreach (Dictionary<string, string> data in Std)
			{
				if (!data.ContainsKey(excludeKey))
					included.Add(data);
			}
			return included;
		}

    }
}
