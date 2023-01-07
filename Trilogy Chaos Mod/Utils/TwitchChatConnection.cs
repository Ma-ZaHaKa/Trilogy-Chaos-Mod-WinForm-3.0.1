using GTAChaos.Effects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TwitchLib.Api;
//using TwitchLib.Api.Auth;
//using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
//using TwitchLib.Communication.Clients;
//using TwitchLib.Communication.Interfaces;
//using TwitchLib.Communication.Models;

namespace GTAChaos.Utils
{
  public class TwitchChatConnection : IStreamConnection
  {
    public TwitchClient Client;
    //private WebSocketClient customClient;
    private readonly TwitchAPI api;
    private readonly string AccessToken;
    private readonly string ClientID;
    private string Channel;
    private string Username;
    private readonly TwitchChatConnection.ChatEffectVoting effectVoting = new TwitchChatConnection.ChatEffectVoting();
    private readonly HashSet<string> rapidFireVoters = new HashSet<string>();
    private Shared.VOTING_MODE VotingMode;
    private int lastChoice = -1;

    public TwitchChatConnection()
    {
      /*this.AccessToken = Config.Instance().StreamAccessToken;
      this.ClientID = Config.Instance().StreamClientID;
      if (string.IsNullOrEmpty(this.AccessToken) || string.IsNullOrEmpty(this.ClientID))
        return;
      this.api = new TwitchAPI((ILoggerFactory) null, (IRateLimiter) null, (IApiSettings) null, (IHttpCallHandler) null);
      this.api.Settings.ClientId = this.ClientID;
      this.api.Settings.AccessToken = this.AccessToken;*/
    }

    public TwitchClient GetTwitchClient() => this.Client;

    private void InitializeTwitchClient()
    {
      /*ConnectionCredentials connectionCredentials = new ConnectionCredentials(this.Username, this.AccessToken, "wss://irc-ws.chat.twitch.tv:443", false, (Capabilities) null);
      this.customClient = new WebSocketClient((IClientOptions) new ClientOptions()
      {
        MessagesAllowedInPeriod = 750,
        ThrottlingPeriod = TimeSpan.FromSeconds(30.0)
      });
      this.Client = new TwitchClient((IClient) this.customClient, (ClientProtocol) 1, (ILogger<TwitchClient>) null);
      this.Client.Initialize(connectionCredentials, this.Channel, '!', '!', true);
      this.Client.OnMessageReceived += new EventHandler<OnMessageReceivedArgs>(this.Client_OnMessageReceived);
      this.Client.OnConnected += new EventHandler<OnConnectedArgs>(this.Client_OnConnected);
      this.Client.OnConnectionError += new EventHandler<OnConnectionErrorArgs>(this.Client_OnConnectionError);
      this.Client.OnIncorrectLogin += new EventHandler<OnIncorrectLoginArgs>(this.Client_OnIncorrectLogin);*/
    }

    public async Task<bool> TryConnect()
    {
      /*TwitchChatConnection twitchChatConnection = this;
      // ISSUE: explicit non-virtual call
      __nonvirtual (twitchChatConnection.Kill());
      ValidateAccessTokenResponse accessTokenResponse = await twitchChatConnection.api.Auth.ValidateAccessTokenAsync((string) null);
      if (accessTokenResponse == null)
      {
        EventHandler<EventArgs> onLoginError = twitchChatConnection.OnLoginError;
        if (onLoginError != null)
          onLoginError((object) twitchChatConnection, new EventArgs());
        return false;
      }
      twitchChatConnection.Username = accessTokenResponse.Login;
      twitchChatConnection.Channel = accessTokenResponse.Login;
      twitchChatConnection.InitializeTwitchClient();
      twitchChatConnection.Client.Connect();*/
      return true;
    }

    public bool IsConnected()
    {
      TwitchClient client = this.Client;
      return client != null && client.IsConnected;
    }

    private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e) => this.Client?.Reconnect();

    private void Client_OnConnected(object sender, OnConnectedArgs e)
    {
      EventHandler<EventArgs> onConnected = this.OnConnected;
      if (onConnected != null)
        onConnected((object) this, (EventArgs) e);
      this.SendMessage("Connected!");
    }

