using GTAChaos.Utils;
using System;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
    public class RapidFireEffect : FunctionEffect
    {
        protected int effects = 5;
        protected int delay = 2000;

        public RapidFireEffect(
          string description,
          string word,
          string id,
          int duration = -1,
          float multiplier = 3f)
          : base(Category.CustomEffects, description, word, id, duration, multiplier)
        {
            this.DisableRapidFire();
        }

        protected void RunRapidFireEffect(AbstractEffect effect)
        {
            if (Shared.Sync != null && !(effect is RapidFireEffect))
                Shared.Sync.SendEffect(effect, 15000);
            else
                EffectDatabase.RunEffect(effect, duration: 15000);
        }

        protected AbstractEffect GetRandomEffect(int attempts = 0)
        {
            if (attempts > 10)
                return (AbstractEffect)null;
            AbstractEffect randomEffect = EffectDatabase.GetRandomEffect(attempts < 5);
            return randomEffect != null && !(randomEffect is RapidFireEffect) && !randomEffect.IsID("reset_effect_timers") ? randomEffect : this.GetRandomEffect(attempts + 1);
        }

        public override async Task RunEffect(int seed = -1, int duration = -1)
        {
            RapidFireEffect rapidFireEffect = this;
            // ISSUE: reference to a compiler-generated method
            //await rapidFireEffect.RunEffect(seed, duration);
            WebsocketHandler.INSTANCE.SendEffectToGame("effect__generic_empty", (object)new
            {
                name = rapidFireEffect.GetDisplayName()
            }, rapidFireEffect.GetDuration(duration), rapidFireEffect.GetDisplayName(), rapidFireEffect.GetSubtext(), rapidFireEffect.GetRapidFire());
            await Task.Delay(250);
            // ISSUE: reference to a compiler-generated method
            //await Task.Run(new Func<Task>(rapidFireEffect.\u003CRunEffect\u003Eb__5_0));
            //await Task.Run(new Func<Task>(rapidFireEffect));
            //await Task.Run(new Func<Task>(rapidFireEffect.<RunEffect>b__5_0));
        }
    }
}
