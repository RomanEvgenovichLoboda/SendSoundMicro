using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Media;

namespace SendSoundMicro
{
    public partial class ClientForm : Form
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        public static extern int mciSendString(
            string lpstrCommand,
            string lpstrReturnString,
            int uReturnLength,
            int hwndCallback
        );
        const int PORT = 8088;
        const string IP = "127.0.0.1";
        IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(IP), PORT);
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public ClientForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Red;
            mciSendString("open new type WAVEAudio alias recsound", "", 0, 0);
            mciSendString("record recsound", "", 0, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.Blue;
            mciSendString("stop recsound", "", 0, 0);
            mciSendString($"save recsound temp.wav", "", 0, 0); //($"save recsound {DateTime.Now.ToString().Replace('.', '_').Replace(':', '_')}.wav", "", 0, 0);
            mciSendString("close recsound", "", 0, 0);
            this.BackColor = Color.Green;

            string path = "temp.wav";

            clientSocket.SendFile(path);
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            clientSocket.Connect(iPEnd);
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}