using GTAChaos.Effects;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib.Api;
//using TwitchLib.Api.Auth;
//using TwitchLib.Api.Core.Interfaces;
//using TwitchLib.Api.Helix.Models.Polls;
//using TwitchLib.Api.Helix.Models.Polls.CreatePoll;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
//using TwitchLib.Communication.Clients;
//using TwitchLib.Communication.Interfaces;
//using TwitchLib.Communication.Models;

namespace GTAChaos.Utils
{
  public class TwitchPollConnection : IStreamConnection
  {
    public TwitchClient Client;
    //private WebSocketClient customClient;
    private readonly TwitchAPI api;
    private readonly string AccessToken;
    private readonly string ClientID;
    private string Username;
    private string Channel;
    private string UserID;
    private readonly TwitchPollConnection.PollEffectVoting effectVoting = new TwitchPollConnection.PollEffectVoting();
    private readonly HashSet<string> rapidFireVoters = new HashSet<string>();
    private Shared.VOTING_MODE VotingMode;
    private int lastChoice = -1;
    private bool createdPoll;
    //private Poll activePoll;
    private readonly Timer activePollTimer;

    public TwitchPollConnection()
    {
      /*this.AccessToken = Config.Instance().StreamAccessToken;
      this.ClientID = Config.Instance().StreamClientID;
      if (string.IsNullOrEmpty(this.AccessToken) || string.IsNullOrEmpty(this.ClientID))
        return;
      this.api = new TwitchAPI((ILoggerFactory) null, (IRateLimiter) null, (IApiSettings) null, (IHttpCallHandler) null);
      this.api.Settings.ClientId = this.ClientID;
      this.api.Settings.AccessToken = this.AccessToken;
      this.activePollTimer = new Timer()
      {
        AutoReset = true,
        Interval = 1000.0
      };
      this.activePollTimer.Elapsed += new ElapsedEventHandler(this.ActivePollTimer_Elapsed);
      this.activePollTimer.Start();*/
    }

    private async void ActivePollTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      /*if (!this.createdPoll || this.activePoll == null)
        return;
      this.activePoll = (await this.api.Helix.Polls.GetPollsAsync(this.UserID, new List<string>()
      {
        this.activePoll.Id
      }, (string) null, 20, (string) null)).Data[0];
      for (int elementId = 0; elementId < 3; ++elementId)
        this.effectVoting.SetVotes(elementId, this.activePoll.Choices[elementId].Votes);*/
    }

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
      /*TwitchPollConnection twitchPollConnection = this;
      // ISSUE: explicit non-virtual call
      __nonvirtual (twitchPollConnection.Kill());
      ValidateAccessTokenResponse accessTokenResponse = await twitchPollConnection.api.Auth.ValidateAccessTokenAsync(twitchPollConnection.AccessToken);
      if (accessTokenResponse == null)
      {
        EventHandler<EventArgs> onLoginError = twitchPollConnection.OnLoginError;
        if (onLoginError != null)
          onLoginError((object) twitchPollConnection, new EventArgs());
        return false;
      }
      twitchPollConnection.Username = accessTokenResponse.Login;
      twitchPollConnection.Channel = accessTokenResponse.Login;
      twitchPollConnection.UserID = accessTokenResponse.UserId;
      twitchPollConnection.InitializeTwitchClient();
      twitchPollConnection.Client.Connect();*/
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

    public int GetRemaining()
    {
            /* if (this.activePoll == null)
               return -1;
             return !(this.activePoll.Status == "COMPLETED") ? this.activePoll.DurationSeconds : 0;*/
            return 0; // CUSTOM
    }

    /*private async Task<bool> TryPoll(CreatePollRequest createPoll, int tries = 0)
    {
      try
      {
        this.activePoll = ((CreatePollResponse) await this.api.Helix.Polls.CreatePollAsync(createPoll, this.AccessToken)).Data[0];
      }
      catch (Exception ex)
      {
        ++tries;
        if (tries > 5)
          return false;
        await Task.Delay(500);
        return await this.TryPoll(createPoll, tries);
      }
      return true;
    }*/

