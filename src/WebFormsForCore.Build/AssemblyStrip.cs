using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace EstrellasDeEsperanza.WebFormsForCore.Build;

public class AssemblyStrip: Task
{
	public ITaskItem Path { get; set; }

	public override bool Execute()
	{
		try
		{
			if (Path == null || string.IsNullOrEmpty(Path.ItemSpec) || !System.IO.Directory.Exists(Path.ItemSpec))
			{
				Log.LogError("Invalid or missing Path item.");
				return false;
			}
			var stripper = new AssemblyStripper();
			stripper.LogWarning = message => Log.LogWarning(message);
			stripper.LogMessage = message => Log.LogMessage(MessageImportance.High, message);
			stripper.StripPath(Path.ItemSpec);
			return true;
		}
		catch (Exception ex)
		{
			Log.LogErrorFromException(ex);
			return false;
		}
	}
}
