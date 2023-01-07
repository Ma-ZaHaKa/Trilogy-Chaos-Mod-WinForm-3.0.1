using Flurl.Http;
using GTAChaos.Effects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace GTAChaos.Utils
{
  public class YouTubeChatConnection : IStreamConnection
  {
    private readonly string liveId;
    private string isReplay;
    private string apiKey;
    private string clientVersion;
    private string continuation;
    private bool isConnected;
    private Shared.VOTING_MODE VotingMode;
    private int lastChoice = -1;
    private readonly YouTubeChatConnection.ChatEffectVoting effectVoting = new YouTubeChatConnection.ChatEffectVoting();
    private readonly HashSet<string> rapidFireVoters = new HashSet<string>();
    private readonly System.Timers.Timer fetchMessagesTimer;

    public YouTubeChatConnection()
    {
      this.liveId = Config.Instance().StreamAccessToken;
      if (string.IsNullOrEmpty(this.liveId))
        return;
      this.fetchMessagesTimer = new System.Timers.Timer()
      {
        AutoReset = true,
        Interval = 1000.0
      };
      this.fetchMessagesTimer.Elapsed += new ElapsedEventHandler(this.FetchMessagesTimer_Elapsed);
    }

    public async Task<bool> TryConnect()
    {
      YouTubeChatConnection tubeChatConnection = this;
            // ISSUE: explicit non-virtual call
            //__nonvirtual (tubeChatConnection.Kill());
            tubeChatConnection.Kill();  //??????
            throw new Exception("not work youtube");
      bool flag = await tubeChatConnection.FetchStreamInformation();
      tubeChatConnection.isConnected = flag;
      if (!tubeChatConnection.isConnected)
      {
        EventHandler<EventArgs> onLoginError = tubeChatConnection.OnLoginError;
        if (onLoginError != null)
          onLoginError((object) tubeChatConnection, new EventArgs());
      }
      else
      {
        EventHandler<EventArgs> onConnected = tubeChatConnection.OnConnected;
        if (onConnected != null)
          onConnected((object) tubeChatConnection, new EventArgs());
        tubeChatConnection.fetchMessagesTimer.Start();
      }
      return tubeChatConnection.isConnected;
    }

    public bool IsConnected() => this.isConnected;

    private async void FetchMessagesTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (!this.isConnected)
        return;
      foreach (YouTubeChatConnection.ChatItem chatItem in await this.FetchChat())
        this.OnChatMessage(chatItem);
    }

    private string TryMatch(string data, string regex)
    {
      Match match = Regex.Match(data, regex);
      return !match.Success ? (string) null : match.Groups[1].Value;
    }

    private async Task<bool> FetchStreamInformation()
    {
      string stringAsync = await GeneratedExtensions.GetStringAsync("https://www.youtube.com/watch?v=" + this.liveId, new CancellationToken(), HttpCompletionOption.ResponseContentRead);
      this.isReplay = this.TryMatch(stringAsync, "['\"]isReplay['\"]:\\s*(true)");
      if (!string.IsNullOrEmpty(this.isReplay))
        return false;
      this.apiKey = this.TryMatch(stringAsync, "['\"]INNERTUBE_API_KEY['\"]:\\s*['\"](.+?)['\"]");
      if (string.IsNullOrEmpty(this.apiKey))
        return false;
      this.clientVersion = this.TryMatch(stringAsync, "['\"]clientVersion['\"]:\\s*['\"]([\\d.]+?)['\"]");
      if (string.IsNullOrEmpty(this.clientVersion))
        return false;
      this.continuation = this.TryMatch(stringAsync, "['\"]continuation['\"]:\\s*['\"](.+?)['\"]");
      if (string.IsNullOrEmpty(this.continuation))
        return false;
      Console.WriteLine(this.liveId);
      Console.WriteLine(this.isReplay);
      Console.WriteLine(this.apiKey);
      Console.WriteLine(this.clientVersion);
      Console.WriteLine(this.continuation);
      return true;
    }

    private async Task<List<YouTubeChatConnection.ChatItem>> FetchChat()
    {
      if (!this.isConnected)
        return (List<YouTubeChatConnection.ChatItem>) null;
      JToken jtoken1 = JObject.Parse(await ResponseExtensions.ReceiveString(GeneratedExtensions.PostJsonAsync("https://www.youtube.com/youtubei/v1/live_chat/get_live_chat?key=" + this.apiKey, (object) new
      {
        context = new
        {
          client = new
          {
            clientVersion = this.clientVersion,
            clientName = "WEB"
          }
        },
        continuation = this.continuation
      }, new CancellationToken(), HttpCompletionOption.ResponseContentRead)))["continuationContents"][(object) "liveChatContinuation"];
      JToken jtoken2 = jtoken1[(object) "continuations"][(object) 0];
      if (jtoken2[(object) "invalidationContinuationData"] != null)
        this.continuation = ((object) jtoken2[(object) "invalidationContinuationData"][(object) "continuation"]).ToString();
      else if (jtoken2[(object) "timedContinuationData"] != null)
        this.continuation = ((object) jtoken2[(object) "timedContinuationData"][(object) "continuation"]).ToString();
      return this.ParseChatMessages(jtoken1[(object) "actions"]);
    }

    private List<YouTubeChatConnection.ChatItem> ParseChatMessages(
      JToken actions)
    {
      List<YouTubeChatConnection.ChatItem> chatItemList = new List<YouTubeChatConnection.ChatItem>();
      if (actions == null)
        return chatItemList;
      foreach (JToken action in (IEnumerable<JToken>) actions)
      {
        JToken jtoken1 = action[(object) "addChatItemAction"]?[(object) "item"];
        if (jtoken1 != null)
        {
          JToken jtoken2 = (JToken) null;
          if (jtoken1[(object) "liveChatTextMessageRenderer"] != null)
            jtoken2 = jtoken1[(object) "liveChatTextMessageRenderer"];
          else if (jtoken1[(object) "liveChatPaidMessageRenderer"] != null)
            jtoken2 = jtoken1[(object) "liveChatPaidMessageRenderer"];
          else if (jtoken1[(object) "liveChatMembershipItemRenderer"] != null)
            jtoken2 = jtoken1[(object) "liveChatMembershipItemRenderer"];
          if (jtoken2 != null)
          {
            JToken jtoken3 = (JToken) null;
            if (jtoken2[(object) "message"] != null)
              jtoken3 = jtoken2[(object) "message"]?[(object) "runs"];
            else if (jtoken1[(object) "headerSubtext"] != null)
              jtoken3 = jtoken2[(object) "headerSubtext"]?[(object) "runs"];
            if (jtoken3 != null && jtoken3[(object) 0] != null)
            {
              string author = (string) null;
              if (jtoken2[(object) "authorName"]?[(object) "simpleText"] != null)
                author = ((object) jtoken2[(object) "authorName"]?[(object) "simpleText"]).ToString();
              List<string> stringList = new List<string>();
              foreach (JToken jtoken4 in (JArray) jtoken3)
              {
                if (jtoken4[(object) "text"] != null)
                  stringList.Add(((object) jtoken4[(object) "text"]).ToString());
              }
              string message = string.Join(" ", stringList.ToArray());
              if (!string.IsNullOrEmpty(author) && !string.IsNullOrEmpty(message))
                chatItemList.Add(new YouTubeChatConnection.ChatItem(author, message));
            }
          }
        }
      }
      return chatItemList;
    }

    private void OnChatMessage(YouTubeChatConnection.ChatItem chatItem)
    {
      string str1 = this.RemoveSpecialCharacters(chatItem.Author);
      string str2 = this.RemoveSpecialCharacters(chatItem.Message);
      if (this.VotingMode == Shared.VOTING_MODE.RAPID_FIRE)
      {
        if (this.rapidFireVoters.Contains(str1))
          return;
        AbstractEffect byWord = EffectDatabase.GetByWord(str2, Config.Instance().StreamAllowOnlyEnabledEffectsRapidFire);
        if (byWord == null || !byWord.IsRapidFire())
          return;
        this.RapidFireEffect(new RapidFireEventArgs()
        {
          Effect = byWord.SetSubtext(str1)
        });
        this.rapidFireVoters.Add(str1);
      }
      else
      {
        if (this.VotingMode != Shared.VOTING_MODE.VOTING)
          return;
        int userChoice = this.TryParseUserChoice(str2);
        switch (userChoice)
        {
          case 0:
          case 1:
          case 2:
            this.effectVoting?.TryAddVote(str1, userChoice);
            break;
        }
      }
    }

    public int GetRemaining() => 0;

    public List<IVotingElement> GetVotedEffects()
    {
      List<IVotingElement> source = Config.Instance().StreamMajorityVotes ? this.effectVoting.GetMajorityVotes() : this.effectVoting.GetTrulyRandomVotes();
      foreach (IVotingElement votingElement in source)
        votingElement.GetEffect().SetSubtext(string.Format("{0}%", (object) votingElement.GetPercentage()));
      this.lastChoice = source.Count > 1 ? -1 : source.First<IVotingElement>().GetId();
      return source;
    }

    public void Kill() => this.fetchMessagesTimer?.Stop();

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

    private void SendMessage(string text)
    {
    }

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
          string text = "Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects! ";
          foreach (YouTubeChatConnection.ChatVotingElement votingElement in this.effectVoting.GetVotingElements())
          {
            string displayName = votingElement.Effect.GetDisplayName(DisplayNameType.STREAM);
            text += string.Format("#{0}: {1}. ", (object) (votingElement.Id + 1), (object) displayName);
          }
          this.SendMessage(text);
        }
        else
        {
          this.SendMessage("Voting has started! Type 1, 2 or 3 (or #1, #2, #3) to vote for one of the effects!");
          foreach (YouTubeChatConnection.ChatVotingElement votingElement in this.effectVoting.GetVotingElements())
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

    private string RemoveSpecialCharacters(string text, bool noSpaces = true)
    {
      string pattern = noSpaces ? "[^A-Za-z0-9]" : "[^A-Za-z0-9 ]";
      return Regex.Replace(text, pattern, "");
    }

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

    private class ChatItem
    {
      public string Author { get; }

      public string Message { get; }

      public ChatItem(string author, string message)
      {
        this.Author = author;
        this.Message = message;
      }
    }

    private class ChatEffectVoting
    {
      private readonly List<YouTubeChatConnection.ChatVotingElement> votingElements = new List<YouTubeChatConnection.ChatVotingElement>();
      private readonly Dictionary<string, YouTubeChatConnection.ChatVotingElement> voters = new Dictionary<string, YouTubeChatConnection.ChatVotingElement>();

      public bool IsEmpty() => this.votingElements.Count == 0;

      public void Clear()
      {
        this.votingElements.Clear();
        this.voters.Clear();
      }

      public List<YouTubeChatConnection.ChatVotingElement> GetVotingElements() => this.votingElements;

      public int GetTotalVotes()
      {
        int num = 0;
        foreach (YouTubeChatConnection.ChatVotingElement votingElement in this.votingElements)
          num += votingElement.Voters.Count;
        return num;
      }

      public bool ContainsEffect(AbstractEffect effect) => this.votingElements.Any<YouTubeChatConnection.ChatVotingElement>((Func<YouTubeChatConnection.ChatVotingElement, bool>) (e => e.Effect.GetDisplayName(DisplayNameType.STREAM).Equals(effect.GetDisplayName(DisplayNameType.STREAM))));

      public void AddEffect(AbstractEffect effect) => this.votingElements.Add(new YouTubeChatConnection.ChatVotingElement(this.votingElements.Count, effect));

      public void GetVotes(out string[] effects, out int[] votes, bool undetermined = false)
      {
        undetermined = false;
        YouTubeChatConnection.ChatVotingElement[] array = this.GetVotingElements().ToArray();
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
        YouTubeChatConnection.ChatVotingElement[] array = this.votingElements.OrderByDescending<YouTubeChatConnection.ChatVotingElement, int>((Func<YouTubeChatConnection.ChatVotingElement, int>) (e =>
        {
          if (e.Voters.Count > maxVotes)
            maxVotes = e.Voters.Count;
          return e.Voters.Count;
        })).Where<YouTubeChatConnection.ChatVotingElement>((Func<YouTubeChatConnection.ChatVotingElement, bool>) (e => e.Voters.Count == maxVotes)).ToArray<YouTubeChatConnection.ChatVotingElement>();
        List<IVotingElement> votingElementList = new List<IVotingElement>();
        if (Config.Instance().StreamEnableMultipleEffects)
          votingElementList.AddRange((IEnumerable<IVotingElement>) array);
        else
          votingElementList.Add((IVotingElement) array[new Random().Next(((IEnumerable<YouTubeChatConnection.ChatVotingElement>) array).Count<YouTubeChatConnection.ChatVotingElement>())]);
        return votingElementList;
      }

      public List<IVotingElement> GetTrulyRandomVotes()
      {
        List<IVotingElement> votingElementList = new List<IVotingElement>();
        int maxValue = 0;
        foreach (YouTubeChatConnection.ChatVotingElement votingElement in this.votingElements)
          maxValue += votingElement.Voters.Count;
        if (maxValue == 0)
          return votingElementList;
        int num = new Random().Next(maxValue) + 1;
        foreach (YouTubeChatConnection.ChatVotingElement votingElement in this.votingElements)
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
        foreach (YouTubeChatConnection.ChatVotingElement votingElement in this.votingElements)
          votingElement.RemoveVoter(username);
        this.votingElements[effectChoice].AddVoter(username);
        this.voters[username] = this.votingElements[effectChoice];
        foreach (YouTubeChatConnection.ChatVotingElement votingElement in this.votingElements)
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
