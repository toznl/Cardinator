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

namespace Coordinator
{
    public partial class MainWindow : Window
    {
        //For using console windows
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        //CoOrd Structs to save and calibration 
        //Head
        CoOrd headPC1 = new CoOrd();
        CoOrd headPC2 = new CoOrd();
        CoOrd headCar = new CoOrd();

        //Neck
        CoOrd neckPC1 = new CoOrd();
        CoOrd neckPC2 = new CoOrd();
        CoOrd neckCar = new CoOrd();

        //ShoulderLeft
        CoOrd shoulderLeftPC1 = new CoOrd();
        CoOrd shoulderLeftPC2 = new CoOrd();
        CoOrd shoulderLeftCar = new CoOrd();

        //ElbowLeft
        CoOrd elbowLeftPC1 = new CoOrd();
        CoOrd elbowLeftPC2= new CoOrd();
        CoOrd elbowLeftCar = new CoOrd();

        //WristLeft
        CoOrd wristLeftPC1 = new CoOrd();
        CoOrd wristLeftPC2 = new CoOrd();
        CoOrd wristLeftCar = new CoOrd();

        //ShoulderRight
        CoOrd shoulderRightPC1 = new CoOrd();
        CoOrd shoulderRightPC2= new CoOrd();
        CoOrd shoulderRightCar = new CoOrd();

        //ElbowRight
        CoOrd elbowRightPC1 = new CoOrd();
        CoOrd elbowRightPC2 = new CoOrd();
        CoOrd elbowRightCar = new CoOrd();

        //WristRight
        CoOrd wristRightPC1 = new CoOrd();
        CoOrd wristRightPC2 = new CoOrd();
        CoOrd wristRightCar = new CoOrd();

        //SpineMid
        CoOrd spineMidPC1 = new CoOrd();
        CoOrd spineMidPC2 = new CoOrd();
        CoOrd spineMidCar = new CoOrd();

        //SpineBase
        CoOrd spineBasePC1 = new CoOrd();
        CoOrd spineBasePC2 = new CoOrd();
        CoOrd spineBaseCar = new CoOrd();

        struct body {
            public CoOrd head;
            public CoOrd neck;
            public CoOrd spineMid;
            public CoOrd spnieBase;
            public CoOrd shoulderRight;
            public CoOrd elbowRight;
            public CoOrd wristRight;
            public CoOrd shoulderLeft;
            public CoOrd elbowLeft;
            public CoOrd wirstLeft;
        }

        body BodySet = new body();

        
        //Ilist Body
        //IList<Body> BodyPC1;
        //IList<Body> BodyPC2;

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

        public float euculidianDistance(CoOrd dotPC1, CoOrd dotPC2)
        {
            float result;
            result = (dotPC1.x - dotPC2.x) ^ 2 + (dotPC1.y - dotPC2.y) ^ 2 + (dotPC1.z - dotPC2.z) ^ 2;


            return result;
        }