    public async void SetVoting(
      Shared.VOTING_MODE votingMode,
      int untilRapidFire = -1,
      List<IVotingElement> votingElements = null)
    {
      /*this.VotingMode = votingMode;
      if (this.VotingMode == Shared.VOTING_MODE.VOTING)
      {
        this.effectVoting.Clear();
        this.effectVoting.GenerateRandomEffects();
        this.lastChoice = -1;
        if (Config.Instance().TwitchPollsPostMessages)
        {
          if (Config.Instance().StreamCombineChatMessages)
          {
            string message = "Voting has started! ";
            foreach (TwitchPollConnection.PollVotingElement votingElement in this.effectVoting.GetVotingElements())
            {
              string displayName = votingElement.Effect.GetDisplayName(DisplayNameType.STREAM);
              message += string.Format("#{0}: {1}. ", (object) (votingElement.Id + 1), (object) displayName);
            }
            this.SendMessage(message);
          }
          else
          {
            this.SendMessage("Voting has started!");
            foreach (TwitchPollConnection.PollVotingElement votingElement in this.effectVoting.GetVotingElements())
            {
              string displayName = votingElement.Effect.GetDisplayName(DisplayNameType.STREAM);
              this.SendMessage(string.Format("#{0}: {1}", (object) (votingElement.Id + 1), (object) displayName));
            }
          }
        }
        CreatePollRequest createPoll = new CreatePollRequest()
        {
          Title = "[GTA Chaos] Next Effect",
          BroadcasterId = this.UserID,
          DurationSeconds = Config.Instance().StreamVotingTime / 1000,
          Choices = this.effectVoting.GetPollChoices(),
          BitsVotingEnabled = Config.Instance().TwitchPollsBitsCost > 0,
          BitsPerVote = Config.Instance().TwitchPollsBitsCost,
          ChannelPointsVotingEnabled = Config.Instance().TwitchPollsChannelPointsCost > 0,
          ChannelPointsPerVote = Config.Instance().TwitchPollsChannelPointsCost
        };
        List<string> stringList = new List<string>();
        foreach (Choice choice in createPoll.Choices)
          stringList.Add(choice.Title);
        this.createdPoll = (bool) await this.TryPoll(createPoll);
      }
      else if (this.VotingMode == Shared.VOTING_MODE.RAPID_FIRE)
      {
        this.rapidFireVoters.Clear();
        this.SendMessage("ATTENTION, ALL GAMERS! RAPID-FIRE HAS BEGUN! VALID EFFECTS WILL BE ENABLED FOR 15 SECONDS!");
      }
      else if (this.VotingMode == Shared.VOTING_MODE.ERROR)
      {
        this.SendEffectVotingToGame(false);
        if (Config.Instance().StreamEnableRapidFire)
        {
          this.SendMessage(string.Format("Cooldown has started! ({0} until Rapid-Fire) - Poll Failed :(", (object) untilRapidFire));
          if (untilRapidFire == 1)
            this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
        }
        else
          this.SendMessage("Cooldown has started! - Poll Failed :(");
        this.activePoll = (Poll) null;
        this.createdPoll = false;
      }
      else if (votingElements != null && votingElements.Count > 0)
      {
        this.SendEffectVotingToGame(false);
        string str = string.Join(", ", votingElements.Select<IVotingElement, string>((Func<IVotingElement, string>) (e => e.GetEffect().GetDisplayName(DisplayNameType.STREAM))));
        if (Config.Instance().StreamEnableRapidFire)
        {
          this.SendMessage(string.Format("Cooldown has started! ({0} until Rapid-Fire) - Enabled effects: {1}", (object) untilRapidFire, (object) str));
          if (untilRapidFire == 1)
            this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
        }
        else
          this.SendMessage("Cooldown has started! - Enabled effects: " + str);
        this.activePoll = (Poll) null;
        this.createdPoll = false;
      }
      else if (Config.Instance().StreamEnableRapidFire)
      {
        this.SendMessage(string.Format("Cooldown has started! ({0} until Rapid-Fire)", (object) untilRapidFire));
        if (untilRapidFire != 1)
          return;
        this.SendMessage("Rapid-Fire is coming up! Get your cheats ready! - List of all effects: https://bit.ly/gta-sa-chaos-mod");
      }
      else
        this.SendMessage("Cooldown has started!");*/
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

    private class PollEffectVoting
    {
      private readonly List<TwitchPollConnection.PollVotingElement> votingElements = new List<TwitchPollConnection.PollVotingElement>();

      public bool IsEmpty() => this.votingElements.Count == 0;

      public void Clear() => this.votingElements.Clear();

      public List<TwitchPollConnection.PollVotingElement> GetVotingElements() => this.votingElements;

      public int GetTotalVotes()
      {
        int num = 0;
        foreach (TwitchPollConnection.PollVotingElement votingElement in this.votingElements)
          num += votingElement.Votes;
        return num;
      }

      /*public Choice[] GetPollChoices()
      {
        List<Choice> choiceList = new List<Choice>();
        foreach (TwitchPollConnection.PollVotingElement votingElement in this.GetVotingElements())
        {
          string displayName = votingElement.Effect.GetDisplayName(DisplayNameType.STREAM);
          choiceList.Add(new Choice()
          {
            Title = displayName.Substring(0, Math.Min(25, displayName.Length))
          });
        }
        return choiceList.ToArray();
      }*/

      public bool ContainsEffect(AbstractEffect effect) => this.votingElements.Any<TwitchPollConnection.PollVotingElement>((Func<TwitchPollConnection.PollVotingElement, bool>) (e => e.Effect.GetDisplayName(DisplayNameType.STREAM).Equals(effect.GetDisplayName(DisplayNameType.STREAM))));

      public void AddEffect(AbstractEffect effect) => this.votingElements.Add(new TwitchPollConnection.PollVotingElement(this.votingElements.Count, effect));

      public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
      {
        TwitchPollConnection.PollVotingElement[] array = this.GetVotingElements().ToArray();
        effects = new string[3]
        {
          undetermined ? "???" : array[0].Effect.GetDisplayName(),
          undetermined ? "???" : array[1].Effect.GetDisplayName(),
          undetermined ? "???" : array[2].Effect.GetDisplayName()
        };
        votes = new int[3]
        {
          array[0].Votes,
          array[1].Votes,
          array[2].Votes
        };
      }

      public void SetVotes(int elementId, int votes)
      {
        if (elementId < 0 || elementId > 2)
          return;
        if (this.votingElements.Count > elementId)
          this.votingElements[elementId].Votes = votes;
        if (this.GetTotalVotes() <= 0)
          return;
        foreach (TwitchPollConnection.PollVotingElement votingElement in this.votingElements)
          votingElement.Percentage = (int) Math.Round((double) votingElement.Votes / (double) this.GetTotalVotes() * 100.0);
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
        TwitchPollConnection.PollVotingElement[] array = this.votingElements.OrderByDescending<TwitchPollConnection.PollVotingElement, int>((Func<TwitchPollConnection.PollVotingElement, int>) (e =>
        {
          if (e.Votes > maxVotes)
            maxVotes = e.Votes;
          return e.Votes;
        })).Where<TwitchPollConnection.PollVotingElement>((Func<TwitchPollConnection.PollVotingElement, bool>) (e => e.Votes == maxVotes)).ToArray<TwitchPollConnection.PollVotingElement>();
        List<IVotingElement> votingElementList = new List<IVotingElement>();
        if (Config.Instance().StreamEnableMultipleEffects)
          votingElementList.AddRange((IEnumerable<IVotingElement>) array);
        else
          votingElementList.Add((IVotingElement) array[new Random().Next(((IEnumerable<TwitchPollConnection.PollVotingElement>) array).Count<TwitchPollConnection.PollVotingElement>())]);
        return votingElementList;
      }

      public List<IVotingElement> GetTrulyRandomVotes()
      {
        List<IVotingElement> votingElementList = new List<IVotingElement>();
        int maxValue = 0;
        foreach (TwitchPollConnection.PollVotingElement votingElement in this.votingElements)
          maxValue += votingElement.Votes;
        if (maxValue == 0)
          return votingElementList;
        int num = new Random().Next(maxValue) + 1;
        foreach (TwitchPollConnection.PollVotingElement votingElement in this.votingElements)
        {
          if (num > votingElement.Votes)
          {
            num -= votingElement.Votes;
          }
          else
          {
            votingElementList.Add((IVotingElement) votingElement);
            break;
          }
        }
        return votingElementList;
      }
    }

    public class PollVotingElement : IVotingElement
    {
      public int Id { get; set; }

      public AbstractEffect Effect { get; set; }

      public int Votes { get; set; }

      public int Percentage { get; set; }

      public PollVotingElement(int id, AbstractEffect effect)
      {
        this.Id = id;
        this.Effect = effect;
        this.Percentage = 0;
      }

      public int GetId() => this.Id;

      public AbstractEffect GetEffect() => this.Effect;

      public int GetVotes() => this.Votes;

      public int GetPercentage() => this.Percentage;
    }
  }
}
