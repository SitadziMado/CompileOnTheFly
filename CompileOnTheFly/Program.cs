using System;
using System.Configuration;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileOnTheFly
{
    class Program
    {
        private static string mSrc = @"
function main() {
  return qwer();
};

function qwer() {
  var v = 'Привет из текста!';
  return v;
};
";

        static void Main(string[] args)
        {
            Microsoft.JScript.Vsa.VsaEngine eng = new Microsoft.JScript.Vsa.VsaEngine();
               var assembly = CompileScript(mSrc);

            var fst = assembly.CompiledAssembly.DefinedTypes.First();

            var entry = fst.DeclaredMethods.Skip(1).First();
            var pars = entry.GetParameters();
            Console.WriteLine(entry.Invoke(eng, new object[] { new object(), eng }));

            // Console.WriteLine(assembly.CompiledAssembly.EntryPoint.Invoke(null, new object[] { new string[] { } }));

            /* var cnt = 0;
            
            foreach (var a in CodeDomProvider.GetAllCompilerInfo())
            {
                Console.WriteLine($"{cnt++} compiler langs:");

                foreach (var b in a.GetLanguages())
                    Console.WriteLine($"\t {b}");

                foreach (var b in a.GetExtensions())
                    Console.WriteLine($"\t {b}");
            } */
        }

        private static CompilerResults CompileScript(string script)
        {
            CompilerResults results;
            var lines = script;

            try
            {
                var provider = CodeDomProvider.CreateProvider("js");

                var ps = new CompilerParameters
                {
                    // CompilerOptions = "";
                    GenerateExecutable = true,
                    GenerateInMemory = true,
                    IncludeDebugInformation = true
                };

                results = provider.CompileAssemblyFromSource(ps, lines);

                if (results == null)
                    throw new ArgumentException("Неверный входной скрипт.");

                foreach (var a in results.Errors)
                {
                    Console.WriteLine(a);
                }
            }
            catch (ConfigurationException)
            {
                throw;
            }

            return results;
        }
    }
}
