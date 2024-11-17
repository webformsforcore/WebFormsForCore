using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build;

namespace EstrellasDeEsperanza.WebFormsForCore.Build
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length == 0) return;

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
					task.Execute();
					break;
				case "createdesignerfiles":
					var task2 = new CreateAspDesignerFiles();
					task2.LogToConsole = true;
					task2.Assembly = new TaskItem(args[1]);
					task2.Directory = new TaskItem(args[2]);
					task2.Files = args[3].Split(';')
						.Select(file => new TaskItem(file))
						.ToArray();
					task2.Execute();
					break;
			}
		}
	}
}
