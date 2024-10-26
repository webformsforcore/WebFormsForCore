using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WebFormsForCore.Build
{
	public class CreateAspDesignerFiles
	{

		public void CreateCodeFromAsp(string file)
		{
			var info = new FileInfo(file);
			if (info.Exists)
			{
				var ext = Path.GetExtension(file);
				var csFile = file + ".cs";
				var vbFile = file + ".vb";
				var isCS = File.Exists(csFile);
				var isVB = File.Exists(vbFile);
				if (isCS || isVB)
				{
					var codeExt = isCS ? ".cs" : ".vb";
					var codeFile = file + codeExt;
					var designerFile = $"{file}.Designer{codeExt}";

					if (File.GetLastWriteTimeUtc(designerFile) > info.LastWriteTimeUtc)
						return; // designer file is up to date

					// Generate designer file


				}

			}
		}
	}
}
