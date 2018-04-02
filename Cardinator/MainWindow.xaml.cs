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
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Cardinator
{
    public partial class MainWindow : Window
    {

     
        public MainWindow()
        {
            InitializeComponent();
            RunServer sv = new RunServer();
            root.Children.Add(sv);
            this.Closing += delegate { Environment.Exit(1); };

            TcpListener server = null;

            try
            {
                server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
                server.Start();

                byte[] buffer = new byte[1024];

                while (true)
                {
                    Console.WriteLine("Waiting for a connection.....");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("\nConnected!!");

                    NetworkStream stream = client.GetStream();

                    while (stream.Read(buffer, 0, buffer.Length) != 0)
                    {
                        // deserializing;
                        CoOrd packet = new CoOrd();
                        packet.Deserialize(ref buffer);

                        float x = packet.x;
                        float y = packet.y;
                        float z = packet.z;

                        Console.WriteLine("===========================================");
                        Console.WriteLine("X : {0}", x);
                        Console.WriteLine("Y : {0}", y);
                        Console.WriteLine("Z : {0}", z);
                        Console.WriteLine("===========================================");
                    }

                    stream.Close();
                    client.Close();
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            Console.ReadLine();

        }

       
    }
}
