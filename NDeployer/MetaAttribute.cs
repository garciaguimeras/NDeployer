using System.Collections.Generic;
using System;

namespace NDeployer
{

	class MetaAttribute
	{
		private Dictionary<string, string> metaAttributes;

		public Dictionary<string, string> MetaAttributes { get { return metaAttributes; } }

		public MetaAttribute()
		{
			metaAttributes = new Dictionary<string, string>();
		}

		public void AddMetaAttribute(string key, string value)
		{
			metaAttributes.Add(key, value);
		}

		public string GetMetaAttribute(string key)
		{
			if (!metaAttributes.ContainsKey(key))
				return null;
			return metaAttributes[key];
		}
	}
}

