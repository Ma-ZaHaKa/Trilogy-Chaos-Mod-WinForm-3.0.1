// Decompiled with JetBrains decompiler
// Type: GTAChaos.Properties.Resources
// Assembly: GTAChaos, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8BFBC3C1-BC3A-4B92-A6C3-E2FA96115BE2
// Assembly location: C:\Users\MaZaHaKa\Desktop\chaos-mod-3-0-1_1662193835_813494\Trilogy Chaos Mod.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace GTAChaos.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (GTAChaos.Properties.Resources.resourceMan == null)
          GTAChaos.Properties.Resources.resourceMan = new ResourceManager("GTAChaos.Properties.Resources", typeof (GTAChaos.Properties.Resources).Assembly);
        return GTAChaos.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => GTAChaos.Properties.Resources.resourceCulture;
      set => GTAChaos.Properties.Resources.resourceCulture = value;
    }
  }
}
