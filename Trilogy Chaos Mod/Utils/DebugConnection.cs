using GTAChaos.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace GTAChaos.Utils
{
  public class DebugConnection : IStreamConnection
  {
    private readonly DebugConnection.ChatEffectVoting effectVoting = new DebugConnection.ChatEffectVoting();
    private readonly HashSet<string> rapidFireVoters = new HashSet<string>();
    private Shared.VOTING_MODE VotingMode;
    private int lastChoice = -1;

    public int GetRemaining() => 0;

    public async Task<bool> TryConnect()
    {
      DebugConnection debugConnection = this;
      EventHandler<EventArgs> onConnected = debugConnection.OnConnected;
      if (onConnected != null)
        onConnected((object) debugConnection, new EventArgs());
      return true;
    }

    public bool IsConnected() => true;

    public void Kill()
    {
    }

    public async void SetVoting(
      Shared.VOTING_MODE votingMode,
      int untilRapidFire = -1,
      List<IVotingElement> votingElements = null)
    {
      this.VotingMode = votingMode;
      if (this.VotingMode == Shared.VOTING_MODE.VOTING)
      {
        this.effectVoting.Clear();
        this.effectVoting.GenerateRandomEffects();
        this.lastChoice = -1;
      }
      else
      {
        if (this.VotingMode != Shared.VOTING_MODE.COOLDOWN)
          return;
        this.SendEffectVotingToGame(false);
      }
    }

    public List<IVotingElement> GetVotedEffects()
    {
      List<IVotingElement> source = Config.Instance().StreamMajorityVotes ? this.effectVoting.GetMajorityVotes() : this.effectVoting.GetTrulyRandomVotes();
      foreach (IVotingElement votingElement in source)
        votingElement.GetEffect().SetSubtext(string.Format("{0}%", (object) votingElement.GetPercentage()));
      this.lastChoice = source.Count > 1 ? -1 : source.First<IVotingElement>().GetId();
      return source;
    }

    private void SendMessage(string message, bool prefix = true)
    {
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
      string username = ((TwitchLibMessage) e.ChatMessage).Username;
      string word = this.RemoveSpecialCharacters(((ChatMessage) e.ChatMessage).Message);
      if (this.VotingMode != Shared.VOTING_MODE.RAPID_FIRE || this.rapidFireVoters.Contains(username))
        return;
      AbstractEffect byWord = EffectDatabase.GetByWord(word, Config.Instance().StreamAllowOnlyEnabledEffectsRapidFire);
      if (byWord == null || !byWord.IsRapidFire())
        return;
      this.RapidFireEffect(new RapidFireEventArgs()
      {
        Effect = byWord.SetSubtext(username)
      });
      this.rapidFireVoters.Add(username);
    }

    private string RemoveSpecialCharacters(string text) => Regex.Replace(text, "[^A-Za-z0-9]", "");

    public void SendEffectVotingToGame(bool undetermined = true)
    {
      if (this.effectVoting.IsEmpty())
        return;
      string[] effects;
      int[] votes;
      this.effectVoting.GetVotes(out effects, out votes, undetermined);
      if (Shared.Sync != null)
        Shared.Sync.SendVotes(effects, votes, this.lastChoice, !undetermined);
      else
        WebsocketHandler.INSTANCE.SendVotes(effects, votes, this.lastChoice);
    }

    public event EventHandler<EventArgs> OnConnected;

    public event EventHandler<EventArgs> OnDisconnected;

    public event EventHandler<EventArgs> OnLoginError;

    public event EventHandler<RapidFireEventArgs> OnRapidFireEffect;

    public virtual void RapidFireEffect(RapidFireEventArgs e)
    {
      EventHandler<RapidFireEventArgs> onRapidFireEffect = this.OnRapidFireEffect;
      if (onRapidFireEffect == null)
        return;
      onRapidFireEffect((object) this, e);
    }

    private class ChatEffectVoting
    {
      private readonly List<DebugConnection.ChatVotingElement> votingElements = new List<DebugConnection.ChatVotingElement>();
      private readonly Dictionary<string, DebugConnection.ChatVotingElement> voters = new Dictionary<string, DebugConnection.ChatVotingElement>();

      public bool IsEmpty() => this.votingElements.Count == 0;

      public void Clear()
      {
        this.votingElements.Clear();
        this.voters.Clear();
      }

      public List<DebugConnection.ChatVotingElement> GetVotingElements() => this.votingElements;

      public int GetTotalVotes()
      {
        int num = 0;
        foreach (DebugConnection.ChatVotingElement votingElement in this.votingElements)
          num += votingElement.Voters.Count;
        return num;
      }

      public bool ContainsEffect(AbstractEffect effect) => this.votingElements.Any<DebugConnection.ChatVotingElement>((Func<DebugConnection.ChatVotingElement, bool>) (e => e.Effect.GetDisplayName(DisplayNameType.STREAM).Equals(effect.GetDisplayName(DisplayNameType.STREAM))));

      public void AddEffect(AbstractEffect effect) => this.votingElements.Add(new DebugConnection.ChatVotingElement(this.votingElements.Count, effect));

      public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
      {
        DebugConnection.ChatVotingElement[] array = this.GetVotingElements().ToArray();
        effects = new string[3]
        {
          undetermined ? "???" : array[0].Effect.GetDisplayName(),
          undetermined ? "???" : array[1].Effect.GetDisplayName(),
          undetermined ? "???" : array[2].Effect.GetDisplayName()
        };
        votes = new int[3]
        {
          array[0].Voters.Count,
          array[1].Voters.Count,
          array[2].Voters.Count
        };
      }

      public void GenerateRandomEffects()
      {
        int num1 = Math.Min(3, EffectDatabase.EnabledEffects.Count);
        int num2 = 0;
        while (this.votingElements.Count != num1)
        {
          AbstractEffect randomEffect = EffectDatabase.GetRandomEffect(true);
          if (randomEffect.IsTwitchEnabled() && !this.ContainsEffect(randomEffect))
            this.AddEffect(randomEffect);
          if (num2++ >= 10)
            EffectDatabase.ResetEffectCooldowns();
          else if (num2++ >= 20)
            break;
        }
        while (this.votingElements.Count < 3)
        {
          AbstractEffect randomEffect = EffectDatabase.GetRandomEffect();
          if (randomEffect.IsTwitchEnabled() && !this.ContainsEffect(randomEffect))
            this.AddEffect(randomEffect);
        }
      }

      public List<IVotingElement> GetMajorityVotes()
      {
        int maxVotes = 0;
        DebugConnection.ChatVotingElement[] array = this.votingElements.OrderByDescending<DebugConnection.ChatVotingElement, int>((Func<DebugConnection.ChatVotingElement, int>) (e =>
        {
          if (e.Voters.Count > maxVotes)
            maxVotes = e.Voters.Count;
          return e.Voters.Count;
        })).Where<DebugConnection.ChatVotingElement>((Func<DebugConnection.ChatVotingElement, bool>) (e => e.Voters.Count == maxVotes)).ToArray<DebugConnection.ChatVotingElement>();
        List<IVotingElement> votingElementList = new List<IVotingElement>();
        if (Config.Instance().StreamEnableMultipleEffects)
          votingElementList.AddRange((IEnumerable<IVotingElement>) array);
        else
          votingElementList.Add((IVotingElement) array[new Random().Next(((IEnumerable<DebugConnection.ChatVotingElement>) array).Count<DebugConnection.ChatVotingElement>())]);
        return votingElementList;
      }

      public List<IVotingElement> GetTrulyRandomVotes()
      {
        List<IVotingElement> votingElementList = new List<IVotingElement>();
        int maxValue = 0;
        foreach (DebugConnection.ChatVotingElement votingElement in this.votingElements)
          maxValue += votingElement.Voters.Count;
        if (maxValue == 0)
          return votingElementList;
        int num = new Random().Next(maxValue) + 1;
        foreach (DebugConnection.ChatVotingElement votingElement in this.votingElements)
        {
          if (num > votingElement.Voters.Count)
          {
            num -= votingElement.Voters.Count;
          }
          else
          {
            votingElementList.Add((IVotingElement) votingElement);
            break;
          }
        }
        return votingElementList;
      }

      public void TryAddVote(string username, int effectChoice)
      {
        foreach (DebugConnection.ChatVotingElement votingElement in this.votingElements)
          votingElement.RemoveVoter(username);
        this.votingElements[effectChoice].AddVoter(username);
        this.voters[username] = this.votingElements[effectChoice];
        foreach (DebugConnection.ChatVotingElement votingElement in this.votingElements)
          votingElement.Percentage = (int) Math.Round((double) votingElement.Voters.Count / (double) this.GetTotalVotes() * 100.0);
      }
    }

    public class ChatVotingElement : IVotingElement
    {
      public int Id { get; set; }

      public AbstractEffect Effect { get; set; }

      public HashSet<string> Voters { get; set; }

      public int Percentage { get; set; }

      public ChatVotingElement(int id, AbstractEffect effect)
      {
        this.Id = id;
        this.Effect = effect;
        this.Percentage = 0;
        this.Voters = new HashSet<string>();
      }

      public int GetId() => this.Id;

      public AbstractEffect GetEffect() => this.Effect;

      public int GetVotes() => this.Voters.Count;

      public int GetPercentage() => this.Percentage;

      public bool ContainsVoter(string username) => this.Voters.Contains(username);

      public void AddVoter(string username) => this.Voters.Add(username);

      public void RemoveVoter(string username) => this.Voters.Remove(username);
    }
  }
}
