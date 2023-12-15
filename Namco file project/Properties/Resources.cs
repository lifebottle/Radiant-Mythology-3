using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Namco_file_project.Properties
{
  [DebuggerNonUserCode]
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Namco_file_project.Properties.Resources.resourceMan == null)
          Namco_file_project.Properties.Resources.resourceMan = new ResourceManager("Namco_file_project.Properties.Resources", typeof (Namco_file_project.Properties.Resources).Assembly);
        return Namco_file_project.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Namco_file_project.Properties.Resources.resourceCulture;
      set => Namco_file_project.Properties.Resources.resourceCulture = value;
    }
  }
}
