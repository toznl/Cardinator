using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;



namespace Cardinator
{
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();


        public MainWindow()
        {
            AllocConsole();
            InitializeComponent();
        }

        public void server(string[] args)
        {
            IPHostEntry ipHost = Dns.Resolve("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 8080);

            // create a Tcp/Ip Socket
            Socket sListener = new Socket(AddressFamily.InterNetwork,
                        SocketType.Stream, ProtocolType.Tcp);


            // bind the socket to the local endpoint and
            // listen to the incoming sockets

            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                // Start listening for connections

                while (true)
                {
                    Console.WriteLine("Waiting for a connection on port {0}", ipEndPoint);

                    // program is suspended while waiting for an incoming connection
                    Socket handler = sListener.Accept();

                    string data = null;

                    // we got the client attempting to connect
                    while (true)
                    {
                        byte[] bytes = new byte[1024];

                        int bytesRec = handler.Receive(bytes);

                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        if (data.IndexOf("<TheEnd>") > -1)
                        {
                            break;
                        }
                    }

                    // show the data on the console
                    Console.WriteLine("Text Received: {0}", data);

                    string theReply = "Thank you for those " + data.Length.ToString()
                                    + " characters...";
                    byte[] msg = Encoding.ASCII.GetBytes(theReply);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }



        }

    }
}
