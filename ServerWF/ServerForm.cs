using System.Media;
using System.Net;
using System.Net.Sockets;

namespace ServerWF
{
    public partial class ServerForm : Form
    {
        const int PORT = 8088;
        const string IP = "127.0.0.1";
        IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse(IP), PORT);
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket clientSocket;
        public ServerForm()
        {
            InitializeComponent();

            fileSystemWatcher1.Path = Directory.GetCurrentDirectory();
            fileSystemWatcher1.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
            fileSystemWatcher1.Filter = "*.wav";
            fileSystemWatcher1.IncludeSubdirectories = true;
            fileSystemWatcher1.EnableRaisingEvents = true;
            foreach (var item in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                if (Path.GetExtension(item).Equals(".wav")) { listBox1.Items.Add(Path.GetFileName(item).Replace("\\","/")); }
            }

            Listen();
        }
        async void Listen()
        {
            await Task.Run(() => {
                try
                {
                    serverSocket.Bind(iPEnd);
                    serverSocket.Listen(10);
                    clientSocket = serverSocket.Accept();
                    int bytes = 2;
                    do
                    {
                        label1.Text = $"Server listen on {IP}:{PORT}";
                        bytes = 0;
                        byte[] buffer = new byte[51024];
                        do
                        {
                            bytes = clientSocket.Receive(buffer);
                        } while (clientSocket.Available > 0);
                        string str = $"message__{DateTime.Now.ToString().Replace('.', '_').Replace(':', '_')}.wav";
                        if(bytes > 0) {
                            File.WriteAllBytes(str, buffer);
                            //listBox1.Items.Add(str);

                        }
                    } while (bytes!=0);
                    clientSocket?.Close();
                    serverSocket.Shutdown(SocketShutdown.Both);
                    serverSocket?.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                label1.Text = "Server end...";
            });
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            clientSocket?.Close();
            //serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket?.Close();
        }

        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                SoundPlayer soundPlayer = new SoundPlayer(listBox1.SelectedItem.ToString());
                soundPlayer.Play();
            });
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            MessageBox.Show("File Ad");
            listBox1.Items.Add(e.Name);
        }
    }
}