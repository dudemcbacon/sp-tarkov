using System;
using System.IO;
using System.Reflection;

namespace EmuTarkov.Launcher
{
    public static class AssemblyLoader
    {
        public static string Filepath { get; private set; }

        public static void Run(string filepath)
        {
            Filepath = filepath;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEvent);
        }

        private static Assembly AssemblyResolveEvent(object sender, ResolveEventArgs args)
        {
            string assembly = new AssemblyName(args.Name).Name;
            string filename = Path.Combine(Environment.CurrentDirectory, Filepath + assembly + ".dll");

            // resources are embedded inside assembly
            if (filename.Contains("resources"))
            {
                return null;
            }

            return Assembly.LoadFrom(filename);
        }
    }
}
