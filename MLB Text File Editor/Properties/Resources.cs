using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace MLB_Text_File_Editor.Properties
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
        if (MLB_Text_File_Editor.Properties.Resources.resourceMan == null)
          MLB_Text_File_Editor.Properties.Resources.resourceMan = new ResourceManager("MLB_Text_File_Editor.Properties.Resources", typeof (MLB_Text_File_Editor.Properties.Resources).Assembly);
        return MLB_Text_File_Editor.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => MLB_Text_File_Editor.Properties.Resources.resourceCulture;
      set => MLB_Text_File_Editor.Properties.Resources.resourceCulture = value;
    }
  }
}
