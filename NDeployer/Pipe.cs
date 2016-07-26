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
		List<Dictionary<string, string>> error;

		public IEnumerable<Dictionary<string, string>> Std { get { return std.AsEnumerable(); } }
		public IEnumerable<Dictionary<string, string>> Error { get { return error.AsEnumerable(); } }

        public Pipe()
        {
			std = new List<Dictionary<string, string>>();
			error = new List<Dictionary<string, string>>();
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

		public List<Dictionary<string, string>> FilterStandardPipe(string includeKey, string excludeKey)
		{
			List<Dictionary<string, string>> notExcluded = new List<Dictionary<string, string>>();
			List<Dictionary<string, string>> included = new List<Dictionary<string, string>>();
			foreach (Dictionary<string, string> data in Std)
			{
				if (data.ContainsKey(includeKey) && data[includeKey].Equals(""))
					included.Add(data);
				if (!data.ContainsKey(excludeKey) || !data[excludeKey].Equals(""))
					notExcluded.Add(data);
			}
			if (included.Count() > 0)
				return included;
			return notExcluded;
		}

		public void AddToErrorPipe(string text, params string[] extra)
		{
			Dictionary<string, string> data = new Dictionary<string, string> ();
			try
			{
				text = string.Format(text, extra);
			}
			catch (Exception e)
			{}
			data.Add("error", text);
			error.Add(data);
		}

        public void PrintErrorPipe()
        {
			foreach (Dictionary<string, string> data in Error)
            {
				if (data.ContainsKey("error"))
					Logger.error(data["error"]);
            }
        }

    }
}
