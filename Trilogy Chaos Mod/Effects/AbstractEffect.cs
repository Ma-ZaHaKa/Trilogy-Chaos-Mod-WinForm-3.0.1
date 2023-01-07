using GTAChaos.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAChaos.Effects
{
  public abstract class AbstractEffect
  {
    public readonly Category Category;
    private readonly Dictionary<DisplayNameType, string> DisplayNames = new Dictionary<DisplayNameType, string>();
    public readonly string Word;
    public readonly int Duration;
    private float Multiplier;
    private string Subtext = "";
    private int rapidFire = 1;
    private bool streamEnabled = true;
    private string audioName = "";
    private int audioVariations;

    public AbstractEffect(
      Category category,
      string displayName,
      string word,
      int duration = -1,
      float multiplier = 3f)
    {
      this.Category = category;
      this.SetDisplayNames(displayName);
      this.Word = word;
      this.Duration = duration;
      this.Multiplier = multiplier;
      category.AddEffectToCategory(this);
    }

    public bool IsID(string id) => this.GetID().Equals(id) || this.GetID().Equals("effect_" + id);

    public abstract string GetID();

    protected void SetDisplayNames(string displayName)
    {
      foreach (DisplayNameType type in Enum.GetValues(typeof (DisplayNameType)))
        this.SetDisplayName(type, displayName);
    }

    public AbstractEffect SetDisplayName(DisplayNameType type, string displayName)
    {
      this.DisplayNames[type] = displayName;
      return this;
    }

    public virtual string GetDisplayName(DisplayNameType type = DisplayNameType.GAME) => !this.DisplayNames.ContainsKey(type) ? this.DisplayNames[DisplayNameType.GAME] : this.DisplayNames[type];

    public string GetSubtext() => this.Subtext;

    public AbstractEffect SetSubtext(string subtext)
    {
      this.Subtext = subtext;
      return this;
    }

    public AbstractEffect ResetSubtext()
    {
      this.Subtext = "";
      return this;
    }

    public AbstractEffect DisableRapidFire()
    {
      this.rapidFire = 0;
      return this;
    }

    public AbstractEffect DisableStream()
    {
      this.streamEnabled = false;
      return this;
    }

    public AbstractEffect SetMultiplier(float multiplier)
    {
      this.Multiplier = multiplier;
      return this;
    }

    public float GetMultiplier() => this.Multiplier;

    public bool IsRapidFire() => this.rapidFire == 1;

    public bool IsTwitchEnabled() => this.streamEnabled;

    public AbstractEffect SetAudioFile(string name)
    {
      this.audioName = name;
      return this;
    }

    public int GetAudioVariations() => this.audioVariations;

    public AbstractEffect SetAudioVariations(int variations = 0)
    {
      this.audioVariations = variations;
      return this;
    }

    public virtual string GetAudioFile()
    {
      string str = string.IsNullOrEmpty(this.audioName) ? this.GetID() : this.audioName;
      if (this.audioVariations == 0)
        return str;
      Random random = new Random();
      return string.Format("{0}_{1}", (object) str, (object) random.Next(this.audioVariations));
    }

    public virtual async Task RunEffect(int seed = -1, int _duration = -1)
    {
      if (!Config.Instance().PlayAudioForEffects || Shared.StreamVotingMode == Shared.VOTING_MODE.RAPID_FIRE)
        return;
      await AudioPlayer.INSTANCE.PlayAudio(this.GetAudioFile());
    }

    public virtual bool IsCooldownable() => true;

    public int GetDuration(int duration = -1)
    {
      if (duration != -1)
        return duration;
      if (this.Duration != -1)
      {
        duration = Math.Min(Config.GetEffectDuration(), this.Duration);
      }
      else
      {
        if (duration == -1)
          duration = Config.GetEffectDuration();
        duration = (int) Math.Round((double) duration * (double) this.Multiplier);
      }
      return duration;
    }

    public bool GetRapidFire(bool overrideCheck = false)
    {
      if (overrideCheck)
        return this.rapidFire == 1;
      bool flag = false;
      if (Shared.IsStreamMode && Shared.StreamVotingMode == Shared.VOTING_MODE.RAPID_FIRE)
        flag = this.rapidFire == 1;
      return flag;
    }
  }
}
