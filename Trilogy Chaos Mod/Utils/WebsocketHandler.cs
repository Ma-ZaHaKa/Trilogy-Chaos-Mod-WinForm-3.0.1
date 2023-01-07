using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSocketSharp;

namespace GTAChaos.Utils
{
    public class WebsocketHandler
    {
        public static WebsocketHandler INSTANCE = new WebsocketHandler();
        private WebSocket socket;
        private bool socketIsConnecting;
        private bool socketConnected;
        private readonly List<string> socketBuffer = new List<string>();

        public event EventHandler<SocketMessageEventArgs> OnSocketMessage;

        public void ConnectWebsocket()
        {
            try
            {
                if (this.socketConnected || this.socketIsConnecting)
                    return;
                //this.socket = new WebSocket("ws://localhost:9001", Array.Empty<string>());

                string adres = "192.168.0.103";
                //adres = "localhost";

                this.socket = new WebSocket($"ws://{adres}:9001", Array.Empty<string>());

                this.socket.OnOpen += new EventHandler(this.Socket_OnOpen);
                this.socket.OnClose += new EventHandler<CloseEventArgs>(this.Socket_OnClose);
                this.socket.OnError += new EventHandler<ErrorEventArgs>(this.Socket_OnError);
                this.socket.OnMessage += new EventHandler<MessageEventArgs>(this.Socket_OnMessage);
                this.socketIsConnecting = true;
                this.socket.Connect();
            }
            catch (Exception ex)
            {
                this.socketConnected = false;
                this.socketIsConnecting = false;
            }
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsText)
                return;
            EventHandler<SocketMessageEventArgs> onSocketMessage = this.OnSocketMessage;
            if (onSocketMessage == null)
                return;
            onSocketMessage((object)this, new SocketMessageEventArgs()
            {
                Data = e.Data
            });
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            this.socketConnected = true;
            this.socketIsConnecting = false;
            this.SendWebsocketBuffer();
        }

        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            this.socketConnected = false;
            this.socketIsConnecting = false;
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            this.socketConnected = false;
            this.socketIsConnecting = false;
        }

        private void SendWebsocketBuffer()
        {
            if (this.socketBuffer.Count <= 0)
                return;
            foreach (string str in this.socketBuffer)
                this.socket?.Send(str);
            this.socketBuffer.Clear();
        }

        public void SendDataToWebsocket(JObject jsonObject) => Task.Run((Action)(() =>
       {
           string str = JsonConvert.SerializeObject((object)jsonObject);
           this.ConnectWebsocket();
           if (this.socketConnected)
           {
               this.SendWebsocketBuffer();
               this.socket?.Send(str);
           }
           else
           {
               if (!(jsonObject["type"].ToObject<string>() != "time"))
                   return;
               this.socketBuffer.Add(str);
           }
       }));

        public void SendTimeToGame(int remaining, int cooldown = 0, string mode = "") => this.SendDataToWebsocket(JObject.FromObject((object)new
        {
            type = "time",
            data = new
            {
                remaining = remaining,
                cooldown = cooldown,
                mode = mode
            }
        }));

        public void SendVotes(string[] effects, int[] votes, int pickedChoice = -1) => this.SendDataToWebsocket(JObject.FromObject((object)new
        {
            type = nameof(votes),
            data = new
            {
                effects = effects,
                votes = votes,
                pickedChoice = pickedChoice
            }
        }));

        public void SendEffectToGame(
          string effectID,
          object effectData = null,
          int duration = -1,
          string displayName = "",
          string subtext = "",
          bool rapidFire = false)
        {
            if (rapidFire)
                duration = Math.Min(duration, 15000);
            this.SendDataToWebsocket(JObject.FromObject((object)new
            {
                type = "effect",
                data = new
                {
                    effectID = effectID,
                    effectData = (effectData ?? (object)new { }),
                    duration = duration,
                    displayName = (Ext.IsNullOrEmpty(displayName) ? effectID : displayName),
                    subtext = subtext
                }
            }));
        }
    }
}
