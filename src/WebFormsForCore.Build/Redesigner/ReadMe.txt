
Redesigner

  Version 1.1

  Copyright (c) 2012-3 by Sean Werkema
  All rights reserved.

-------------------------------------------------------------------------------------------------

OVERVIEW

  ASP.NET is a powerful website development framework, but the .designer files it requires can
  be a pain to work with. They can often get broken, corrupted, or out-of-sync with the markup,
  leading to complicated problems that are often difficult to resolve.

  Redesigner is a command-line tool that can read and parse .aspx and .ascx files and generate
  .designer files for them. It runs quickly and reliably, and can display lots of useful
  information about how the .designer file was generated. It does not use or require Visual
  Studio, either!

  It has an open-source license (the New BSD license) and can be reused anywhere for nearly any
  purpose, for free!

-------------------------------------------------------------------------------------------------

USAGE

  Redesigner is a command-line tool, and is invoked like this:
  
    Redesigner.exe [-w website.dll] [-r path\to\root\of\website] [options] files.aspx ...

  A simple use to regenerate a .designer file for MyPage.aspx might look like this:

    Redesigner.exe -w MyWebsite\bin\MyWebsite.dll -r MyWebsite MyWebsite\MyPage.aspx

  Note:  Your website *must* be able to be compiled!  Redesigner *must* be able to
  reflect against your website's DLL, so if you can't compile your website's DLL, do whatever
  you have to do to hack your website until you can get it to compile.
	
  Currently, Redesigner can generate .designer.cs files for .aspx and .ascx files.  It can
  handle <%@ Register %> directives found in markup, and registered namespaces and user
  controls found in your website's web.config.  It can handle server controls, user controls,
  and HTML controls, data-bound controls and template controls, and complex nested control
  properties.  It uses the same parsing regular expressions ASP.NET uses to ensure that it
  will analyze your markup the same way that ASP.NET will.
  
  Redesigner can process whole directories of .ascx and .aspx files simply by specifying a
  directory name instead of just a .aspx or .ascx filename.  It will recursively search the
  directory for all .ascx and .aspx files contained therein and will process all of them in a
  batch.

GENERATE MODE

  In generate mode (the default mode), Redesigner will recreate one or more .designer files for
  the given .aspx or .ascx input file.  Any existing .designer file will be replaced with one
  that Redesigner generates based on the control declarations found in the markup.  The
  formatting and structure matches Visual Studio's, and should be readable by Visual Studio.
  
  Regenerating a .designer file for MyPage.aspx might look like this:

    Redesigner.exe -w MyWebsite\bin\MyWebsite.dll -r MyWebsite MyWebsite\MyPage.aspx
  
  Regenerating .designer files for a whole directory tree might look like this:

    Redesigner.exe -w MyWebsite\bin\MyWebsite.dll -r MyWebsite MyWebsite
  
  Generate mode will display detailed error messages when it finds an error while parsing and
  analyzing a .aspx or .ascx file.  In addition, it will also set Redesigner's exit code to
  describe the overall state of the generation task (see EXIT CODES below).

VERIFY MODE
  
  In verify mode, turned on with the --verify option, Redesigner will attempt to determine
  whether existing .designer files are "correct" for their markup.  It checks the layout of the
  file, whether the types match, whether all the control properties exist and are in the right
  order, and even whether the curly braces are in the right places (relative to how Visual Studio
  expects them).

  A simple use to validate an existing .designer file for MyPage.aspx might look like this:

    Redesigner.exe --verify -w MyWebsite\bin\MyWebsite.dll -r MyWebsite MyWebsite\MyPage.aspx

  Validating an entire directory tree of .designer files might look like this:
	
    Redesigner.exe --verify -w MyWebsite\bin\MyWebsite.dll -r MyWebsite MyWebsite

  Verify mode will display detailed error messages when it finds an error in a .designer file.
  In addition, it will also set Redesigner's exit code to describe the overall state of the
  validation (see EXIT CODES below).
	
