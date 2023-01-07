using GTAChaos.Utils;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
  public class FakeCrashEffect : AbstractEffect
  {
    private readonly string EffectID = "effect_fake_crash";

    public FakeCrashEffect(string description, string word)
      : base(Category.CustomEffects, description, word)
    {
      this.SetDisplayName(DisplayNameType.UI, "Fake Crash");
      this.DisableRapidFire();
    }

    public override string GetID() => this.EffectID;

    public override async Task RunEffect(int seed = -1, int duration = -1)
    {
      FakeCrashEffect fakeCrashEffect = this;
      // ISSUE: reference to a compiler-generated method
      //await fakeCrashEffect.RunEffect(seed, duration);
      WebsocketHandler.INSTANCE.SendEffectToGame(fakeCrashEffect.EffectID, (object) new
      {
        realEffectName = "Fake Crash"
      }, fakeCrashEffect.GetDuration(duration), fakeCrashEffect.GetDisplayName(), fakeCrashEffect.GetSubtext(), fakeCrashEffect.GetRapidFire());
    }
  }
}
