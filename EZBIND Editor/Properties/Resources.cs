using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace EZBIND_Editor.Properties
{
  [DebuggerNonUserCode]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [CompilerGenerated]
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
        if (EZBIND_Editor.Properties.Resources.resourceMan == null)
          EZBIND_Editor.Properties.Resources.resourceMan = new ResourceManager("EZBIND_Editor.Properties.Resources", typeof (EZBIND_Editor.Properties.Resources).Assembly);
        return EZBIND_Editor.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => EZBIND_Editor.Properties.Resources.resourceCulture;
      set => EZBIND_Editor.Properties.Resources.resourceCulture = value;
    }
  }
}
