using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
//using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WebFormsForCore.Build
{
    public class AspNetCoreCompiler: Task
    {
        public ITaskItem VirtualPath { get; set; }
        public ITaskItem PhysicalPath { get; set; }
        public ITaskItem TargetPath { get; set; }

        private ITaskItem targetFramework = null;
        public ITaskItem TargetFramework { 
            get => targetFramework ?? TargetFrameworkMoniker;
            set => targetFramework = value;
        }
        public ITaskItem TargetFrameworkMoniker { get; set; } = null;
        public ITaskItem BinFolder { get; set; }

        public bool Force { get; set; }
        public bool Debug { get; set; }
        public bool Clean { get; set; }
        public bool Updateable { get; set; }
        public bool FixedNames { get; set; }
        public ITaskItem KeyFile { get; set; }
        public ITaskItem KeyContainer { get; set; }
        public bool DelaySing { get; set; } = false;
        public bool AllowPartiallyTrustedCallers { get; set; } = true;

        public override bool Execute()
        {
            
        }
    }
}