    private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs e)
    {
      EventHandler<EventArgs> onLoginError = this.OnLoginError;
      if (onLoginError == null)
        return;
      onLoginError(sender, (EventArgs) e);
    }

    public void Kill()
    {
      /*if (this.Client != null)
      {
        EventHandler<EventArgs> onDisconnected = this.OnDisconnected;
        if (onDisconnected != null)
          onDisconnected((object) this, new EventArgs());
        this.Client.Disconnect();
      }
      this.Client = (TwitchClient) null;
      this.customClient?.Dispose();
      this.customClient = (WebSocketClient) null;*/
    }

    public int GetRemaining() => 0;

    public void SetVoting(
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
        if (Config.Instance().StreamCombineChatMessages)
        {
          string message = "Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects! ";
          foreach (TwitchChatConnection.ChatVotingElement votingElement in this.effectVoting.GetVotingElements())
          {
            string displayName = votingElement.Effect.GetDisplayName(DisplayNameType.STREAM);
            message += string.Format("#{0}: {1}. ", (object) (votingElement.Id + 1), (object) displayName);
          }
          this.SendMessage(message);
        }
        else
        {
          this.SendMessage("Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects!");
          foreach (TwitchChatConnection.ChatVotingElement votingElement in this.effectVoting.GetVotingElements())
          {
            string displayName = votingElement.Effect.GetDisplayName(DisplayNameType.STREAM);
            this.SendMessage(string.Format("#{0}: {1}", (object) (votingElement.Id + 1), (object) displayName));
          }
        }
      }
      else if (this.VotingMode == Shared.VOTING_MODE.RAPID_FIRE)
      {
        this.rapidFireVoters.Clear();
        this.SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 15 SECONDS!");
      }
      else if (votingElements != null && votingElements.Count > 0)
      {
        this.SendEffectVotingToGame(false);
        string str = string.Join(", ", votingElements.Select<IVotingElement, string>((Func<IVotingElement, string>) (e => e.GetEffect().GetDisplayName(DisplayNameType.STREAM))));
        if (Config.Instance().StreamEnableRapidFire)
        {
          this.SendMessage(string.Format("Cooldown has started! ({0} until Rapid-Fire) - Enabled effects: {1}", (object) untilRapidFire, (object) str));
          if (untilRapidFire != 1)
            return;
          this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
        }
        else
          this.SendMessage("Cooldown has started! - Enabled effects: " + str);
      }
      else if (Config.Instance().StreamEnableRapidFire)
      {
        this.SendMessage(string.Format("Cooldown has started! ({0} until Rapid-Fire)", (object) untilRapidFire));
        if (untilRapidFire != 1)
          return;
        this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
      }
      else
        this.SendMessage("Cooldown has started!");
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
      if (!this.IsConnected() || this.Channel == null || message == null)
        return;
      if (!this.Client.IsConnected)
        this.Client.Connect();
      else if (((IReadOnlyCollection<JoinedChannel>) this.Client.JoinedChannels).Count == 0)
        this.Client.JoinChannel(this.Channel, false);
      else
        this.Client.SendMessage(this.Channel, (prefix ? "[GTA Chaos] " : "") + message, false);
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
      string username = ((TwitchLibMessage) e.ChatMessage).Username;
      string str = this.RemoveSpecialCharacters(((ChatMessage) e.ChatMessage).Message);
      if (this.VotingMode == Shared.VOTING_MODE.RAPID_FIRE)
      {
        if (this.rapidFireVoters.Contains(username))
          return;
        AbstractEffect byWord = EffectDatabase.GetByWord(str, Config.Instance().StreamAllowOnlyEnabledEffectsRapidFire);
        if (byWord == null || !byWord.IsRapidFire())
          return;
        this.RapidFireEffect(new RapidFireEventArgs()
        {
          Effect = byWord.SetSubtext(username)
        });
        this.rapidFireVoters.Add(username);
      }
      else
      {
        if (this.VotingMode != Shared.VOTING_MODE.VOTING)
          return;
        int userChoice = this.TryParseUserChoice(str);
        switch (userChoice)
        {
          case 0:
          case 1:
          case 2:
            this.effectVoting?.TryAddVote(username, userChoice);
            break;
        }
      }
    }

    private string RemoveSpecialCharacters(string text) => Regex.Replace(text, "[^A-Za-z0-9]", "");

    private int TryParseUserChoice(string text)
    {
      try
      {
        return int.Parse(text) - 1;
      }
      catch
      {
        return -1;
      }
    }

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
      private readonly List<TwitchChatConnection.ChatVotingElement> votingElements = new List<TwitchChatConnection.ChatVotingElement>();
      private readonly Dictionary<string, TwitchChatConnection.ChatVotingElement> voters = new Dictionary<string, TwitchChatConnection.ChatVotingElement>();

      public bool IsEmpty() => this.votingElements.Count == 0;

      public void Clear()
      {
        this.votingElements.Clear();
        this.voters.Clear();
      }

      public List<TwitchChatConnection.ChatVotingElement> GetVotingElements() => this.votingElements;

      public int GetTotalVotes()
      {
        int num = 0;
        foreach (TwitchChatConnection.ChatVotingElement votingElement in this.votingElements)
          num += votingElement.Voters.Count;
        return num;
      }

      public bool ContainsEffect(AbstractEffect effect) => this.votingElements.Any<TwitchChatConnection.ChatVotingElement>((Func<TwitchChatConnection.ChatVotingElement, bool>) (e => e.Effect.GetDisplayName(DisplayNameType.STREAM).Equals(effect.GetDisplayName(DisplayNameType.STREAM))));

      public void AddEffect(AbstractEffect effect) => this.votingElements.Add(new TwitchChatConnection.ChatVotingElement(this.votingElements.Count, effect));

      public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
      {
        TwitchChatConnection.ChatVotingElement[] array = this.GetVotingElements().ToArray();
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
          AbstractEffect randomEffect = EffectDatabase.GetRandomEffect(true, addEffectToCooldown: true);
          if (randomEffect.IsTwitchEnabled() && !this.ContainsEffect(randomEffect))
            this.AddEffect(randomEffect);
          if (num2++ >= 10)
            EffectDatabase.ResetEffectCooldowns();
          else if (num2++ >= 20)
            break;
        }
        while (this.votingElements.Count < 3)
        {
          AbstractEffect randomEffect = EffectDatabase.GetRandomEffect(addEffectToCooldown: true);
          if (randomEffect.IsTwitchEnabled() && !this.ContainsEffect(randomEffect))
            this.AddEffect(randomEffect);
        }
      }

      public List<IVotingElement> GetMajorityVotes()
      {
        int maxVotes = 0;
        TwitchChatConnection.ChatVotingElement[] array = this.votingElements.OrderByDescending<TwitchChatConnection.ChatVotingElement, int>((Func<TwitchChatConnection.ChatVotingElement, int>) (e =>
        {
          if (e.Voters.Count > maxVotes)
            maxVotes = e.Voters.Count;
          return e.Voters.Count;
        })).Where<TwitchChatConnection.ChatVotingElement>((Func<TwitchChatConnection.ChatVotingElement, bool>) (e => e.Voters.Count == maxVotes)).ToArray<TwitchChatConnection.ChatVotingElement>();
        List<IVotingElement> votingElementList = new List<IVotingElement>();
        if (Config.Instance().StreamEnableMultipleEffects)
          votingElementList.AddRange((IEnumerable<IVotingElement>) array);
        else
          votingElementList.Add((IVotingElement) array[new Random().Next(((IEnumerable<TwitchChatConnection.ChatVotingElement>) array).Count<TwitchChatConnection.ChatVotingElement>())]);
        return votingElementList;
      }

      public List<IVotingElement> GetTrulyRandomVotes()
      {
        List<IVotingElement> votingElementList = new List<IVotingElement>();
        int maxValue = 0;
        foreach (TwitchChatConnection.ChatVotingElement votingElement in this.votingElements)
          maxValue += votingElement.Voters.Count;
        if (maxValue == 0)
          return votingElementList;
        int num = new Random().Next(maxValue) + 1;
        foreach (TwitchChatConnection.ChatVotingElement votingElement in this.votingElements)
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
        foreach (TwitchChatConnection.ChatVotingElement votingElement in this.votingElements)
          votingElement.RemoveVoter(username);
        this.votingElements[effectChoice].AddVoter(username);
        this.voters[username] = this.votingElements[effectChoice];
        foreach (TwitchChatConnection.ChatVotingElement votingElement in this.votingElements)
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
