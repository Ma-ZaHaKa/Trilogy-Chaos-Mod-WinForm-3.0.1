using GTAChaos.Effects;
using System;
using System.Collections.Generic;

namespace GTAChaos.Utils
{
  public sealed class Category
  {
    public readonly string Name;
    public readonly List<AbstractEffect> Effects;
    public static readonly List<Category> Categories = new List<Category>();
    public static readonly Category WeaponsAndHealth = new Category("Weapons & Health");
    public static readonly Category WantedLevel = new Category("Wanted Level");
    public static readonly Category Weather = new Category(nameof (Weather));
    public static readonly Category Spawning = new Category(nameof (Spawning));
    public static readonly Category Time = new Category(nameof (Time));
    public static readonly Category VehiclesTraffic = new Category("Vehicles & Traffic");
    public static readonly Category NPCs = new Category(nameof (NPCs));
    public static readonly Category PlayerModifications = new Category("Player Modifications");
    public static readonly Category Stats = new Category(nameof (Stats));
    public static readonly Category CustomEffects = new Category("Custom Effects");
    public static readonly Category Teleportation = new Category(nameof (Teleportation));

    private Category(string name)
    {
      this.Name = name;
      this.Effects = new List<AbstractEffect>();
      if (Category.Categories.Contains(this))
        return;
      Category.Categories.Add(this);
      Category.Categories.Sort((Comparison<Category>) ((first, second) => string.Compare(first.Name, second.Name, StringComparison.CurrentCultureIgnoreCase)));
    }

    public void AddEffectToCategory(AbstractEffect effect) => this.Effects.Add(effect);

    public int GetEffectCount() => this.Effects.Count;

    public void ClearEffects() => this.Effects.Clear();
  }
}
