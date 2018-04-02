﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections;
using System.Runtime.InteropServices;

namespace Cardinator
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CoOrd
    {
        [MarshalAs(UnmanagedType.R4)]
        public float x;

        [MarshalAs(UnmanagedType.R4)]
        public float y;

        [MarshalAs(UnmanagedType.R4)]
        public float z;

        public CoOrd(float x1, float y1, float z1)
        {
            x = x1;
            y = y1;
            z = z1;
        }

        // Calling this method will return a byte array with the contents
        // of the struct ready to be sent via the tcp socket.
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(CoOrd))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }

        // this method will deserialize a byte array into the struct.
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (CoOrd)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(CoOrd));
            gch.Free();
        }
    }

    public partial class RunServer : UserControl
    {
        private Socket socket = null;
        private Socket sockUser = null;
        private Thread waitConnecter = null;
        private Thread waitMsg = null;
        private Hashtable Users = new Hashtable();
        private Hashtable Waits = new Hashtable();
        private Grid root;
        private List<string> ipList = new List<string>();

        //For using console windows
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public RunServer()
        {
            AllocConsole();
            InitializeComponent();
            startServer();
        }

        private void startServer()
        {
            try
            {
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 8080);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(iep);
                socket.Listen(100);
                waitConnecter = new Thread(new ThreadStart(waitConn));
                waitConnecter.Start();
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }
        }

        private void waitConn()
        {
            while (true)
            {
                sockUser = socket.Accept();
                string ip = (sockUser.LocalEndPoint.ToString().Split(':'))[0];
                try
                {
                    Users.Add(ip, sockUser);
                }
                catch (ArgumentException ae) { ip = ip + "!"; Users.Add(ip, sockUser); }
                ipList.Add(ip);
                waitMsg = new Thread(waitText);
                //waitMsg.IsBackground = true;
                waitMsg.Start(ip);
            }
        }

        private void waitText(object key)
        {
            string ip = key as string;
            Socket user = Users[ip] as Socket;
            bool ck = true;
            while (ck)
            {
                try
                {
                    byte[] data2 = new byte[1024];  //msg2: 클라이언트에서 보낸 string
                    string msg2;
                    user.Receive(data2, data2.Length, SocketFlags.None);
                    msg2 = Encoding.Default.GetString(data2);
                    msg2 = msg2.TrimEnd('\0');
                    Console.WriteLine(msg2);
                    for (int i = 0; i < Users.Count; i++)
                    {
                        ((Socket)Users[ipList[i]]).Send(Encoding.Default.GetBytes(msg2));
                    }

                }
                catch (SocketException se) { ((Socket)Users[ip]).Close(); ck = false; waitMsg.Abort(); waitMsg = null; GC.Collect(); }
                catch (NullReferenceException) { MessageBox.Show("n!"); }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); waitMsg = null; ck = false; waitMsg.Abort(); waitMsg = null; GC.Collect(); }

            }
        }
    }
}
