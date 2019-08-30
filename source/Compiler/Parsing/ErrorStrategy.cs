using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using FluentLang.Compiler.Model.Diagnostic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

/*
* Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
* Use of this file is governed by the BSD 3-clause license that
* can be found in the LICENSE.txt file in the project root.
*/

namespace FluentLang.Compiler.Parsing
{
	/// <summary>
	/// This attempts to build a valid parse tree after an error: See https://github.com/antlr/antlr4/pull/2487/files.
	/// Once that is merged, this can be removed.
	/// </summary>
	public class ErrorStrategy : DefaultErrorStrategy
	{
		public ErrorStrategy(ImmutableList<Diagnostic> diagnostics)
		{
			Diagnostics = diagnostics;
		}

		public ImmutableList<Diagnostic> Diagnostics { get; private set; }

		public override void ReportError(Parser recognizer, RecognitionException e)
		{
			base.ReportError(recognizer, e);
			Diagnostics = Diagnostics.Add(new Diagnostic(new Location(e.OffendingToken), ErrorCode.SyntaxError, ImmutableArray.Create<object>(e.Message)));
		}
	}
}