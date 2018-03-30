﻿using System;
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
        }

       
    }
}
