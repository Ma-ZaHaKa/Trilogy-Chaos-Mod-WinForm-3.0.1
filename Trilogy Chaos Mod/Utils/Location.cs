using GTAChaos.Effects;
using System.Collections.Generic;

namespace GTAChaos.Utils
{
  public sealed class Location
  {
    public readonly string DisplayName;
    public readonly string Cheat;
    public readonly int X;
    public readonly int Y;
    public readonly int Z;
    public static readonly List<Location> Locations = new List<Location>();
    public static readonly Location GrooveStreet = new Location("Grove Street", "BringMeHome", 2493, -1670, 15);
    public static readonly Location LSTower = new Location("A Tower", "BringMeToATower", 1544, -1353, 332);
    public static readonly Location LSPier = new Location("A Pier", "BringMeToAPier", 836, -2061, 15);
    public static readonly Location LSAirport = new Location("The LS Airport", "BringMeToTheLSAirport", 2109, -2544, 16);
    public static readonly Location LSDocks = new Location("The Docks", "BringMeToTheDocks", 2760, -2456, 16);
    public static readonly Location MountChiliad = new Location("A Mountain", "BringMeToAMountain", -2233, -1737, 483);
    public static readonly Location SFAirport = new Location("The SF Airport", "BringMeToTheSFAirport", -1083, 409, 17);
    public static readonly Location SFBridge = new Location("A Bridge", "BringMeToABridge", -2669, 1595, 220);
    public static readonly Location Area52 = new Location("A Secret Place", "BringMeToASecretPlace", 213, 1911, 20);
    public static readonly Location LVQuarry = new Location("A Quarry", "BringMeToAQuarry", 614, 856, -40);
    public static readonly Location LVAirport = new Location("The LV Airport", "BringMeToTheLVAirport", 1612, 1166, 17);
    public static readonly Location LVSatellite = new Location("Big Ear", "BringMeToBigEar", -310, 1524, 78);

    public Location(string displayName, string cheat, int x, int y, int z)
    {
      this.DisplayName = displayName;
      this.Cheat = cheat;
      this.X = x;
      this.Y = y;
      this.Z = z;
      Location.Locations.Add(this);
    }

    public string GetID() => this.DisplayName.ToLower().Replace(" ", "_");

    public string GetDisplayName(DisplayNameType type = DisplayNameType.GAME) => type != DisplayNameType.STREAM ? "Teleport To " + this.DisplayName : "TP To " + this.DisplayName;
  }
}
