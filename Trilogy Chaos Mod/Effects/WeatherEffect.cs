using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class WeatherEffect : AbstractEffect
    {
        private readonly int weatherID;

        public WeatherEffect(string description, string word, int _weatherID, int duration = -1)
          : base(Category.Weather, description, word, duration)
        {
            this.weatherID = _weatherID;
        }

        public override string GetID() => string.Format("weather_{0}", (object)this.weatherID);

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            WeatherEffect weatherEffect = this;
            // ISSUE: reference to a compiler-generated method
            //await weatherEffect.RunEffect(seed, duration);
            WebsocketHandler.INSTANCE.SendEffectToGame("effect_weather", (object)new
            {
                weatherID = weatherEffect.weatherID
            }, weatherEffect.GetDuration(duration), weatherEffect.GetDisplayName(), weatherEffect.GetSubtext(), weatherEffect.GetRapidFire());
        }
    }
}
