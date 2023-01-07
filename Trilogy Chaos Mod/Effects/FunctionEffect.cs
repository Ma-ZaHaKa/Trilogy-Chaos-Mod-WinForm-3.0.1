using GTAChaos.Utils;
using System;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
  public class FunctionEffect : AbstractEffect
  {
    private readonly string EffectID;

    public FunctionEffect(
      Category category,
      string displayName,
      string word,
      string effectID,
      int duration = -1,
      float multiplier = 3f)
      : base(category, displayName, word, duration, multiplier)
    {
      if (effectID.StartsWith("effect_"))
        throw new Exception("Effect '" + displayName + "' has the 'effect_' prefix!");
      this.EffectID = "effect_" + effectID;
    }

    public override string GetID() => this.EffectID;

    public override string GetAudioFile()
    {
      string audioFile = base.GetAudioFile();
      return !string.IsNullOrWhiteSpace(audioFile) ? audioFile : this.EffectID;
    }

    public override async Task RunEffect(int seed = -1, int duration = -1)
    {
      FunctionEffect functionEffect = this;
      // ISSUE: reference to a compiler-generated method
      //await functionEffect.RunEffect(seed, duration);
      //await functionEffect.RunEffect(seed, duration);
      seed = seed == -1 ? RandomHandler.Next(9999999) : seed;
      WebsocketHandler.INSTANCE.SendEffectToGame(functionEffect.EffectID, (object) new
      {
        seed = seed
      }, functionEffect.GetDuration(duration), functionEffect.GetDisplayName(), functionEffect.GetSubtext(), functionEffect.GetRapidFire());
    }
  }
}
