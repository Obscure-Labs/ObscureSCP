using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.CompilerServices.SymbolWriter;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using LabApi.Features.Console;

namespace ObscureLabs.API.Features
{
    public unsafe class ModulesManager
    {
        private List<Module> _moduleList = new();
        //public Module*[] _postStartModules = new Module*[1];
        public List<Module> Modules => _moduleList;

        public List<FileInfo> ModuleFiles { get; private set; } = new List<FileInfo>();

        public void AddModule(Module* module)
        {
            if (module != null)
            {
                _moduleList.Add(*module);
            }
        }

        public void AddModule(Module module)
        {
            _moduleList.Add(module);
        }

        public void AddModules(Module*[] modules)
        {
            foreach (var module in modules)
            {
                AddModule(module);
            }
        }

        public void AddModules(List<Module> modules)
        {
            _moduleList.AddRange(modules);
        }

        public Module GetModule(string name)
        {
            if(_moduleList.FirstOrDefault(x => x.Name.ToLower() == name.ToLower()) is var module && module != null)
            {
                return module;
            }
            //else
            //{
            //    foreach(var mod in _moduleList)
            //    {
            //        Logger.Info($"Checking module: {mod.Name}");
            //        if (mod != null && mod.Name.ToLower() == name.ToLower())
            //        {
            //            return &mod;
            //        }
            //    }
            //    //for (int i = 0; i < _postStartModules.Length; i++)
            //    //{
            //    //    Logger.Info($"Checking module: {_postStartModules[i]->Name}");
            //    //    if (_postStartModules[i] != null && _postStartModules[i]->Name.ToLower() == name.ToLower())
            //    //    {
            //    //        return _postStartModules[i];
            //    //    }
            //    //}
            //    return null;
            //}
            return null;
        }

        public void Clear()
        {
            _moduleList.Clear();
        }

        public void DelModule(string name)
        {
            _moduleList.Remove(_moduleList.FirstOrDefault(x => x.Name.ToLower() == name.ToLower()));
        }

        public void ReloadModule(string moduleName)
        {
            var module = GetModule(moduleName);
            if (module == null)
            {
                LabApi.Features.Console.Logger.Error($"Module {moduleName} not found.");
                return;
            }

            module.Disable();

            if (ModuleFiles.FirstOrDefault(x => x.Name.Contains(moduleName)) is var file && file != null )
            {
                LabApi.Features.Console.Logger.Info($"New module found: {file.Name}. Attempting Compilation...");
                string outputFile = $"{Plugin.SpireConfigLocation}/Modules/Compiled/{file.Name.Replace(".cs", ".dll")}";
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                var pars = new CompilerParameters()
                {
                    GenerateExecutable = false,
                    OutputAssembly = outputFile,
                    GenerateInMemory = false
                };

                pars.ReferencedAssemblies.Add("System.dll");
                pars.ReferencedAssemblies.Add("System.Core.dll");
                pars.ReferencedAssemblies.Add("/home/container/SCPSL_Data/Managed/LabApi.dll");
                foreach (var f in Directory.EnumerateFiles(
                             Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                             "/EXILED/Plugins/"))
                {
                    pars.ReferencedAssemblies.Add(f);
                }
                foreach (var f in Directory.EnumerateFiles(
                             Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                             "/EXILED/Plugins/dependencies/"))
                {
                    pars.ReferencedAssemblies.Add(f);
                }

                CompilerResults results = provider.CompileAssemblyFromFile(pars, file.FullName);

                if (results.Errors.Count > 0)
                {
                    LabApi.Features.Console.Logger.Error($"Failed to compile module {file.Name}");
                    foreach (CompilerError error in results.Errors)
                    {
                        LabApi.Features.Console.Logger.Error($"Error: {error.ErrorText} at line {error.Line}");
                    }
                }
                else
                {
                    LabApi.Features.Console.Logger.Info($"Module compiled successfully: {file.Name}");
                    Assembly assembly = Assembly.LoadFile(outputFile);
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(Module)))
                        {
                            Module mod = (Module)assembly.CreateInstance(type.FullName);
                            AddModule(mod);
                        }
                    }
                }
            }
        }

        public void RefreshModuleFolder()
        {
            var files = Directory.EnumerateFiles($"{Plugin.SpireConfigLocation}/Modules/");
            List<FileInfo> tempModuleFiles = new List<FileInfo>();
            foreach(var file in files)
            {
                if (file.EndsWith(".cs"))
                {
                    tempModuleFiles.Add(new FileInfo(file));
                }
            }
            if (tempModuleFiles.Count != ModuleFiles.Count)
            {
                foreach (var file in tempModuleFiles)
                {
                    if (!ModuleFiles.Contains(file))
                    {
                        LabApi.Features.Console.Logger.Info($"New module found: {file.Name}. Attempting Compilation...");
                        string outputFile = $"{Plugin.SpireConfigLocation}/Modules/Compiled/{file.Name.Replace(".cs", ".dll")}";
                        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                        var pars = new CompilerParameters()
                        {
                            GenerateExecutable = false,
                            OutputAssembly = outputFile,
                            GenerateInMemory = false
                        };

                        pars.ReferencedAssemblies.Add("System.dll");
                        pars.ReferencedAssemblies.Add("System.Core.dll");
                        pars.ReferencedAssemblies.Add("/home/container/SCPSL_Data/Managed/LabApi.dll");
                        foreach (var f in Directory.EnumerateFiles(
                                     Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                     "/EXILED/Plugins/"))
                        {
                            pars.ReferencedAssemblies.Add(f);
                        }
                        foreach (var f in Directory.EnumerateFiles(
                                     Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                     "/EXILED/Plugins/dependencies/"))
                        {
                            pars.ReferencedAssemblies.Add(f);
                        }

                        CompilerResults results = provider.CompileAssemblyFromFile(pars, file.FullName);

                        if(results.Errors.Count > 0)
                        {
                            LabApi.Features.Console.Logger.Error($"Failed to compile module {file.Name}");
                            foreach (CompilerError error in results.Errors)
                            {
                                LabApi.Features.Console.Logger.Error($"Error: {error.ErrorText} at line {error.Line}");
                            }
                        }
                        else
                        {
                            LabApi.Features.Console.Logger.Info($"Module compiled successfully: {file.Name}");
                            Assembly assembly = Assembly.LoadFile(outputFile);
                            foreach (var type in assembly.GetTypes())
                            {
                                if (type.IsSubclassOf(typeof(Module)))
                                {
                                    Module module = (Module)assembly.CreateInstance(type.FullName);
                                    AddModule(module);
                                }
                            }
                        }
                    }
                }
                foreach (var file in ModuleFiles)
                {
                    if (!tempModuleFiles.Contains(file))
                    {
                        LabApi.Features.Console.Logger.Info($"Module removed: {file.Name}");
                        _moduleList.Remove(_moduleList.FirstOrDefault(x => x.Name == file.Name.Replace(".cs", "")));
                    }
                }
                ModuleFiles = tempModuleFiles;
            }
            else
            {
                ModuleFiles = tempModuleFiles;
            }
        }
    }
}
