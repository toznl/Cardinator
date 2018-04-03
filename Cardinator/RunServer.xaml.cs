using System;
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

using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Security.Cryptography;

    

namespace Cardinator
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    [Serializable]
    public struct CoOrd
    {

        [MarshalAs(UnmanagedType.R4)]
        public float x;

        [MarshalAs(UnmanagedType.R4)]
        public float y;

        [MarshalAs(UnmanagedType.R4)]
        public float z;

        [MarshalAs(UnmanagedType.IUnknown)]
        public object markerType;

        [MarshalAs(UnmanagedType.IUnknown)]
        public object markerTrackingState;

        public CoOrd(float x1, float y1, float z1, object m1, object m2)
        {
            x = x1;
            y = y1;
            z = z1;
            markerType = m1;
            markerTrackingState = m2;

        }
    }

    public partial class RunServer : UserControl
    {
        private Grid root;

        ////For using console windows
        //[DllImport("kernel32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool AllocConsole();

        public RunServer()
        {
            InitializeComponent();
        }

       
    }
}