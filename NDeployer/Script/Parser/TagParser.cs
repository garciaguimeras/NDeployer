using System;

namespace NDeployer.Script.Parser
{

	enum TagType
	{
		NAME,
		VALUE,
		NEWLINE,
		EQUALS_SIGN,
		BEGIN_BRACKET_SIGN,
		END_BRACKET_SIGN
	}

	class Tag
	{
		public TagType Type { get; set; }
		public string Value { get; set; }
	}

	class TagParser
	{

		public TagParser(string filename)
		{
			
		}

	}

}

