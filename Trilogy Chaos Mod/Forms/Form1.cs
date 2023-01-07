// Decompiled with JetBrains decompiler
// Type: GTAChaos.Forms.Form1
// Assembly: GTAChaos, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8BFBC3C1-BC3A-4B92-A6C3-E2FA96115BE2
// Assembly location: C:\Users\MaZaHaKa\Desktop\chaos-mod-3-0-1_1662193835_813494\Trilogy Chaos Mod.exe

using GTAChaos.Effects;
using GTAChaos.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace GTAChaos.Forms
{
    public class Form1 : Form
    {
        private readonly string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.cfg");
        private readonly Stopwatch stopwatch;
        private readonly Dictionary<string, Form1.EffectTreeNode> idToEffectNodeMap = new Dictionary<string, Form1.EffectTreeNode>();
        private IStreamConnection stream;
        private readonly System.Timers.Timer websocketReconnectionTimer;
        private int elapsedCount;
        private int timesUntilRapidFire;
        private readonly bool debug;
        private IContainer components;
        private Button buttonMainToggle;
        private ProgressBar progressBarMain;
        private TabControl tabs;
        private TabPage tabMain;
        private TabPage tabEffects;
        private TreeView enabledEffectsView;
        private ListBox listLastEffectsMain;
        private ComboBox comboBoxMainCooldown;
        private Label label2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadPresetToolStripMenuItem;
        private ToolStripMenuItem savePresetToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private TabPage tabStream;
        private TextBox textBoxStreamAccessToken;
        private Button buttonConnectStream;
        private ListBox listLastEffectsStream;
        private ToolTip toolTipHandler;
        private ComboBox comboBoxVotingCooldown;
        private Label label7;
        private Label label6;
        private ComboBox comboBoxVotingTime;
        private ProgressBar progressBarStream;
        private Button buttonStreamToggle;
        private System.Windows.Forms.Timer timerMain;
        private Button buttonSwitchMode;
        private Label labelStreamCurrentMode;
        private TabPage tabSettings;
        private Label label8;
        private TextBox textBoxSeed;
        private Button buttonResetMain;
        private CheckBox checkBoxShowLastEffectsMain;
        private CheckBox checkBoxShowLastEffectsStream;
        private CheckBox checkBoxStreamAllowOnlyEnabledEffects;
        private Button buttonResetStream;
        private CheckBox checkBoxStream3TimesCooldown;
        private Button buttonEffectsToggleAll;
        private ToolStripMenuItem gameToolStripMenuItem;
        private ToolStripMenuItem viceCityToolStripMenuItem;
        private ToolStripMenuItem sanAndreasToolStripMenuItem;
        private CheckBox checkBoxStreamEnableMultipleEffects;
        private TabPage tabPolls;
        private Label labelTwitchPollsBitsCost;
        private NumericUpDown numericUpDownTwitchPollsBitsCost;
        private CheckBox checkBoxTwitchPollsPostMessages;
        private CheckBox checkBoxPlayAudioForEffects;
        private TabPage tabSync;
        private TextBox textBoxSyncUsername;
        private Label labelSyncUsername;
        private Label labelSyncChannel;
        private TextBox textBoxSyncChannel;
        private Label labelSyncServer;
        private TextBox textBoxSyncServer;
        private Button buttonSyncConnect;
        private Label labelSyncHost;
        private Button buttonSyncSend;
        private TextBox textBoxSyncChat;
        private ListBox listBoxSyncChat;
        private TabPage tabExperimental;
        private ToolStripMenuItem experimentalToolStripMenuItem;
        private CheckBox checkBoxExperimental_RunEffectOnAutoStart;
        private Button buttonExperimentalRunEffect;
        private TextBox textBoxExperimentalEffectName;
        private CheckBox checkBoxAutoStart;
        private CheckBox checkBoxStreamEnableRapidFire;
        private CheckBox checkBoxStreamCombineVotingMessages;
        private CheckBox checkBoxTwitchUsePolls;
        private CheckBox checkBoxStreamMajorityVotes;
        private Label label1;
        private NumericUpDown numericUpDownTwitchPollsChannelPointsCost;
        private LinkLabel linkLabelTwitchGetAccessToken;
        private Label labelTwitchAccessToken;
        private CheckBox checkBoxExperimentalYouTubeConnection;
        private Label label3;
        private TextBox textBoxStreamClientID;
        private CheckBox checkBoxSettingsPlayAudioSequentially;
        private Label labelEffectCooldown;
        private NumericUpDown numericUpDownEffectCooldown;
        private CheckBox checkBoxStreamHideVotingEffectsIngame;
        private Button connectb;
        private Label label17;
        private Label label16;
        private NumericUpDown port;
        private TextBox ip;
        private Label label19;
        private Label label4;
        private Label label5;
        private NumericUpDown gtaport;
        private TextBox gtaip;
        private Label label9;
        private Button button1;
        private ComboBox effect;
        private Button button2;
        private ToolStripMenuItem aboutToolStripMenuItem;

        public Form1()
        {
            this.InitializeComponent();
            if (this.debug)
            {
                this.gameToolStripMenuItem.Visible = false;
                this.tabs.TabPages.Remove(this.tabExperimental);
                this.textBoxExperimentalEffectName.Visible = false;
                this.buttonExperimentalRunEffect.Visible = false;
            }
            else
                Shared.Version += " (DEBUG)";
            this.Text = "GTA Trilogy Chaos Mod v" + Shared.Version;
            this.tabs.TabPages.Remove(this.tabPolls);
            this.stopwatch = new Stopwatch();
            EffectDatabase.PopulateEffects("san_andreas");
            this.PopulateEffectTreeList();
            this.PopulateMainCooldowns();
            this.PopulatePresets();
            this.tabs.TabPages.Remove(this.tabStream);
            this.PopulateVotingTimes();
            this.PopulateVotingCooldowns();
            this.TryLoadConfig();
            this.timesUntilRapidFire = new Random().Next(10, 15);
            this.numericUpDownEffectCooldown.Maximum = (Decimal)EffectDatabase.Effects.Count;
            WebsocketHandler.INSTANCE.ConnectWebsocket();
            WebsocketHandler.INSTANCE.OnSocketMessage += new EventHandler<SocketMessageEventArgs>(this.OnSocketMessage);
            this.websocketReconnectionTimer = new System.Timers.Timer()
            {
                Interval = 1000.0,
                AutoReset = true
            };
            this.websocketReconnectionTimer.Elapsed += (ElapsedEventHandler)((sender, e) => WebsocketHandler.INSTANCE.ConnectWebsocket());
            this.websocketReconnectionTimer.Start();
        }

        private void OnSocketMessage(object sender, SocketMessageEventArgs e)
        {
            try
            {
                JObject jobject = JObject.Parse(e.Data);
                string str1 = Convert.ToString((object)jobject["type"]);
                string str2 = Convert.ToString((object)jobject["state"]);
                if (!(str1 == "ChaosMod") || !(str2 == "auto_start") || Shared.Sync != null && !Shared.Sync.IsHost)
                    return;
                this.Invoke((Action)(() => this.DoAutostart()));
            }
            catch (Exception ex)
            {
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) => this.SaveConfig();

        private void TryLoadConfig()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(this.configPath))
                {
                    using (JsonReader jsonReader = (JsonReader)new JsonTextReader((TextReader)streamReader))
                    {
                        Config.SetInstance(new JsonSerializer().Deserialize<Config>(jsonReader));
                        RandomHandler.SetSeed(Config.Instance().Seed);
                        AudioPlayer.INSTANCE.SetAudioVolume(Config.Instance().AudioVolume);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            this.LoadPreset(Config.Instance().EnabledEffects);
            this.UpdateInterface();
        }

        private void SaveConfig()
        {
            try
            {
                Config.Instance().AudioVolume = AudioPlayer.INSTANCE.GetAudioVolume();
                Config.Instance().EnabledEffects.Clear();
                foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.Effects.Get())
                {
                    bool flag = EffectDatabase.EnabledEffects.Contains(entry);
                    Config.Instance().EnabledEffects.Add(entry.item.GetID(), flag);
                }
                using (StreamWriter streamWriter = new StreamWriter(this.configPath))
                {
                    using (JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter)streamWriter))
                        new JsonSerializer().Serialize((JsonWriter)jsonTextWriter, (object)Config.Instance());
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateInterface()
        {
            bool flag1 = false;
            foreach (Form1.MainCooldownComboBoxItem cooldownComboBoxItem in this.comboBoxMainCooldown.Items)
            {
                if (cooldownComboBoxItem.Time == Config.Instance().MainCooldown)
                {
                    this.comboBoxMainCooldown.SelectedItem = (object)cooldownComboBoxItem;
                    flag1 = true;
                    break;
                }
            }
            if (!flag1)
            {
                this.comboBoxMainCooldown.SelectedIndex = 3;
                Config.Instance().MainCooldown = 60000;
            }
            this.checkBoxAutoStart.Checked = Config.Instance().AutoStart;
            this.checkBoxStreamAllowOnlyEnabledEffects.Checked = Config.Instance().StreamAllowOnlyEnabledEffectsRapidFire;
            bool flag2 = false;
            foreach (Form1.VotingTimeComboBoxItem timeComboBoxItem in this.comboBoxVotingTime.Items)
            {
                if (timeComboBoxItem.VotingTime == Config.Instance().StreamVotingTime)
                {
                    this.comboBoxVotingTime.SelectedItem = (object)timeComboBoxItem;
                    flag2 = true;
                    break;
                }
            }
            if (!flag2)
            {
                this.comboBoxVotingTime.SelectedIndex = 0;
                Config.Instance().StreamVotingTime = 30000;
            }
            bool flag3 = false;
            foreach (Form1.VotingCooldownComboBoxItem cooldownComboBoxItem in this.comboBoxVotingCooldown.Items)
            {
                if (cooldownComboBoxItem.VotingCooldown == Config.Instance().StreamVotingCooldown)
                {
                    this.comboBoxVotingCooldown.SelectedItem = (object)cooldownComboBoxItem;
                    flag3 = true;
                    break;
                }
            }
            if (!flag3)
            {
                this.comboBoxVotingCooldown.SelectedIndex = 1;
                Config.Instance().StreamVotingCooldown = 60000;
            }
            this.textBoxStreamAccessToken.Text = Config.Instance().StreamAccessToken;
            this.textBoxStreamClientID.Text = Config.Instance().StreamClientID;
            this.checkBoxPlayAudioForEffects.Checked = Config.Instance().PlayAudioForEffects;
            this.checkBoxSettingsPlayAudioSequentially.Enabled = Config.Instance().PlayAudioForEffects;
            this.checkBoxSettingsPlayAudioSequentially.Checked = Config.Instance().PlayAudioSequentially;
            this.checkBoxShowLastEffectsMain.Checked = Config.Instance().MainShowLastEffects;
            this.checkBoxShowLastEffectsStream.Checked = Config.Instance().StreamShowLastEffects;
            this.checkBoxStream3TimesCooldown.Checked = Config.Instance().Stream3TimesCooldown;
            this.checkBoxStreamCombineVotingMessages.Checked = Config.Instance().StreamCombineChatMessages;
            this.checkBoxStreamEnableMultipleEffects.Checked = Config.Instance().StreamEnableMultipleEffects;
            this.checkBoxStreamEnableRapidFire.Checked = Config.Instance().StreamEnableRapidFire;
            this.checkBoxStreamHideVotingEffectsIngame.Checked = Config.Instance().StreamHideVotingEffectsIngame;
            this.checkBoxStreamMajorityVotes.Checked = Config.Instance().StreamMajorityVotes;
            this.checkBoxStreamEnableMultipleEffects.Enabled = Config.Instance().StreamMajorityVotes;
            this.checkBoxTwitchUsePolls.Checked = Config.Instance().TwitchUsePolls;
            this.checkBoxTwitchPollsPostMessages.Checked = Config.Instance().TwitchPollsPostMessages;
            this.numericUpDownTwitchPollsBitsCost.Value = (Decimal)Config.Instance().TwitchPollsBitsCost;
            this.numericUpDownTwitchPollsChannelPointsCost.Value = (Decimal)Config.Instance().TwitchPollsChannelPointsCost;
            this.textBoxSyncServer.Text = Config.Instance().SyncServer;
            this.textBoxSyncChannel.Text = Config.Instance().SyncChannel;
            this.textBoxSyncUsername.Text = Config.Instance().SyncUsername;
            this.checkBoxExperimental_RunEffectOnAutoStart.Checked = Config.Instance().Experimental_RunEffectOnAutoStart;
            this.textBoxExperimentalEffectName.Text = Config.Instance().Experimental_EffectName;
            this.checkBoxExperimentalYouTubeConnection.Checked = Config.Instance().Experimental_YouTubeConnection;
            this.numericUpDownEffectCooldown.Value = Math.Min((Decimal)Config.Instance().EffectsCooldownNotActivating, this.numericUpDownEffectCooldown.Maximum);
            this.textBoxSeed.Text = Config.Instance().Seed;
        }

        public void AddEffectToListBox(AbstractEffect effect)
        {
            string str = "Invalid";
            if (effect != null)
            {
                str = effect.GetDisplayName(DisplayNameType.UI);
                if (!string.IsNullOrEmpty(effect.Word))
                    str = str + " (" + effect.Word + ")";
            }
            ListBox listBox = Shared.IsStreamMode ? this.listLastEffectsStream : this.listLastEffectsMain;
            listBox.Items.Insert(0, (object)str);
            int index = (int)Math.Floor((double)listBox.Height / (double)listBox.ItemHeight) - 1;
            if (listBox.Items.Count <= index)
                return;
            listBox.Items.RemoveAt(index);
        }

        private void CallEffect(AbstractEffect effect = null, bool is_youtube = false, string youtube_name = "")
        {
            if ((effect == null) && is_youtube)
            {
                effect = EffectDatabase.GetRandomEffect(true, is_youtube: is_youtube, youtube: youtube_name);
                if (effect == null)
                    effect = EffectDatabase.GetRandomEffect();
                if (effect == null)
                    return;
                if (Shared.Sync != null && !(effect is RapidFireEffect))
                {
                    Shared.Sync.SendEffect(effect);
                }
                else
                {
                    effect = EffectDatabase.RunEffect(effect);
                    effect?.ResetSubtext();
                    if (effect == null)
                        return;
                    this.AddEffectToListBox(effect);
                }
            }
            else if (effect == null)
            {
                effect = EffectDatabase.GetRandomEffect(true);
                if (effect == null)
                    effect = EffectDatabase.GetRandomEffect();
                if (effect == null)
                    return;
                if (Shared.Sync != null && !(effect is RapidFireEffect))
                {
                    Shared.Sync.SendEffect(effect);
                }
                else
                {
                    effect = EffectDatabase.RunEffect(effect);
                    effect?.ResetSubtext();
                    if (effect == null)
                        return;
                    this.AddEffectToListBox(effect);
                }
            }
            else if (Shared.Sync != null && !(effect is RapidFireEffect))
            {
                Shared.Sync.SendEffect(effect);
            }
            else
            {
                EffectDatabase.RunEffect(effect);
                this.AddEffectToListBox(effect);
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (Shared.IsStreamMode)
                this.TickStream();
            else
                this.TickMain();
        }

        private void TickMain()   //MAIN
        {
            if (!Shared.TimerEnabled)
                return;
            int val1 = Math.Max(1, (int)this.stopwatch.ElapsedMilliseconds);
            this.progressBarMain.Value = Math.Min(val1, this.progressBarMain.Maximum);
            this.progressBarMain.Value = Math.Min(val1 - 1, this.progressBarMain.Maximum);
            int remaining = (int)Math.Max(0L, (long)Config.Instance().MainCooldown - this.stopwatch.ElapsedMilliseconds);
            WebsocketHandler.INSTANCE.SendTimeToGame(remaining, Config.Instance().MainCooldown);
            if (this.stopwatch.ElapsedMilliseconds - (long)this.elapsedCount > 100L)
            {
                Shared.Sync?.SendTimeUpdate(remaining, Config.Instance().MainCooldown);
                this.elapsedCount = (int)this.stopwatch.ElapsedMilliseconds;
            }
            if (this.stopwatch.ElapsedMilliseconds < (long)Config.Instance().MainCooldown)
                return;
            this.progressBarMain.Value = 0;
            this.CallEffect();
            this.elapsedCount = 0;
            this.stopwatch.Restart();
        }

        private void TickStream()
        {
            if (!Shared.TimerEnabled)
                return;
            switch (Shared.StreamVotingMode)
            {
                case Shared.VOTING_MODE.COOLDOWN:
                    if (this.progressBarStream.Maximum != Config.Instance().StreamVotingCooldown)
                        this.progressBarStream.Maximum = Config.Instance().StreamVotingCooldown;
                    int val1 = Math.Max(1, (int)this.stopwatch.ElapsedMilliseconds);
                    this.progressBarStream.Value = Math.Min(val1 + 1, this.progressBarStream.Maximum);
                    this.progressBarStream.Value = Math.Min(val1, this.progressBarStream.Maximum);
                    int remaining1 = (int)Math.Max(0L, (long)Config.Instance().StreamVotingCooldown - this.stopwatch.ElapsedMilliseconds);
                    WebsocketHandler.INSTANCE.SendTimeToGame(remaining1, Config.Instance().StreamVotingCooldown, "Cooldown");
                    if (this.stopwatch.ElapsedMilliseconds - (long)this.elapsedCount > 100L)
                    {
                        Shared.Sync?.SendTimeUpdate(remaining1, Config.Instance().StreamVotingCooldown);
                        this.elapsedCount = (int)this.stopwatch.ElapsedMilliseconds;
                    }
                    if (this.stopwatch.ElapsedMilliseconds < (long)Config.Instance().StreamVotingCooldown)
                        break;
                    this.elapsedCount = 0;
                    if (Config.Instance().StreamEnableRapidFire)
                        --this.timesUntilRapidFire;
                    if (this.timesUntilRapidFire == 0 && Config.Instance().StreamEnableRapidFire)
                    {
                        this.progressBarStream.Value = this.progressBarStream.Maximum = 10000;
                        this.timesUntilRapidFire = new Random().Next(10, 15);
                        Shared.StreamVotingMode = Shared.VOTING_MODE.RAPID_FIRE;
                        this.labelStreamCurrentMode.Text = "Current Mode: Rapid-Fire";
                        this.stream?.SetVoting(Shared.VOTING_MODE.RAPID_FIRE, this.timesUntilRapidFire);
                    }
                    else
                    {
                        this.progressBarStream.Value = this.progressBarStream.Maximum = Config.Instance().StreamVotingTime;
                        Shared.StreamVotingMode = Shared.VOTING_MODE.VOTING;
                        this.labelStreamCurrentMode.Text = "Current Mode: Voting";
                        this.stream?.SetVoting(Shared.VOTING_MODE.VOTING, this.timesUntilRapidFire);
                    }
                    this.stopwatch.Restart();
                    break;
                case Shared.VOTING_MODE.VOTING:
                    if (this.progressBarStream.Maximum != Config.Instance().StreamVotingTime)
                        this.progressBarStream.Maximum = Config.Instance().StreamVotingTime;
                    int num1 = Math.Max(1, (int)this.stopwatch.ElapsedMilliseconds);
                    this.progressBarStream.Value = Math.Max(this.progressBarStream.Maximum - num1, 0);
                    this.progressBarStream.Value = Math.Max(this.progressBarStream.Maximum - num1 - 1, 0);
                    int remaining2 = (int)Math.Max(0L, (long)Config.Instance().StreamVotingTime - this.stopwatch.ElapsedMilliseconds);
                    WebsocketHandler.INSTANCE.SendTimeToGame(remaining2, Config.Instance().StreamVotingTime, "Voting");
                    if (this.stopwatch.ElapsedMilliseconds - (long)this.elapsedCount > 100L)
                    {
                        Shared.Sync?.SendTimeUpdate(remaining2, Config.Instance().StreamVotingTime);
                        this.stream?.SendEffectVotingToGame(Config.Instance().StreamHideVotingEffectsIngame);
                        this.elapsedCount = (int)this.stopwatch.ElapsedMilliseconds;
                    }
                    bool flag1;
                    if (Config.Instance().TwitchUsePolls && this.stream != null && !Config.Instance().Experimental_YouTubeConnection)
                    {
                        flag1 = this.stream.GetRemaining() == 0;
                        if (this.stopwatch.ElapsedMilliseconds >= (long)Config.Instance().StreamVotingTime)
                        {
                            int num2 = Math.Max(0, 15000 - Decimal.ToInt32((Decimal)(this.stopwatch.ElapsedMilliseconds - (long)Config.Instance().StreamVotingTime)));
                            this.labelStreamCurrentMode.Text = string.Format("Current Mode: Waiting For Poll... ({0}s left)", (object)Math.Ceiling((double)num2 / 1000.0));
                            if (num2 == 0)
                            {
                                this.labelStreamCurrentMode.Text = "Current Mode: Cooldown (Poll Failed)";
                                WebsocketHandler.INSTANCE.SendTimeToGame(0);
                                Shared.Sync?.SendTimeUpdate(0, Config.Instance().StreamVotingCooldown);
                                this.elapsedCount = 0;
                                this.progressBarStream.Value = 0;
                                this.progressBarStream.Maximum = Config.Instance().StreamVotingCooldown;
                                this.stopwatch.Restart();
                                Shared.StreamVotingMode = Shared.VOTING_MODE.COOLDOWN;
                                this.stream?.SetVoting(Shared.VOTING_MODE.ERROR, this.timesUntilRapidFire);
                                break;
                            }
                        }
                    }
                    else
                        flag1 = this.stopwatch.ElapsedMilliseconds >= (long)Config.Instance().StreamVotingTime;
                    if (!flag1)
                        break;
                    WebsocketHandler.INSTANCE.SendTimeToGame(0);
                    Shared.Sync?.SendTimeUpdate(0, Config.Instance().StreamVotingCooldown);
                    this.elapsedCount = 0;
                    this.progressBarStream.Value = 0;
                    this.progressBarStream.Maximum = Config.Instance().StreamVotingCooldown;
                    this.stopwatch.Restart();
                    Shared.StreamVotingMode = Shared.VOTING_MODE.COOLDOWN;
                    this.labelStreamCurrentMode.Text = "Current Mode: Cooldown";
                    if (this.stream == null)
                        break;
                    List<IVotingElement> votedEffects = this.stream.GetVotedEffects();
                    bool flag2 = true;
                    foreach (IVotingElement votingElement in votedEffects)
                    {
                        if (votingElement.GetVotes() > 0)
                            flag2 = false;
                    }
                    if (!flag2)
                    {
                        foreach (IVotingElement votingElement in votedEffects)
                        {
                            float multiplier = votingElement.GetEffect().GetMultiplier();
                            votingElement.GetEffect().SetMultiplier(multiplier / (float)votedEffects.Count);
                            this.CallEffect(votingElement.GetEffect());
                            votingElement.GetEffect().SetMultiplier(multiplier);
                        }
                    }
                    else
                    {
                        int index = RandomHandler.Next(votedEffects.Count);
                        AbstractEffect effect = votedEffects[index].GetEffect();
                        effect.ResetSubtext();
                        this.CallEffect(effect);
                    }
                    this.stream.SendEffectVotingToGame(false);
                    this.stream.SetVoting(Shared.VOTING_MODE.COOLDOWN, this.timesUntilRapidFire, votedEffects);
                    break;
                case Shared.VOTING_MODE.RAPID_FIRE:
                    if (this.progressBarStream.Maximum != 10000)
                        this.progressBarStream.Maximum = 10000;
                    int num3 = Math.Max(1, (int)this.stopwatch.ElapsedMilliseconds);
                    this.progressBarStream.Value = Math.Max(this.progressBarStream.Maximum - num3, 0);
                    this.progressBarStream.Value = Math.Max(this.progressBarStream.Maximum - num3 - 1, 0);
                    int remaining3 = (int)Math.Max(0L, 10000L - this.stopwatch.ElapsedMilliseconds);
                    WebsocketHandler.INSTANCE.SendTimeToGame(remaining3, 10000, "Rapid-Fire");
                    if (this.stopwatch.ElapsedMilliseconds - (long)this.elapsedCount > 100L)
                    {
                        Shared.Sync?.SendTimeUpdate(remaining3, 10000);
                        this.elapsedCount = (int)this.stopwatch.ElapsedMilliseconds;
                    }
                    if (this.stopwatch.ElapsedMilliseconds < 10000L)
                        break;
                    WebsocketHandler.INSTANCE.SendTimeToGame(0);
                    Shared.Sync?.SendTimeUpdate(0, Config.Instance().StreamVotingCooldown);
                    this.elapsedCount = 0;
                    this.progressBarStream.Value = 0;
                    this.progressBarStream.Maximum = Config.Instance().StreamVotingCooldown;
                    this.stopwatch.Restart();
                    Shared.StreamVotingMode = Shared.VOTING_MODE.COOLDOWN;
                    this.labelStreamCurrentMode.Text = "Current Mode: Cooldown";
                    this.stream?.SetVoting(Shared.VOTING_MODE.COOLDOWN, this.timesUntilRapidFire);
                    break;
            }
        }

        private void PopulateEffectTreeList()
        {
            this.enabledEffectsView.Nodes.Clear();
            this.idToEffectNodeMap.Clear();
            foreach (Category category in Category.Categories)
            {
                if (category.GetEffectCount() > 0)
                    this.enabledEffectsView.Nodes.Add((TreeNode)new Form1.CategoryTreeNode(category));
            }
            foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.Effects.Get())
            {
                AbstractEffect effect = entry.item;
                if (this.idToEffectNodeMap.ContainsKey(effect.GetID()))
                {
                    int num = (int)MessageBox.Show("Tried adding effect with ID that was already present: '" + effect.GetID() + "'");
                }
                TreeNode treeNode = ((IEnumerable<TreeNode>)this.enabledEffectsView.Nodes.Find(effect.Category.Name, false)).FirstOrDefault<TreeNode>();
                string displayName = effect.GetDisplayName(DisplayNameType.UI);
                Form1.EffectTreeNode effectTreeNode1 = new Form1.EffectTreeNode(effect, displayName);
                effectTreeNode1.Checked = true;
                Form1.EffectTreeNode effectTreeNode2 = effectTreeNode1;
                treeNode.Nodes.Add((TreeNode)effectTreeNode2);
                this.idToEffectNodeMap[effect.GetID()] = effectTreeNode2;
            }
        }

        private void PopulatePresets()
        {
            foreach (Form1.CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.Checked = true;
                this.CheckAllChildNodes((TreeNode)node, true);
                node.UpdateCategory();
            }
        }

        private void PopulateMainCooldowns()
        {
            this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("10 seconds", 10000));
            this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("20 seconds", 20000));
            this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("30 seconds", 30000));
            this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("1 minute", 60000));
            this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("2 minutes", 120000));
            this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("3 minutes", 180000));
            this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("5 minutes", 300000));
            this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("10 minutes", 600000));
            if (this.debug)
            {
                this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("DEBUG - 1 second", 1000));
                this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("DEBUG - 500ms", 500));
                this.comboBoxMainCooldown.Items.Add((object)new Form1.MainCooldownComboBoxItem("DEBUG - 50ms", 50));
            }
            this.comboBoxMainCooldown.SelectedIndex = 3;
            Config.Instance().MainCooldown = 60000;
        }

        private void MainCooldownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1.MainCooldownComboBoxItem selectedItem = (Form1.MainCooldownComboBoxItem)this.comboBoxMainCooldown.SelectedItem;
            Config.Instance().MainCooldown = selectedItem.Time;
            if (Shared.TimerEnabled)
                return;
            this.progressBarMain.Value = 0;
            this.progressBarMain.Maximum = Config.Instance().MainCooldown;
            this.elapsedCount = 0;
            this.stopwatch.Reset();
        }

        private void PopulateVotingTimes()
        {
            this.comboBoxVotingTime.Items.Add((object)new Form1.VotingTimeComboBoxItem("30 seconds", 30000));
            this.comboBoxVotingTime.Items.Add((object)new Form1.VotingTimeComboBoxItem("1 minute", 60000));
            if (this.debug)
                this.comboBoxVotingTime.Items.Add((object)new Form1.VotingTimeComboBoxItem("25ms", 25));
            this.comboBoxVotingTime.SelectedIndex = 0;
            Config.Instance().StreamVotingTime = 30000;
        }

        private void ComboBoxVotingTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1.VotingTimeComboBoxItem selectedItem = (Form1.VotingTimeComboBoxItem)this.comboBoxVotingTime.SelectedItem;
            Config.Instance().StreamVotingTime = selectedItem.VotingTime;
        }

        private void PopulateVotingCooldowns()
        {
            this.comboBoxVotingCooldown.Items.Add((object)new Form1.VotingCooldownComboBoxItem("30 seconds", 30000));
            this.comboBoxVotingCooldown.Items.Add((object)new Form1.VotingCooldownComboBoxItem("1 minute", 60000));
            this.comboBoxVotingCooldown.Items.Add((object)new Form1.VotingCooldownComboBoxItem("2 minutes", 120000));
            this.comboBoxVotingCooldown.Items.Add((object)new Form1.VotingCooldownComboBoxItem("3 minutes", 180000));
            this.comboBoxVotingCooldown.Items.Add((object)new Form1.VotingCooldownComboBoxItem("5 minutes", 300000));
            this.comboBoxVotingCooldown.Items.Add((object)new Form1.VotingCooldownComboBoxItem("10 minutes", 600000));
            if (this.debug)
            {
                this.comboBoxVotingCooldown.Items.Add((object)new Form1.VotingCooldownComboBoxItem("5 seconds", 5000));
                this.comboBoxVotingCooldown.Items.Add((object)new Form1.VotingCooldownComboBoxItem("1 second", 1000));
                this.comboBoxVotingCooldown.Items.Add((object)new Form1.VotingCooldownComboBoxItem("25ms", 25));
            }
            this.comboBoxVotingCooldown.SelectedIndex = 1;
            Config.Instance().StreamVotingCooldown = 60000;
        }

        private void ComboBoxVotingCooldown_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1.VotingCooldownComboBoxItem selectedItem = (Form1.VotingCooldownComboBoxItem)this.comboBoxVotingCooldown.SelectedItem;
            Config.Instance().StreamVotingCooldown = selectedItem.VotingCooldown;
        }

        private void DoAutostart()
        {
            if (!this.checkBoxAutoStart.Checked)
                return;
            this.elapsedCount = 0;
            this.stopwatch.Reset();
            this.SetEnabled(true);
            EffectDatabase.ResetEffectCooldowns();
            if (!Config.Instance().Experimental_RunEffectOnAutoStart || Shared.IsStreamMode)
                return;
            this.CallEffect();
        }

        private void SetEnabled(bool enabled)
        {
            Shared.TimerEnabled = enabled;
            if (Shared.TimerEnabled)
                this.stopwatch.Start();
            else
                this.stopwatch.Stop();
            this.buttonMainToggle.Enabled = true;
            (Shared.IsStreamMode ? (Control)this.buttonStreamToggle : (Control)this.buttonMainToggle).Text = Shared.TimerEnabled ? "Stop / Pause" : "Start / Resume";
            this.comboBoxMainCooldown.Enabled = this.buttonSwitchMode.Enabled = this.buttonResetMain.Enabled = this.buttonResetStream.Enabled = !Shared.TimerEnabled;
            this.comboBoxVotingTime.Enabled = this.comboBoxVotingCooldown.Enabled = this.textBoxSeed.Enabled = !Shared.TimerEnabled;
        }

        private void ButtonMainToggle_Click(object sender, EventArgs e) => this.SetEnabled(!Shared.TimerEnabled);

        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node is Form1.EffectTreeNode effectTreeNode2)
                    EffectDatabase.SetEffectEnabled(effectTreeNode2.Effect, effectTreeNode2.Checked);
                if (node.Nodes.Count > 0)
                    this.CheckAllChildNodes(node, nodeChecked);
            }
        }

        private void EnabledEffectsView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;
            if (e.Node is Form1.EffectTreeNode node1)
                EffectDatabase.SetEffectEnabled(node1.Effect, node1.Checked);
            if (e.Node.Nodes.Count > 0)
                this.CheckAllChildNodes(e.Node, e.Node.Checked);
            foreach (Form1.CategoryTreeNode node2 in this.enabledEffectsView.Nodes)
                node2.UpdateCategory();
        }

        private void LoadPreset(Dictionary<string, bool> enabledEffects)
        {
            this.PopulatePresets();
            foreach (KeyValuePair<string, bool> enabledEffect in enabledEffects)
            {
                Form1.EffectTreeNode effectTreeNode;
                if (this.idToEffectNodeMap.TryGetValue(enabledEffect.Key, out effectTreeNode))
                {
                    effectTreeNode.Checked = enabledEffect.Value;
                    EffectDatabase.SetEffectEnabled(effectTreeNode.Effect, enabledEffect.Value);
                }
            }
            foreach (Form1.CategoryTreeNode node in this.enabledEffectsView.Nodes)
                node.UpdateCategory();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => this.Close();

        private void LoadPresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Preset File|*.cfg";
            openFileDialog1.Title = "Load Preset";
            OpenFileDialog openFileDialog2 = openFileDialog1;
            int num = (int)openFileDialog2.ShowDialog();
            if (openFileDialog2.FileName != "")
            {
                string[] strArray = File.ReadAllText(openFileDialog2.FileName).Split(',');
                Dictionary<string, bool> enabledEffects = new Dictionary<string, bool>();
                foreach (WeightedRandomBag<AbstractEffect>.Entry entry in EffectDatabase.Effects.Get())
                    enabledEffects.Add(entry.item.GetID(), false);
                foreach (string key in strArray)
                    enabledEffects[key] = true;
                this.LoadPreset(enabledEffects);
            }
            openFileDialog2.Dispose();
        }

        private void SavePresetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> stringList = new List<string>();
            foreach (Form1.EffectTreeNode effectTreeNode in this.idToEffectNodeMap.Values)
            {
                if (effectTreeNode.Checked)
                    stringList.Add(effectTreeNode.Effect.GetID());
            }
            string contents = string.Join(",", (IEnumerable<string>)stringList);
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Preset File|*.cfg";
            saveFileDialog1.Title = "Save Preset";
            SaveFileDialog saveFileDialog2 = saveFileDialog1;
            int num = (int)saveFileDialog2.ShowDialog();
            if (saveFileDialog2.FileName != "")
                File.WriteAllText(saveFileDialog2.FileName, contents);
            saveFileDialog2.Dispose();
        }

        private async void ButtonConnectStream_Click(object sender, EventArgs e)
        {
            Form1 form1 = this;
            IStreamConnection stream = form1.stream;
            if ((stream != null ? (stream.IsConnected() ? 1 : 0) : 0) != 0 || form1.buttonConnectStream.Text == "Disconnect")
            {
                form1.stream?.Kill();
                form1.stream = (IStreamConnection)null;
                form1.buttonSwitchMode.Enabled = true;
                form1.comboBoxVotingTime.Enabled = true;
                form1.comboBoxVotingCooldown.Enabled = true;
                form1.textBoxStreamAccessToken.Enabled = true;
                form1.textBoxStreamClientID.Enabled = true;
                form1.buttonStreamToggle.Enabled = false;
                form1.checkBoxTwitchUsePolls.Enabled = true;
                form1.buttonConnectStream.Text = "Connect to Stream";
                if (!form1.tabs.TabPages.Contains(form1.tabEffects))
                    form1.tabs.TabPages.Insert(form1.tabs.TabPages.IndexOf(form1.tabStream), form1.tabEffects);
                form1.SetEnabled(false);
            }
            else
            {
                if (string.IsNullOrEmpty(Config.Instance().StreamAccessToken) || string.IsNullOrEmpty(Config.Instance().StreamClientID))
                    return;
                form1.buttonSwitchMode.Enabled = false;
                form1.buttonConnectStream.Enabled = false;
                form1.textBoxStreamAccessToken.Enabled = false;
                form1.textBoxStreamClientID.Enabled = false;
                form1.stream = Config.Instance().Experimental_YouTubeConnection ? (IStreamConnection)new YouTubeChatConnection() : (Config.Instance().TwitchUsePolls ? (IStreamConnection)new TwitchPollConnection() : (IStreamConnection)new TwitchChatConnection());
                // ISSUE: reference to a compiler-generated method
                //form1.stream.OnRapidFireEffect += new EventHandler<RapidFireEventArgs>(form1.\u003CButtonConnectStream_Click\u003Eb__41_0);
                /*form1.stream.OnRapidFireEffect += new EventHandler<RapidFireEventArgs>(form1.buttonConnectStream.GetType);
                // ISSUE: reference to a compiler-generated method
                form1.stream.OnLoginError += new EventHandler<EventArgs>(form1.\u003CButtonConnectStream_Click\u003Eb__41_1);
                // ISSUE: reference to a compiler-generated method
                form1.stream.OnConnected += new EventHandler<EventArgs>(form1.\u003CButtonConnectStream_Click\u003Eb__41_2);
                // ISSUE: reference to a compiler-generated method
                form1.stream.OnDisconnected += new EventHandler<EventArgs>(form1.\u003CButtonConnectStream_Click\u003Eb__41_3);*/
                throw new Exception("stream not work kto to slomal");
                //int num = (int) await form1.stream.TryConnect();
                int num = (await form1.stream.TryConnect()) ? 1 : 0;
            }
        }

        private void UpdateStreamConnectButtonState() => this.buttonConnectStream.Enabled = !string.IsNullOrEmpty(Config.Instance().StreamAccessToken) && !string.IsNullOrEmpty(Config.Instance().StreamClientID);

        private void TextBoxOAuth_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().StreamAccessToken = this.textBoxStreamAccessToken.Text;
            this.UpdateStreamConnectButtonState();
        }

        private void SwitchMode(bool isStreamMode)
        {
            if (!isStreamMode)
            {
                this.buttonSwitchMode.Text = "Stream";
                if (!this.tabs.TabPages.Contains(this.tabMain))
                    this.tabs.TabPages.Insert(0, this.tabMain);
                this.tabs.SelectedIndex = 0;
                this.tabs.TabPages.Remove(this.tabStream);
                this.tabs.TabPages.Remove(this.tabPolls);
                this.listLastEffectsMain.Items.Clear();
                this.progressBarMain.Value = 0;
                this.elapsedCount = 0;
                this.stopwatch.Reset();
                this.SetEnabled(false);
            }
            else
            {
                this.buttonSwitchMode.Text = "Main";
                if (!this.tabs.TabPages.Contains(this.tabStream))
                    this.tabs.TabPages.Insert(0, this.tabStream);
                this.tabs.SelectedIndex = 0;
                this.tabs.TabPages.Remove(this.tabMain);
                if (Config.Instance().TwitchUsePolls && !this.tabs.TabPages.Contains(this.tabPolls))
                    this.tabs.TabPages.Insert(2, this.tabPolls);
                this.listLastEffectsStream.Items.Clear();
                this.progressBarStream.Value = 0;
                this.elapsedCount = 0;
                this.stopwatch.Reset();
                this.SetEnabled(false);
            }
            EffectDatabase.ResetEffectCooldowns();
        }

        private void ButtonSwitchMode_Click(object sender, EventArgs e)
        {
            Shared.IsStreamMode = !Shared.IsStreamMode;
            this.SwitchMode(Shared.IsStreamMode);
        }

        private void ButtonStreamToggle_Click(object sender, EventArgs e) => this.SetEnabled(!Shared.TimerEnabled);

        private void TextBoxSeed_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().Seed = this.textBoxSeed.Text;
            RandomHandler.SetSeed(Config.Instance().Seed);
        }

        private void ButtonResetMain_Click(object sender, EventArgs e)
        {
            this.SetEnabled(false);
            RandomHandler.SetSeed(Config.Instance().Seed);
            this.stopwatch.Reset();
            this.elapsedCount = 0;
            this.progressBarMain.Value = 0;
            this.buttonMainToggle.Enabled = true;
            this.buttonMainToggle.Text = "Start / Resume";
            EffectDatabase.ResetEffectCooldowns();
        }

        private void CheckBoxShowLastEffectsMain_CheckedChanged(object sender, EventArgs e) => Config.Instance().MainShowLastEffects = this.listLastEffectsMain.Visible = this.checkBoxShowLastEffectsMain.Checked;

        private void CheckBoxShowLastEffectsStream_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamShowLastEffects = this.listLastEffectsStream.Visible = this.checkBoxShowLastEffectsStream.Checked;

        private void CheckBoxStreamAllowOnlyEnabledEffects_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamAllowOnlyEnabledEffectsRapidFire = this.checkBoxStreamAllowOnlyEnabledEffects.Checked;

        private void ButtonResetStream_Click(object sender, EventArgs e)
        {
            this.SetEnabled(false);
            RandomHandler.SetSeed(Config.Instance().Seed);
            this.stopwatch.Reset();
            this.elapsedCount = 0;
            this.labelStreamCurrentMode.Text = "Current Mode: Cooldown";
            Shared.StreamVotingMode = Shared.VOTING_MODE.COOLDOWN;
            this.timesUntilRapidFire = new Random().Next(10, 15);
            this.progressBarStream.Value = 0;
            Button buttonStreamToggle = this.buttonStreamToggle;
            IStreamConnection stream = this.stream;
            int num = stream != null ? (stream.IsConnected() ? 1 : 0) : 0;
            buttonStreamToggle.Enabled = num != 0;
            this.buttonStreamToggle.Text = "Start / Resume";
            EffectDatabase.ResetEffectCooldowns();
        }

        private void CheckBoxStream3TimesCooldown_CheckedChanged(object sender, EventArgs e) => Config.Instance().Stream3TimesCooldown = this.checkBoxStream3TimesCooldown.Checked;

        private void ButtonEffectsToggleAll_Click(object sender, EventArgs e)
        {
            bool flag = false;
            foreach (Form1.CategoryTreeNode node1 in this.enabledEffectsView.Nodes)
            {
                if (node1.Checked)
                {
                    flag = true;
                    break;
                }
                foreach (TreeNode node2 in node1.Nodes)
                {
                    if (node2.Checked)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            foreach (Form1.CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.Checked = !flag;
                this.CheckAllChildNodes((TreeNode)node, !flag);
                node.UpdateCategory();
            }
        }

        private void ViceCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shared.SelectedGame = "vice_city";
            Config.Instance().EnabledEffects.Clear();
            EffectDatabase.PopulateEffects("vice_city");
            this.PopulateEffectTreeList();
            foreach (Form1.CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.Checked = false;
                this.CheckAllChildNodes((TreeNode)node, false);
                node.UpdateCategory();
            }
            foreach (Form1.CategoryTreeNode node in this.enabledEffectsView.Nodes)
                node.UpdateCategory();
        }

        private void SanAndreasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shared.SelectedGame = "san_andreas";
            Config.Instance().EnabledEffects.Clear();
            EffectDatabase.PopulateEffects("san_andreas");
            this.PopulateEffectTreeList();
            foreach (Form1.CategoryTreeNode node in this.enabledEffectsView.Nodes)
            {
                node.Checked = false;
                this.CheckAllChildNodes((TreeNode)node, false);
                node.UpdateCategory();
            }
            foreach (Form1.CategoryTreeNode node in this.enabledEffectsView.Nodes)
                node.UpdateCategory();
        }

        private void CheckBoxStreamCombineVotingMessages_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamCombineChatMessages = this.checkBoxStreamCombineVotingMessages.Checked;

        private void UpdatePollTabVisibility()
        {
            if (Config.Instance().TwitchUsePolls)
            {
                if (this.tabs.TabPages.Contains(this.tabPolls) || !Shared.IsStreamMode)
                    return;
                this.tabs.TabPages.Insert(2, this.tabPolls);
            }
            else
                this.tabs.TabPages.Remove(this.tabPolls);
        }

        private void CheckBoxTwitchUsePolls_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().TwitchUsePolls = this.checkBoxTwitchUsePolls.Checked;
            this.UpdatePollTabVisibility();
        }

        private void CheckBoxStreamEnableMultipleEffects_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamEnableMultipleEffects = this.checkBoxStreamEnableMultipleEffects.Checked;

        private void NumericUpDownTwitchPollsBitsCost_ValueChanged(object sender, EventArgs e) => Config.Instance().TwitchPollsBitsCost = Decimal.ToInt32(this.numericUpDownTwitchPollsBitsCost.Value);

        private void NumericUpDownTwitchPollsChannelPointsCost_ValueChanged(object sender, EventArgs e) => Config.Instance().TwitchPollsChannelPointsCost = Decimal.ToInt32(this.numericUpDownTwitchPollsChannelPointsCost.Value);

        private void CheckBoxTwitchPollsPostMessages_CheckedChanged(object sender, EventArgs e) => Config.Instance().TwitchPollsPostMessages = this.checkBoxTwitchPollsPostMessages.Checked;

        private void EnabledEffectsView_NodeMouseDoubleClick(
          object sender,
          TreeNodeMouseClickEventArgs e)
        {
            if (!(e.Node is Form1.EffectTreeNode node) || !this.debug)
                return;
            EffectDatabase.ShouldCooldown = false;
            this.CallEffect(node.Effect);
            EffectDatabase.ShouldCooldown = true;
        }

        private void CheckBoxPlayAudioForEffects_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().PlayAudioForEffects = this.checkBoxPlayAudioForEffects.Checked;
            this.checkBoxSettingsPlayAudioSequentially.Enabled = Config.Instance().PlayAudioForEffects;
        }

        private string FilterSyncCharacters(string text) => Regex.Replace(text, "[^A-Za-z0-9]", "");

        private void UpdateButtonState() => this.buttonSyncConnect.Enabled = !string.IsNullOrEmpty(this.textBoxSyncServer.Text) && !string.IsNullOrEmpty(this.textBoxSyncChannel.Text) && !string.IsNullOrEmpty(this.textBoxSyncUsername.Text);

        private void TextBoxSyncServer_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().SyncServer = this.textBoxSyncServer.Text;
            this.UpdateButtonState();
        }

        private void TextBoxSyncChannel_TextChanged(object sender, EventArgs e)
        {
            this.textBoxSyncChannel.Text = this.FilterSyncCharacters(this.textBoxSyncChannel.Text);
            Config.Instance().SyncChannel = this.textBoxSyncChannel.Text;
            this.UpdateButtonState();
        }

        private void TextBoxSyncUsername_TextChanged(object sender, EventArgs e)
        {
            this.textBoxSyncUsername.Text = this.FilterSyncCharacters(this.textBoxSyncUsername.Text);
            Config.Instance().SyncUsername = this.textBoxSyncUsername.Text;
            this.UpdateButtonState();
        }

        private void UpdateSyncConnectionState(int state) => this.Invoke((Action)(() =>
       {
           switch (state)
           {
               case 0:
                   this.textBoxSyncServer.Enabled = this.textBoxSyncChannel.Enabled = this.textBoxSyncUsername.Enabled = this.buttonSyncConnect.Enabled = true;
                   this.buttonSyncConnect.Text = "Connect";
                   this.buttonSwitchMode.Enabled = true;
                   this.buttonMainToggle.Enabled = true;
                   this.buttonResetMain.Enabled = true;
                   this.comboBoxMainCooldown.Enabled = true;
                   this.enabledEffectsView.Enabled = true;
                   this.textBoxSeed.Enabled = true;
                   this.checkBoxAutoStart.Enabled = true;
                   this.buttonEffectsToggleAll.Enabled = true;
                   this.numericUpDownEffectCooldown.Enabled = true;
                   this.buttonExperimentalRunEffect.Enabled = true;
                   Shared.Sync = (Sync)null;
                   break;
               case 1:
                   this.textBoxSyncServer.Enabled = this.textBoxSyncChannel.Enabled = this.textBoxSyncUsername.Enabled = this.buttonSyncConnect.Enabled = false;
                   this.buttonSyncConnect.Text = "Connecting...";
                   break;
               case 2:
                   this.textBoxSyncServer.Enabled = this.textBoxSyncChannel.Enabled = this.textBoxSyncUsername.Enabled = false;
                   this.buttonSyncConnect.Enabled = true;
                   this.buttonSyncConnect.Text = "Disconnect";
                   break;
           }
       }));

        private void ShowMessageBox(string text, string caption)
        {
            int num;
            this.Invoke((Action)(() => num = (int)MessageBox.Show((IWin32Window)this, text, caption)));
        }

        private void AddToSyncChatHistory(string message) => this.Invoke((Action)(() =>
       {
           this.listBoxSyncChat.Items.Add((object)message);
           this.listBoxSyncChat.TopIndex = this.listBoxSyncChat.Items.Count - 1;
       }));

        private void ClearSyncChatHistory() => this.Invoke((Action)(() => this.listBoxSyncChat.Items.Clear()));

        private void ButtonSyncConnect_Click(object sender, EventArgs e)
        {
            Shared.Sync?.Disconnect();
            if (this.buttonSyncConnect.Text == "Disconnect")
            {
                this.UpdateSyncConnectionState(0);
            }
            else
            {
                this.ClearSyncChatHistory();
                Shared.Sync = new Sync(this.textBoxSyncServer.Text, this.textBoxSyncChannel.Text, this.textBoxSyncUsername.Text);
                this.UpdateSyncConnectionState(1);
                Shared.Sync.OnConnectionFailed += (EventHandler<ConnectionFailedEventArgs>)((_sender, args) =>
               {
                   this.ShowMessageBox("Connection failed - is the server running?", "Error");
                   this.UpdateSyncConnectionState(0);
               });
                Shared.Sync.OnUsernameInUse += (EventHandler<UsernameInUseEventArgs>)((_sender, args) =>
               {
                   this.ShowMessageBox("Username already in use!", "Error");
                   this.UpdateSyncConnectionState(0);
               });
                Shared.Sync.OnConnectionSuccessful += (EventHandler<ConnectionSuccessfulEventArgs>)((_sender, args) =>
               {
                   this.ShowMessageBox("Successfully connected!", "Connected");
                   this.AddToSyncChatHistory("Successfully connected to channel: " + this.textBoxSyncChannel.Text);
                   this.UpdateSyncConnectionState(2);
                   this.Invoke((Action)(() =>
           {
                     if (!args.IsHost)
                     {
                         this.SwitchMode(false);
                         this.buttonSwitchMode.Enabled = false;
                         this.buttonMainToggle.Enabled = false;
                         this.buttonResetMain.Enabled = false;
                         this.comboBoxMainCooldown.Enabled = false;
                         this.enabledEffectsView.Enabled = false;
                         this.buttonResetMain.Enabled = false;
                         this.textBoxSeed.Enabled = false;
                         this.checkBoxAutoStart.Enabled = false;
                         this.buttonEffectsToggleAll.Enabled = false;
                         this.numericUpDownEffectCooldown.Enabled = false;
                         this.buttonExperimentalRunEffect.Enabled = false;
                     }
                     this.labelSyncHost.Text = "Host: " + args.HostUsername;
                     if (!args.IsHost)
                         return;
                     this.labelSyncHost.Text += " (You!)";
                 }));
               });
                Shared.Sync.OnHostLeftChannel += (EventHandler<HostLeftChannelEventArgs>)((_sender, args) =>
               {
                   this.ShowMessageBox("Host has left the channel; Disconnected.", "Host Left");
                   this.AddToSyncChatHistory("Host has left the channel; Disconnected.");
                   this.UpdateSyncConnectionState(0);
               });
                Shared.Sync.OnVersionMismatch += (EventHandler<VersionMismatchEventArgs>)((_sender, args) =>
               {
                   this.ShowMessageBox("Channel is v" + args.Version + " but you have v" + Shared.Version + "; Disconnected.", "Version Mismatch");
                   this.AddToSyncChatHistory("Channel is v" + args.Version + " but you have v" + Shared.Version + "; Disconnected.");
                   this.UpdateSyncConnectionState(0);
               });
                Shared.Sync.OnUserJoined += (EventHandler<UserJoinedEventArgs>)((_sender, args) => this.AddToSyncChatHistory(args.Username + " joined!"));
                Shared.Sync.OnUserLeft += (EventHandler<UserLeftEventArgs>)((_sender, args) => this.AddToSyncChatHistory(args.Username + " left!"));
                Shared.Sync.OnChatMessage += (EventHandler<ChatMessageEventArgs>)((_sender, args) => this.AddToSyncChatHistory(args.Username + ": " + args.Message));
                Shared.Sync.OnTimeUpdate += (EventHandler<TimeUpdateEventArgs>)((_sender, args) =>
               {
                   if (Shared.Sync.IsHost)
                       return;
                   WebsocketHandler.INSTANCE.SendTimeToGame(args.Remaining, args.Total);
               });
                Shared.Sync.OnEffect += (EventHandler<EffectEventArgs>)((_sender, args) =>
               {
                   AbstractEffect byWord = EffectDatabase.GetByWord(args.Word);
                   if (byWord == null)
                       return;
                   if (string.IsNullOrEmpty(args.Subtext) || args.Subtext == "N/A")
                       byWord.ResetSubtext();
                   else
                       byWord.SetSubtext(args.Subtext);
                   EffectDatabase.RunEffect(byWord, args.Seed, args.Duration);
                   this.AddEffectToListBox(byWord);
               });
                Shared.Sync.OnVotes += (EventHandler<VotesEventArgs>)((_sender, args) =>
               {
                   string[] effects = args.Effects;
                   int[] votes = args.Votes;
                   WebsocketHandler.INSTANCE.SendVotes(effects, votes, args.LastChoice);
               });
                Shared.Sync.Connect();
            }
        }

        private void TextBoxSyncChat_TextChanged(object sender, EventArgs e) => this.buttonSyncSend.Enabled = Shared.Sync != null && !string.IsNullOrEmpty(this.textBoxSyncChat.Text);

        private void ButtonSyncSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBoxSyncChat.Text))
                return;
            Shared.Sync?.SendChatMessage(this.textBoxSyncChat.Text);
            this.textBoxSyncChat.Text = "";
        }

        private void TextBoxSyncChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(this.textBoxSyncChat.Text))
                return;
            Shared.Sync?.SendChatMessage(this.textBoxSyncChat.Text);
            this.textBoxSyncChat.Text = "";
        }

        private void CheckBoxExperimental_EnableEffectOnAutoStart_CheckedChanged(
          object sender,
          EventArgs e)
        {
            Config.Instance().Experimental_RunEffectOnAutoStart = this.checkBoxExperimental_RunEffectOnAutoStart.Checked;
        }

        private void ExperimentalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.tabs.TabPages.Contains(this.tabExperimental))
                this.tabs.TabPages.Add(this.tabExperimental);
            this.experimentalToolStripMenuItem.Visible = false;
        }

        private void ButtonExperimentalRunEffect_Click(object sender, EventArgs e)
        {
            string text = this.textBoxExperimentalEffectName.Text;
            if (string.IsNullOrEmpty(text))
                return;
            AbstractEffect effect = (EffectDatabase.GetByID(text) ?? EffectDatabase.GetByID("effect_" + text)) ?? EffectDatabase.GetByWord(text);
            if (effect != null)
            {
                EffectDatabase.ShouldCooldown = false;
                this.CallEffect(effect);
                EffectDatabase.ShouldCooldown = true;
            }
            else
            {
                int effectDuration = Config.GetEffectDuration();
                WebsocketHandler.INSTANCE.SendEffectToGame(this.textBoxExperimentalEffectName.Text, (object)new
                {
                    seed = RandomHandler.Next(9999999)
                }, effectDuration);
            }
        }

        private void TextBoxExperimentalEffectName_TextChanged(object sender, EventArgs e) => Config.Instance().Experimental_EffectName = this.textBoxExperimentalEffectName.Text;

        private void CheckBoxAutoStart_CheckedChanged(object sender, EventArgs e) => Config.Instance().AutoStart = this.checkBoxAutoStart.Checked;

        private void CheckBoxStreamEnableRapidFire_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamEnableRapidFire = this.checkBoxStreamEnableRapidFire.Checked;

        private void CheckBoxStreamMajorityVotes_CheckedChanged(object sender, EventArgs e)
        {
            Config.Instance().StreamMajorityVotes = this.checkBoxStreamMajorityVotes.Checked;
            this.checkBoxStreamEnableMultipleEffects.Enabled = Config.Instance().StreamMajorityVotes;
        }

        private void LinkLabelTwitchGetAccessToken_LinkClicked(
          object sender,
          LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://chaos.lord.moe/");
        }

        private void CheckBoxExperimentalYouTubeConnection_CheckedChanged(object sender, EventArgs e) => Config.Instance().Experimental_YouTubeConnection = this.checkBoxExperimentalYouTubeConnection.Checked;

        private void TextBoxStreamClientID_TextChanged(object sender, EventArgs e)
        {
            Config.Instance().StreamClientID = this.textBoxStreamClientID.Text;
            this.UpdateStreamConnectButtonState();
        }

        private void CheckBoxSettingsPlayAudioSequentially_CheckedChanged(object sender, EventArgs e) => Config.Instance().PlayAudioSequentially = this.checkBoxSettingsPlayAudioSequentially.Checked;

        private void NumericUpDownEffectCooldown_ValueChanged(object sender, EventArgs e) => Config.Instance().EffectsCooldownNotActivating = (int)this.numericUpDownEffectCooldown.Value;

        private void checkBoxStreamHideVotingEffectsIngame_CheckedChanged(object sender, EventArgs e) => Config.Instance().StreamHideVotingEffectsIngame = this.checkBoxStreamHideVotingEffectsIngame.Checked;

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int num = (int)MessageBox.Show((IWin32Window)this, string.Format("Version: v{0}\nTotal Effects: {1}", (object)Shared.Version, (object)EffectDatabase.Effects.Count), "About");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonMainToggle = new System.Windows.Forms.Button();
            this.progressBarMain = new System.Windows.Forms.ProgressBar();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.checkBoxShowLastEffectsMain = new System.Windows.Forms.CheckBox();
            this.buttonResetMain = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxMainCooldown = new System.Windows.Forms.ComboBox();
            this.listLastEffectsMain = new System.Windows.Forms.ListBox();
            this.tabStream = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxStreamClientID = new System.Windows.Forms.TextBox();
            this.labelTwitchAccessToken = new System.Windows.Forms.Label();
            this.linkLabelTwitchGetAccessToken = new System.Windows.Forms.LinkLabel();
            this.checkBoxStreamCombineVotingMessages = new System.Windows.Forms.CheckBox();
            this.checkBoxTwitchUsePolls = new System.Windows.Forms.CheckBox();
            this.checkBoxStreamMajorityVotes = new System.Windows.Forms.CheckBox();
            this.checkBoxStreamEnableRapidFire = new System.Windows.Forms.CheckBox();
            this.checkBoxStreamEnableMultipleEffects = new System.Windows.Forms.CheckBox();
            this.checkBoxStream3TimesCooldown = new System.Windows.Forms.CheckBox();
            this.buttonResetStream = new System.Windows.Forms.Button();
            this.checkBoxStreamAllowOnlyEnabledEffects = new System.Windows.Forms.CheckBox();
            this.checkBoxShowLastEffectsStream = new System.Windows.Forms.CheckBox();
            this.labelStreamCurrentMode = new System.Windows.Forms.Label();
            this.buttonStreamToggle = new System.Windows.Forms.Button();
            this.comboBoxVotingCooldown = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxVotingTime = new System.Windows.Forms.ComboBox();
            this.progressBarStream = new System.Windows.Forms.ProgressBar();
            this.listLastEffectsStream = new System.Windows.Forms.ListBox();
            this.textBoxStreamAccessToken = new System.Windows.Forms.TextBox();
            this.buttonConnectStream = new System.Windows.Forms.Button();
            this.tabPolls = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownTwitchPollsChannelPointsCost = new System.Windows.Forms.NumericUpDown();
            this.checkBoxTwitchPollsPostMessages = new System.Windows.Forms.CheckBox();
            this.labelTwitchPollsBitsCost = new System.Windows.Forms.Label();
            this.numericUpDownTwitchPollsBitsCost = new System.Windows.Forms.NumericUpDown();
            this.tabEffects = new System.Windows.Forms.TabPage();
            this.labelEffectCooldown = new System.Windows.Forms.Label();
            this.numericUpDownEffectCooldown = new System.Windows.Forms.NumericUpDown();
            this.buttonEffectsToggleAll = new System.Windows.Forms.Button();
            this.enabledEffectsView = new System.Windows.Forms.TreeView();
            this.tabSync = new System.Windows.Forms.TabPage();
            this.buttonSyncSend = new System.Windows.Forms.Button();
            this.textBoxSyncChat = new System.Windows.Forms.TextBox();
            this.listBoxSyncChat = new System.Windows.Forms.ListBox();
            this.labelSyncHost = new System.Windows.Forms.Label();
            this.textBoxSyncUsername = new System.Windows.Forms.TextBox();
            this.labelSyncUsername = new System.Windows.Forms.Label();
            this.labelSyncChannel = new System.Windows.Forms.Label();
            this.textBoxSyncChannel = new System.Windows.Forms.TextBox();
            this.labelSyncServer = new System.Windows.Forms.Label();
            this.textBoxSyncServer = new System.Windows.Forms.TextBox();
            this.buttonSyncConnect = new System.Windows.Forms.Button();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.checkBoxStreamHideVotingEffectsIngame = new System.Windows.Forms.CheckBox();
            this.checkBoxSettingsPlayAudioSequentially = new System.Windows.Forms.CheckBox();
            this.checkBoxPlayAudioForEffects = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxSeed = new System.Windows.Forms.TextBox();
            this.tabExperimental = new System.Windows.Forms.TabPage();
            this.effect = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.gtaport = new System.Windows.Forms.NumericUpDown();
            this.gtaip = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.connectb = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.port = new System.Windows.Forms.NumericUpDown();
            this.ip = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.checkBoxExperimentalYouTubeConnection = new System.Windows.Forms.CheckBox();
            this.buttonExperimentalRunEffect = new System.Windows.Forms.Button();
            this.textBoxExperimentalEffectName = new System.Windows.Forms.TextBox();
            this.checkBoxExperimental_RunEffectOnAutoStart = new System.Windows.Forms.CheckBox();
            this.checkBoxAutoStart = new System.Windows.Forms.CheckBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savePresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.experimentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viceCityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sanAndreasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTipHandler = new System.Windows.Forms.ToolTip(this.components);
            this.timerMain = new System.Windows.Forms.Timer(this.components);
            this.buttonSwitchMode = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabs.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabStream.SuspendLayout();
            this.tabPolls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTwitchPollsChannelPointsCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTwitchPollsBitsCost)).BeginInit();
            this.tabEffects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEffectCooldown)).BeginInit();
            this.tabSync.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabExperimental.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gtaport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.port)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonMainToggle
            // 
            this.buttonMainToggle.Location = new System.Drawing.Point(6, 6);
            this.buttonMainToggle.Name = "buttonMainToggle";
            this.buttonMainToggle.Size = new System.Drawing.Size(94, 23);
            this.buttonMainToggle.TabIndex = 0;
            this.buttonMainToggle.Text = "Start / Resume";
            this.buttonMainToggle.UseVisualStyleBackColor = true;
            this.buttonMainToggle.Click += new System.EventHandler(this.ButtonMainToggle_Click);
            // 
            // progressBarMain
            // 
            this.progressBarMain.Location = new System.Drawing.Point(206, 6);
            this.progressBarMain.Maximum = 60;
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(338, 23);
            this.progressBarMain.Step = 1;
            this.progressBarMain.TabIndex = 1;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabMain);
            this.tabs.Controls.Add(this.tabStream);
            this.tabs.Controls.Add(this.tabPolls);
            this.tabs.Controls.Add(this.tabEffects);
            this.tabs.Controls.Add(this.tabSync);
            this.tabs.Controls.Add(this.tabSettings);
            this.tabs.Controls.Add(this.tabExperimental);
            this.tabs.Location = new System.Drawing.Point(0, 41);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(560, 319);
            this.tabs.TabIndex = 4;
            // 
            // tabMain
            // 
            this.tabMain.BackColor = System.Drawing.Color.Transparent;
            this.tabMain.Controls.Add(this.checkBoxShowLastEffectsMain);
            this.tabMain.Controls.Add(this.buttonResetMain);
            this.tabMain.Controls.Add(this.label2);
            this.tabMain.Controls.Add(this.comboBoxMainCooldown);
            this.tabMain.Controls.Add(this.buttonMainToggle);
            this.tabMain.Controls.Add(this.listLastEffectsMain);
            this.tabMain.Controls.Add(this.progressBarMain);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain.Size = new System.Drawing.Size(552, 293);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            // 
            // checkBoxShowLastEffectsMain
            // 
            this.checkBoxShowLastEffectsMain.AutoSize = true;
            this.checkBoxShowLastEffectsMain.Checked = true;
            this.checkBoxShowLastEffectsMain.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowLastEffectsMain.Location = new System.Drawing.Point(6, 111);
            this.checkBoxShowLastEffectsMain.Name = "checkBoxShowLastEffectsMain";
            this.checkBoxShowLastEffectsMain.Size = new System.Drawing.Size(112, 17);
            this.checkBoxShowLastEffectsMain.TabIndex = 8;
            this.checkBoxShowLastEffectsMain.Text = "Show Last Effects";
            this.checkBoxShowLastEffectsMain.UseVisualStyleBackColor = true;
            this.checkBoxShowLastEffectsMain.CheckedChanged += new System.EventHandler(this.CheckBoxShowLastEffectsMain_CheckedChanged);
            // 
            // buttonResetMain
            // 
            this.buttonResetMain.Location = new System.Drawing.Point(106, 6);
            this.buttonResetMain.Name = "buttonResetMain";
            this.buttonResetMain.Size = new System.Drawing.Size(94, 23);
            this.buttonResetMain.TabIndex = 7;
            this.buttonResetMain.Text = "Reset";
            this.buttonResetMain.UseVisualStyleBackColor = true;
            this.buttonResetMain.Click += new System.EventHandler(this.ButtonResetMain_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(360, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Cooldown:";
            // 
            // comboBoxMainCooldown
            // 
            this.comboBoxMainCooldown.FormattingEnabled = true;
            this.comboBoxMainCooldown.Location = new System.Drawing.Point(423, 35);
            this.comboBoxMainCooldown.Name = "comboBoxMainCooldown";
            this.comboBoxMainCooldown.Size = new System.Drawing.Size(121, 21);
            this.comboBoxMainCooldown.TabIndex = 5;
            this.comboBoxMainCooldown.SelectedIndexChanged += new System.EventHandler(this.MainCooldownComboBox_SelectedIndexChanged);
            // 
            // listLastEffectsMain
            // 
            this.listLastEffectsMain.FormattingEnabled = true;
            this.listLastEffectsMain.Location = new System.Drawing.Point(6, 134);
            this.listLastEffectsMain.Name = "listLastEffectsMain";
            this.listLastEffectsMain.Size = new System.Drawing.Size(538, 147);
            this.listLastEffectsMain.TabIndex = 4;
            // 
            // tabStream
            // 
            this.tabStream.BackColor = System.Drawing.Color.Transparent;
            this.tabStream.Controls.Add(this.label3);
            this.tabStream.Controls.Add(this.textBoxStreamClientID);
            this.tabStream.Controls.Add(this.labelTwitchAccessToken);
            this.tabStream.Controls.Add(this.linkLabelTwitchGetAccessToken);
            this.tabStream.Controls.Add(this.checkBoxStreamCombineVotingMessages);
            this.tabStream.Controls.Add(this.checkBoxTwitchUsePolls);
            this.tabStream.Controls.Add(this.checkBoxStreamMajorityVotes);
            this.tabStream.Controls.Add(this.checkBoxStreamEnableRapidFire);
            this.tabStream.Controls.Add(this.checkBoxStreamEnableMultipleEffects);
            this.tabStream.Controls.Add(this.checkBoxStream3TimesCooldown);
            this.tabStream.Controls.Add(this.buttonResetStream);
            this.tabStream.Controls.Add(this.checkBoxStreamAllowOnlyEnabledEffects);
            this.tabStream.Controls.Add(this.checkBoxShowLastEffectsStream);
            this.tabStream.Controls.Add(this.labelStreamCurrentMode);
            this.tabStream.Controls.Add(this.buttonStreamToggle);
            this.tabStream.Controls.Add(this.comboBoxVotingCooldown);
            this.tabStream.Controls.Add(this.label7);
            this.tabStream.Controls.Add(this.label6);
            this.tabStream.Controls.Add(this.comboBoxVotingTime);
            this.tabStream.Controls.Add(this.progressBarStream);
            this.tabStream.Controls.Add(this.listLastEffectsStream);
            this.tabStream.Controls.Add(this.textBoxStreamAccessToken);
            this.tabStream.Controls.Add(this.buttonConnectStream);
            this.tabStream.Location = new System.Drawing.Point(4, 22);
            this.tabStream.Name = "tabStream";
            this.tabStream.Size = new System.Drawing.Size(552, 293);
            this.tabStream.TabIndex = 2;
            this.tabStream.Text = "Stream";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Client ID:";
            // 
            // textBoxStreamClientID
            // 
            this.textBoxStreamClientID.Location = new System.Drawing.Point(93, 7);
            this.textBoxStreamClientID.Name = "textBoxStreamClientID";
            this.textBoxStreamClientID.PasswordChar = '*';
            this.textBoxStreamClientID.Size = new System.Drawing.Size(125, 20);
            this.textBoxStreamClientID.TabIndex = 32;
            this.textBoxStreamClientID.TextChanged += new System.EventHandler(this.TextBoxStreamClientID_TextChanged);
            // 
            // labelTwitchAccessToken
            // 
            this.labelTwitchAccessToken.AutoSize = true;
            this.labelTwitchAccessToken.Location = new System.Drawing.Point(8, 35);
            this.labelTwitchAccessToken.Name = "labelTwitchAccessToken";
            this.labelTwitchAccessToken.Size = new System.Drawing.Size(79, 13);
            this.labelTwitchAccessToken.TabIndex = 31;
            this.labelTwitchAccessToken.Text = "Access Token:";
            // 
            // linkLabelTwitchGetAccessToken
            // 
            this.linkLabelTwitchGetAccessToken.AutoSize = true;
            this.linkLabelTwitchGetAccessToken.Location = new System.Drawing.Point(224, 10);
            this.linkLabelTwitchGetAccessToken.Name = "linkLabelTwitchGetAccessToken";
            this.linkLabelTwitchGetAccessToken.Size = new System.Drawing.Size(131, 13);
            this.linkLabelTwitchGetAccessToken.TabIndex = 30;
            this.linkLabelTwitchGetAccessToken.TabStop = true;
            this.linkLabelTwitchGetAccessToken.Text = "Get Twitch Access Token";
            this.linkLabelTwitchGetAccessToken.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelTwitchGetAccessToken_LinkClicked);
            // 
            // checkBoxStreamCombineVotingMessages
            // 
            this.checkBoxStreamCombineVotingMessages.AutoSize = true;
            this.checkBoxStreamCombineVotingMessages.Location = new System.Drawing.Point(124, 121);
            this.checkBoxStreamCombineVotingMessages.Name = "checkBoxStreamCombineVotingMessages";
            this.checkBoxStreamCombineVotingMessages.Size = new System.Drawing.Size(151, 17);
            this.checkBoxStreamCombineVotingMessages.TabIndex = 29;
            this.checkBoxStreamCombineVotingMessages.Text = "Combine Voting Messages";
            this.checkBoxStreamCombineVotingMessages.UseVisualStyleBackColor = true;
            // 
            // checkBoxTwitchUsePolls
            // 
            this.checkBoxTwitchUsePolls.AutoSize = true;
            this.checkBoxTwitchUsePolls.Location = new System.Drawing.Point(391, 64);
            this.checkBoxTwitchUsePolls.Name = "checkBoxTwitchUsePolls";
            this.checkBoxTwitchUsePolls.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxTwitchUsePolls.Size = new System.Drawing.Size(153, 17);
            this.checkBoxTwitchUsePolls.TabIndex = 28;
            this.checkBoxTwitchUsePolls.Text = "Use Twitch Polls For Votes";
            this.toolTipHandler.SetToolTip(this.checkBoxTwitchUsePolls, "This will force majority voting,\r\nno matter what the checkbox is.\r\nThere is no in" +
        "formation on\r\nwhich user voted for which vote.");
            this.checkBoxTwitchUsePolls.UseVisualStyleBackColor = true;
            this.checkBoxTwitchUsePolls.CheckedChanged += new System.EventHandler(this.CheckBoxTwitchUsePolls_CheckedChanged);
            // 
            // checkBoxStreamMajorityVotes
            // 
            this.checkBoxStreamMajorityVotes.AutoSize = true;
            this.checkBoxStreamMajorityVotes.Checked = true;
            this.checkBoxStreamMajorityVotes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStreamMajorityVotes.Location = new System.Drawing.Point(452, 116);
            this.checkBoxStreamMajorityVotes.Name = "checkBoxStreamMajorityVotes";
            this.checkBoxStreamMajorityVotes.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxStreamMajorityVotes.Size = new System.Drawing.Size(92, 17);
            this.checkBoxStreamMajorityVotes.TabIndex = 27;
            this.checkBoxStreamMajorityVotes.Text = "Majority Votes";
            this.toolTipHandler.SetToolTip(this.checkBoxStreamMajorityVotes, "This will force majority voting,\r\nno matter what the checkbox is.\r\nThere is no in" +
        "formation on\r\nwhich user voted for which vote.");
            this.checkBoxStreamMajorityVotes.UseVisualStyleBackColor = true;
            this.checkBoxStreamMajorityVotes.CheckedChanged += new System.EventHandler(this.CheckBoxStreamMajorityVotes_CheckedChanged);
            // 
            // checkBoxStreamEnableRapidFire
            // 
            this.checkBoxStreamEnableRapidFire.AutoSize = true;
            this.checkBoxStreamEnableRapidFire.Checked = true;
            this.checkBoxStreamEnableRapidFire.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStreamEnableRapidFire.Location = new System.Drawing.Point(434, 185);
            this.checkBoxStreamEnableRapidFire.Name = "checkBoxStreamEnableRapidFire";
            this.checkBoxStreamEnableRapidFire.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxStreamEnableRapidFire.Size = new System.Drawing.Size(110, 17);
            this.checkBoxStreamEnableRapidFire.TabIndex = 26;
            this.checkBoxStreamEnableRapidFire.Text = "Enable Rapid-Fire";
            this.checkBoxStreamEnableRapidFire.UseVisualStyleBackColor = true;
            this.checkBoxStreamEnableRapidFire.CheckedChanged += new System.EventHandler(this.CheckBoxStreamEnableRapidFire_CheckedChanged);
            // 
            // checkBoxStreamEnableMultipleEffects
            // 
            this.checkBoxStreamEnableMultipleEffects.AutoSize = true;
            this.checkBoxStreamEnableMultipleEffects.Location = new System.Drawing.Point(418, 139);
            this.checkBoxStreamEnableMultipleEffects.Name = "checkBoxStreamEnableMultipleEffects";
            this.checkBoxStreamEnableMultipleEffects.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxStreamEnableMultipleEffects.Size = new System.Drawing.Size(126, 17);
            this.checkBoxStreamEnableMultipleEffects.TabIndex = 25;
            this.checkBoxStreamEnableMultipleEffects.Text = "Allow Multiple Effects";
            this.checkBoxStreamEnableMultipleEffects.UseVisualStyleBackColor = true;
            this.checkBoxStreamEnableMultipleEffects.CheckedChanged += new System.EventHandler(this.CheckBoxStreamEnableMultipleEffects_CheckedChanged);
            // 
            // checkBoxStream3TimesCooldown
            // 
            this.checkBoxStream3TimesCooldown.AutoSize = true;
            this.checkBoxStream3TimesCooldown.Checked = true;
            this.checkBoxStream3TimesCooldown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStream3TimesCooldown.Location = new System.Drawing.Point(457, 162);
            this.checkBoxStream3TimesCooldown.Name = "checkBoxStream3TimesCooldown";
            this.checkBoxStream3TimesCooldown.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxStream3TimesCooldown.Size = new System.Drawing.Size(87, 17);
            this.checkBoxStream3TimesCooldown.TabIndex = 24;
            this.checkBoxStream3TimesCooldown.Text = "3x Cooldown";
            this.toolTipHandler.SetToolTip(this.checkBoxStream3TimesCooldown, "When enabled effects will have 3x their cooldown.\r\n(Cooldown in this case is the " +
        "Voting Time + Voting Cooldown)");
            this.checkBoxStream3TimesCooldown.UseVisualStyleBackColor = true;
            this.checkBoxStream3TimesCooldown.CheckedChanged += new System.EventHandler(this.CheckBoxStream3TimesCooldown_CheckedChanged);
            // 
            // buttonResetStream
            // 
            this.buttonResetStream.Enabled = false;
            this.buttonResetStream.Location = new System.Drawing.Point(477, 7);
            this.buttonResetStream.Name = "buttonResetStream";
            this.buttonResetStream.Size = new System.Drawing.Size(67, 23);
            this.buttonResetStream.TabIndex = 21;
            this.buttonResetStream.Text = "Reset";
            this.buttonResetStream.UseVisualStyleBackColor = true;
            this.buttonResetStream.Click += new System.EventHandler(this.ButtonResetStream_Click);
            // 
            // checkBoxStreamAllowOnlyEnabledEffects
            // 
            this.checkBoxStreamAllowOnlyEnabledEffects.AutoSize = true;
            this.checkBoxStreamAllowOnlyEnabledEffects.Location = new System.Drawing.Point(396, 207);
            this.checkBoxStreamAllowOnlyEnabledEffects.Name = "checkBoxStreamAllowOnlyEnabledEffects";
            this.checkBoxStreamAllowOnlyEnabledEffects.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxStreamAllowOnlyEnabledEffects.Size = new System.Drawing.Size(148, 17);
            this.checkBoxStreamAllowOnlyEnabledEffects.TabIndex = 19;
            this.checkBoxStreamAllowOnlyEnabledEffects.Text = "Only Enabled Effects (RF)";
            this.toolTipHandler.SetToolTip(this.checkBoxStreamAllowOnlyEnabledEffects, "Only allow effects that are enabled\r\nin the currently active preset during Rapid-" +
        "Fire.");
            this.checkBoxStreamAllowOnlyEnabledEffects.UseVisualStyleBackColor = true;
            this.checkBoxStreamAllowOnlyEnabledEffects.CheckedChanged += new System.EventHandler(this.CheckBoxStreamAllowOnlyEnabledEffects_CheckedChanged);
            // 
            // checkBoxShowLastEffectsStream
            // 
            this.checkBoxShowLastEffectsStream.AutoSize = true;
            this.checkBoxShowLastEffectsStream.Checked = true;
            this.checkBoxShowLastEffectsStream.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowLastEffectsStream.Location = new System.Drawing.Point(8, 121);
            this.checkBoxShowLastEffectsStream.Name = "checkBoxShowLastEffectsStream";
            this.checkBoxShowLastEffectsStream.Size = new System.Drawing.Size(112, 17);
            this.checkBoxShowLastEffectsStream.TabIndex = 18;
            this.checkBoxShowLastEffectsStream.Text = "Show Last Effects";
            this.checkBoxShowLastEffectsStream.UseVisualStyleBackColor = true;
            this.checkBoxShowLastEffectsStream.CheckedChanged += new System.EventHandler(this.CheckBoxShowLastEffectsStream_CheckedChanged);
            // 
            // labelStreamCurrentMode
            // 
            this.labelStreamCurrentMode.AutoSize = true;
            this.labelStreamCurrentMode.Location = new System.Drawing.Point(8, 71);
            this.labelStreamCurrentMode.Name = "labelStreamCurrentMode";
            this.labelStreamCurrentMode.Size = new System.Drawing.Size(124, 13);
            this.labelStreamCurrentMode.TabIndex = 17;
            this.labelStreamCurrentMode.Text = "Current Mode: Cooldown";
            // 
            // buttonStreamToggle
            // 
            this.buttonStreamToggle.Enabled = false;
            this.buttonStreamToggle.Location = new System.Drawing.Point(368, 7);
            this.buttonStreamToggle.Name = "buttonStreamToggle";
            this.buttonStreamToggle.Size = new System.Drawing.Size(103, 23);
            this.buttonStreamToggle.TabIndex = 15;
            this.buttonStreamToggle.Text = "Start / Resume";
            this.buttonStreamToggle.UseVisualStyleBackColor = true;
            this.buttonStreamToggle.Click += new System.EventHandler(this.ButtonStreamToggle_Click);
            // 
            // comboBoxVotingCooldown
            // 
            this.comboBoxVotingCooldown.FormattingEnabled = true;
            this.comboBoxVotingCooldown.Location = new System.Drawing.Point(389, 257);
            this.comboBoxVotingCooldown.Name = "comboBoxVotingCooldown";
            this.comboBoxVotingCooldown.Size = new System.Drawing.Size(155, 21);
            this.comboBoxVotingCooldown.TabIndex = 14;
            this.comboBoxVotingCooldown.SelectedIndexChanged += new System.EventHandler(this.ComboBoxVotingCooldown_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(293, 260);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Voting Cooldown:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(317, 233);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Voting Time:";
            // 
            // comboBoxVotingTime
            // 
            this.comboBoxVotingTime.FormattingEnabled = true;
            this.comboBoxVotingTime.Location = new System.Drawing.Point(389, 230);
            this.comboBoxVotingTime.Name = "comboBoxVotingTime";
            this.comboBoxVotingTime.Size = new System.Drawing.Size(155, 21);
            this.comboBoxVotingTime.TabIndex = 11;
            this.comboBoxVotingTime.SelectedIndexChanged += new System.EventHandler(this.ComboBoxVotingTime_SelectedIndexChanged);
            // 
            // progressBarStream
            // 
            this.progressBarStream.Location = new System.Drawing.Point(8, 87);
            this.progressBarStream.Name = "progressBarStream";
            this.progressBarStream.Size = new System.Drawing.Size(536, 23);
            this.progressBarStream.TabIndex = 10;
            // 
            // listLastEffectsStream
            // 
            this.listLastEffectsStream.FormattingEnabled = true;
            this.listLastEffectsStream.Location = new System.Drawing.Point(8, 144);
            this.listLastEffectsStream.Name = "listLastEffectsStream";
            this.listLastEffectsStream.Size = new System.Drawing.Size(273, 134);
            this.listLastEffectsStream.TabIndex = 8;
            // 
            // textBoxStreamAccessToken
            // 
            this.textBoxStreamAccessToken.Location = new System.Drawing.Point(93, 32);
            this.textBoxStreamAccessToken.Name = "textBoxStreamAccessToken";
            this.textBoxStreamAccessToken.PasswordChar = '*';
            this.textBoxStreamAccessToken.Size = new System.Drawing.Size(125, 20);
            this.textBoxStreamAccessToken.TabIndex = 3;
            this.textBoxStreamAccessToken.TextChanged += new System.EventHandler(this.TextBoxOAuth_TextChanged);
            // 
            // buttonConnectStream
            // 
            this.buttonConnectStream.Enabled = false;
            this.buttonConnectStream.Location = new System.Drawing.Point(224, 32);
            this.buttonConnectStream.Name = "buttonConnectStream";
            this.buttonConnectStream.Size = new System.Drawing.Size(121, 22);
            this.buttonConnectStream.TabIndex = 1;
            this.buttonConnectStream.Text = "Connect to Stream";
            this.buttonConnectStream.UseVisualStyleBackColor = true;
            this.buttonConnectStream.Click += new System.EventHandler(this.ButtonConnectStream_Click);
            // 
            // tabPolls
            // 
            this.tabPolls.BackColor = System.Drawing.Color.Transparent;
            this.tabPolls.Controls.Add(this.label1);
            this.tabPolls.Controls.Add(this.numericUpDownTwitchPollsChannelPointsCost);
            this.tabPolls.Controls.Add(this.checkBoxTwitchPollsPostMessages);
            this.tabPolls.Controls.Add(this.labelTwitchPollsBitsCost);
            this.tabPolls.Controls.Add(this.numericUpDownTwitchPollsBitsCost);
            this.tabPolls.Location = new System.Drawing.Point(4, 22);
            this.tabPolls.Name = "tabPolls";
            this.tabPolls.Padding = new System.Windows.Forms.Padding(3);
            this.tabPolls.Size = new System.Drawing.Size(552, 293);
            this.tabPolls.TabIndex = 5;
            this.tabPolls.Text = "Polls";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Channel Points Cost For Voting (0 = Disabled)";
            // 
            // numericUpDownTwitchPollsChannelPointsCost
            // 
            this.numericUpDownTwitchPollsChannelPointsCost.Location = new System.Drawing.Point(233, 56);
            this.numericUpDownTwitchPollsChannelPointsCost.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownTwitchPollsChannelPointsCost.Name = "numericUpDownTwitchPollsChannelPointsCost";
            this.numericUpDownTwitchPollsChannelPointsCost.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownTwitchPollsChannelPointsCost.TabIndex = 7;
            this.numericUpDownTwitchPollsChannelPointsCost.ValueChanged += new System.EventHandler(this.NumericUpDownTwitchPollsChannelPointsCost_ValueChanged);
            // 
            // checkBoxTwitchPollsPostMessages
            // 
            this.checkBoxTwitchPollsPostMessages.AutoSize = true;
            this.checkBoxTwitchPollsPostMessages.Location = new System.Drawing.Point(8, 6);
            this.checkBoxTwitchPollsPostMessages.Name = "checkBoxTwitchPollsPostMessages";
            this.checkBoxTwitchPollsPostMessages.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.checkBoxTwitchPollsPostMessages.Size = new System.Drawing.Size(148, 17);
            this.checkBoxTwitchPollsPostMessages.TabIndex = 6;
            this.checkBoxTwitchPollsPostMessages.Text = "Post Vote Options In Chat";
            this.checkBoxTwitchPollsPostMessages.UseVisualStyleBackColor = true;
            this.checkBoxTwitchPollsPostMessages.CheckedChanged += new System.EventHandler(this.CheckBoxTwitchPollsPostMessages_CheckedChanged);
            // 
            // labelTwitchPollsBitsCost
            // 
            this.labelTwitchPollsBitsCost.AutoSize = true;
            this.labelTwitchPollsBitsCost.Location = new System.Drawing.Point(6, 35);
            this.labelTwitchPollsBitsCost.Name = "labelTwitchPollsBitsCost";
            this.labelTwitchPollsBitsCost.Size = new System.Drawing.Size(167, 13);
            this.labelTwitchPollsBitsCost.TabIndex = 3;
            this.labelTwitchPollsBitsCost.Text = "Bits Cost For Voting (0 = Disabled)";
            // 
            // numericUpDownTwitchPollsBitsCost
            // 
            this.numericUpDownTwitchPollsBitsCost.Location = new System.Drawing.Point(233, 33);
            this.numericUpDownTwitchPollsBitsCost.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownTwitchPollsBitsCost.Name = "numericUpDownTwitchPollsBitsCost";
            this.numericUpDownTwitchPollsBitsCost.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownTwitchPollsBitsCost.TabIndex = 0;
            this.numericUpDownTwitchPollsBitsCost.ValueChanged += new System.EventHandler(this.NumericUpDownTwitchPollsBitsCost_ValueChanged);
            // 
            // tabEffects
            // 
            this.tabEffects.BackColor = System.Drawing.Color.Transparent;
            this.tabEffects.Controls.Add(this.labelEffectCooldown);
            this.tabEffects.Controls.Add(this.numericUpDownEffectCooldown);
            this.tabEffects.Controls.Add(this.buttonEffectsToggleAll);
            this.tabEffects.Controls.Add(this.enabledEffectsView);
            this.tabEffects.Location = new System.Drawing.Point(4, 22);
            this.tabEffects.Name = "tabEffects";
            this.tabEffects.Padding = new System.Windows.Forms.Padding(3);
            this.tabEffects.Size = new System.Drawing.Size(552, 293);
            this.tabEffects.TabIndex = 1;
            this.tabEffects.Text = "Effects";
            // 
            // labelEffectCooldown
            // 
            this.labelEffectCooldown.AutoSize = true;
            this.labelEffectCooldown.Location = new System.Drawing.Point(385, 264);
            this.labelEffectCooldown.Name = "labelEffectCooldown";
            this.labelEffectCooldown.Size = new System.Drawing.Size(88, 13);
            this.labelEffectCooldown.TabIndex = 20;
            this.labelEffectCooldown.Text = "Effect Cooldown:";
            // 
            // numericUpDownEffectCooldown
            // 
            this.numericUpDownEffectCooldown.Location = new System.Drawing.Point(479, 262);
            this.numericUpDownEffectCooldown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownEffectCooldown.Name = "numericUpDownEffectCooldown";
            this.numericUpDownEffectCooldown.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownEffectCooldown.TabIndex = 19;
            this.numericUpDownEffectCooldown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownEffectCooldown.ValueChanged += new System.EventHandler(this.NumericUpDownEffectCooldown_ValueChanged);
            // 
            // buttonEffectsToggleAll
            // 
            this.buttonEffectsToggleAll.Location = new System.Drawing.Point(6, 259);
            this.buttonEffectsToggleAll.Name = "buttonEffectsToggleAll";
            this.buttonEffectsToggleAll.Size = new System.Drawing.Size(277, 23);
            this.buttonEffectsToggleAll.TabIndex = 7;
            this.buttonEffectsToggleAll.Text = "Toggle All";
            this.buttonEffectsToggleAll.UseVisualStyleBackColor = true;
            this.buttonEffectsToggleAll.Click += new System.EventHandler(this.ButtonEffectsToggleAll_Click);
            // 
            // enabledEffectsView
            // 
            this.enabledEffectsView.CheckBoxes = true;
            this.enabledEffectsView.Location = new System.Drawing.Point(6, 6);
            this.enabledEffectsView.Name = "enabledEffectsView";
            this.enabledEffectsView.Size = new System.Drawing.Size(538, 247);
            this.enabledEffectsView.TabIndex = 3;
            this.enabledEffectsView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.EnabledEffectsView_AfterCheck);
            this.enabledEffectsView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.EnabledEffectsView_NodeMouseDoubleClick);
            // 
            // tabSync
            // 
            this.tabSync.BackColor = System.Drawing.Color.Transparent;
            this.tabSync.Controls.Add(this.buttonSyncSend);
            this.tabSync.Controls.Add(this.textBoxSyncChat);
            this.tabSync.Controls.Add(this.listBoxSyncChat);
            this.tabSync.Controls.Add(this.labelSyncHost);
            this.tabSync.Controls.Add(this.textBoxSyncUsername);
            this.tabSync.Controls.Add(this.labelSyncUsername);
            this.tabSync.Controls.Add(this.labelSyncChannel);
            this.tabSync.Controls.Add(this.textBoxSyncChannel);
            this.tabSync.Controls.Add(this.labelSyncServer);
            this.tabSync.Controls.Add(this.textBoxSyncServer);
            this.tabSync.Controls.Add(this.buttonSyncConnect);
            this.tabSync.Location = new System.Drawing.Point(4, 22);
            this.tabSync.Name = "tabSync";
            this.tabSync.Padding = new System.Windows.Forms.Padding(3);
            this.tabSync.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabSync.Size = new System.Drawing.Size(552, 293);
            this.tabSync.TabIndex = 6;
            this.tabSync.Text = "Sync";
            // 
            // buttonSyncSend
            // 
            this.buttonSyncSend.Enabled = false;
            this.buttonSyncSend.Location = new System.Drawing.Point(457, 260);
            this.buttonSyncSend.Name = "buttonSyncSend";
            this.buttonSyncSend.Size = new System.Drawing.Size(87, 22);
            this.buttonSyncSend.TabIndex = 10;
            this.buttonSyncSend.Text = "Send";
            this.buttonSyncSend.UseVisualStyleBackColor = true;
            this.buttonSyncSend.Click += new System.EventHandler(this.ButtonSyncSend_Click);
            // 
            // textBoxSyncChat
            // 
            this.textBoxSyncChat.Location = new System.Drawing.Point(6, 261);
            this.textBoxSyncChat.MaxLength = 128;
            this.textBoxSyncChat.Name = "textBoxSyncChat";
            this.textBoxSyncChat.Size = new System.Drawing.Size(445, 20);
            this.textBoxSyncChat.TabIndex = 9;
            this.textBoxSyncChat.TextChanged += new System.EventHandler(this.TextBoxSyncChat_TextChanged);
            this.textBoxSyncChat.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxSyncChat_KeyDown);
            // 
            // listBoxSyncChat
            // 
            this.listBoxSyncChat.FormattingEnabled = true;
            this.listBoxSyncChat.Location = new System.Drawing.Point(6, 95);
            this.listBoxSyncChat.Name = "listBoxSyncChat";
            this.listBoxSyncChat.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxSyncChat.Size = new System.Drawing.Size(538, 160);
            this.listBoxSyncChat.TabIndex = 8;
            // 
            // labelSyncHost
            // 
            this.labelSyncHost.AutoSize = true;
            this.labelSyncHost.Location = new System.Drawing.Point(404, 9);
            this.labelSyncHost.Name = "labelSyncHost";
            this.labelSyncHost.Size = new System.Drawing.Size(87, 13);
            this.labelSyncHost.TabIndex = 7;
            this.labelSyncHost.Text = "Not connected...";
            // 
            // textBoxSyncUsername
            // 
            this.textBoxSyncUsername.Location = new System.Drawing.Point(70, 32);
            this.textBoxSyncUsername.MaxLength = 16;
            this.textBoxSyncUsername.Name = "textBoxSyncUsername";
            this.textBoxSyncUsername.Size = new System.Drawing.Size(328, 20);
            this.textBoxSyncUsername.TabIndex = 2;
            this.textBoxSyncUsername.TextChanged += new System.EventHandler(this.TextBoxSyncUsername_TextChanged);
            // 
            // labelSyncUsername
            // 
            this.labelSyncUsername.AutoSize = true;
            this.labelSyncUsername.Location = new System.Drawing.Point(8, 35);
            this.labelSyncUsername.Name = "labelSyncUsername";
            this.labelSyncUsername.Size = new System.Drawing.Size(58, 13);
            this.labelSyncUsername.TabIndex = 5;
            this.labelSyncUsername.Text = "Username:";
            // 
            // labelSyncChannel
            // 
            this.labelSyncChannel.AutoSize = true;
            this.labelSyncChannel.Location = new System.Drawing.Point(17, 61);
            this.labelSyncChannel.Name = "labelSyncChannel";
            this.labelSyncChannel.Size = new System.Drawing.Size(49, 13);
            this.labelSyncChannel.TabIndex = 4;
            this.labelSyncChannel.Text = "Channel:";
            // 
            // textBoxSyncChannel
            // 
            this.textBoxSyncChannel.Location = new System.Drawing.Point(70, 58);
            this.textBoxSyncChannel.MaxLength = 16;
            this.textBoxSyncChannel.Name = "textBoxSyncChannel";
            this.textBoxSyncChannel.Size = new System.Drawing.Size(328, 20);
            this.textBoxSyncChannel.TabIndex = 3;
            this.textBoxSyncChannel.TextChanged += new System.EventHandler(this.TextBoxSyncChannel_TextChanged);
            // 
            // labelSyncServer
            // 
            this.labelSyncServer.AutoSize = true;
            this.labelSyncServer.Location = new System.Drawing.Point(25, 9);
            this.labelSyncServer.Name = "labelSyncServer";
            this.labelSyncServer.Size = new System.Drawing.Size(41, 13);
            this.labelSyncServer.TabIndex = 2;
            this.labelSyncServer.Text = "Server:";
            // 
            // textBoxSyncServer
            // 
            this.textBoxSyncServer.Location = new System.Drawing.Point(70, 6);
            this.textBoxSyncServer.Name = "textBoxSyncServer";
            this.textBoxSyncServer.Size = new System.Drawing.Size(328, 20);
            this.textBoxSyncServer.TabIndex = 1;
            this.textBoxSyncServer.TextChanged += new System.EventHandler(this.TextBoxSyncServer_TextChanged);
            // 
            // buttonSyncConnect
            // 
            this.buttonSyncConnect.Enabled = false;
            this.buttonSyncConnect.Location = new System.Drawing.Point(404, 31);
            this.buttonSyncConnect.Name = "buttonSyncConnect";
            this.buttonSyncConnect.Size = new System.Drawing.Size(140, 22);
            this.buttonSyncConnect.TabIndex = 4;
            this.buttonSyncConnect.Text = "Connect";
            this.buttonSyncConnect.UseVisualStyleBackColor = true;
            this.buttonSyncConnect.Click += new System.EventHandler(this.ButtonSyncConnect_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.BackColor = System.Drawing.Color.Transparent;
            this.tabSettings.Controls.Add(this.checkBoxStreamHideVotingEffectsIngame);
            this.tabSettings.Controls.Add(this.checkBoxSettingsPlayAudioSequentially);
            this.tabSettings.Controls.Add(this.checkBoxPlayAudioForEffects);
            this.tabSettings.Controls.Add(this.label8);
            this.tabSettings.Controls.Add(this.textBoxSeed);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(552, 293);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Settings";
            // 
            // checkBoxStreamHideVotingEffectsIngame
            // 
            this.checkBoxStreamHideVotingEffectsIngame.AutoSize = true;
            this.checkBoxStreamHideVotingEffectsIngame.Checked = true;
            this.checkBoxStreamHideVotingEffectsIngame.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStreamHideVotingEffectsIngame.Location = new System.Drawing.Point(9, 268);
            this.checkBoxStreamHideVotingEffectsIngame.Name = "checkBoxStreamHideVotingEffectsIngame";
            this.checkBoxStreamHideVotingEffectsIngame.Size = new System.Drawing.Size(155, 17);
            this.checkBoxStreamHideVotingEffectsIngame.TabIndex = 10;
            this.checkBoxStreamHideVotingEffectsIngame.Text = "Hide Voting Effects Ingame";
            this.toolTipHandler.SetToolTip(this.checkBoxStreamHideVotingEffectsIngame, "Some effects play a sound clip when\r\nthey get activated. Check this to have\r\nthem" +
        " play.");
            this.checkBoxStreamHideVotingEffectsIngame.UseVisualStyleBackColor = true;
            this.checkBoxStreamHideVotingEffectsIngame.CheckedChanged += new System.EventHandler(this.checkBoxStreamHideVotingEffectsIngame_CheckedChanged);
            // 
            // checkBoxSettingsPlayAudioSequentially
            // 
            this.checkBoxSettingsPlayAudioSequentially.AutoSize = true;
            this.checkBoxSettingsPlayAudioSequentially.Checked = true;
            this.checkBoxSettingsPlayAudioSequentially.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSettingsPlayAudioSequentially.Location = new System.Drawing.Point(9, 55);
            this.checkBoxSettingsPlayAudioSequentially.Name = "checkBoxSettingsPlayAudioSequentially";
            this.checkBoxSettingsPlayAudioSequentially.Size = new System.Drawing.Size(136, 17);
            this.checkBoxSettingsPlayAudioSequentially.TabIndex = 9;
            this.checkBoxSettingsPlayAudioSequentially.Text = "Play Audio Sequentially";
            this.toolTipHandler.SetToolTip(this.checkBoxSettingsPlayAudioSequentially, "Some effects play a sound clip when\r\nthey get activated. Check this to have\r\nthem" +
        " play.");
            this.checkBoxSettingsPlayAudioSequentially.UseVisualStyleBackColor = true;
            this.checkBoxSettingsPlayAudioSequentially.CheckedChanged += new System.EventHandler(this.CheckBoxSettingsPlayAudioSequentially_CheckedChanged);
            // 
            // checkBoxPlayAudioForEffects
            // 
            this.checkBoxPlayAudioForEffects.AutoSize = true;
            this.checkBoxPlayAudioForEffects.Checked = true;
            this.checkBoxPlayAudioForEffects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPlayAudioForEffects.Location = new System.Drawing.Point(9, 32);
            this.checkBoxPlayAudioForEffects.Name = "checkBoxPlayAudioForEffects";
            this.checkBoxPlayAudioForEffects.Size = new System.Drawing.Size(130, 17);
            this.checkBoxPlayAudioForEffects.TabIndex = 8;
            this.checkBoxPlayAudioForEffects.Text = "Play Audio For Effects";
            this.toolTipHandler.SetToolTip(this.checkBoxPlayAudioForEffects, "Some effects play a sound clip when\r\nthey get activated. Check this to have\r\nthem" +
        " play.");
            this.checkBoxPlayAudioForEffects.UseVisualStyleBackColor = true;
            this.checkBoxPlayAudioForEffects.CheckedChanged += new System.EventHandler(this.CheckBoxPlayAudioForEffects_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Seed:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBoxSeed
            // 
            this.textBoxSeed.Location = new System.Drawing.Point(47, 6);
            this.textBoxSeed.Name = "textBoxSeed";
            this.textBoxSeed.Size = new System.Drawing.Size(497, 20);
            this.textBoxSeed.TabIndex = 1;
            this.textBoxSeed.TextChanged += new System.EventHandler(this.TextBoxSeed_TextChanged);
            // 
            // tabExperimental
            // 
            this.tabExperimental.BackColor = System.Drawing.Color.Transparent;
            this.tabExperimental.Controls.Add(this.button2);
            this.tabExperimental.Controls.Add(this.effect);
            this.tabExperimental.Controls.Add(this.button1);
            this.tabExperimental.Controls.Add(this.label4);
            this.tabExperimental.Controls.Add(this.label5);
            this.tabExperimental.Controls.Add(this.gtaport);
            this.tabExperimental.Controls.Add(this.gtaip);
            this.tabExperimental.Controls.Add(this.label9);
            this.tabExperimental.Controls.Add(this.connectb);
            this.tabExperimental.Controls.Add(this.label17);
            this.tabExperimental.Controls.Add(this.label16);
            this.tabExperimental.Controls.Add(this.port);
            this.tabExperimental.Controls.Add(this.ip);
            this.tabExperimental.Controls.Add(this.label19);
            this.tabExperimental.Controls.Add(this.checkBoxExperimentalYouTubeConnection);
            this.tabExperimental.Controls.Add(this.buttonExperimentalRunEffect);
            this.tabExperimental.Controls.Add(this.textBoxExperimentalEffectName);
            this.tabExperimental.Controls.Add(this.checkBoxExperimental_RunEffectOnAutoStart);
            this.tabExperimental.Location = new System.Drawing.Point(4, 22);
            this.tabExperimental.Name = "tabExperimental";
            this.tabExperimental.Padding = new System.Windows.Forms.Padding(3);
            this.tabExperimental.Size = new System.Drawing.Size(552, 293);
            this.tabExperimental.TabIndex = 7;
            this.tabExperimental.Text = "Experimental";
            // 
            // effect
            // 
            this.effect.FormattingEnabled = true;
            this.effect.Location = new System.Drawing.Point(101, 225);
            this.effect.Name = "effect";
            this.effect.Size = new System.Drawing.Size(121, 21);
            this.effect.TabIndex = 44;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 225);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 23);
            this.button1.TabIndex = 43;
            this.button1.Text = "Send Effect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(136, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "Port";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 41;
            this.label5.Text = "IP";
            // 
            // gtaport
            // 
            this.gtaport.Location = new System.Drawing.Point(127, 181);
            this.gtaport.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.gtaport.Name = "gtaport";
            this.gtaport.Size = new System.Drawing.Size(59, 20);
            this.gtaport.TabIndex = 40;
            this.gtaport.TabStop = false;
            this.gtaport.Value = new decimal(new int[] {
            9001,
            0,
            0,
            0});
            // 
            // gtaip
            // 
            this.gtaip.Location = new System.Drawing.Point(10, 181);
            this.gtaip.Name = "gtaip";
            this.gtaip.Size = new System.Drawing.Size(94, 20);
            this.gtaip.TabIndex = 39;
            this.gtaip.TabStop = false;
            this.gtaip.Text = "localhost";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(25, 143);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 15);
            this.label9.TabIndex = 38;
            this.label9.Text = "TCP / IP GTA";
            // 
            // connectb
            // 
            this.connectb.Location = new System.Drawing.Point(205, 94);
            this.connectb.Name = "connectb";
            this.connectb.Size = new System.Drawing.Size(73, 23);
            this.connectb.TabIndex = 9;
            this.connectb.Text = " Connect";
            this.connectb.UseVisualStyleBackColor = true;
            this.connectb.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(136, 78);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(26, 13);
            this.label17.TabIndex = 36;
            this.label17.Text = "Port";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(10, 80);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(17, 13);
            this.label16.TabIndex = 35;
            this.label16.Text = "IP";
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(127, 97);
            this.port.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(59, 20);
            this.port.TabIndex = 34;
            this.port.TabStop = false;
            this.port.Value = new decimal(new int[] {
            15636,
            0,
            0,
            0});
            // 
            // ip
            // 
            this.ip.Location = new System.Drawing.Point(10, 97);
            this.ip.Name = "ip";
            this.ip.Size = new System.Drawing.Size(94, 20);
            this.ip.TabIndex = 33;
            this.ip.TabStop = false;
            this.ip.Text = "127.0.0.1";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label19.Location = new System.Drawing.Point(25, 59);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(114, 15);
            this.label19.TabIndex = 32;
            this.label19.Text = "TCP / IP Youtube";
            // 
            // checkBoxExperimentalYouTubeConnection
            // 
            this.checkBoxExperimentalYouTubeConnection.AutoSize = true;
            this.checkBoxExperimentalYouTubeConnection.Location = new System.Drawing.Point(6, 29);
            this.checkBoxExperimentalYouTubeConnection.Name = "checkBoxExperimentalYouTubeConnection";
            this.checkBoxExperimentalYouTubeConnection.Size = new System.Drawing.Size(127, 17);
            this.checkBoxExperimentalYouTubeConnection.TabIndex = 16;
            this.checkBoxExperimentalYouTubeConnection.Text = "YouTube Connection";
            this.toolTipHandler.SetToolTip(this.checkBoxExperimentalYouTubeConnection, "When auto-start kicks in\r\nit will enable an effect immediately\r\ninstead of only s" +
        "tarting the\r\ntimer.\r\nDoesn\'t work for Twitch mode.");
            this.checkBoxExperimentalYouTubeConnection.UseVisualStyleBackColor = true;
            this.checkBoxExperimentalYouTubeConnection.CheckedChanged += new System.EventHandler(this.CheckBoxExperimentalYouTubeConnection_CheckedChanged);
            // 
            // buttonExperimentalRunEffect
            // 
            this.buttonExperimentalRunEffect.Location = new System.Drawing.Point(469, 263);
            this.buttonExperimentalRunEffect.Name = "buttonExperimentalRunEffect";
            this.buttonExperimentalRunEffect.Size = new System.Drawing.Size(75, 22);
            this.buttonExperimentalRunEffect.TabIndex = 15;
            this.buttonExperimentalRunEffect.Text = "Run";
            this.buttonExperimentalRunEffect.UseVisualStyleBackColor = true;
            this.buttonExperimentalRunEffect.Click += new System.EventHandler(this.ButtonExperimentalRunEffect_Click);
            // 
            // textBoxExperimentalEffectName
            // 
            this.textBoxExperimentalEffectName.Location = new System.Drawing.Point(8, 264);
            this.textBoxExperimentalEffectName.Name = "textBoxExperimentalEffectName";
            this.textBoxExperimentalEffectName.Size = new System.Drawing.Size(455, 20);
            this.textBoxExperimentalEffectName.TabIndex = 14;
            this.textBoxExperimentalEffectName.TextChanged += new System.EventHandler(this.TextBoxExperimentalEffectName_TextChanged);
            // 
            // checkBoxExperimental_RunEffectOnAutoStart
            // 
            this.checkBoxExperimental_RunEffectOnAutoStart.AutoSize = true;
            this.checkBoxExperimental_RunEffectOnAutoStart.Location = new System.Drawing.Point(6, 6);
            this.checkBoxExperimental_RunEffectOnAutoStart.Name = "checkBoxExperimental_RunEffectOnAutoStart";
            this.checkBoxExperimental_RunEffectOnAutoStart.Size = new System.Drawing.Size(157, 17);
            this.checkBoxExperimental_RunEffectOnAutoStart.TabIndex = 12;
            this.checkBoxExperimental_RunEffectOnAutoStart.Text = "Enable Effect On Auto-Start";
            this.toolTipHandler.SetToolTip(this.checkBoxExperimental_RunEffectOnAutoStart, "When auto-start kicks in\r\nit will enable an effect immediately\r\ninstead of only s" +
        "tarting the\r\ntimer.\r\nDoesn\'t work for Twitch mode.");
            this.checkBoxExperimental_RunEffectOnAutoStart.UseVisualStyleBackColor = true;
            this.checkBoxExperimental_RunEffectOnAutoStart.Click += new System.EventHandler(this.CheckBoxExperimental_EnableEffectOnAutoStart_CheckedChanged);
            // 
            // checkBoxAutoStart
            // 
            this.checkBoxAutoStart.AutoSize = true;
            this.checkBoxAutoStart.Location = new System.Drawing.Point(402, 16);
            this.checkBoxAutoStart.Name = "checkBoxAutoStart";
            this.checkBoxAutoStart.Size = new System.Drawing.Size(73, 17);
            this.checkBoxAutoStart.TabIndex = 8;
            this.checkBoxAutoStart.Text = "Auto-Start";
            this.checkBoxAutoStart.UseVisualStyleBackColor = true;
            this.checkBoxAutoStart.CheckedChanged += new System.EventHandler(this.CheckBoxAutoStart_CheckedChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.gameToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(560, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPresetToolStripMenuItem,
            this.savePresetToolStripMenuItem,
            this.experimentalToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadPresetToolStripMenuItem
            // 
            this.loadPresetToolStripMenuItem.Name = "loadPresetToolStripMenuItem";
            this.loadPresetToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.loadPresetToolStripMenuItem.Text = "Load Preset";
            this.loadPresetToolStripMenuItem.Click += new System.EventHandler(this.LoadPresetToolStripMenuItem_Click);
            // 
            // savePresetToolStripMenuItem
            // 
            this.savePresetToolStripMenuItem.Name = "savePresetToolStripMenuItem";
            this.savePresetToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.savePresetToolStripMenuItem.Text = "Save Preset";
            this.savePresetToolStripMenuItem.Click += new System.EventHandler(this.SavePresetToolStripMenuItem_Click);
            // 
            // experimentalToolStripMenuItem
            // 
            this.experimentalToolStripMenuItem.Name = "experimentalToolStripMenuItem";
            this.experimentalToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.experimentalToolStripMenuItem.Text = "Experimental";
            this.experimentalToolStripMenuItem.Click += new System.EventHandler(this.ExperimentalToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viceCityToolStripMenuItem,
            this.sanAndreasToolStripMenuItem});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // viceCityToolStripMenuItem
            // 
            this.viceCityToolStripMenuItem.Name = "viceCityToolStripMenuItem";
            this.viceCityToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.viceCityToolStripMenuItem.Text = "Vice City";
            this.viceCityToolStripMenuItem.Click += new System.EventHandler(this.ViceCityToolStripMenuItem_Click);
            // 
            // sanAndreasToolStripMenuItem
            // 
            this.sanAndreasToolStripMenuItem.Name = "sanAndreasToolStripMenuItem";
            this.sanAndreasToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.sanAndreasToolStripMenuItem.Text = "San Andreas";
            this.sanAndreasToolStripMenuItem.Click += new System.EventHandler(this.SanAndreasToolStripMenuItem_Click);
            // 
            // timerMain
            // 
            this.timerMain.Enabled = true;
            this.timerMain.Interval = 10;
            this.timerMain.Tick += new System.EventHandler(this.OnTimerTick);
            // 
            // buttonSwitchMode
            // 
            this.buttonSwitchMode.Location = new System.Drawing.Point(481, 12);
            this.buttonSwitchMode.Name = "buttonSwitchMode";
            this.buttonSwitchMode.Size = new System.Drawing.Size(73, 23);
            this.buttonSwitchMode.TabIndex = 7;
            this.buttonSwitchMode.Text = "Stream";
            this.buttonSwitchMode.UseVisualStyleBackColor = true;
            this.buttonSwitchMode.Click += new System.EventHandler(this.ButtonSwitchMode_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(258, 223);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(73, 23);
            this.button2.TabIndex = 45;
            this.button2.Text = "Test";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 360);
            this.Controls.Add(this.checkBoxAutoStart);
            this.Controls.Add(this.buttonSwitchMode);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "GTA:SA Chaos";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabs.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabMain.PerformLayout();
            this.tabStream.ResumeLayout(false);
            this.tabStream.PerformLayout();
            this.tabPolls.ResumeLayout(false);
            this.tabPolls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTwitchPollsChannelPointsCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTwitchPollsBitsCost)).EndInit();
            this.tabEffects.ResumeLayout(false);
            this.tabEffects.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEffectCooldown)).EndInit();
            this.tabSync.ResumeLayout(false);
            this.tabSync.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.tabExperimental.ResumeLayout(false);
            this.tabExperimental.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gtaport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.port)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private class MainCooldownComboBoxItem
        {
            public readonly string Text;
            public readonly int Time;

            public MainCooldownComboBoxItem(string text, int time)
            {
                this.Text = text;
                this.Time = time;
            }

            public override string ToString() => this.Text;
        }

        private class VotingTimeComboBoxItem
        {
            public readonly int VotingTime;
            public readonly string Text;

            public VotingTimeComboBoxItem(string text, int votingTime)
            {
                this.Text = text;
                this.VotingTime = votingTime;
            }

            public override string ToString() => this.Text;
        }

        private class VotingCooldownComboBoxItem
        {
            public readonly int VotingCooldown;
            public readonly string Text;

            public VotingCooldownComboBoxItem(string text, int votingCooldown)
            {
                this.Text = text;
                this.VotingCooldown = votingCooldown;
            }

            public override string ToString() => this.Text;
        }

        private class CategoryTreeNode : TreeNode
        {
            private readonly Category category;

            public CategoryTreeNode(Category _category)
            {
                this.category = _category;
                this.Name = this.Text = this.category.Name;
            }

            public void UpdateCategory()
            {
                bool flag = true;
                int num = 0;
                foreach (TreeNode node in this.Nodes)
                {
                    if (node.Checked)
                        ++num;
                    else
                        flag = false;
                }
                this.Checked = flag;
                this.Text = this.Name + string.Format(" ({0}/{1})", (object)num, (object)this.Nodes.Count);
            }
        }

        private class EffectTreeNode : TreeNode
        {
            public readonly AbstractEffect Effect;

            public EffectTreeNode(AbstractEffect effect, string description)
            {
                this.Effect = effect;
                this.Name = this.Text = description;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LoopRead();
            connectb.Enabled = false;
        }
        public async void LoopRead()
        {
            await Task.Run(()=> {
                TcpListener server = null;
                try
                {
                    IPAddress localAddr = IPAddress.Parse(ip.Text);
                    server = new TcpListener(localAddr, (int)port.Value);
                    server.Start();

                    while (true)
                    {
                        TcpClient client = server.AcceptTcpClient();
                        NetworkStream stream = client.GetStream();

                        byte[] inputbytes = new byte[1024];
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(inputbytes, 0, inputbytes.Length);
                            builder.Append(Encoding.Unicode.GetString(inputbytes, 0, bytes));
                        }
                        while (stream.DataAvailable);
                        string[] cheats = builder.ToString().Split(';');    //   INPUT

                        /*string tmp = "";
                        foreach (var item in cheats)
                        {
                            tmp += item + ";";
                        }
                        MessageBox.Show(tmp);*/

                        foreach (string cheat in cheats)
                        {
                            this.CallEffect(is_youtube: true, youtube_name: cheat);
                        }

                        string response = "200 OK";    //  OUTPUT
                        //Console.WriteLine(message);
                        byte[] data = Encoding.Unicode.GetBytes(response);
                        stream.Write(data, 0, data.Length);

                        stream.Close();
                        client.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    if (server != null)
                        server.Stop();
                }
            });
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            this.CallEffect(is_youtube: true, youtube_name: effect.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                effect.Items.AddRange(File.ReadAllLines("effects.txt"));
                effect.SelectedIndex = 0;
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var r = File.ReadAllLines("effects.txt");
            foreach (var item in r)
            {
                MessageBox.Show(item);
                this.CallEffect(is_youtube: true, youtube_name: item);
            }
        }
    }
}
