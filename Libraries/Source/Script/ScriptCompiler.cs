#region License

// Copyright (c) 2005-2014, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

namespace Script
{
    #region Usings ...

    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;

    using Microsoft.CSharp;

    #endregion

    /// <summary>
    /// </summary>
    public class ScriptCompiler
    {
        #region Fields

        /// <summary>
        /// </summary>
        private readonly CodeDomProvider compiler =
            new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });

        /// <summary>
        /// </summary>
        private readonly List<Assembly> multipleDllList = new List<Assembly>();

        /// <summary>
        /// </summary>
        private readonly CompilerParameters p = new CompilerParameters
                                                {
                                                    GenerateInMemory = false, 
                                                    GenerateExecutable = false, 
                                                    IncludeDebugInformation = true, 
                                                    OutputAssembly = "Scripts.dll", 

                                                    // TODO: Figure out how to parse the file and return the usings, then load those.
                                                    ReferencedAssemblies =
                                                    {
                                                        "System.dll", 
                                                        "System.Core.dll", 
                                                        "System.Data.dll", 
                                                        "System.Windows.Forms.dll", 
                                                        Path.Combine(
                                                            Path
                                                            .GetDirectoryName(
                                                                Application
                                                            .ExecutablePath), 
                                                            "WeifenLuo.WinFormsUI.Docking.dll"), 
                                                        Path.Combine(
                                                            Path
                                                            .GetDirectoryName(
                                                                Application
                                                            .ExecutablePath), 
                                                            "SmokeLounge.AOtomation.Messaging.dll")
                                                    }, 
                                                    TreatWarningsAsErrors = false, 
                                                    WarningLevel = 3, 
                                                    CompilerOptions = "/optimize"
                                                };

        /// <summary>
        /// </summary>
        private readonly Dictionary<string, Type> scriptList = new Dictionary<string, Type>();

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        private string[] ScriptsList { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="scriptName">
        /// </param>
        /// <returns>
        /// </returns>
        public static string DllName(string scriptName)
        {
            scriptName = RemoveCharactersAfterChar(scriptName, '.');
            scriptName = RemoveCharactersBeforeChar(scriptName, '\\');
            scriptName = RemoveCharactersBeforeChar(scriptName, '/');

            return scriptName + ".dll";
        }

        /// <summary>
        /// Removes all text from a string
        /// after char chars
        /// </summary>
        /// <param name="hayStack">
        /// The string to trim.
        /// </param>
        /// <param name="needle">
        /// The char to remove all text after EX: '.'
        /// </param>
        /// <returns>
        /// The corrected string.
        /// </returns>
        public static string RemoveCharactersAfterChar(string hayStack, char needle)
        {
            string input = hayStack;
            int index = input.IndexOf(needle);
            if (index > 0)
            {
                input = input.Substring(0, index);
            }

            return input;
        }

        /// <summary>
        /// Remove all text in a string before
        /// the first chars it finds.
        /// If chars is '\\' then 
        /// Debug\\Scripts turns into Scripts
        /// </summary>
        /// <param name="hayStack">
        /// The string to trim the front of.
        /// </param>
        /// <param name="needle">
        /// The first text char in the string 
        /// that matches this, and everything before it will be removed.
        /// </param>
        /// <returns>
        /// The corrected string.
        /// </returns>
        public static string RemoveCharactersBeforeChar(string hayStack, char needle)
        {
            string input = hayStack;
            int index = input.IndexOf(needle);
            if (index >= 0)
            {
                return input.Substring(index + 1);
            }

            // Hmm if we got here then it has no .'s in it so just return input
            return input;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public int AddScriptMembers()
        {
            this.scriptList.Clear();
            foreach (Assembly assembly in this.multipleDllList)
            {
                foreach (Type t in assembly.GetTypes())
                {
                    // returns all public types in the asembly
                    foreach (Type iface in t.GetInterfaces())
                    {
                        if (iface.FullName == typeof(IAOToolerScript).FullName)
                        {
                            if (t.Name != "IAOToolerScript")
                            {
                                foreach (MemberInfo mi in t.GetMembers())
                                {
                                    if ((mi.Name == "GetType") || (mi.Name == ".ctor") || (mi.Name == "GetHashCode")
                                        || (mi.Name == "ToString") || (mi.Name == "Equals"))
                                    {
                                        continue;
                                    }

                                    if (mi.MemberType == MemberTypes.Method)
                                    {
                                        if (!this.scriptList.ContainsKey(t.Namespace + "." + t.Name + ":" + mi.Name))
                                        {
                                            this.scriptList.Add(t.Namespace + "." + t.Name + ":" + mi.Name, t);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return this.scriptList.Count;
        }

        /// <summary>
        /// </summary>
        /// <param name="multipleFiles">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Compile(bool multipleFiles)
        {
            string[] scriptFolders = Directory.GetDirectories("Scripts", "*.*", SearchOption.TopDirectoryOnly);

            foreach (string scriptFolder in scriptFolders)
            {
                if (!this.LoadFiles(scriptFolder))
                {
                    MessageBox.Show("Could not load files in folder " + scriptFolder);
                    continue;
                }

                if (multipleFiles)
                {
                    // MessageBox.Show("ScriptCompiler: multiple scripts configuration active.");
                    this.p.OutputAssembly = string.Format(
                        CultureInfo.CurrentCulture, 
                        Path.Combine("tmp", DllName(scriptFolder)));

                    // CreateIM the directory if it doesnt exist
                    FileInfo file = new FileInfo(Path.Combine("tmp", DllName(scriptFolder)));
                    if (file.Directory != null)
                    {
                        file.Directory.Create();
                    }

                    // Now compile the dll's
                    CompilerResults results = this.compiler.CompileAssemblyFromFile(this.p, this.ScriptsList);

                    // And check for errors
                    if (ErrorReporting(results).Length != 0)
                    {
                        // We have errors, display them
                        MessageBox.Show("Error: " + ErrorReporting(results));
                        return false;
                    }

                    // Add the compiled assembly to our list
                    this.multipleDllList.Add(Assembly.LoadFile(file.FullName));

                    // Ok all good, load em
                    foreach (Assembly a in this.multipleDllList)
                    {
                        RunScript(a);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="disposing">
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.compiler.Dispose();
            }
        }

        /// <summary>
        /// Our Error reporting method.
        /// </summary>
        /// <param name="results">
        /// </param>
        /// <returns>
        /// </returns>
        private static string ErrorReporting(CompilerResults results)
        {
            StringBuilder report = new StringBuilder();
            if (results.Errors.HasErrors)
            {
                // Count the errors and return them

                int count = results.Errors.Count;
                for (int i = 0; i < count; i++)
                {
                    report.Append(results.Errors[i].FileName);
                    report.AppendLine(
                        " In Line: " + results.Errors[i].Line + " Error: " + results.Errors[i].ErrorNumber + " "
                        + results.Errors[i].ErrorText);
                }
            }

            return report.ToString();
        }

        /// <summary>
        /// Loads all classes contained in our
        /// Assembly file that publically inherit
        /// our IAOScript class.
        /// Entry point for each script is public void Main(string[] args){}
        /// </summary>
        /// <param name="script">
        /// Our .NET dll or exe file.
        /// </param>
        private static void RunScript(Assembly script)
        {
            // Now that we have a compiled script, lets run them
            foreach (Type type in script.GetExportedTypes())
            {
                // returns all public types in the asembly
                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface.FullName == typeof(IAOToolerScript).FullName)
                    {
                        // yay, we found a script interface, lets create it and run it!
                        // Get the constructor for the current type
                        // you can also specify what creation parameter types you want to pass to it,
                        // so you could possibly pass in data it might need, or a class that it can use to query the host application
                        ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                        if (constructor != null && constructor.IsPublic)
                        {
                            // lets be friendly and only do things legitimitely by only using valid constructors

                            // we specified that we wanted a constructor that doesn't take parameters, so don't pass parameters
                            IAOToolerScript scriptObject = constructor.Invoke(null) as IAOToolerScript;
                            if (scriptObject != null)
                            {
                                // Lets run our script and display its results
                                scriptObject.Initialize(null);
                            }
                            else
                            {
                                // hmmm, for some reason it didn't create the object
                                // this shouldn't happen, as we have been doing checks all along, but we should
                                // inform the user something bad has happened, and possibly request them to send
                                // you the script so you can debug this problem
                                MessageBox.Show("Error: Script not loaded.");
                            }
                        }
                        else
                        {
                            // and even more friendly and explain that there was no valid constructor
                            // found and thats why this script object wasn't run
                            MessageBox.Show("Error: No valid constructor found.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If the Scripts directory is empty
        /// or the Scripts directory is missing
        /// Give the correct error.
        /// </summary>
        /// <param name="subFolder">
        /// </param>
        /// <returns>
        /// true if the Scripts directory exsits, and there is at least one script in it.
        /// </returns>
        private bool LoadFiles(string subFolder)
        {
            // Seperated like this, because i want to display different custom errors.
            try
            {
                this.ScriptsList = Directory.GetFiles(subFolder, "*.cs", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("Error: Scripts directory does not exist!");
                return false;
            }
            catch (PathTooLongException)
            {
                MessageBox.Show("Error: Path name is too long");
                return false;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Error: Path is zero length or has invalid chars");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Error: You don't have permission to access this directory");
                return false;
            }
            catch (IOException)
            {
                MessageBox.Show("Error: I/O Error occured. (Path is filename or network error)");
                return false;
            }

            if (this.ScriptsList.Length == 0)
            {
                MessageBox.Show("Error: Scripts directory contains no scripts!");
                return false;
            }

            return true;
        }

        #endregion
    }
}