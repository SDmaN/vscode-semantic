﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CompillerServices.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CompillerServices.Resources.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not find directory {0}..
        /// </summary>
        internal static string CouldNotFindDirectory {
            get {
                return ResourceManager.GetString("CouldNotFindDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to #include &quot;###.h&quot;
        ///using namespace ###;
        ///
        ///int main(int argc, char* argv[])
        ///{
        ///    start();
        ///    return 0;
        ///}.
        /// </summary>
        internal static string MainContent {
            get {
                return ResourceManager.GetString("MainContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Main module file {0} not found..
        /// </summary>
        internal static string MainModuleFileNotFound {
            get {
                return ResourceManager.GetString("MainModuleFileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Main module is not specified in project file..
        /// </summary>
        internal static string MainModuleNotSpecified {
            get {
                return ResourceManager.GetString("MainModuleNotSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module name {0} does not match file {1}..
        /// </summary>
        internal static string ModuleDoesNotMatchFile {
            get {
                return ResourceManager.GetString("ModuleDoesNotMatchFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Module {0} is not declared..
        /// </summary>
        internal static string ModuleIsNotDeclared {
            get {
                return ResourceManager.GetString("ModuleIsNotDeclared", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project file does not exists in directory {0}.
        /// </summary>
        internal static string ProjectFileNotFound {
            get {
                return ResourceManager.GetString("ProjectFileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Project file parsing error occurred..
        /// </summary>
        internal static string ProjectFileParsingException {
            get {
                return ResourceManager.GetString("ProjectFileParsingException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The directory must contain only one project file..
        /// </summary>
        internal static string TooManyProjectFiles {
            get {
                return ResourceManager.GetString("TooManyProjectFiles", resourceCulture);
            }
        }
    }
}
