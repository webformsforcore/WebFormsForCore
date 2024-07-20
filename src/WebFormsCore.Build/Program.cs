using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build;

namespace EstrellasDeEsperanza.WebFormsCore.Build
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var task = new FakeStrongName();
			task.LogToConsole = true;
			task.Assemblies = args[0].Split(';')
				.Select(file => new TaskItem(file))
				.ToArray();
			task.PublicKey = args[1];
			task.PublicKeyToken = args[2];
			task.Key = new TaskItem(args[3]);
			task.Source = new TaskItem(args[4]);
			task.Execute();
		}
	}
}
