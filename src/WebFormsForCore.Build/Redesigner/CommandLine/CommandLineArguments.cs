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
using System.Collections.Generic;
using System.IO;

namespace Redesigner.CommandLine
{
	/// <summary>
	/// The complete set of decoded and parsed command-line arguments.
	/// </summary>
	public class CommandLineArguments
	{
		/// <summary>
		/// The list of markup files to process.
		/// </summary>
		public List<string> Filenames = new List<string>();

		/// <summary>
		/// What action the user has asked this program to perform.
		/// </summary>
		public ProgramAction Action = ProgramAction.Generate;

		/// <summary>
		/// The pathname to the directory where the web.config file can be found.  By default, it's ".", the current directory.
		/// </summary>
		public string RootPath = ".";

		/// <summary>
		/// The pathname to the website's DLL.  By default, it's "website.dll" in the current directory.
		/// </summary>
		public string WebsiteDllFileName = "website.dll";

		/// <summary>
		/// Whether to show lots and lots of debugging information while this program is working.
		/// </summary>
		public bool Verbose;

		/// <summary>
		/// Whether to hide verbose messages and warnings, and only show errors.
		/// </summary>
		public bool Quiet;

		/// <summary>
		/// Decode the given command-line arguments.
		/// </summary>
		public CommandLineArguments(IList<string> args)
		{
			for (int i = 0; i < args.Count; i++)
			{
				string arg = args[i];
				if (arg[0] != '-')
				{
					Filenames.Add(arg);
					continue;
				}

				switch (arg[1])
				{
					case 'v':
						Action = ProgramAction.Verify;
						break;

					case 'h':
						Action = ProgramAction.Help;
						break;

					case 'r':
						if (arg.Length <= 2)
						{
							if (i + 1 >= args.Count)
							{
								throw new ArgumentException("Missing pathname for root of website after \"-r\"");
							}
							RootPath = args[++i];
						}
						else
						{
							RootPath = arg.Substring(2);
						}
						break;

					case 'w':
						if (arg.Length <= 2)
						{
							if (i + 1 >= args.Count)
							{
								throw new ArgumentException("Missing filename for \"website.dll\" after \"-w\"");
							}
							WebsiteDllFileName = args[++i];
						}
						else
						{
							WebsiteDllFileName = arg.Substring(2);
						}
						break;

					case '-':
						switch (arg.Substring(2))
						{
							case "verify":
								Action = ProgramAction.Verify;
								break;

							case "help":
								Action = ProgramAction.Help;
								break;

							case "root":
								if (i + 1 >= args.Count)
								{
									throw new ArgumentException("Missing pathname for root of website after \"--root\"");
								}
								RootPath = args[++i];
								break;

							case "website":
								if (i + 1 >= args.Count)
								{
									throw new ArgumentException("Missing filename for \"website.dll\" after \"--website\"");
								}
								WebsiteDllFileName = args[++i];
								break;

							case "verbose":
								Verbose = true;
								break;

							case "quiet":
								Quiet = true;
								break;
						}
						break;
				}
			}

			RootPath = Path.GetFullPath(RootPath);
		}
	}
}
