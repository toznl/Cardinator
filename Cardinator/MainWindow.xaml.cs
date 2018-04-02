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

                byte[] buffer = new byte[8092];

                while (true)
                {
                    Console.WriteLine("Waiting for a connection.....");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("\nConnected!!");

                    NetworkStream stream = client.GetStream();

                    CoOrd packet = new CoOrd();
                    //buffer = null;

                    while (stream.Read(buffer, 0, Marshal.SizeOf(packet)) != 0)
                    {

                        float x = packet.x;
                        float y = packet.y;
                        float z = packet.z;


                        Console.WriteLine("이 름 : {0}", x);
                        Console.WriteLine("과 목 : {0}", y);
                        Console.WriteLine("점 수 : {0}", z);
                        Console.WriteLine("");
                        Console.WriteLine("===========================================");
                        Console.WriteLine("");
                    }

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
