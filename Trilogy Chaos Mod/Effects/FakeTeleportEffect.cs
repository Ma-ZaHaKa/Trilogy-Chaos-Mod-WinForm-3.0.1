using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
  public class FakeTeleportEffect : AbstractEffect
  {
    private readonly string EffectID = "effect_fake_teleport";

    public FakeTeleportEffect(string description, string word)
      : base(Category.Teleportation, description, word)
    {
      this.DisableRapidFire();
    }

    public override string GetID() => this.EffectID;

    public override async Task RunEffect(int seed = -1, int duration = -1)
    {
      FakeTeleportEffect fakeTeleportEffect = this;
      // ISSUE: reference to a compiler-generated method
      //await fakeTeleportEffect.RunEffect(seed, duration);
      Location location = Location.Locations[RandomHandler.Next(Location.Locations.Count)];
      WebsocketHandler.INSTANCE.SendEffectToGame(fakeTeleportEffect.EffectID, (object) new
      {
        realEffectName = "Fake Teleport",
        posX = location.X,
        posY = location.Y,
        posZ = location.Z
      }, fakeTeleportEffect.GetDuration(duration), location.GetDisplayName(), fakeTeleportEffect.GetSubtext(), fakeTeleportEffect.GetRapidFire());
    }
  }
}
