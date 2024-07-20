using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Mono.Cecil;


namespace EstrellasDeEsperanza.WebFormsCore.Build
{

    public class AssemblyPatcher
    {

        static readonly Dictionary<string, string> mappings = new Dictionary<string, string>(
            new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("System.Web", "WebFormsCore.Web"),
                new KeyValuePair<string, string>("System.Configuration", "WebFormsCore.Configuration"),
                new KeyValuePair<string, string>("System.Web.RegularExpressions", "WebFormsCore.Web.RegularExpressions"),
                new KeyValuePair<string, string>("System.Web.ApplicationServices", "WebFormsCore.Web.ApplicationServices"),
                new KeyValuePair<string, string>("System.Web.Services", "WebFormsCore.Web.Services") });

        static readonly HashSet<string> destinationAssemblyNames = mappings.Values.ToHashSet();

        class DestinationAssemblies : KeyedCollection<string, AssemblyDefinition>
        {
            protected override string GetKeyForItem(AssemblyDefinition a) => a.Name.Name;

            public bool IsWebFormsCore(AssemblyDefinition a) => destinationAssemblyNames.Contains(a.Name.Name);

			protected override void InsertItem(int index, AssemblyDefinition item)
			{
				if (IsWebFormsCore(item)) base.InsertItem(index, item);
			}

			protected override void RemoveItem(int index)
			{
				base.RemoveItem(index);
			}

			protected override void SetItem(int index, AssemblyDefinition item)
			{
				if (IsWebFormsCore(item)) base.SetItem(index, item);
			}
		}

        static readonly DestinationAssemblies destinations = new DestinationAssemblies();

        bool NeedsPatch(AssemblyNameReference r)
        {
            // special case System.Configuration, only map Version below 4
            if (r.Name == "System.Configuration" && r.Version.Major <= 4) return true;

            return mappings.ContainsKey(r.Name);
        }

        AssemblyNameReference PatchAssemblyReference(AssemblyNameReference r, ref bool modified)
        {
            // special case System.Configuration, only map Version below 4
            if (r.Name == "System.Configuration" && r.Version.Major > 4) return r;

            string mappedName;
            if (mappings.TryGetValue(r.Name, out mappedName!))
            {
                modified = true;
                
                return destinations[mappedName];
            }
            else return r;
        }

        public void CreateForwarderAssemblies(IEnumerable<string> files, string signWithKeyFile)
        {
            var assemblies = files.Select(file => new { File = file, Assembly = AssemblyDefinition.ReadAssembly(file) })
                .ToArray();
            /*
            foreach (var ad in assemblies) {
                if (mappings.ContainsKey(ad.Assembly.Name)) {
                    destinations.AddIfWebFormsCore(new AssemblyNameReference(mappings[ad.Assembly.Name];
            */
            foreach (var ad in assemblies)
            {
                if (ad.Assembly.Name.HasPublicKey &&
                    ad.Assembly.Modules
                        .SelectMany(mod => mod.AssemblyReferences)
                        .Any(r => NeedsPatch(r)))
                    // re-sign the assmbly
                    ad.Assembly.Name.HashAlgorithm = AssemblyHashAlgorithm.SHA256;
                }
            // re-sign Assembly


            bool modified = false;
            foreach (var mod in ad.Assembly.Modules)
            {
                for (int i = 0; i < mod.AssemblyReferences.Count; i++)
                {
                    var r = mod.AssemblyReferences[i];
                    mod.AssemblyReferences[i] = PatchAssemblyReference(r, ref modified);
                }

            }

            if (modified)
            {
            }
        }

        public static void CreateForwarderAssembly(string source, string destination)
        {
            var src = AssemblyDefinition.ReadAssembly(source);
            var dest = AssemblyDefinition.ReadAssembly(destination);

            var mod = src.MainModule;

            var srctypes = src.Modules
                .SelectMany(m => m.Types)
                .Where(t => t.IsPublic)
                .ToArray();
            var desttypes = dest.Modules
                .SelectMany(m => m.Types)
                .Where(t => t.IsPublic)
                .ToArray();
            var forwarders = srctypes
                .Join(desttypes, s => s.Name, d => d.Name, (s, d) => d)
                .Select(d =>
                {
                    var constructor = mod.ImportReference(typeof(TypeForwardedToAttribute).GetConstructors().FirstOrDefault());
                    var systemTypeRef = mod.ImportReference(typeof(Type));
                    var customAttribute = new CustomAttribute(constructor);
                    customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(systemTypeRef, mod.ImportReference(d)));
                    return customAttribute;
                })
                .ToArray();

            if (forwarders.Length != srctypes.Length)
            {
                throw new NotSupportedException($"Not all types in {src.Name.Name} can be forwarded to {dest.Name.Name}");
            }

            // remove all modules except main module
            foreach (var m in src.Modules.ToArray())
            {
                if (m != src.MainModule) src.Modules.Remove(m);
            }

            // remove all types
            mod.Types.Clear();
            mod.Resources.Clear();
            mod.CustomAttributes.Clear();

            foreach (var att in forwarders) mod.CustomAttributes.Add(att);
        }
    }
}
