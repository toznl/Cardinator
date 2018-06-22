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

        //Calibration of PC1, PC2
        public CoOrd preProcess(float x, float y, float z, ErrorVar errorVar)
        {
            CoOrd preProcess = new CoOrd();

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

        //Partial Differential // ErrorFunciton
        public float errorFunc0(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {
            float result;

            result = 2 * (x * (Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) + Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) - Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(errorVar.transY) - y1 - x * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) - Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - 2 * (y * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - x * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) - Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) + z * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(errorVar.transZ) - z1 + x * (Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) + Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) - Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)));

            return result;
        }

        public float errorFunc1(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {
            float result;
            result = 2 * (z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaY)) + x * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + y * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) * (Convert.ToSingle(errorVar.transZ) - z1 + x * (Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) + Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) - Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) - 2 * (z * Convert.ToSingle(Math.Cos(errorVar.thetaY)) + x * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) + y * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) * (Convert.ToSingle(errorVar.transX) - x1 - z * Convert.ToSingle(Math.Sin(errorVar.thetaY)) + x * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + y * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + 2 * (z * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) + x * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) + y * Convert.ToSingle(Math.Cos(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) * (Convert.ToSingle(errorVar.transY) - y1 - x * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ)) - Convert.ToSingle(Math.Cos(errorVar.thetaZ)) * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaX)) * Convert.ToSingle(Math.Cos(errorVar.thetaZ)) + Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)) * Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + z * Convert.ToSingle(Math.Sin(errorVar.thetaX)) * Convert.ToSingle(Math.Sin(errorVar.thetaY)));
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

        //GradientDecsent Diffrential 
        public ErrorVar gradientDecsent(
            float x0, float y0, float z0, float x1, float y1, float z1,
            float x2, float y2, float z2, float x3, float y3, float z3,
            float x4, float y4, float z4, float x5, float y5, float z5, ErrorVar errorVar)
        {

            float eta = 0.1f;
            float tol = 0.1f;

            ErrorVar resultVar;

            for (int i = 0; i < 1000; i++)
            {
                float grad0 = errorFunc0(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc0(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc0(x4, y4, z4, x5, y5, z5, errorVar);
                float grad1 = errorFunc1(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc1(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc1(x4, y4, z4, x5, y5, z5, errorVar);
                float grad2 = errorFunc2(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc2(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc2(x4, y4, z4, x5, y5, z5, errorVar);
                float grad3 = errorFunc3(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc3(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc3(x4, y4, z4, x5, y5, z5, errorVar);
                float grad4 = errorFunc4(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc4(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc4(x4, y4, z4, x5, y5, z5, errorVar);
                float grad5 = errorFunc5(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc5(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc5(x4, y4, z4, x5, y5, z5, errorVar);

                errorVar.thetaX = errorVar.thetaX - eta * grad0;
                errorVar.thetaY = errorVar.thetaY - eta * grad1;
                errorVar.thetaZ = errorVar.thetaZ - eta * grad2;
                errorVar.transX = errorVar.transX - eta * grad3;
                errorVar.transY = errorVar.transY - eta * grad4;
                errorVar.transZ = errorVar.transZ - eta * grad5;

                // 종료조건
                if ((grad0 < tol) && (grad1 < tol) && (grad2 < tol) && (grad3 < tol) && (grad4 < tol) && (grad5 < tol)) break;

                resultVar = errorVar;
                //Console.WriteLine("thX : {0}, thY : {1}, thZ : {2}, tlX : {3}, tlY : {4}, tlZ : {5}", errorVar.thetaX, errorVar.thetaY, errorVar.thetaZ, errorVar.transX, errorVar.transY, errorVar.transZ);
                //Console.WriteLine("thX : {0}, thY : {1}, thZ : {2}, tlX : {3}, tlY : {4}, tlZ : {5}", grad0, grad1, grad2, grad3, grad4, grad5);
            }
            resultVar = errorVar;

            return resultVar;

        }

        public void Calibration()
        {

            //Call Variables of Rotation and Translation and initialize
            ErrorVar calVar;
            calVar.thetaX = -1;
            calVar.thetaY = -3;
            calVar.thetaZ = 0.5;
            calVar.transX = 0.7;
            calVar.transY = 0;
            calVar.transZ = 0.2;

            //Variables for Socket
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 11000);
            Socket sListner = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sListner.Bind(ipEndPoint);
            sListner.Listen(1024);

            while (true)
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

                try
                {

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

                    //Gradient Decsent
                    calVar = gradientDecsent(
                        headPC1.x, headPC1.y, headPC1.z, headPC2.x, headPC2.y, headPC2.z,
                        shoulderLeftPC1.x, shoulderLeftPC1.y, shoulderLeftPC1.z, shoulderLeftPC2.x, shoulderLeftPC2.y, shoulderLeftPC2.z,
                        shoulderRightPC1.x, shoulderRightPC1.y, shoulderRightPC1.z, shoulderRightPC2.x, shoulderRightPC2.y, shoulderRightPC2.z,
                        calVar
                        );

                    //Rotate and translation of pc1 with Variables from Gradient Decsent
                    headCar = preProcess(headPC1.x, headPC1.y, headPC1.z, calVar);
                    neckCar = preProcess(neckPC1.x, neckPC1.y, neckPC1.z, calVar);
                    spineBaseCar = preProcess(spineBasePC1.x, spineBasePC1.y, spineBasePC1.z, calVar);
                    spineMidCar = preProcess(spineMidPC1.x, spineMidPC1.y, spineMidPC1.z, calVar);
                    shoulderLeftCar = preProcess(shoulderLeftPC1.x, shoulderLeftPC1.y, shoulderLeftPC1.z, calVar);
                    shoulderRightCar = preProcess(shoulderRightPC1.x, shoulderRightPC1.y, shoulderRightPC1.z, calVar);
                    wristLeftCar = preProcess(wristLeftPC1.x, wristLeftPC1.y, wristLeftPC1.z, calVar);
                    wristRightCar = preProcess(wristRightPC1.x, wristRightPC1.y, wristRightPC1.z, calVar);
                    elbowLeftCar = preProcess(elbowLeftPC1.x, elbowLeftPC1.y, elbowLeftPC1.z, calVar);
                    elbowRightCar = preProcess(elbowRightPC1.x, elbowRightPC1.y, elbowRightPC1.z, calVar);

                    canvas.Children.Clear();
                    //DrawSkeleton
                    //Head
                    Ellipse drawHead = new Ellipse
                    {
                        Fill = Brushes.Red,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawHead, headCar.x - drawHead.Width / 2);
                    Canvas.SetTop(drawHead, headCar.y - drawHead.Height / 2);
                    canvas.Children.Add(drawHead);

                    //Neck
                    Ellipse drawNeck = new Ellipse
                    {
                        Fill = Brushes.Orange,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawNeck, neckCar.x - drawNeck.Width / 2);
                    Canvas.SetTop(drawNeck, neckCar.y - drawNeck.Height / 2);
                    canvas.Children.Add(drawNeck);

                    //Left Shoulder
                    Ellipse drawLeftShoulder = new Ellipse
                    {
                        Fill = Brushes.Yellow,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawLeftShoulder, shoulderLeftCar.x - drawLeftShoulder.Width / 2);
                    Canvas.SetTop(drawLeftShoulder, shoulderLeftCar.y - drawLeftShoulder.Height / 2);
                    canvas.Children.Add(drawLeftShoulder);


                    //Left Elbow
                    Ellipse drawElbowLeft = new Ellipse
                    {
                        Fill = Brushes.Yellow,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawElbowLeft, elbowLeftCar.x - drawElbowLeft.Width / 2);
                    Canvas.SetTop(drawElbowLeft, elbowLeftCar.y - drawElbowLeft.Height / 2);
                    canvas.Children.Add(drawElbowLeft);


                    //Left Wrist
                    Ellipse drawWristLeft = new Ellipse
                    {
                        Fill = Brushes.Yellow,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawWristLeft, wristLeftCar.x - drawWristLeft.Width / 2);
                    Canvas.SetTop(drawWristLeft, wristLeftCar.y - drawWristLeft.Height / 2);
                    canvas.Children.Add(drawWristLeft);

                    //Right Shoulder
                    Ellipse drawRightShoulder = new Ellipse
                    {
                        Fill = Brushes.Green,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawRightShoulder, shoulderRightCar.x - drawRightShoulder.Width / 2);
                    Canvas.SetTop(drawRightShoulder, shoulderRightCar.y - drawRightShoulder.Height / 2);
                    canvas.Children.Add(drawRightShoulder);

                    //Right Elbow
                    Ellipse drawElbowRight = new Ellipse
                    {
                        Fill = Brushes.Green,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawElbowRight, elbowRightCar.x - drawElbowRight.Width / 2);
                    Canvas.SetTop(drawElbowRight, elbowRightCar.y - drawElbowRight.Height / 2);
                    canvas.Children.Add(drawElbowRight);

                    Ellipse drawWristRight = new Ellipse
                    {
                        Fill = Brushes.Green,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawWristRight, wristRightCar.x - drawWristRight.Width / 2);
                    Canvas.SetTop(drawWristRight, wristRightCar.y - drawWristRight.Height / 2);
                    canvas.Children.Add(drawWristRight);

                    Ellipse drawSpineBase = new Ellipse
                    {
                        Fill = Brushes.Orange,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawSpineBase, spineBaseCar.x - drawSpineBase.Width / 2);
                    Canvas.SetTop(drawSpineBase, spineBaseCar.y - drawSpineBase.Height / 2);
                    canvas.Children.Add(drawSpineBase);

                    Ellipse drawSpineMid = new Ellipse
                    {
                        Fill = Brushes.Orange,
                        Width = 20,
                        Height = 20
                    };

                    Canvas.SetLeft(drawSpineMid, spineMidCar.x - drawSpineMid.Width / 2);
                    Canvas.SetTop(drawSpineMid, spineMidCar.y - drawSpineMid.Height / 2);
                    canvas.Children.Add(drawSpineMid);

                    //Head to Neck
                    Line lineHeadToNeck = new Line();
                    lineHeadToNeck.Stroke = Brushes.LightSteelBlue;
                    lineHeadToNeck.X1 = headCar.x;
                    lineHeadToNeck.Y1 = headCar.y;
                    lineHeadToNeck.X2 = neckCar.x;
                    lineHeadToNeck.Y2 = neckCar.y;
                    lineHeadToNeck.StrokeThickness = 2;

                    //Neck to LeftShoulder
                    Line lineNeckToLeftShoulder = new Line();
                    lineNeckToLeftShoulder.Stroke = Brushes.LightSteelBlue;
                    lineNeckToLeftShoulder.X1 = shoulderLeftCar.x;
                    lineNeckToLeftShoulder.Y1 = shoulderLeftCar.y;
                    lineNeckToLeftShoulder.X2 = neckCar.x;
                    lineNeckToLeftShoulder.Y2 = neckCar.y;
                    lineNeckToLeftShoulder.StrokeThickness = 2;

                    //LeftShoulder to LeftElbow
                    Line lineLeftShoulderToElbowLeft = new Line();
                    lineLeftShoulderToElbowLeft.Stroke = Brushes.LightSteelBlue;
                    lineLeftShoulderToElbowLeft.X1 = shoulderLeftCar.x;
                    lineLeftShoulderToElbowLeft.Y1 = shoulderLeftCar.y;
                    lineLeftShoulderToElbowLeft.X2 = elbowLeftCar.x;
                    lineLeftShoulderToElbowLeft.Y2 = elbowLeftCar.y;
                    lineLeftShoulderToElbowLeft.StrokeThickness = 2;

                    //LeftElbow to LeftWrist
                    Line lineElbowLeftToWristLeft = new Line();
                    lineElbowLeftToWristLeft.Stroke = Brushes.LightSteelBlue;
                    lineElbowLeftToWristLeft.X1 = elbowLeftCar.x;
                    lineElbowLeftToWristLeft.Y1 = elbowLeftCar.y;
                    lineElbowLeftToWristLeft.X2 = wristLeftCar.x;
                    lineElbowLeftToWristLeft.Y2 = wristLeftCar.y;
                    lineElbowLeftToWristLeft.StrokeThickness = 2;

                    //Neck to RightShoulder
                    Line lineNeckToRightShoulder = new Line();
                    lineNeckToRightShoulder.Stroke = Brushes.LightSteelBlue;
                    lineNeckToRightShoulder.X1 = shoulderRightCar.x;
                    lineNeckToRightShoulder.Y1 = shoulderRightCar.y;
                    lineNeckToRightShoulder.X2 = neckCar.x;
                    lineNeckToRightShoulder.Y2 = neckCar.y;
                    lineNeckToRightShoulder.StrokeThickness = 2;

                    //RightShoulder to RightElbow
                    Line lineRightShoulderToElbowRight = new Line();
                    lineRightShoulderToElbowRight.Stroke = Brushes.LightSteelBlue;
                    lineRightShoulderToElbowRight.X1 = shoulderRightCar.x;
                    lineRightShoulderToElbowRight.Y1 = shoulderRightCar.y;
                    lineRightShoulderToElbowRight.X2 = elbowRightCar.x;
                    lineRightShoulderToElbowRight.Y2 = elbowRightCar.y;
                    lineRightShoulderToElbowRight.StrokeThickness = 2;

                    //RightElbow to RightWrist
                    Line lineElbowRightToWristRight = new Line();
                    lineElbowRightToWristRight.Stroke = Brushes.LightSteelBlue;
                    lineElbowRightToWristRight.X1 = elbowRightCar.x;
                    lineElbowRightToWristRight.Y1 = elbowRightCar.y;
                    lineElbowRightToWristRight.X2 = wristRightCar.x;
                    lineElbowRightToWristRight.Y2 = wristRightCar.y;
                    lineElbowRightToWristRight.StrokeThickness = 2;

                    //Neck to SpineMid
                    Line lineNeckToSpineMid = new Line();
                    lineNeckToSpineMid.Stroke = Brushes.LightSteelBlue;
                    lineNeckToSpineMid.X1 = neckCar.x;
                    lineNeckToSpineMid.Y1 = neckCar.y;
                    lineNeckToSpineMid.X2 = spineMidCar.x;
                    lineNeckToSpineMid.Y2 = spineMidCar.y;
                    lineNeckToSpineMid.StrokeThickness = 2;

                    //SpineMid to SpineBase
                    Line lineSpineMidToSpineBase = new Line();
                    lineSpineMidToSpineBase.Stroke = Brushes.LightSteelBlue;
                    lineSpineMidToSpineBase.X1 = spineMidCar.x;
                    lineSpineMidToSpineBase.Y1 = spineMidCar.y;
                    lineSpineMidToSpineBase.X2 = spineBaseCar.x;
                    lineSpineMidToSpineBase.Y2 = spineBaseCar.y;
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

                    handler.Close();
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public MainWindow()
        {
            //Alloc Console Window
            AllocConsole();
            DrawSkeleeton.InitializeComponent();
            Calibration();
        }
        
    }
}