VERBOSE OPTION
  
  If you want detailed information about what Redesigner is doing, you can add the --verbose
  option to the command-line.  This will cause Redesigner to print out a huge quantity of
  text describing what it found in your markup, where it found the controls while reflecting,
  and lots of useful information that may be valuable when tracking down errors in your markup.
  The --verbose option prints a lot of text, but that's better than just saying "Exception of
  type System.Exception was thrown", isn't it? :-)
  
QUIET OPTION

  Redesigner can generate a lot of output.  If you want it to only display text when it
  encounters an error, and to omit all other text, add the --quiet option to the command-line.

EXIT CODES

  Redesigner attempts to be helpful by setting its exit code to a meaningful value.  The exit
  codes it uses are as follows:
  
    0:   Success: All tasks completed successfully.
	
    1:   Nothing to do: No .aspx or .ascx files were given on the command-line.
    2:   Help: The user requested help via the -h or --help command-line options.
    3:   Command-line error: The command-line is not well-formed or has incompatible options.
	
    -1:  Failed generation: At least one of the .designer files that was to be generated failed
          because of errors in the markup.
    -2:  Failed validation: At least one of the .designer files that was to be verified was
          either broken, internally inconsistent, or failed to match its associated markup.
    -3:  Redesigner encountered an internal processing error and was forced to stop.
	
  Note that in some environments, the negative exit codes may become large positive numbers (for
  example, MS-DOS would turn -1 into 255).  To ensure scripts behave correctly, it is safest
  to test only that the exit code was zero (success) or nonzero (failure).
  
-------------------------------------------------------------------------------------------------

OPTIONS

  --help            Show help on usage and command-line options.

  --website website.dll
  -w website.dll    The path to the DLL that contains the website's
                    code-behind. [required]

  --root pathname
  -r pathname       Specify the path to the website's root, where the web.config
                    file is located. [required]

  --verify          Verify that the existing designer file(s) were
                    generated correctly.

  --generate        Generate replacement designer file(s) for the given
                    pages/controls. [default]

  --verbose         Turn on verbose logging of page processing, which can help
                    locate and diagnose errors.

  --quiet           Turn off all messages except errors.

-------------------------------------------------------------------------------------------------

TODO
  
  Future plans include:
  
    * Support for reading and processing .master files.

    * Generating and verifying .designer.vb files (not just .designer.cs files).

    * Creating a Redesigner.dll that can be reused in other applications.
	
    * Creating a right-click plugin for Visual Studio to allow Redesigner to generate and verify
       .designer files for individual pages as well as folders.
	   
    * Creating a "Repair-If-Needed" mode that regenerates .designer files only for those that
       fail verification.
	   
-------------------------------------------------------------------------------------------------

BUGS

    * Redesigner does not currently support controls that do clever things with ControlBuilder
       classes, such as using them to switch "obvious" types via GetChildControlType().  Any
       custom controls that heavily use ControlBuilder may mis-compile.

    * Controls that have been moved from the .designer file to the code-behind will be
       considered "missing" in verify mode, and will be regenerated in the .designer file in
       generate-mode.  Redesigner has no way to know that the required control declarations
       were moved, and adding support for this is somewhere between "challenging" and
       "impossible".

    * Redesigner currently requires your website to be a compiled DLL.  It is theoretically
       possible to generate and verify using only the website's source code, but it's a
       nontrivial task to parse it on the fly to extract the class declarations and property
       names.  Support for this is possible but not currently planned.
	   
-------------------------------------------------------------------------------------------------

LICENSE

  This software is released under the terms of the "New BSD License," as follows:

  Redistribution and use in source and binary forms, with or without modification, are permitted
  provided that the following conditions are met:

   * Redistributions of source code must retain the above copyright notice, this list of
     conditions and the following disclaimer.

   * Redistributions in binary form must reproduce the above copyright notice, this list of
     conditions and the following disclaimer in the documentation and/or other materials
     provided with the distribution.

  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
  AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
  OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
  POSSIBILITY OF SUCH DAMAGE.
