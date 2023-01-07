using GTAChaos.Effects;
using Newtonsoft.Json;
using System;
using WebSocketSharp;

namespace GTAChaos.Utils
{
  public class Sync
  {
    private readonly string Channel;
    private readonly string Username;
    private bool ManualClose;
    private readonly WebSocket socket;
    private DateTime lastTimeUpdate;
    private DateTime lastVotesUpdate;

    public event EventHandler<ConnectionFailedEventArgs> OnConnectionFailed;

    public event EventHandler<UsernameInUseEventArgs> OnUsernameInUse;

    public event EventHandler<ConnectionSuccessfulEventArgs> OnConnectionSuccessful;

    public event EventHandler<HostLeftChannelEventArgs> OnHostLeftChannel;

    public event EventHandler<VersionMismatchEventArgs> OnVersionMismatch;

    public event EventHandler<UserJoinedEventArgs> OnUserJoined;

    public event EventHandler<UserLeftEventArgs> OnUserLeft;

    public event EventHandler<ChatMessageEventArgs> OnChatMessage;

    public event EventHandler<TimeUpdateEventArgs> OnTimeUpdate;

    public event EventHandler<EffectEventArgs> OnEffect;

    public event EventHandler<VotesEventArgs> OnVotes;

    public bool IsHost { get; private set; }

    public Sync(string Server, string Channel, string Username)
    {
      this.Channel = Channel;
      this.Username = Username;
      if (!Server.StartsWith("ws://") && !Server.StartsWith("wss://"))
        Server = "ws://" + Server;
      this.socket = new WebSocket(Server, Array.Empty<string>());
      this.socket.OnOpen += (EventHandler) ((sender, e) => this.socket.Send(JsonConvert.SerializeObject((object) new MessageConnect()
      {
        Channel = this.Channel,
        Username = this.Username,
        Version = Shared.Version
      })));
      this.socket.OnClose += (EventHandler<CloseEventArgs>) ((sender, e) =>
      {
        if (this.ManualClose || string.IsNullOrWhiteSpace(e.Reason))
          return;
        EventHandler<ConnectionFailedEventArgs> connectionFailed = this.OnConnectionFailed;
        if (connectionFailed == null)
          return;
        connectionFailed((object) this, new ConnectionFailedEventArgs());
      });
      this.socket.OnMessage += (EventHandler<MessageEventArgs>) ((sender, e) =>
      {
        MessageType messageType = JsonConvert.DeserializeObject<MessageType>(e.Data);
        if (messageType.Type == 0)
        {
          MessageConnectionSuccessful connectionSuccessful1 = JsonConvert.DeserializeObject<MessageConnectionSuccessful>(e.Data);
          this.IsHost = connectionSuccessful1.IsHost;
          ConnectionSuccessfulEventArgs e1 = new ConnectionSuccessfulEventArgs()
          {
            IsHost = connectionSuccessful1.IsHost,
            HostUsername = connectionSuccessful1.HostUsername
          };
          EventHandler<ConnectionSuccessfulEventArgs> connectionSuccessful2 = this.OnConnectionSuccessful;
          if (connectionSuccessful2 == null)
            return;
          connectionSuccessful2((object) this, e1);
        }
        else if (messageType.Type == 1)
        {
          EventHandler<UsernameInUseEventArgs> onUsernameInUse = this.OnUsernameInUse;
          if (onUsernameInUse != null)
            onUsernameInUse((object) this, new UsernameInUseEventArgs());
          this.socket.Close();
        }
        else if (messageType.Type == 2)
        {
          EventHandler<HostLeftChannelEventArgs> onHostLeftChannel = this.OnHostLeftChannel;
          if (onHostLeftChannel != null)
            onHostLeftChannel((object) this, new HostLeftChannelEventArgs());
          this.socket.Close();
        }
        else if (messageType.Type == 3)
        {
          MessageVersionMismatch messageVersionMismatch = JsonConvert.DeserializeObject<MessageVersionMismatch>(e.Data);
          VersionMismatchEventArgs e2 = new VersionMismatchEventArgs()
          {
            Version = messageVersionMismatch.Version
          };
          EventHandler<VersionMismatchEventArgs> onVersionMismatch = this.OnVersionMismatch;
          if (onVersionMismatch == null)
            return;
          onVersionMismatch((object) this, e2);
        }
        else if (messageType.Type == 10)
        {
          MessageUserJoined messageUserJoined = JsonConvert.DeserializeObject<MessageUserJoined>(e.Data);
          UserJoinedEventArgs e3 = new UserJoinedEventArgs()
          {
            Username = messageUserJoined.Username
          };
          EventHandler<UserJoinedEventArgs> onUserJoined = this.OnUserJoined;
          if (onUserJoined == null)
            return;
          onUserJoined((object) this, e3);
        }
        else if (messageType.Type == 11)
        {
          MessageUserLeft messageUserLeft = JsonConvert.DeserializeObject<MessageUserLeft>(e.Data);
          UserLeftEventArgs e4 = new UserLeftEventArgs()
          {
            Username = messageUserLeft.Username
          };
          EventHandler<UserLeftEventArgs> onUserLeft = this.OnUserLeft;
          if (onUserLeft == null)
            return;
          onUserLeft((object) this, e4);
        }
        else if (messageType.Type == 12)
        {
          MessageChatMessage messageChatMessage = JsonConvert.DeserializeObject<MessageChatMessage>(e.Data);
          ChatMessageEventArgs e5 = new ChatMessageEventArgs()
          {
            Username = messageChatMessage.Username,
            Message = messageChatMessage.Message
          };
          EventHandler<ChatMessageEventArgs> onChatMessage = this.OnChatMessage;
          if (onChatMessage == null)
            return;
          onChatMessage((object) this, e5);
        }
        else if (messageType.Type == 20)
        {
          MessageTimeUpdate messageTimeUpdate = JsonConvert.DeserializeObject<MessageTimeUpdate>(e.Data);
          TimeUpdateEventArgs e6 = new TimeUpdateEventArgs()
          {
            Remaining = messageTimeUpdate.Remaining,
            Total = messageTimeUpdate.Total
          };
          EventHandler<TimeUpdateEventArgs> onTimeUpdate = this.OnTimeUpdate;
          if (onTimeUpdate == null)
            return;
          onTimeUpdate((object) this, e6);
        }
        else if (messageType.Type == 21)
        {
          MessageEffect messageEffect = JsonConvert.DeserializeObject<MessageEffect>(e.Data);
          EffectEventArgs e7 = new EffectEventArgs()
          {
            Word = messageEffect.Word,
            Duration = messageEffect.Duration,
            Subtext = messageEffect.Subtext,
            Seed = messageEffect.Seed
          };
          EventHandler<EffectEventArgs> onEffect = this.OnEffect;
          if (onEffect == null)
            return;
          onEffect((object) this, e7);
        }
        else
        {
          if (messageType.Type != 22)
            return;
          MessageVotes messageVotes = JsonConvert.DeserializeObject<MessageVotes>(e.Data);
          VotesEventArgs e8 = new VotesEventArgs()
          {
            Effects = messageVotes.Effects,
            Votes = messageVotes.Votes,
            LastChoice = messageVotes.LastChoice
          };
          EventHandler<VotesEventArgs> onVotes = this.OnVotes;
          if (onVotes == null)
            return;
          onVotes((object) this, e8);
        }
      });
    }