        public void Cardination (body bodySetPC1, body bodySetPC2)
        {
            
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

                    x = handler.Receive(buffer);
                    BinaryFormatter forDeserialize = new BinaryFormatter();
                    forDeserialize.Binder = new AllowAssemblyDeserializationBinder();

                    CoOrd buf = new CoOrd();
                    buf = Deserialize<CoOrd>(buffer);
                    Console.WriteLine(buf.markerPC);
                    if (buf.markerPC == 1) {
                        switch (buf.markerType)
                        {
                            case JointType.Head:
                                headPC1 = buf;
                                break;
                            case JointType.Neck:
                                neckPC1 = buf;
                                break;
                            case JointType.SpineBase:
                                spineBasePC1 = buf;
                                break;
                            case JointType.SpineMid:
                                spineMidPC1 = buf;
                                break;
                            case JointType.ShoulderLeft:
                                shoulderLeftPC1 = buf;
                                break;
                            case JointType.ElbowLeft:
                                elbowLeftPC1 = buf;
                                break;
                            case JointType.WristLeft:
                                wristLeftPC1 = buf;
                                break;
                            case JointType.ShoulderRight:
                                shoulderRightPC1 = buf;
                                break;
                            case JointType.ElbowRight:
                                elbowRightPC1 = buf;
                                break;
                            case JointType.WristRight:
                                wristRightPC1 = buf;
                                break;
                        }

                        Console.WriteLine("===============PC1==============");
                        if (wristRightPC1.markerTrackingState != null)
                        {

                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", headPC1.markerType, headPC1.markerTrackingState, headPC1.x, headPC1.y, headPC1.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", neckPC1.markerType, neckPC1.markerTrackingState, neckPC1.x, neckPC1.y, neckPC1.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", spineMidPC1.markerType, spineMidPC1.markerTrackingState, spineMidPC1.x, spineMidPC1.y, spineMidPC1.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", spineBasePC1.markerType, spineBasePC1.markerTrackingState, spineBasePC1.x, spineBasePC1.y, spineBasePC1.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", shoulderLeftPC1.markerType, shoulderLeftPC1.markerTrackingState, shoulderLeftPC1.x, shoulderLeftPC1.y, shoulderLeftPC1.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", elbowLeftPC1.markerType, elbowLeftPC1.markerTrackingState, elbowLeftPC1.x, elbowLeftPC1.y, elbowLeftPC1.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", wristLeftPC1.markerType, wristLeftPC1.markerTrackingState, wristLeftPC1.x, wristLeftPC1.y, wristLeftPC1.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", shoulderRightPC1.markerType, shoulderRightPC1.markerTrackingState, shoulderRightPC1.x, shoulderRightPC1.y, shoulderRightPC1.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", elbowRightPC1.markerType, elbowRightPC1.markerTrackingState, elbowRightPC1.x, elbowRightPC1.y, elbowRightPC1.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", wristRightPC1.markerType, wristRightPC1.markerTrackingState, wristRightPC1.x, wristRightPC1.y, wristRightPC1.z);

                        }
                        handler.Close();
                        Console.WriteLine("File Received");
                    }
                    else if (buf.markerPC == 2)
                    {
                        switch (buf.markerType)
                        {
                            case JointType.Head:
                                headPC2 = buf;
                                break;
                            case JointType.Neck:
                                neckPC2 = buf;
                                break;
                            case JointType.SpineBase:
                                spineBasePC2 = buf;
                                break;
                            case JointType.SpineMid:
                                spineMidPC2 = buf;
                                break;
                            case JointType.ShoulderLeft:
                                shoulderLeftPC2 = buf;
                                break;
                            case JointType.ElbowLeft:
                                elbowLeftPC2 = buf;
                                break;
                            case JointType.WristLeft:
                                wristLeftPC2 = buf;
                                break;
                            case JointType.ShoulderRight:
                                shoulderRightPC2 = buf;
                                break;
                            case JointType.ElbowRight:
                                elbowRightPC2 = buf;
                                break;
                            case JointType.WristRight:
                                wristRightPC2 = buf;
                                break;
                        }

                        Console.WriteLine("===============PC2==============");
                        if (wristRightPC2.markerTrackingState != null)
                        {

                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", headPC2.markerType, headPC2.markerTrackingState, headPC2.x, headPC2.y, headPC2.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", neckPC2.markerType, neckPC2.markerTrackingState, neckPC2.x, neckPC2.y, neckPC2.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", spineMidPC2.markerType, spineMidPC2.markerTrackingState, spineMidPC2.x, spineMidPC2.y, spineMidPC2.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", spineBasePC2.markerType, spineBasePC2.markerTrackingState, spineBasePC2.x, spineBasePC2.y, spineBasePC2.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", shoulderLeftPC2.markerType, shoulderLeftPC2.markerTrackingState, shoulderLeftPC2.x, shoulderLeftPC2.y, shoulderLeftPC2.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", elbowLeftPC2.markerType, elbowLeftPC2.markerTrackingState, elbowLeftPC2.x, elbowLeftPC2.y, elbowLeftPC2.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", wristLeftPC2.markerType, wristLeftPC2.markerTrackingState, wristLeftPC2.x, wristLeftPC2.y, wristLeftPC2.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", shoulderRightPC2.markerType, shoulderRightPC2.markerTrackingState, shoulderRightPC2.x, shoulderRightPC2.y, shoulderRightPC2.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", elbowRightPC2.markerType, elbowRightPC2.markerTrackingState, elbowRightPC2.x, elbowRightPC2.y, elbowRightPC2.z);
                            Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", wristRightPC2.markerType, wristRightPC2.markerTrackingState, wristRightPC2.x, wristRightPC2.y, wristRightPC2.z);

                        }
                        handler.Close();
                        Console.WriteLine("File Received");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : {0}", e.ToString());

                }
            }
        }
    }
}
