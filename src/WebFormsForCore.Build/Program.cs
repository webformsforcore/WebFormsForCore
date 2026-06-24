using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Build.Utilities;
using Microsoft.Build;

namespace WebFormsForCore.Build
{
	public class Program
	{
		public static int Main(string[] args)
		{
			if (args.Length == 0) return -2;

			switch (args[0])
			{
				case "fakestrongname":
					var task = new FakeStrongNameTask();
					task.LogToConsole = true;
					task.Assemblies = args[1].Split(';')
						.Select(file => new TaskItem(file))
						.ToArray();
					task.PublicKey = args[2];
					task.PublicKeyToken = args[3];
					task.Key = new TaskItem(args[4]);
					task.Source = new TaskItem(args[4]);
					return task.Execute() ? 0 : -1;
				case "createdesignerfiles":
					var task2 = new CreateAspDesignerFiles();
					task2.LogToConsole = true;
					task2.SeparateProcess = false;
					task2.Assembly = new TaskItem(args[1]);
					task2.Directory = new TaskItem(args[2]);
					task2.Files = args[3].Split(';')
						.Select(file => new TaskItem(file))
						.ToArray();
					return task2.Execute() ? 0 : -1;
				case "aspnetcompile":
					var task3 = new AspNetCoreCompiler();
					task3.PhysicalPath = new TaskItem(args[1]);
					task3.VirtualPath = new TaskItem(args[2]);
					task3.MetabasePath = new TaskItem(args[3]);
					task3.TargetPath = new TaskItem(args[4]);
					task3.BinFolder = new TaskItem(args[5]);
					task3.TargetFramework = new TaskItem(args[6]);
					task3.Clean = bool.Parse(args[7]);
					task3.Force = bool.Parse(args[8]);
					task3.Updateable = bool.Parse(args[9]);
					task3.Debug = bool.Parse(args[10]);
					task3.DelaySing = bool.Parse(args[11]);
					task3.FixedNames = bool.Parse(args[12]);
					task3.KeyFile = new TaskItem(args[13]);
					task3.KeyContainer = new TaskItem(args[14]);
					task3.LogToConsole = true;
					return task3.Execute() ? 0 : -1;
			}
			return -3;
		}
	}
}
