using GTAChaos.Utils;
using System;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    internal class SpawnVehicleEffect : AbstractEffect
    {
        private readonly int VehicleID;

        public SpawnVehicleEffect(string word, int vehicleID)
          : base(Category.Spawning, "Spawn Vehicle", word)
        {
            this.VehicleID = vehicleID;
            if (this.VehicleID == -1)
            {
                this.SetDisplayNames("Spawn Random Vehicle");
            }
            else
            {
                this.VehicleID = Math.Max(400, Math.Min(vehicleID, 611));
                this.SetDisplayNames("Spawn " + VehicleNames.GetVehicleName(vehicleID));
            }
        }

        public override string GetID() => string.Format("spawn_vehicle_{0}", (object)this.VehicleID);

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            SpawnVehicleEffect spawnVehicleEffect = this;
            // ISSUE: reference to a compiler-generated method
            //await spawnVehicleEffect.RunEffect(seed, duration);
            string displayName = spawnVehicleEffect.GetDisplayName();
            int modelID = spawnVehicleEffect.VehicleID;
            if (modelID == -1)
            {
                modelID = RandomHandler.Next(400, 611);
                displayName = "Spawn " + VehicleNames.GetVehicleName(modelID);
            }
            WebsocketHandler.INSTANCE.SendEffectToGame("effect_spawn_vehicle", (object)new
            {
                vehicleID = modelID
            }, spawnVehicleEffect.GetDuration(duration), displayName, spawnVehicleEffect.GetSubtext(), spawnVehicleEffect.GetRapidFire());
        }
    }
}
