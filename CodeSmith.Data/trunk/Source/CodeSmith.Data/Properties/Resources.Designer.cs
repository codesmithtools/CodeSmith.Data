﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.33440
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CodeSmith.Data.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CodeSmith.Data.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Validation Error.
        /// </summary>
        internal static string DefaultValidationErrorMessage {
            get {
                return ResourceManager.GetString("DefaultValidationErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be applied to a(n) {1}..
        /// </summary>
        internal static string InvalidValidatorMessage {
            get {
                return ResourceManager.GetString("InvalidValidatorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} must be {1} to {2}..
        /// </summary>
        internal static string ValidatorCompareMessage {
            get {
                return ResourceManager.GetString("ValidatorCompareMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is not valid..
        /// </summary>
        internal static string ValidatorDefaultMessage {
            get {
                return ResourceManager.GetString("ValidatorDefaultMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} must be of length {1} - {2}..
        /// </summary>
        internal static string ValidatorLengthMessage {
            get {
                return ResourceManager.GetString("ValidatorLengthMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be empty..
        /// </summary>
        internal static string ValidatorNotEmptyMessage {
            get {
                return ResourceManager.GetString("ValidatorNotEmptyMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} cannot be null..
        /// </summary>
        internal static string ValidatorNotNullMessage {
            get {
                return ResourceManager.GetString("ValidatorNotNullMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} must fall between {1} and {2}..
        /// </summary>
        internal static string ValidatorRangeMessage {
            get {
                return ResourceManager.GetString("ValidatorRangeMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} must match the expression {1}..
        /// </summary>
        internal static string ValidatorRegexMessage {
            get {
                return ResourceManager.GetString("ValidatorRegexMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is a required field..
        /// </summary>
        internal static string ValidatorRequiredMessage {
            get {
                return ResourceManager.GetString("ValidatorRequiredMessage", resourceCulture);
            }
        }
    }
}
