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
        //Theta and Translation Valiables for Error function and initialized
        public struct ErrorVar
        {
            public double thetaX;
            public double thetaY;
            public double thetaZ;

            public double transX;
            public double transY;
            public double transZ;

            public ErrorVar(double x1, double y1, double z1, double x2, double y2, double z2)
            {
                thetaX = x1;
                thetaY = y1;
                thetaZ = z1;

                transX = x2;
                transY = y2;
                transZ = z2;

            }
        }


        public struct CoOrdXYZ
        {
            public float x;
            public float y;
            public float z;
            public CoOrdXYZ(float x1, float y1, float z1)
            {
                x = x1;
                y = y1;
                z = z1;
            }
        }


        //For using console windows
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        CoOrdXYZ preProPC1 = new CoOrdXYZ();
        CoOrdXYZ preProPC2 = new CoOrdXYZ();

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
        CoOrd elbowLeftPC2 = new CoOrd();
        CoOrd elbowLeftCar = new CoOrd();

        //WristLeft
        CoOrd wristLeftPC1 = new CoOrd();
        CoOrd wristLeftPC2 = new CoOrd();
        CoOrd wristLeftCar = new CoOrd();

        //ShoulderRight
        CoOrd shoulderRightPC1 = new CoOrd();
        CoOrd shoulderRightPC2 = new CoOrd();
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

        //struct body {
        //    public CoOrd head;
        //    public CoOrd neck;
        //    public CoOrd spineMid;
        //    public CoOrd spnieBase;
        //    public CoOrd shoulderRight;
        //    public CoOrd elbowRight;
        //    public CoOrd wristRight;
        //    public CoOrd shoulderLeft;
        //    public CoOrd elbowLeft;
        //    public CoOrd wirstLeft;
        //}

        //body BodySet = new body();

        ////Ilist Body
        ////IList<Body> BodyPC1;
        ////IList<Body> BodyPC2;

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

        //Square product
        public float square(float a)
        {
            float z;
            z = a * a;

            return z;
        }


        //Euculidian Distance of PC1, PC2

        public float distanceXYZ(CoOrd DOT1, CoOrd DOT2)
        {
            float distanceResult;

            distanceResult = (square(DOT1.x - DOT2.x) + square(DOT1.y - DOT2.y) + square(DOT1.z - DOT2.z));

            return distanceResult;
        }

        //Calibration of PC1, PC2
        public CoOrdXYZ preProcess(float x, float y, float z)
        {
            ErrorVar errorVar = new ErrorVar(10, 10, 10, 10, 10, 10);
            CoOrdXYZ preProcess = new CoOrdXYZ();

            preProcess.x = (Convert.ToSingle(Math.Cos(errorVar.thetaY) * Math.Cos(errorVar.thetaZ)) * (x))
                - (Convert.ToSingle(Math.Sin(errorVar.thetaY)) * z)
                + (Convert.ToSingle(Math.Cos(errorVar.thetaY) * Math.Sin(errorVar.thetaZ)) * y
                + Convert.ToSingle(errorVar.transX));

            preProcess.y = (y * Convert.ToSingle(((Math.Cos(errorVar.thetaX) * Math.Cos(errorVar.thetaZ)) + (Math.Sin(errorVar.thetaX) * Math.Sin(errorVar.thetaY) * Math.Sin(errorVar.thetaZ)))))
                - (x * Convert.ToSingle(((Math.Cos(errorVar.thetaX) * Math.Sin(errorVar.thetaZ)) - (Math.Cos(errorVar.thetaZ) * Math.Sin(errorVar.thetaX) * Math.Sin(errorVar.thetaY)))))
                + (z * Convert.ToSingle((Math.Sin(errorVar.thetaX) * Math.Sin(errorVar.thetaY))))
                + (Convert.ToSingle(errorVar.transY));

            preProcess.z = (x * Convert.ToSingle((Math.Sin(errorVar.thetaX) * Math.Sin(errorVar.thetaZ)) + (Math.Cos(errorVar.thetaX) * Math.Cos(errorVar.thetaZ) * Math.Sin(errorVar.thetaY))))
                - (y * Convert.ToSingle(((Math.Cos(errorVar.thetaZ) * Math.Sin(errorVar.thetaX)) - (Math.Cos(errorVar.thetaX) * Math.Sin(errorVar.thetaY) * Math.Sin(errorVar.thetaZ)))))
                + (z * Convert.ToSingle((Math.Cos(errorVar.thetaX) * Math.Sin(errorVar.thetaY))))
                + (Convert.ToSingle(errorVar.transZ));

            return preProcess;

        }

        public float errorFunc0(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {
            float result;

            result = 2 * (x * (Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) + Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) - Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(errorVar.transY) - y1 - x * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) - Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - 2 * (y * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - x * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) - Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) + z * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(errorVar.transZ) - z1 + x * (Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) + Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) - Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)));

            return result;
        }

        public float errorFunc1(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {
            float result;
            result= 2 * (z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaY)) + x * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + y * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) * (Convert.ToSingle(errorVar.transZ) - z1 + x * (Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) + Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) - Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - 2 * (z * Convert.ToSingle(Math.Cos(errorVar.thetaY)) + x * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) + y * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) * (Convert.ToSingle(errorVar.transX) - x1 - z * Convert.ToSingle(Math.Sin(errorVar.thetaY)) + x * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + y * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + 2 * (z * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) + x * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) + y * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) * (Convert.ToSingle(errorVar.transY) - y1 - x * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) - Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)));
            return result;
        }
        public float errorFunc2(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {
            float result;
            result = 2 * (y * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) - x * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) * (Convert.ToSingle(errorVar.transX) - x1 - z * Convert.ToSingle(Math.Sin(errorVar.thetaY)) + x * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + y * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + 2 * (x * (Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) - Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + y * (Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) + Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)))) * (Convert.ToSingle(errorVar.transZ) - z1 + x * (Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) + Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) - Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - 2 * (x * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) - Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)))) * (Convert.ToSingle(errorVar.transY) - y1 - x * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) - Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)));
            return result;
        }
        public float errorFunc3(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {
            float result;
            result = 2 * Convert.ToSingle(errorVar.transX) - 2 * x1 - 2 * z * Convert.ToSingle(Math.Sin(errorVar.thetaY)) + 2 * x * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + 2 * y * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ));
            return result;
        }
        public float errorFunc4(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {
            float result;
            result = 2 * Convert.ToSingle(errorVar.transY) - 2 * y1 - 2 * x * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) - Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) + 2 * y * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + 2 * z * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY));
            return result;
        }
        public float errorFunc5(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {
            float result;
            result = 2 * Convert.ToSingle(errorVar.transZ) - 2 * z1 + 2 * x * (Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) + Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - 2 * y * (Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) - Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + 2 * z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY));
            return result;
        }

        public ErrorVar gradientDecsent(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {

            float etaX = 0.01f;
            float etaY = 0.01f;
            float etaZ = 0.01f;
            float etaXt = 0.01f;
            float etaYt = 0.01f;
            float etaZt = 0.01f;

            ErrorVar resultVar;

            for (int i = 0; i < 500; i++)
            {
              

                float temp0 = errorFunc0(x, y, z, x1, y1, z1, errorVar);
                float temp1 = errorFunc1(x, y, z, x1, y1, z1, errorVar);
                float temp2 = errorFunc2(x, y, z, x1, y1, z1, errorVar);
                float temp3 = errorFunc3(x, y, z, x1, y1, z1, errorVar);
                float temp4 = errorFunc4(x, y, z, x1, y1, z1, errorVar);
                float temp5 = errorFunc5(x, y, z, x1, y1, z1, errorVar);

                errorVar.thetaX = errorVar.thetaX + etaX;
                errorVar.thetaY = errorVar.thetaY + etaY;
                errorVar.thetaZ = errorVar.thetaZ + etaZ;
                errorVar.transX = errorVar.transX + etaXt;
                errorVar.transY = errorVar.transY + etaYt;
                errorVar.transZ = errorVar.transZ + etaZt;

                if (errorFunc0(x, y, z, x1, y1, z1, errorVar) > temp0)
                {
                    etaX += 0.01f;
                }
                else if (errorFunc0(x, y, z, x1, y1, z1, errorVar) < temp0)
                {
                    etaX -= 0.01f;
                }
                else if (temp0 == 0)
                {
                    etaX = 0;
                }

                if (errorFunc1(x, y, z, x1, y1, z1, errorVar) < temp1)
                {
                    etaY += 0.01f;
                }
                else if (errorFunc0(x, y, z, x1, y1, z1, errorVar) > temp1)
                {
                    etaY -= 0.01f;
                }
                else if (temp1 == 0)
                {
                    etaY = 0;
                }


                if (errorFunc0(x, y, z, x1, y1, z1, errorVar) < temp2)
                {
                    etaZ += 0.01f;
                }
                else if (errorFunc0(x, y, z, x1, y1, z1, errorVar) > temp2)
                {
                    etaZ -= 0.01f;
                }
                if (temp2 == 0)
                {
                    etaZ = 0;
                }

                if (errorFunc0(x, y, z, x1, y1, z1, errorVar) < temp3)
                {
                    etaXt += 0.01f;
                }
                else if (errorFunc0(x, y, z, x1, y1, z1, errorVar) > temp3)                {
                    etaXt -= 0.01f;
                }
                else if (temp3 == 0)
                {
                    etaXt = 0;
                }

                if (errorFunc1(x, y, z, x1, y1, z1, errorVar) < temp4)
                {
                    etaYt += 0.01f;
                }
                else if (errorFunc0(x, y, z, x1, y1, z1, errorVar) > temp4)
                {
                    etaYt -= 0.01f;
                }
                else if (temp4 == 0)
                {
                    etaYt = 0;
                }


                if (errorFunc0(x, y, z, x1, y1, z1, errorVar) < temp5)
                {
                    etaZt += 0.01f;
                }
                else if (errorFunc0(x, y, z, x1, y1, z1, errorVar) > temp5)
                {
                    etaZt -= 0.01f;
                }
                if (temp5 == 0)
                {
                    etaZt = 0;
                }

                if (temp0 < 0.1 && temp1 < 1 && temp2 < 1 && temp3 < 1 && temp4 < 1 && temp5 < 1) break;



                resultVar = errorVar;
                //Console.WriteLine("thX : {0}, thY : {1}, thZ : {2}, tlX : {3}, tlY : {4}, tlZ : {5}", errorVar.thetaX, errorVar.thetaY, errorVar.thetaZ, errorVar.transX, errorVar.transY, errorVar.transZ);
                //Console.WriteLine("thX : {0}, thY : {1}, thZ : {2}, tlX : {3}, tlY : {4}, tlZ : {5}", temp0, temp1, temp2, temp3, temp4, temp5);
            }
            resultVar = errorVar;

            return resultVar;

        }

        public MainWindow()
        {
            //Alloc Console Window
            InitializeComponent();
            AllocConsole();

            RunServer cl = new RunServer(root);
            root.Children.Add(cl);

            //Variables for Socket
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 11000);
            Socket sListner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Waiting for connection on port {0}", ipEndPoint);
            sListner.Bind(ipEndPoint);
            sListner.Listen(1024);

            ErrorVar calVar;
            calVar.thetaX = 1;
            calVar.thetaY = 1;
            calVar.thetaZ = 1;
            calVar.transX = 1;
            calVar.transY = 1;
            calVar.transZ = 1;

            while (true)
            {
                try
                {

                    //Variables for Socket and Transport Coordination
                    int x = 0;
                    Socket handler = sListner.Accept();
                    byte[] buffer = new byte[10000];

                    x = handler.Receive(buffer);
                    BinaryFormatter forDeserialize = new BinaryFormatter();
                    forDeserialize.Binder = new AllowAssemblyDeserializationBinder();

                    CoOrd buf = new CoOrd();
                    buf = Deserialize<CoOrd>(buffer);

                    if (buf.markerPC == 1)
                    {
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
                    }

                    //Console.WriteLine("===============PC1==============");
                    //if (wristRightPC1.markerTrackingState != null)
                    //{

                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", headPC1.markerType, headPC1.markerTrackingState, headPC1.x, headPC1.y, headPC1.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", neckPC1.markerType, neckPC1.markerTrackingState, neckPC1.x, neckPC1.y, neckPC1.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", spineMidPC1.markerType, spineMidPC1.markerTrackingState, spineMidPC1.x, spineMidPC1.y, spineMidPC1.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", spineBasePC1.markerType, spineBasePC1.markerTrackingState, spineBasePC1.x, spineBasePC1.y, spineBasePC1.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", shoulderLeftPC1.markerType, shoulderLeftPC1.markerTrackingState, shoulderLeftPC1.x, shoulderLeftPC1.y, shoulderLeftPC1.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", elbowLeftPC1.markerType, elbowLeftPC1.markerTrackingState, elbowLeftPC1.x, elbowLeftPC1.y, elbowLeftPC1.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", wristLeftPC1.markerType, wristLeftPC1.markerTrackingState, wristLeftPC1.x, wristLeftPC1.y, wristLeftPC1.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", shoulderRightPC1.markerType, shoulderRightPC1.markerTrackingState, shoulderRightPC1.x, shoulderRightPC1.y, shoulderRightPC1.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", elbowRightPC1.markerType, elbowRightPC1.markerTrackingState, elbowRightPC1.x, elbowRightPC1.y, elbowRightPC1.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", wristRightPC1.markerType, wristRightPC1.markerTrackingState, wristRightPC1.x, wristRightPC1.y, wristRightPC1.z);

                    //}
                    //handler.Close();
                    //Console.WriteLine("File Received");


                    //Console.WriteLine("===============PC2==============");
                    //if (wristRightPC2.markerTrackingState != null)
                    //{

                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", headPC2.markerType, headPC2.markerTrackingState, headPC2.x, headPC2.y, headPC2.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", neckPC2.markerType, neckPC2.markerTrackingState, neckPC2.x, neckPC2.y, neckPC2.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", spineMidPC2.markerType, spineMidPC2.markerTrackingState, spineMidPC2.x, spineMidPC2.y, spineMidPC2.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", spineBasePC2.markerType, spineBasePC2.markerTrackingState, spineBasePC2.x, spineBasePC2.y, spineBasePC2.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", shoulderLeftPC2.markerType, shoulderLeftPC2.markerTrackingState, shoulderLeftPC2.x, shoulderLeftPC2.y, shoulderLeftPC2.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", elbowLeftPC2.markerType, elbowLeftPC2.markerTrackingState, elbowLeftPC2.x, elbowLeftPC2.y, elbowLeftPC2.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", wristLeftPC2.markerType, wristLeftPC2.markerTrackingState, wristLeftPC2.x, wristLeftPC2.y, wristLeftPC2.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", shoulderRightPC2.markerType, shoulderRightPC2.markerTrackingState, shoulderRightPC2.x, shoulderRightPC2.y, shoulderRightPC2.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", elbowRightPC2.markerType, elbowRightPC2.markerTrackingState, elbowRightPC2.x, elbowRightPC2.y, elbowRightPC2.z);
                    //    Console.WriteLine("JonitType : {0}, JointState : {1}, x : {2}, y : {3}, z : {4}", wristRightPC2.markerType, wristRightPC2.markerTrackingState, wristRightPC2.x, wristRightPC2.y, wristRightPC2.z);

                    //}

                    float disHead = distanceXYZ(headPC1, headPC2);
                    float disNeck = distanceXYZ(neckPC1, neckPC2);
                    float disSpineMid = distanceXYZ(spineMidPC1, spineMidPC2);
                    float disSpineBase = distanceXYZ(spineBasePC1, spineBasePC2);
                    float disShoulderLeft = distanceXYZ(shoulderLeftPC1, shoulderLeftPC2);
                    float disElbowLeft = distanceXYZ(elbowLeftPC1, elbowLeftPC2);
                    float disWristLeft = distanceXYZ(wristLeftPC1, wristLeftPC2);
                    float disShoulderRight = distanceXYZ(shoulderRightPC1, shoulderRightPC2);
                    float disElbowRight = distanceXYZ(elbowRightPC1, elbowRightPC2);
                    float disWristRight = distanceXYZ(wristRightPC1, wristRightPC2);

                    Console.WriteLine("dishead : {0} ", disHead);

                    //Decide which one is the shortest one

                    float shortestDIs;
                    String shortestDisName;

                    if (disHead < disNeck)
                    {
                        shortestDisName = "Head";
                        shortestDIs = disHead;

                    }
                    else
                    {
                        shortestDisName = "Neck";
                        shortestDIs = disNeck;
                    }

                    if (disSpineMid < shortestDIs)
                    {
                        shortestDisName = "SpineMid";
                        shortestDIs = disSpineMid;
                    }

                    if (disSpineBase < shortestDIs)
                    {
                        shortestDisName = "SpineBase";
                        shortestDIs = disSpineBase;
                    }

                    if (disShoulderLeft < shortestDIs)
                    {
                        shortestDisName = "ShoulderLeft";
                        shortestDIs = disShoulderLeft;
                    }

                    if (disElbowLeft < shortestDIs)
                    {
                        shortestDisName = "ElbowLeft";
                        shortestDIs = disElbowLeft;
                    }

                    if (disWristLeft < shortestDIs)
                    {
                        shortestDisName = "WristLeft";
                        shortestDIs = disWristLeft;
                    }

                    if (disShoulderRight < shortestDIs)
                    {
                        shortestDisName = "ShoulderRight";
                        shortestDIs = disShoulderRight;
                    }

                    if (disElbowRight < shortestDIs)
                    {
                        shortestDisName = "ElbowRight";
                        shortestDIs = disElbowRight;
                    }

                    if (disWristRight < shortestDIs)
                    {
                        shortestDisName = "WristRight";
                        shortestDIs = disWristRight;
                    }

                    switch (shortestDisName)
                    {
                        case "Head":
                            preProPC1 = preProcess(headPC1.x, headPC1.y, headPC1.z);
                            preProPC2.x = headPC2.x;
                            preProPC2.y = headPC2.y;
                            preProPC2.z = headPC2.z;
                            break;

                        case "Neck":
                            preProPC1 = preProcess(neckPC1.x, neckPC1.y, neckPC1.z);
                            preProPC2.x = neckPC2.x;
                            preProPC2.y = neckPC2.y;
                            preProPC2.z = neckPC2.z;
                            break;

                        case "SpineMid":
                            preProPC1 = preProcess(spineMidPC1.x, spineMidPC1.y, spineMidPC1.z);
                            preProPC2.x = spineMidPC2.x;
                            preProPC2.y = spineMidPC2.y;
                            preProPC2.z = spineMidPC2.z;
                            break;

                        case "SpineBase":
                            preProPC1 = preProcess(spineBasePC1.x, spineBasePC1.y, spineBasePC1.z);
                            preProPC2.x = spineBasePC2.x;
                            preProPC2.y = spineBasePC2.y;
                            preProPC2.z = spineBasePC2.z;
                            break;

                        case "ShoulderLeft":
                            preProPC1 = preProcess(shoulderLeftPC1.x, shoulderLeftPC1.y, shoulderLeftPC1.z);
                            preProPC2.x = shoulderLeftPC2.x;
                            preProPC2.y = shoulderLeftPC2.y;
                            preProPC2.z = shoulderLeftPC2.z;
                            break;

                        case "ShoulderRight":
                            preProPC1 = preProcess(shoulderRightPC1.x, shoulderRightPC1.y, shoulderRightPC1.z);
                            preProPC2.x = shoulderRightPC2.x;
                            preProPC2.y = shoulderRightPC2.y;
                            preProPC2.z = shoulderRightPC2.z;
                            break;

                        case "ElbowLeft":
                            preProPC1 = preProcess(elbowLeftPC1.x, elbowLeftPC1.y, elbowLeftPC1.z);
                            preProPC2.x = elbowLeftPC2.x;
                            preProPC2.y = elbowLeftPC2.y;
                            preProPC2.z = elbowLeftPC2.z;
                            break;

                        case "ElbowRight":
                            preProPC1 = preProcess(elbowRightPC1.x, elbowRightPC1.y, elbowRightPC1.z);
                            preProPC2.x = elbowRightPC2.x;
                            preProPC2.y = elbowRightPC2.y;
                            preProPC2.z = elbowRightPC2.z;
                            break;

                        case "WristLeft":
                            preProPC1 = preProcess(wristLeftPC1.x, wristLeftPC1.y, wristLeftPC1.z);
                            preProPC2.x = wristLeftPC2.x;
                            preProPC2.y = wristLeftPC2.y;
                            preProPC2.z = wristLeftPC2.z;
                            break;

                        case "WristRight":
                            preProPC1 = preProcess(wristRightPC1.x, wristRightPC1.y, wristRightPC1.z);
                            preProPC2.x = wristRightPC2.x;
                            preProPC2.y = wristRightPC2.y;
                            preProPC2.z = wristRightPC2.z;
                            break;
                    }



                    handler.Close();

                    calVar = gradientDecsent(preProPC1.x, preProPC1.y, preProPC1.z, preProPC2.x, preProPC2.y, preProPC2.z, calVar);

                    Console.WriteLine("Shortest Joint is {0}", shortestDisName);
                    Console.WriteLine("Shortest length of X,Y,Z = {0}", shortestDIs);
                    //Console.WriteLine("Preprocessing of shortest Joint x={0}, y={1}, z={2}", preProPC1.x, preProPC1.y, preProPC1.z);
                    //Console.WriteLine("wristPC1 Z = {0}, wristPC2 Z = {1}", wristLeftPC1.z, wristLeftPC2.z);

                    headCar.x =;



                    //Drawing Skeleton

                    canvas.Children.Clear();
                    Ellipse drawHead = new Ellipse
                    {
                        Fill = Brushes.Red,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawHead, headPC1.x - drawHead.Width / 2);
                    Canvas.SetTop(drawHead, headPC1.y - drawHead.Height / 2);
                    canvas.Children.Add(drawHead);

                    Ellipse drawLeftShoulder = new Ellipse
                    {
                        Fill = Brushes.Yellow,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawLeftShoulder, shoulderLeftPC1.x - drawLeftShoulder.Width / 2);
                    Canvas.SetTop(drawLeftShoulder, shoulderLeftPC1.y - drawLeftShoulder.Height / 2);
                    canvas.Children.Add(drawLeftShoulder);

                    Ellipse drawNeck = new Ellipse
                    {
                        Fill = Brushes.Orange,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawNeck, neckPC1.x - drawNeck.Width / 2);
                    Canvas.SetTop(drawNeck, neckPC1.y - drawNeck.Height / 2);
                    canvas.Children.Add(drawNeck);

                    Ellipse drawElbowLeft = new Ellipse
                    {
                        Fill = Brushes.Yellow,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawElbowLeft, elbowLeftPC1.x - drawElbowLeft.Width / 2);
                    Canvas.SetTop(drawElbowLeft, elbowLeftPC1.y - drawElbowLeft.Height / 2);
                    canvas.Children.Add(drawElbowLeft);

                    Ellipse drawWristLeft = new Ellipse
                    {
                        Fill = Brushes.Yellow,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawWristLeft, wristLeftPC1.x - drawWristLeft.Width / 2);
                    Canvas.SetTop(drawWristLeft, wristLeftPC1.y - drawWristLeft.Height / 2);
                    canvas.Children.Add(drawWristLeft);

                    Ellipse drawRightShoulder = new Ellipse
                    {
                        Fill = Brushes.Green,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawRightShoulder, shoulderRightPC1.x - drawRightShoulder.Width / 2);
                    Canvas.SetTop(drawRightShoulder, shoulderRightPC1.y - drawRightShoulder.Height / 2);
                    canvas.Children.Add(drawRightShoulder);

                    Ellipse drawElbowRight = new Ellipse
                    {
                        Fill = Brushes.Green,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawElbowRight, elbowRightPC1.x - drawElbowRight.Width / 2);
                    Canvas.SetTop(drawElbowRight, elbowRightPC1.y - drawElbowRight.Height / 2);
                    canvas.Children.Add(drawElbowRight);

                    Ellipse drawWristRight = new Ellipse
                    {
                        Fill = Brushes.Green,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawWristRight, wristRightPC1.x - drawWristRight.Width / 2);
                    Canvas.SetTop(drawWristRight, wristRightPC1.y - drawWristRight.Height / 2);
                    canvas.Children.Add(drawWristRight);

                    Ellipse drawSpineBase = new Ellipse
                    {
                        Fill = Brushes.Orange,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawSpineBase, spineBasePC1.x - drawSpineBase.Width / 2);
                    Canvas.SetTop(drawSpineBase, spineBasePC1.y - drawSpineBase.Height / 2);
                    canvas.Children.Add(drawSpineBase);

                    Ellipse drawSpineMid = new Ellipse
                    {
                        Fill = Brushes.Orange,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawSpineMid, spineMidPC1.x - drawSpineMid.Width / 2);
                    Canvas.SetTop(drawSpineMid, spineMidPC1.y - drawSpineMid.Height / 2);
                    canvas.Children.Add(drawSpineMid);

                    //Drawing Skeleton
                    //Head to Neck
                    Line lineHeadToNeck = new Line();
                    lineHeadToNeck.Stroke = Brushes.LightSteelBlue;
                    lineHeadToNeck.X1 = headPC1.x;
                    lineHeadToNeck.Y1 = headPC1.y;
                    lineHeadToNeck.X2 = neckPC1.x;
                    lineHeadToNeck.Y2 = neckPC1.y;
                    lineHeadToNeck.StrokeThickness = 2;

                    //Neck to LeftShoulder
                    Line lineNeckToLeftShoulder = new Line();
                    lineNeckToLeftShoulder.Stroke = Brushes.LightSteelBlue;
                    lineNeckToLeftShoulder.X1 = shoulderLeftPC1.x;
                    lineNeckToLeftShoulder.Y1 = shoulderLeftPC1.y;
                    lineNeckToLeftShoulder.X2 = neckPC1.x;
                    lineNeckToLeftShoulder.Y2 = neckPC1.y;
                    lineNeckToLeftShoulder.StrokeThickness = 2;

                    //LeftShoulder to LeftElbow
                    Line lineLeftShoulderToElbowLeft = new Line();
                    lineLeftShoulderToElbowLeft.Stroke = Brushes.LightSteelBlue;
                    lineLeftShoulderToElbowLeft.X1 = shoulderLeftPC1.x;
                    lineLeftShoulderToElbowLeft.Y1 = shoulderLeftPC1.y;
                    lineLeftShoulderToElbowLeft.X2 = elbowLeftPC1.x;
                    lineLeftShoulderToElbowLeft.Y2 = elbowLeftPC1.y;
                    lineLeftShoulderToElbowLeft.StrokeThickness = 2;

                    //LeftElbow to LeftWrist
                    Line lineElbowLeftToWristLeft = new Line();
                    lineElbowLeftToWristLeft.Stroke = Brushes.LightSteelBlue;
                    lineElbowLeftToWristLeft.X1 = elbowLeftPC1.x;
                    lineElbowLeftToWristLeft.Y1 = elbowLeftPC1.y;
                    lineElbowLeftToWristLeft.X2 = wristLeftPC1.x;
                    lineElbowLeftToWristLeft.Y2 = wristLeftPC1.y;
                    lineElbowLeftToWristLeft.StrokeThickness = 2;

                    //Neck to RightShoulder
                    Line lineNeckToRightShoulder = new Line();
                    lineNeckToRightShoulder.Stroke = Brushes.LightSteelBlue;
                    lineNeckToRightShoulder.X1 = shoulderRightPC1.x;
                    lineNeckToRightShoulder.Y1 = shoulderRightPC1.y;
                    lineNeckToRightShoulder.X2 = neckPC1.x;
                    lineNeckToRightShoulder.Y2 = neckPC1.y;
                    lineNeckToRightShoulder.StrokeThickness = 2;

                    //RightShoulder to RightElbow
                    Line lineRightShoulderToElbowRight = new Line();
                    lineRightShoulderToElbowRight.Stroke = Brushes.LightSteelBlue;
                    lineRightShoulderToElbowRight.X1 = shoulderRightPC1.x;
                    lineRightShoulderToElbowRight.Y1 = shoulderRightPC1.y;
                    lineRightShoulderToElbowRight.X2 = elbowRightPC1.x;
                    lineRightShoulderToElbowRight.Y2 = elbowRightPC1.y;
                    lineRightShoulderToElbowRight.StrokeThickness = 2;

                    //RightElbow to RightWrist
                    Line lineElbowRightToWristRight = new Line();
                    lineElbowRightToWristRight.Stroke = Brushes.LightSteelBlue;
                    lineElbowRightToWristRight.X1 = elbowRightPC1.x;
                    lineElbowRightToWristRight.Y1 = elbowRightPC1.y;
                    lineElbowRightToWristRight.X2 = wristRightPC1.x;
                    lineElbowRightToWristRight.Y2 = wristRightPC1.y;
                    lineElbowRightToWristRight.StrokeThickness = 2;

                    //Neck to SpineMid
                    Line lineNeckToSpineMid = new Line();
                    lineNeckToSpineMid.Stroke = Brushes.LightSteelBlue;
                    lineNeckToSpineMid.X1 = neckPC1.x;
                    lineNeckToSpineMid.Y1 = neckPC1.y;
                    lineNeckToSpineMid.X2 = spineMidPC1.x;
                    lineNeckToSpineMid.Y2 = spineMidPC1.y;
                    lineNeckToSpineMid.StrokeThickness = 2;

                    //SpineMid to SpineBase
                    Line lineSpineMidToSpineBase = new Line();
                    lineSpineMidToSpineBase.Stroke = Brushes.LightSteelBlue;
                    lineSpineMidToSpineBase.X1 = spineMidPC1.x;
                    lineSpineMidToSpineBase.Y1 = spineMidPC1.y;
                    lineSpineMidToSpineBase.X2 = spineBasePC1.x;
                    lineSpineMidToSpineBase.Y2 = spineBasePC1.y;
                    lineSpineMidToSpineBase.StrokeThickness = 2;


                    canvas.Children.Add(lineHeadToNeck);
                    canvas.Children.Add(lineNeckToLeftShoulder);
                    canvas.Children.Add(lineNeckToRightShoulder);
                    canvas.Children.Add(lineLeftShoulderToElbowLeft);
                    canvas.Children.Add(lineRightShoulderToElbowRight);
                    canvas.Children.Add(lineElbowRightToWristRight);
                    canvas.Children.Add(lineElbowLeftToWristLeft);
                    canvas.Children.Add(lineNeckToSpineMid);
                    canvas.Children.Add(lineSpineMidToSpineBase);

                }



                catch (Exception e)
                {
                    Console.WriteLine("Exception : {0}", e.ToString());

                }
            }
        }


    }
}
