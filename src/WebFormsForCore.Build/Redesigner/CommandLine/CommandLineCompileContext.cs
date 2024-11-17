//-------------------------------------------------------------------------------------------------
//
//  Redesigner
//
//  Copyright (c) 2012-3 by Sean Werkema
//  All rights reserved.
//
//  This software is released under the terms of the "New BSD License," as follows:
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted
//  provided that the following conditions are met:
//
//   * Redistributions of source code must retain the above copyright notice, this list of
//     conditions and the following disclaimer.
//
//   * Redistributions in binary form must reproduce the above copyright notice, this list of
//     conditions and the following disclaimer in the documentation and/or other materials
//     provided with the distribution.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
//  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
//  AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
//  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
//  OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//
//-------------------------------------------------------------------------------------------------

using System;

using Redesigner.Library;

namespace Redesigner.CommandLine
{
	public class CommandLineCompileContext : ICompileContext
	{
		private readonly CommandLineArguments _commandLineArguments;

		private readonly string _programName;

		private int _filenameCount;

		public CommandLineCompileContext(string programName, CommandLineArguments commandLineArguments)
		{
			_commandLineArguments = commandLineArguments;
			_programName = programName;
		}

		public void BeginTask(int filenameCount)
		{
			_filenameCount = filenameCount;
		}

		/// <summary>
		/// If we are compiling more than one file, and we're not in verbose mode, display the name of
		/// the file that we are compiling.
		/// </summary>
		/// <param name="filename">The name of the file to display.</param>
		public void BeginFile(string filename)
		{
			if (_filenameCount > 1 && !_commandLineArguments.Verbose && !_commandLineArguments.Quiet)
			{
				Console.WriteLine(filename + "...");
			}
		}

		/// <summary>
		/// Called to notify us when the current file has finished processing.
		/// </summary>
		/// <param name="filename">The name of the file we just finished.</param>
		/// <param name="succeeded">Whether processing that file succeeded.</param>
		public void EndFile(string filename, bool succeeded)
		{
			if (!succeeded)
			{
				Console.WriteLine(string.Empty);
			}
		}

		/// <summary>
		/// How many hyphens to add before verbose messages.
		/// </summary>
		public int VerboseNesting { get; set; }

		/// <summary>
		/// Show a message when --verbose is turned on.
		/// </summary>
		public void Verbose(string format, params object[] args)
		{
			if (!_commandLineArguments.Verbose || _commandLineArguments.Quiet) return;

			if (string.IsNullOrEmpty(format))
			{
				Console.WriteLine(string.Empty);
			}
			else
			{
				string message = string.Format(format, args);
				Console.WriteLine(string.Format("{0} {1}", Common.RepeatString("-", VerboseNesting + 1), message));
			}
		}

		/// <summary>
		/// Show a warning message, unless --quiet is turned on.
		/// </summary>
		public void Warning(string format, params object[] args)
		{
			if (_commandLineArguments.Quiet) return;

			string message = string.Format(format, args);
			Console.WriteLine(_programName + ": Warning: " + message);
		}

		/// <summary>
		/// Show an error message.
		/// </summary>
		public void Error(string format, params object[] args)
		{
			if (_commandLineArguments.Verbose)
			{
				Console.WriteLine("");
				Console.WriteLine("**********");
			}

			string message = string.Format(format, args);
			Console.WriteLine(_programName + ": " + message);

			if (_commandLineArguments.Verbose)
			{
				Console.WriteLine("**********");
				Console.WriteLine("");
			}
		}
	}
}