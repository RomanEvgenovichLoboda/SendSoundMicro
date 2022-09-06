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
                        // StringBuilder builder = new StringBuilder();
                        do
                        {
                            bytes = clientSocket.Receive(buffer);
                            //File.WriteAllBytes($"123.wav", buffer);
                            //builder.Append(Encoding.Unicode.GetString(buffer, 0, bytes));
                        } while (clientSocket.Available > 0);
                        string str = $"{DateTime.Now.ToString().Replace('.', '_').Replace(':', '_')}.wav";
                        if(bytes > 0) {
                            File.WriteAllBytes(str, buffer);
                            listBox1.Items.Add(str);
                        }
                        
                        //await Task.Delay(1);
                    } while (bytes!=0);

                    //Console.WriteLine($"ok");



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
            

            
           // Console.ReadLine();
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            clientSocket?.Close();
            //serverSocket.Shutdown(SocketShutdown.Both);
            serverSocket?.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Listen();
        }
    }
}