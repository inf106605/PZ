﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SkyCrabServer.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SkyCrabServer.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to                                              .:/+oo++:.                        
        ///                                         .+hNMMMMMMMMMMNh+.                    
        ///                                       -yMMMMMMMMMMMMMMMMMMh:                  
        ///                                     `yMMMMMMMMMMMMMMMMMMMMMMy`                
        ///                                    .dMMMMMMMMMMMMMMMMMMMMMMMMm-               
        ///                        ./osyhhyso/:mMMMMMMMMMMMMMMMMMMMMMMMMMMm.              
        ///                     .odNM [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string banner {
            get {
                return ResourceManager.GetString("banner", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Generated by Oracle SQL Developer Data Modeler 4.1.3.901
        ///--   at:        2016-04-30 16:07:20 CEST
        ///--   site:      Oracle Database 10g
        ///--   type:      Oracle Database 10g
        ///
        ///
        ///
        ///
        ///CREATE TABLE friend
        ///  (
        ///    player_id_player_data INTEGER NOT NULL ,
        ///    friend_id_player_data INTEGER NOT NULL
        ///  ) ;
        ///CREATE INDEX friend__IDX_player ON friend
        ///  ( player_id_player_data ASC
        ///  ) ;
        ///CREATE INDEX friend__IDX_friend ON friend
        ///  ( friend_id_player_data ASC
        ///  ) ;
        ///CREATE INDEX friend__IDX_player_friend ON  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string create_tables {
            get {
                return ResourceManager.GetString("create_tables", resourceCulture);
            }
        }
    }
}