    public bool IsAlive() => this.socket?.IsAlive ?? false;

    public void Connect() => this.socket?.Connect();

    public void Disconnect()
    {
      this.ManualClose = true;
      this.socket?.Close();
    }

    private void SendToSocket(string data)
    {
      if (!this.IsAlive())
      {
        this.Connect();
        if (!this.IsAlive())
          return;
      }
      this.socket?.Send(data);
    }

    public void SendChatMessage(string message) => this.SendToSocket(JsonConvert.SerializeObject((object) new MessageChatMessage()
    {
      Username = this.Username,
      Message = message
    }));

    public void SendTimeUpdate(int remaining, int total)
    {
      DateTime now = DateTime.Now;
      if (!(this.lastTimeUpdate < now))
        return;
      this.lastTimeUpdate = now.AddMilliseconds(500.0);
      this.SendToSocket(JsonConvert.SerializeObject((object) new MessageTimeUpdate()
      {
        Remaining = remaining,
        Total = total
      }));
    }

    public void SendEffect(AbstractEffect effect, int _duration = -1)
    {
      int duration = effect.GetDuration(_duration);
      this.SendToSocket(JsonConvert.SerializeObject((object) new MessageEffect()
      {
        Word = effect.Word,
        Duration = duration,
        Subtext = effect.GetSubtext(),
        Seed = RandomHandler.Next(9999999)
      }));
    }

    public void SendVotes(string[] effects, int[] votes, int lastChoice, bool force = false)
    {
      DateTime now = DateTime.Now;
      if (!(this.lastVotesUpdate < now | force))
        return;
      this.lastVotesUpdate = now.AddSeconds(1.0);
      this.SendToSocket(JsonConvert.SerializeObject((object) new MessageVotes()
      {
        Effects = effects,
        Votes = votes,
        LastChoice = lastChoice
      }));
    }
  }
}
