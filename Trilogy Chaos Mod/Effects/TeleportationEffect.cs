using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class TeleportationEffect : AbstractEffect
    {
        private readonly Location location;

        public TeleportationEffect(Location location)
          : base(Category.Teleportation, location.DisplayName, location.Cheat)
        {
            this.location = location;
            this.SetDisplayNames(location.GetDisplayName());
            this.SetDisplayName(DisplayNameType.STREAM, location.GetDisplayName(DisplayNameType.STREAM));
            this.DisableRapidFire();
        }

        public override string GetID() => "teleport_" + this.location.GetID();

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            TeleportationEffect teleportationEffect = this;
            // ISSUE: reference to a compiler-generated method
            //await teleportationEffect.RunEffect(seed, duration);
            WebsocketHandler.INSTANCE.SendEffectToGame("effect_teleport", (object)new
            {
                posX = teleportationEffect.location.X,
                posY = teleportationEffect.location.Y,
                posZ = teleportationEffect.location.Z
            }, teleportationEffect.GetDuration(duration), teleportationEffect.GetDisplayName(), teleportationEffect.GetSubtext(), teleportationEffect.GetRapidFire());
        }
    }
}
