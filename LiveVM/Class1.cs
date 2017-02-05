using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveVM
{
    public class LiveVM : BilibiliDM_PluginFramework.DMPlugin
    {
        private Socket s;
        public static IPAddress ip = IPAddress.Parse("192.168.127.129");
        public static int port = 12344;

        public LiveVM()
        {
            this.Connected += Class1_Connected;
            this.Disconnected += Class1_Disconnected;
            this.ReceivedDanmaku += Class1_ReceivedDanmaku;
            this.ReceivedRoomCount += Class1_ReceivedRoomCount;
            this.PluginAuth = "StoneMoe";
            this.PluginName = "LiveVM";
            this.PluginDesc = "作为TCPClient与虚拟机进行通信";
            this.PluginCont = "example@example.com";
            this.PluginVer = "v0.0.1";
        }
        private void shutdown()
        {
            s.Dispose();
        }
        public void ConnectHandler(IPAddress ip, int port)
        {
            try
            {
                Log(string.Format("Connecting to Host... {0}:{1}", ip.ToString(), port.ToString()));

                IPEndPoint iep = new IPEndPoint(ip, port);

                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.BeginConnect(
                    new IPEndPoint(ip, port),
                    new AsyncCallback(connectEnd),
                    s);
            }
            catch (Exception)
            {
                s.Dispose();
                Log("Connect Failed.");
            }
        }

        public void connectEnd(IAsyncResult iars)
        {
            Socket end = (Socket)iars.AsyncState;
            try
            {
                end.EndConnect(iars);
                s = end;
                Log("Connected");
            }
            catch (SocketException)
            {
                s.Dispose();
                Log("Can't reach Host");
            }
            catch (Exception)
            {
                s.Dispose();
                Log("Unknown Exception Occured");
            }
        }

        private byte[] parseMsg(string msg)
        {
            return Encoding.UTF8.GetBytes(Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(msg)) + "\r\n");
        }

        public void Sendmsg(string msg)
        {
            //Send Button
            try
            {
                s.Send(parseMsg(msg));
            }
            catch (Exception)
            {
                Log("Lost connection with Host,Your last message may lose");
                return;
            }
        }
        private void Class1_ReceivedRoomCount(object sender, BilibiliDM_PluginFramework.ReceivedRoomCountArgs e)
        {
        }

        private void Class1_ReceivedDanmaku(object sender, BilibiliDM_PluginFramework.ReceivedDanmakuArgs e)
        {
            if (e.Danmaku.CommentText == null)
            {
                return;
            }
            if (e.Danmaku.CommentText.Length < 2)
            {
                return;
            }
            if (e.Danmaku.CommentText.Substring(0, 1) == "!")
            {
                if (s.Connected && s != null)
                {
                    Sendmsg(e.Danmaku.CommentText.Substring(1).Replace('|','/'));
                }

            }
        }

        private void Class1_Disconnected(object sender, BilibiliDM_PluginFramework.DisconnectEvtArgs e)
        {
        }

        private void Class1_Connected(object sender, BilibiliDM_PluginFramework.ConnectedEvtArgs e)
        {
        }

        public override void Admin()
        {
            base.Admin();
            Form a = new setting();
            a.ShowDialog();
        }

        public override void Stop()
        {
            base.Stop();
            //請勿使用任何阻塞方法
            shutdown();
        }

        public override void Start()
        {
            base.Start();
            //請勿使用任何阻塞方法
            ConnectHandler(ip, port);
        }
    }
}
