﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Xtend.Dac.Rules {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class RuleResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal RuleResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Xtend.Dac.Rules.RuleResources", typeof(RuleResources).Assembly);
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
        ///   Looks up a localized string similar to Can&apos;t create ResourceManager for {0} from {1}..
        /// </summary>
        internal static string CannotCreateResourceManager {
            get {
                return ResourceManager.GetString("CannotCreateResourceManager", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Column {0} has no description.
        /// </summary>
        internal static string ColumnWithoutDescription_ProblemDescription {
            get {
                return ResourceManager.GetString("ColumnWithoutDescription_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All columns should have a description.
        /// </summary>
        internal static string ColumnWithoutDescription_RuleName {
            get {
                return ResourceManager.GetString("ColumnWithoutDescription_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Procedure {0} sets concat_null_yields_null option to off.
        /// </summary>
        internal static string ConcatNullYieldsNullEnabled_ProblemDescription {
            get {
                return ResourceManager.GetString("ConcatNullYieldsNullEnabled_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Avoid setting concat_null_yields_null option to off.
        /// </summary>
        internal static string ConcatNullYieldsNullEnabled_RuleName {
            get {
                return ResourceManager.GetString("ConcatNullYieldsNullEnabled_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Column count of fetch ({1}) does not match cursor &apos;{0}&apos;..
        /// </summary>
        internal static string CursorColumnMismatch_ProblemDescription {
            get {
                return ResourceManager.GetString("CursorColumnMismatch_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Column count mismatch of fetch and cursor.
        /// </summary>
        internal static string CursorColumnMismatch_RuleName {
            get {
                return ResourceManager.GetString("CursorColumnMismatch_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} of cursor &apos;{1}&apos; in procedure {2}.
        /// </summary>
        internal static string CursorMissingStatement_ProblemDescription {
            get {
                return ResourceManager.GetString("CursorMissingStatement_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing or invalid deallocate or open of cursor.
        /// </summary>
        internal static string CursorMissingStatement_RuleName {
            get {
                return ResourceManager.GetString("CursorMissingStatement_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Foreign key {0} was found without index on table {1}.
        /// </summary>
        internal static string ForeignKeyWithoutIndex_ProblemDescription {
            get {
                return ResourceManager.GetString("ForeignKeyWithoutIndex_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All foreign keys should have an index that covers the foreign key.
        /// </summary>
        internal static string ForeignKeyWithoutIndex_RuleName {
            get {
                return ResourceManager.GetString("ForeignKeyWithoutIndex_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Call to {0}.{1} is missing a mandatory parameter. Parameters missing: {2}.
        /// </summary>
        internal static string MissingMandatoryParameter_ProblemDescription {
            get {
                return ResourceManager.GetString("MissingMandatoryParameter_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All mandatory parameters must be filled in.
        /// </summary>
        internal static string MissingMandatoryParameter_RuleName {
            get {
                return ResourceManager.GetString("MissingMandatoryParameter_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Procedure &apos;{0}&apos; queries partitioned table &apos;{1}&apos;, and needs to be constrained on column &apos;{2}&apos;.
        /// </summary>
        internal static string MissingPartitioningColumn_ProblemDescription {
            get {
                return ResourceManager.GetString("MissingPartitioningColumn_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing partitioning constraint.
        /// </summary>
        internal static string MissingPartitioningColumn_RuleName {
            get {
                return ResourceManager.GetString("MissingPartitioningColumn_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Procedure {0} does not set nocount.
        /// </summary>
        internal static string NoExplicitNoCountSet_ProblemDescription {
            get {
                return ResourceManager.GetString("NoExplicitNoCountSet_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All procedures must set nocount.
        /// </summary>
        internal static string NoExplicitNoCountSet_RuleName {
            get {
                return ResourceManager.GetString("NoExplicitNoCountSet_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Procedure {0} does not set xact_abort.
        /// </summary>
        internal static string NoExplicitXActAbortSet_ProblemDescription {
            get {
                return ResourceManager.GetString("NoExplicitXActAbortSet_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All procedures must set xact_abort.
        /// </summary>
        internal static string NoExplicitXActAbortSet_RuleName {
            get {
                return ResourceManager.GetString("NoExplicitXActAbortSet_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid null comparison: {0} ({1}).
        /// </summary>
        internal static string NullBooleanComparison_ProblemDescription {
            get {
                return ResourceManager.GetString("NullBooleanComparison_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid null comparison.
        /// </summary>
        internal static string NullBooleanComparison_RuleName {
            get {
                return ResourceManager.GetString("NullBooleanComparison_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Table {0} has no description.
        /// </summary>
        internal static string TableWithoutDescription_ProblemDescription {
            get {
                return ResourceManager.GetString("TableWithoutDescription_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to All tables should have a description.
        /// </summary>
        internal static string TableWithoutDescription_RuleName {
            get {
                return ResourceManager.GetString("TableWithoutDescription_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter {0} is unused in procedure {1}.
        /// </summary>
        internal static string UnusedParameter_ProblemDescription {
            get {
                return ResourceManager.GetString("UnusedParameter_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No unused parameter may exist.
        /// </summary>
        internal static string UnusedParameter_RuleName {
            get {
                return ResourceManager.GetString("UnusedParameter_RuleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Variable {0} is unused in procedure {1}.
        /// </summary>
        internal static string UnusedVariable_ProblemDescription {
            get {
                return ResourceManager.GetString("UnusedVariable_ProblemDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No unused variables may exist.
        /// </summary>
        internal static string UnusedVariable_RuleName {
            get {
                return ResourceManager.GetString("UnusedVariable_RuleName", resourceCulture);
            }
        }
    }
}
