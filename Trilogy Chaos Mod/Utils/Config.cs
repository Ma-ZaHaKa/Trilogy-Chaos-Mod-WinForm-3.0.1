using GTAChaos.Effects;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GTAChaos.Utils
{
  public class Config
  {
    private static Config _Instance;
    public int MainCooldown;
    public bool AutoStart = false;
    public string Seed;
    public bool MainShowLastEffects;
    public Dictionary<string, bool> EnabledEffects = new Dictionary<string, bool>();
    public bool PlayAudioForEffects = true;
    public bool PlayAudioSequentially = true;
    public float AudioVolume = 1f;
    public int EffectsCooldownNotActivating = -1;
    public bool TwitchUsePolls;
    public bool TwitchPollsPostMessages;
    public int TwitchPollsBitsCost;
    public int TwitchPollsChannelPointsCost;
    public string StreamAccessToken;
    public string StreamClientID;
    public int StreamVotingTime;
    public int StreamVotingCooldown;
    public bool StreamAllowOnlyEnabledEffectsRapidFire;
    public bool StreamShowLastEffects = true;
    public bool Stream3TimesCooldown = true;
    public bool StreamCombineChatMessages;
    public bool StreamEnableMultipleEffects;
    public bool StreamEnableRapidFire = true;
    public bool StreamMajorityVotes = true;
    public bool StreamHideVotingEffectsIngame = true;
    public string SyncServer;
    public string SyncChannel;
    public string SyncUsername;
    public bool Experimental_RunEffectOnAutoStart;
    public string Experimental_EffectName;
    public bool Experimental_YouTubeConnection;

    public static Config Instance()
    {
      if (Config._Instance == null)
      {
        Config._Instance = new Config();
        foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.Effects.Get())
          Config._Instance.EnabledEffects.Add(entry.item.GetID(), true);
        Config._Instance.EffectsCooldownNotActivating = Config._Instance.EnabledEffects.Count;
      }
      return Config._Instance;
    }

    public static void SetInstance(Config inst) => Config._Instance = inst;

    public static int GetEffectDuration()
    {
      if (!Shared.IsStreamMode)
        return Config.Instance().MainCooldown;
      int num = Config.Instance().StreamVotingCooldown + Config.Instance().StreamVotingTime;
      return !Config.Instance().Stream3TimesCooldown ? (int) Math.Round((double) num / 3.0) : num;
    }

    public static int GetEffectCooldowns() => Math.Min(Config.Instance().EffectsCooldownNotActivating, EffectDatabase.GetEnabledEffectsCount());

    public static string FToString(float value) => value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
