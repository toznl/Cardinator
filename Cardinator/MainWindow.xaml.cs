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
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Cardinator
{
    public partial class MainWindow : Window
    {
        //For using console windows
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        CoOrd coHeadPc1 = new CoOrd();
        public byte[] Serialize(object param)
        {
            byte[] encMsg = null;
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter br = new BinaryFormatter();
                br.Serialize(ms, param);
                encMsg = ms.ToArray();
            }

            return encMsg;
        }

        public T Deserialize<T>(byte[] param)
        {
            using (MemoryStream ms = new MemoryStream(param))
            {
                IFormatter br = new BinaryFormatter();
                br.Binder = new AllowAssemblyDeserializationBinder();
                return (T)br.Deserialize(ms);
            }
        }

        sealed class AllowAssemblyDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string TypeName)
            {
                Type typeToDeserialize = null;
                String currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                assemblyName = currentAssembly;

                typeToDeserialize = Type.GetType(String.Format("{0}, {1}", TypeName, assemblyName));

                return typeToDeserialize;

            }
        }


        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 11000);
            Socket sListner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Waiting for connection on port {0}", ipEndPoint);
            sListner.Bind(ipEndPoint);
            sListner.Listen(1024);
            while (true)
            {

                try
                {
                    int x = 0;
                    Socket handler = sListner.Accept();
                    byte[] buffer = new byte[10000];
                    Console.WriteLine("connected"); 
                    x = handler.Receive(buffer);
                    BinaryFormatter forDeserialize = new BinaryFormatter();
                    forDeserialize.Binder = new AllowAssemblyDeserializationBinder();

                    Deserialize<CoOrd>(buffer);
                    Console.WriteLine(x);

                    handler.Close();
                    


                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : {0}", e.ToString());

                }
            }
        }
    }
}
