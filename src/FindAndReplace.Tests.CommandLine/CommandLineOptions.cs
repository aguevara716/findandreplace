using System;
using CommandLine;
using CommandLine.Text;

namespace FindAndReplace.Tests.CommandLine
{
    public class CommandLineOptions
	{
		[ParserState]
		public IParserState LastParserState { get; set; }

		[Option("testVal", Required = true, HelpText = "Test value to parse.")]
		public string TestValue { get; set; }

		[Option("hasRegEx", HelpText = "Is Regex.")]
		public bool HasRegEx { get; set; }

		[Option("useEscapeChars", HelpText = "Use Escape Chars.")]
		public bool UseEscapeChars { get; set; }

		[Option("skipDecoding", HelpText = "Skip calling DecodeText.")]
		public bool SkipDecoding { get; set; }

		[Option("someFlag", HelpText = "Not used. For testing.")]
		public bool SomeFlag { get; set; }

		[HelpOption("help", HelpText = "Display this help screen.")]
		public string GetUsage()
		{
			var help = new HelpText("FindAndReplace.Tests.CommandLine")
			{
				Copyright = new CopyrightInfo("ENTech Solutions", DateTime.Now.Year)
			};

			if (this.LastParserState != null && this.LastParserState.Errors.Count > 0)
			{
				HandleParsingErrorsInHelp(help);
			}


			return help;
		}

		private void HandleParsingErrorsInHelp(HelpText help)
		{
			var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces
			if (!string.IsNullOrEmpty(errors))
			{
				help.MaximumDisplayWidth = 160;
				help.AddPreOptionsLine(string.Concat("\n", "ERROR(S):"));
				help.AddPreOptionsLine(errors);
			}
		}
	}
}
