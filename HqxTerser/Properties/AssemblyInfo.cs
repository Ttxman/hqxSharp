using System;
using System.Reflection;
using System.Runtime.InteropServices;

#region General Information
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#endregion
[assembly: AssemblyTitle("HqxTerser")]
[assembly: AssemblyDescription("Utility to reduce hqxSharp source code size by finding ans replacing patterns")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("hqxSharp")]
[assembly: AssemblyCopyright("Copyright (C) 2020-2021 René Rhéaume. This program is licensed under the GNU Lesser General Public License version 3 or any later version.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#region Interoperability Attributes
// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.

// The GUID is for the ID of the typelib if this project is exposed to COM
#endregion
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]
[assembly: Guid("4c1bbe95-1124-4d88-9a92-e9ade8c64b19")]

#region Version Information
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
#endregion
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
