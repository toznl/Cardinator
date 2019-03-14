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

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;


using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Windows.Threading;


namespace Coordinator
{
    public partial class MainWindow : Window
    {
        /*
        Region for all Functions and Variables
        */
        public string faceData = "0";
        public string faceIP = "192.168.0.166";
        public string clientIP;

        #region ScailingVariables
        private float alpha_scalePC1;
        private float beta_scalePC1;
        private float gamma_scalePC1;
        private float alpha_scalePC2;
        private float beta_scalePC2;
        private float gamma_scalePC2;


        private float[] scailing_vectorPC1 = new float[3];
        private float[] scailing_vectorPC2 = new float[3];
        public float[] scailing_vector_temp = new float[3];

        //Real Parameters of EveR Skeleton (cm)
        private static float wrist_elbow = 26;
        private static float elbow_shoulder = 32;
        private static float shoulder = 36;
        private static float head_neck = 15;
        private static float neck_spine = 44;

        #endregion
        #region KalmanVariables

        private SyntheticData headSynPC1 = new SyntheticData();
        private Kalman kalmanheadPC1;
        private Emgu.CV.Matrix<float> headPC1Mat = new Matrix<float>(6, 1);

        private SyntheticData neckSynPC1 = new SyntheticData();
        private Kalman kalmanneckPC1;
        private Emgu.CV.Matrix<float> neckPC1Mat = new Matrix<float>(6, 1);

        private SyntheticData spineMidSynPC1 = new SyntheticData();
        private Kalman kalmanspineMidPC1;
        private Emgu.CV.Matrix<float> spineMidPC1Mat = new Matrix<float>(6, 1);

        private SyntheticData spineBaseSynPC1 = new SyntheticData();
        private Kalman kalmanspineBasePC1;
        private Emgu.CV.Matrix<float> spineBasePC1Mat = new Matrix<float>(6, 1);

        private SyntheticData shoulderLeftSynPC1 = new SyntheticData();
        private Kalman kalmanshoulderLeftPC1;
        private Emgu.CV.Matrix<float> shoulderLeftPC1Mat = new Matrix<float>(6, 1);

        private SyntheticData elbowLeftSynPC1 = new SyntheticData();
        private Kalman kalmanelbowLeftPC1;
        private Emgu.CV.Matrix<float> elbowLeftPC1Mat = new Matrix<float>(6, 1);

        private SyntheticData wristLeftSynPC1 = new SyntheticData();
        private Kalman kalmanwristLeftPC1;
        private Emgu.CV.Matrix<float> wristLeftPC1Mat = new Matrix<float>(6, 1);

        private SyntheticData shoulderRightSynPC1 = new SyntheticData();
        private Kalman kalmanshoulderRightPC1;
        private Emgu.CV.Matrix<float> shoulderRightPC1Mat = new Matrix<float>(6, 1);

        private SyntheticData elbowRightSynPC1 = new SyntheticData();
        private Kalman kalmanelbowRightPC1;
        private Emgu.CV.Matrix<float> elbowRightPC1Mat = new Matrix<float>(6, 1);

        private SyntheticData wristRightSynPC1 = new SyntheticData();
        private Kalman kalmanwristRightPC1;
        private Emgu.CV.Matrix<float> wristRightPC1Mat = new Matrix<float>(6, 1);

        private SyntheticData headSynPC2 = new SyntheticData();
        private Kalman kalmanheadPC2;
        private Emgu.CV.Matrix<float> headPC2Mat = new Matrix<float>(6, 1);

        private SyntheticData neckSynPC2 = new SyntheticData();
        private Kalman kalmanneckPC2;
        private Emgu.CV.Matrix<float> neckPC2Mat = new Matrix<float>(6, 1);

        private SyntheticData spineMidSynPC2 = new SyntheticData();
        private Kalman kalmanspineMidPC2;
        private Emgu.CV.Matrix<float> spineMidPC2Mat = new Matrix<float>(6, 1);

        private SyntheticData spineBaseSynPC2 = new SyntheticData();
        private Kalman kalmanspineBasePC2;
        private Emgu.CV.Matrix<float> spineBasePC2Mat = new Matrix<float>(6, 1);

        private SyntheticData shoulderLeftSynPC2 = new SyntheticData();
        private Kalman kalmanshoulderLeftPC2;
        private Emgu.CV.Matrix<float> shoulderLeftPC2Mat = new Matrix<float>(6, 1);

        private SyntheticData elbowLeftSynPC2 = new SyntheticData();
        private Kalman kalmanelbowLeftPC2;
        private Emgu.CV.Matrix<float> elbowLeftPC2Mat = new Matrix<float>(6, 1);

        private SyntheticData wristLeftSynPC2 = new SyntheticData();
        private Kalman kalmanwristLeftPC2;
        private Emgu.CV.Matrix<float> wristLeftPC2Mat = new Matrix<float>(6, 1);

        private SyntheticData shoulderRightSynPC2 = new SyntheticData();
        private Kalman kalmanshoulderRightPC2;
        private Emgu.CV.Matrix<float> shoulderRightPC2Mat = new Matrix<float>(6, 1);

        private SyntheticData elbowRightSynPC2 = new SyntheticData();
        private Kalman kalmanelbowRightPC2;
        private Emgu.CV.Matrix<float> elbowRightPC2Mat = new Matrix<float>(6, 1);

        private SyntheticData wristRightSynPC2 = new SyntheticData();
        private Kalman kalmanwristRightPC2;
        private Emgu.CV.Matrix<float> wristRightPC2Mat = new Matrix<float>(6, 1);

        private float dtheadPC1;
        private float dtneckPC1;
        private float dtspineBasePC1;
        private float dtspineMidPC1;
        private float dtshoulderLeftPC1;
        private float dtelbowLeftPC1;
        private float dtwristLeftPC1;
        private float dtshoulderRightPC1;
        private float dtelbowRightPC1;
        private float dtwristRightPC1;

        private float dtheadPC2;
        private float dtneckPC2;
        private float dtspineBasePC2;
        private float dtspineMidPC2;
        private float dtshoulderLeftPC2;
        private float dtelbowLeftPC2;
        private float dtwristLeftPC2;
        private float dtshoulderRightPC2;
        private float dtelbowRightPC2;
        private float dtwristRightPC2;

        private float formalXheadPC1;
        private float formalYheadPC1;
        private float formalZheadPC1;

        private float formalXneckPC1;
        private float formalYneckPC1;
        private float formalZneckPC1;

        private float formalXspineBasePC1;
        private float formalYspineBasePC1;
        private float formalZspineBasePC1;

        private float formalXspineMidPC1;
        private float formalYspineMidPC1;
        private float formalZspineMidPC1;

        private float formalXshoulderLeftPC1;
        private float formalYshoulderLeftPC1;
        private float formalZshoulderLeftPC1;

        private float formalXelbowLeftPC1;
        private float formalYelbowLeftPC1;
        private float formalZelbowLeftPC1;

        private float formalXwristLeftPC1;
        private float formalYwristLeftPC1;
        private float formalZwristLeftPC1;

        private float formalXshoulderRightPC1;
        private float formalYshoulderRightPC1;
        private float formalZshoulderRightPC1;

        private float formalXelbowRightPC1;
        private float formalYelbowRightPC1;
        private float formalZelbowRightPC1;

        private float formalXwristRightPC1;
        private float formalYwristRightPC1;
        private float formalZwristRightPC1;

        private float formalXheadPC2;
        private float formalYheadPC2;
        private float formalZheadPC2;

        private float formalXneckPC2;
        private float formalYneckPC2;
        private float formalZneckPC2;

        private float formalXspineBasePC2;
        private float formalYspineBasePC2;
        private float formalZspineBasePC2;

        private float formalXspineMidPC2;
        private float formalYspineMidPC2;
        private float formalZspineMidPC2;

        private float formalXshoulderLeftPC2;
        private float formalYshoulderLeftPC2;
        private float formalZshoulderLeftPC2;

        private float formalXelbowLeftPC2;
        private float formalYelbowLeftPC2;
        private float formalZelbowLeftPC2;

        private float formalXwristLeftPC2;
        private float formalYwristLeftPC2;
        private float formalZwristLeftPC2;

        private float formalXshoulderRightPC2;
        private float formalYshoulderRightPC2;
        private float formalZshoulderRightPC2;

        private float formalXelbowRightPC2;
        private float formalYelbowRightPC2;
        private float formalZelbowRightPC2;

        private float formalXwristRightPC2;
        private float formalYwristRightPC2;
        private float formalZwristRightPC2;

        #endregion
        #region CoordVariables
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

        public static ErrorVar calVar;

        #endregion
        #region ConsoleWindows
        //For using console windows
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        #endregion
        #region Serialize
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

        #endregion
        #region FunctionsForCalibration

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

            preProcess.x = (Convert.ToSingle(errorVar.transX)) + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)));

            preProcess.y = (Convert.ToSingle(errorVar.transY)) + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX)));

            preProcess.z = (Convert.ToSingle(errorVar.transZ)) + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY)));

            return preProcess;

        }

        public float errorFunc(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
        {
            float result = square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))));
            ;


            return result;

        }

        //Partial Differential // ErrorFunciton
        public float errorFunc0(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar, double eta)
        {
            float result;

            result = ((square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin((errorVar.thetaX + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos((errorVar.thetaX + eta)))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin((errorVar.thetaX + eta)))) + (Convert.ToSingle(Math.Cos((errorVar.thetaX + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos((errorVar.thetaX + eta)))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos((errorVar.thetaX + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin((errorVar.thetaX + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos((errorVar.thetaX + eta)))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin((errorVar.thetaX + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin((errorVar.thetaX + eta)))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))
            - (square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))) / Convert.ToSingle(eta);



            return result;
        }

        public float errorFunc1(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar, double eta)
        {
            float result;
            result =
                ((square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin((errorVar.thetaY + eta))))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin((errorVar.thetaY + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos((errorVar.thetaY + eta)))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin((errorVar.thetaY + eta))))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin((errorVar.thetaY + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos((errorVar.thetaY + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin((errorVar.thetaY + eta)))) + x * (Convert.ToSingle(Math.Cos((errorVar.thetaY + eta)))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos((errorVar.thetaY + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))
                - (square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))) / Convert.ToSingle(eta);

            return result;
        }
        public float errorFunc2(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar, double eta)
        {
            float result;
            result =
               ((square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin((errorVar.thetaZ + eta)))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos((errorVar.thetaZ + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos((errorVar.thetaZ + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin((errorVar.thetaZ + eta))))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin((errorVar.thetaZ + eta)))) + (Convert.ToSingle(Math.Cos((errorVar.thetaZ + eta)))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos((errorVar.thetaZ + eta)))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin((errorVar.thetaZ + eta))))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos((errorVar.thetaZ + eta)))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin((errorVar.thetaZ + eta)))))))
                - (square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))) / Convert.ToSingle(eta);

            return result;
        }



        public float errorFunc3(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar, double eta)
        {
            float result;
            result =
                ((square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle((errorVar.transX + eta))) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))
              - (square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))) / Convert.ToSingle(eta);

            return result;
        }
        public float errorFunc4(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar, double eta)
        {
            float result;
            result =
                ((square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle((errorVar.transY + eta))) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))

              - (square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))) / Convert.ToSingle(eta);
            return result;
        }
        public float errorFunc5(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar, double eta)
        {
            float result;
            result =
                ((square(((Convert.ToSingle((errorVar.transZ + eta))) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))
              - (square(((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))))) + square(((Convert.ToSingle(errorVar.transY)) - y1 + x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))))) + square(((Convert.ToSingle(errorVar.transX)) - x1 + z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))))))) / Convert.ToSingle(eta);

            return result;
        }

        //GradientDecsent Diffrential 
        public ErrorVar gradientDecsent(
            float x0, float y0, float z0, float x1, float y1, float z1,
            float x2, float y2, float z2, float x3, float y3, float z3,
            float x4, float y4, float z4, float x5, float y5, float z5, ErrorVar errorVar)
        {

            double eta = 0.005;
            float etaFlo = 0.005f;

            ErrorVar resultVar;

            float grad0 = errorFunc0(x0, y0, z0, x1, y1, z1, errorVar, eta) + errorFunc0(x2, y2, z2, x3, y3, z3, errorVar, eta) + errorFunc0(x4, y4, z4, x5, y5, z5, errorVar, eta);
            float grad1 = errorFunc1(x0, y0, z0, x1, y1, z1, errorVar, eta) + errorFunc1(x2, y2, z2, x3, y3, z3, errorVar, eta) + errorFunc1(x4, y4, z4, x5, y5, z5, errorVar, eta);
            float grad2 = errorFunc2(x0, y0, z0, x1, y1, z1, errorVar, eta) + errorFunc2(x2, y2, z2, x3, y3, z3, errorVar, eta) + errorFunc2(x4, y4, z4, x5, y5, z5, errorVar, eta);
            float grad3 = errorFunc3(x0, y0, z0, x1, y1, z1, errorVar, eta) + errorFunc3(x2, y2, z2, x3, y3, z3, errorVar, eta) + errorFunc3(x4, y4, z4, x5, y5, z5, errorVar, eta);
            float grad4 = errorFunc4(x0, y0, z0, x1, y1, z1, errorVar, eta) + errorFunc4(x2, y2, z2, x3, y3, z3, errorVar, eta) + errorFunc4(x4, y4, z4, x5, y5, z5, errorVar, eta);
            float grad5 = errorFunc5(x0, y0, z0, x1, y1, z1, errorVar, eta) + errorFunc5(x2, y2, z2, x3, y3, z3, errorVar, eta) + errorFunc5(x4, y4, z4, x5, y5, z5, errorVar, eta);

            // ROTATION VAR
            //errorVar.thetaX = errorVar.thetaX - eta * grad0;
            //errorVar.thetaY = errorVar.thetaY - eta * grad1;
            //errorVar.thetaZ = errorVar.thetaZ - eta * grad2;

            float den = 2.0f * Convert.ToSingle(Math.PI);
            errorVar.thetaX = (errorVar.thetaX % den) - etaFlo * (grad0 % den);
            errorVar.thetaY = (errorVar.thetaY % den) - etaFlo * (grad1 % den);
            errorVar.thetaZ = (errorVar.thetaZ % den) - etaFlo * (grad2 % den);

            // TRANS VAR
            errorVar.transX = errorVar.transX - etaFlo * grad3;
            errorVar.transY = errorVar.transY - etaFlo * grad4;
            errorVar.transZ = errorVar.transZ - etaFlo * grad5;


            resultVar = errorVar;

            return resultVar;

        }
        #endregion

        #region Variables for Angular Transform
        private double angle1;   //Waist Yaw       (허리 회전)
        private double angle2;   //Neck Roll       (고개 좌우)
        private double angle3;   //Neck Pitch      (고개 앞뒤)
        private double angle4;   //Neck Yaw        (고개 회전)
        private double angle5;   //LeftArm1        (왼팔 어깨 위아래)
        private double angle6;   //LeftArm2        (왼팔 어깨 펼침)
        private double angle7;   //LeftArm3        (왼팔 어깨 회전)
        private double angle8;   //LeftArm4        (왼팔 팔꿈치 위아래)
        private double angle9;   //LeftArm5        (왼팔 팔꿈치 회전)
        private double angle10;  //LeftArm6        (왼팔 손목 꺾임)
        private double angle11;  //RightArm1       (오른팔 어깨 위아래)
        private double angle12;  //RightArm2       (오른팔 어깨 펼침)
        private double angle13;  //RightArm3       (오른팔 어깨 회전)
        private double angle14;  //RightArm4       (오른팔 팔꿈치 위아래)
        private double angle15;  //RightArm5       (오른팔 팔꿈치 회전)
        private double angle16;  //RightArm6       (오른팔 손목 꺾임)

        private CoOrdXYZ defineVectorAngleArm;
        private CoOrdXYZ defineVectorAngleLeftArmRotate;
        private CoOrdXYZ defineVectorAngleRightArmRotate;




        #endregion
        #region FunctionsForAngularTransform
        public CoOrdXYZ Vectorize(CoOrd co1, CoOrd co2)
        {
            CoOrdXYZ result;
            result.x = co1.x - co2.x;
            result.y = co1.y - co2.y;
            result.z = co1.z - co2.z;

            return result;
        }

        public float DotProduct(CoOrdXYZ vector1, CoOrdXYZ vector2)
        {
            float result;

            result = (vector1.x * vector2.x) + (vector1.y * vector2.y) + (vector1.z * vector2.z);

            return result;
        }

        public float VectorSize(CoOrdXYZ vector1)
        {
            float result;


            result = Convert.ToSingle(Math.Sqrt(Convert.ToDouble(square(vector1.x) + square(vector1.y) + square(vector1.z))));

            return result;
        }

        public double AngularTransform(CoOrd co1, CoOrd co2, CoOrd co3)
        {
            double result;

            CoOrdXYZ vector1, vector2;

            vector1 = Vectorize(co1, co2);
            vector2 = Vectorize(co3, co2);

            result = Math.Acos(((vector1.x * vector2.x) + (vector1.y * vector2.y) + (vector1.z * vector2.z)) / (VectorSize(vector1) * VectorSize(vector2)));

            result = result * 180 / Math.PI;
            return result;
        }



        public double AngularTransform2(CoOrdXYZ vector1, CoOrdXYZ vector2)
        {
            double result;

            result = Math.Acos(((vector1.x * vector2.x) + (vector1.y * vector2.y) + (vector1.z * vector2.z)) / (VectorSize(vector1) * VectorSize(vector2)));

            result = result * 180 / Math.PI;

            return result;
        }

        public double AngularTransform3(CoOrd co1, CoOrd co2, CoOrd co3)
        {
            double result;

            CoOrdXYZ vector1, vector2;

            vector1 = Vectorize(co2, co1);
            vector2 = Vectorize(co3, co2);

            result = Math.Acos(((vector1.x * vector2.x) + (vector1.y * vector2.y) + (vector1.z * vector2.z)) / (VectorSize(vector1) * VectorSize(vector2)));

            result = result * 180 / Math.PI;
            return result;
        }

        public CoOrdXYZ VectorFlat(CoOrd co1, CoOrd co2, CoOrd co3)
        {
            CoOrdXYZ result;

            result.x = (co1.y * (co2.z - co3.z)) + (co2.y * (co3.z - co1.z)) + (co3.y * (co1.z - co2.z));
            result.y = (co1.z * (co2.x - co3.x)) + (co2.z * (co3.x - co1.x)) + (co3.z * (co1.x - co2.x));
            result.z = (co1.x * (co2.y - co3.y)) + (co2.x * (co3.y - co1.y)) + (co3.x * (co1.y - co2.y));

            float sizeOfh = VectorSize(result);

            result.x = result.x / sizeOfh;
            result.y = result.y / sizeOfh;
            result.z = result.z / sizeOfh;

            return result;
        }

        public float ProjD(CoOrd co1, CoOrd co2, CoOrd co3)
        {
            float result;

            CoOrdXYZ vectorFlat = VectorFlat(co1, co2, co3);
            CoOrdXYZ vectorFlatNegative;
            vectorFlatNegative.x = -vectorFlat.x;
            vectorFlatNegative.y = -vectorFlat.y;
            vectorFlatNegative.z = -vectorFlat.z;

            CoOrdXYZ vectorOrd;
            vectorOrd.x = 0;
            vectorOrd.y = 0;
            vectorOrd.z = 0;

            result = DotProduct(vectorFlat, vectorOrd);

            return result;

        }

        public CoOrdXYZ VectorProj(CoOrd co1, CoOrd co2, CoOrd co3, CoOrd co4, CoOrd co5)
        {
            CoOrdXYZ result;

            CoOrdXYZ vectorFlat = VectorFlat(co1, co2, co3);
            float planeD = ProjD(co1, co2, co3);

            float distanceCo4 = Math.Abs(((vectorFlat.x * co4.x) + (vectorFlat.y * co4.y) + (vectorFlat.z * co4.z) + planeD)) / VectorSize(vectorFlat);

            CoOrdXYZ prjCo4;
            prjCo4.x = co4.x - distanceCo4 * vectorFlat.x;
            prjCo4.y = co4.y - distanceCo4 * vectorFlat.y;
            prjCo4.z = co4.z - distanceCo4 * vectorFlat.z;


            float distanceCo5 = Math.Abs(((vectorFlat.x * co5.x) + (vectorFlat.y * co5.y) + (vectorFlat.z * co5.z) + planeD)) / VectorSize(vectorFlat);

            CoOrdXYZ prjCo5;
            prjCo5.x = co5.x - distanceCo5 * vectorFlat.x;
            prjCo5.y = co5.y - distanceCo5 * vectorFlat.y;
            prjCo5.z = co5.z - distanceCo5 * vectorFlat.z;

            result.x = prjCo5.x - prjCo4.x;
            result.y = prjCo5.y - prjCo4.y;
            result.z = prjCo5.z - prjCo4.z;

            //double rVar = Math.Sqrt(square(co5.x - co4.x) + square(co5.y - co4.y) + square(co5.z - co4.z));

            return result;
        }

        public double VectorProjAngle(CoOrd co1, CoOrd co2, CoOrd co3, CoOrd co4, CoOrd co5, CoOrdXYZ defineVector)
        {
            double result;

            CoOrdXYZ prjVector = VectorProj(co1, co2, co3, co4, co5);

            result = AngularTransform2(prjVector, defineVector);

            return result;
        }

        //public double VectorProjAngleRotation()


        #endregion
        #region Scailing Functions
        public float distance(CoOrd dot1, CoOrd dot2)
        {
            float result;

            result = Convert.ToSingle(Math.Sqrt(Math.Pow(Convert.ToDouble(dot1.x - dot2.x), 2) + Math.Pow(Convert.ToDouble(dot1.y - dot2.y), 2) + Math.Pow(Convert.ToDouble(dot1.z - dot2.z), 2)));

            //Console.WriteLine(result);

            return result;
        }

        public CoOrd scale_skeleton(CoOrd co1, float[] scaleVector)
        {
            CoOrd result;

            result.markerPC = co1.markerPC;
            result.markerTrackingState = co1.markerTrackingState;
            result.markerType = co1.markerType;
            result.x = co1.x * scaleVector[0];
            result.y = co1.y * scaleVector[1];
            result.z = co1.z * scaleVector[2];

            return result;
        }

        public float[] scale_vector_update(CoOrd head, CoOrd neck, CoOrd shoulder_left, CoOrd elbow_left, CoOrd wrist_left, CoOrd shoulder_right, CoOrd elbow_right, CoOrd wrist_right, CoOrd spine_mid, CoOrd spine_base, float[] scale_Vector)
        {
            float[] result = new float[3];
            result[0] = 1;
            result[1] = 1;
            result[2] = 1;
            result = scale_Vector;

            float error0 = 0;
            float error1 = 0;
            float error2 = 0;
            float error3 = 0;
            float error4 = 0;

            float error_Sum = 1;

            float errorAlpha = 0.1f;
            float errorBeta = 0.1f;
            float errorGamma = 0.1f;


            float eta = 0.1f;
            float error_Sum_Temp;



            while (error_Sum > 0.1)
            {
                error0 = wrist_elbow - distance((scale_skeleton(wrist_left, result)), (scale_skeleton(elbow_left, result)));
                error1 = elbow_shoulder - distance(scale_skeleton(elbow_left, result), scale_skeleton(shoulder_left, result));
                error2 = shoulder - distance(scale_skeleton(shoulder_right, result), scale_skeleton(shoulder_left, result));
                error3 = head_neck - distance(scale_skeleton(head, result), scale_skeleton(neck, result));
                error4 = neck_spine - distance(scale_skeleton(neck, result), scale_skeleton(spine_mid, result));

                error_Sum = error0 + error1 + error2 + error3 + error4; // Error_Sum
                error_Sum_Temp = error_Sum;
                Console.WriteLine(scale_skeleton(wrist_left, scale_Vector).x);
                //Console.WriteLine(error0);


                result[0] = result[0] - eta * errorAlpha;
                result[1] = result[1] - eta * errorBeta;
                result[2] = result[2] - eta * errorGamma;

                error0 = Math.Abs(wrist_elbow - distance(scale_skeleton(wrist_left, result), scale_skeleton(elbow_left, result)));
                error1 = Math.Abs(elbow_shoulder - distance(scale_skeleton(elbow_left, result), scale_skeleton(shoulder_left, result)));
                error2 = Math.Abs(shoulder - distance(scale_skeleton(shoulder_right, result), scale_skeleton(shoulder_left, result)));
                error3 = Math.Abs(head_neck - distance(scale_skeleton(head, result), scale_skeleton(neck, result)));
                error4 = Math.Abs(neck_spine - distance(scale_skeleton(neck, result), scale_skeleton(spine_mid, result)));

                error_Sum = error0 + error1 + error2 + error3 + error4;


            }

            return result;
        }

        public float[] ROIx(CoOrd leftshoulder, CoOrd rightshoulder, CoOrd leftwrist, CoOrd rightwrist, CoOrd leftelbow, CoOrd rightelbow)
        {
            float[] roiRect = new float[2];

            float[] leftx = { leftshoulder.x, leftelbow.x, leftwrist.x };
            float[] rightx = { rightshoulder.x, rightelbow.x, rightwrist.x };

            float minx = 0;
            float maxx = 0;

            for (int i = 0; i < leftx.Length; i++)
            {
                if (minx > leftx[i])
                {
                    minx = leftx[i];
                }
            }

            for (int i = 0; i < rightx.Length; i++)
            {
                if (maxx < leftx[i])
                {
                    maxx = leftx[i];
                }
            }

            roiRect[0] = minx;
            roiRect[1] = maxx;

            return roiRect;
        }

        public float[] ROIy(CoOrd head, CoOrd leftwrist, CoOrd rightwrist, CoOrd spinebase)
        {
            float[] roiRect = new float[2];

            float[] topy = { head.y, leftwrist.y, rightwrist.y };
            float[] bottomy = { spinebase.y, leftwrist.y, rightwrist.y };

            float miny = 0;
            float maxy = 0;

            for (int i = 0; i < topy.Length; i++)
            {
                if (miny > topy[i])
                {
                    miny = topy[i];
                }
            }

            for (int i = 0; i < bottomy.Length; i++)
            {
                if (maxy < bottomy[i])
                {
                    maxy = bottomy[i];
                }
            }

            roiRect[0] = miny;
            roiRect[1] = maxy;

            return roiRect;
        }

        public float[] ROIz(CoOrd head, CoOrd spinebase, CoOrd spinemid, CoOrd leftelbow, CoOrd rightelbow, CoOrd leftwrist, CoOrd rightwrist, CoOrd shoulderleft, CoOrd shoulderright)
        {
            float[] roiRect = new float[2];

            float[] frontz = { head.z, spinebase.z, spinemid.z, leftelbow.z, rightelbow.z, leftwrist.z, rightwrist.z, shoulderleft.z, shoulderright.z };

            float minz = 0;
            float maxz = 0;

            for (int i = 0; i < frontz.Length; i++)
            {
                if (minz > frontz[i])
                {
                    minz = frontz[i];
                }
            }

            for (int i = 0; i < frontz.Length; i++)
            {
                if (maxz < frontz[i])
                {
                    maxz = frontz[i];
                }
            }

            roiRect[0] = minz;
            roiRect[1] = maxz;

            return roiRect;
        }

        public CoOrd setScreenMid(float x, float y, CoOrd co)
        {
            CoOrd result;

            result.markerPC = co.markerPC;
            result.markerTrackingState = co.markerTrackingState;
            result.markerType = co.markerType;

            result.x = co.x - x;
            result.y = co.y - y;
            result.z = co.z;

            return result;
        }
        #endregion
        #region Angular Transferring
        void SendBuffer(string str)
        {
            TcpClient tc = new TcpClient("192.168.0.100", 5001);                   // 에버 제어 PC 아이피 확인 후 변경
            NetworkStream stream = tc.GetStream();

            byte[] check_sum = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int packet_type = 0x02;
            int tr_no = 0;
            int data_type = 5000;
            string[] stringArray = new string[1492];

            string data = str;
            byte[] buff = Encoding.ASCII.GetBytes(data);
            int data_len = data.Length;

            check_sum[0] = 0x55;
            check_sum[1] = (byte)(packet_type & 0xff);
            check_sum[2] = (byte)(tr_no & 0xff);
            check_sum[3] = (byte)((tr_no >> 8) & 0xff);
            check_sum[4] = (byte)(data_len & 0xff);
            check_sum[5] = (byte)((data_len >> 8) & 0xff);
            check_sum[6] = (byte)(data_type & 0xff);
            check_sum[7] = (byte)((data_type >> 8) & 0xff);

            int sum = check_sum[0] + check_sum[1] + check_sum[2] + check_sum[3] + check_sum[4] + check_sum[5] +
                      check_sum[6] + check_sum[7];
            int data_sum = 0;

            for (int i = 0; i < data_len; i++)
            {
                data_sum += buff[i];
            }

            check_sum[8] = (byte)(sum & 0xff);
            check_sum[9] = (byte)(data_sum & 0xff);

            byte[] msg = new byte[check_sum.Length + buff.Length];
            Buffer.BlockCopy(check_sum, 0, msg, 0, check_sum.Length);
            Buffer.BlockCopy(buff, 0, msg, check_sum.Length, buff.Length);


            stream.Write(msg, 0, msg.Length);

            stream.Close();
            tc.Close();

        }


        #endregion


        /*
        Region for all Functions and Variables
        */

        //MainWindow
        public MainWindow()
        {
            //Alloc Console Window
            AllocConsole();
            InitializeComponent();
            for (int i = 0; i < 5; i++)
            {
                int abc = i;
                kinectv1connected.Text = "ArmData = {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7} ";
            }
            
            //Calibration Thread
            new Thread(new ThreadStart(Calibration)).Start();
            new Thread(new ThreadStart(Sending)).Start();
            //new Thread(new ThreadStart(RecvFace)).Start();
        }

        //Receive Face Values <Thread>
        public void RecvFace()
        {
            #region Socket Listner
            //Fundamental Variables for Socket
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 5050);
            Socket sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sListener.Bind(ipEndPoint);
            sListener.Listen(1024);
            Socket handler = sListener.Accept();

            while (true)
            {
                int x = 0;


                byte[] buffer = new byte[50];
                x = handler.Receive(buffer);
                clientIP = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();
                try
                {

                    Console.WriteLine(clientIP);
                    faceData = Encoding.Default.GetString(buffer);
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());

                }
                #endregion

            }

        }

        //Calibration
        public void Calibration()
        {

            #region Variables for Calibration
            //Initialize 6 variables for Calibration
            calVar.thetaX = -0.3;
            calVar.thetaY = -0.3;
            calVar.thetaZ = -0.3;
            calVar.transX = -143;
            calVar.transY = 33;
            calVar.transZ = 29;

            alpha_scalePC1 = 1;
            beta_scalePC1 = 1;
            gamma_scalePC1 = 1;
            alpha_scalePC2 = 1;
            beta_scalePC2 = 1;
            gamma_scalePC2 = 1;


            scailing_vectorPC1[0] = alpha_scalePC1;
            scailing_vectorPC1[1] = beta_scalePC1;
            scailing_vectorPC1[2] = gamma_scalePC1;
            scailing_vectorPC2[0] = alpha_scalePC2;
            scailing_vectorPC2[1] = beta_scalePC2;
            scailing_vectorPC2[2] = gamma_scalePC2;

            #endregion

            #region Socket Listner
            //Fundamental Variables for Socket
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 5000);
            Socket sListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sListener.Bind(ipEndPoint);
            sListener.Listen(1024);
            CoOrd buf = new CoOrd();

            #endregion

            while (true)
            {

                #region Socket Listner & Buffer
                //Variables for Socket and Transport Coordination

                int x = 0;

                Socket handler = sListener.Accept();
                byte[] buffer = new byte[10000];
                x = handler.Receive(buffer);
                clientIP = ((IPEndPoint)handler.RemoteEndPoint).Address.ToString();

                #endregion
                BinaryFormatter forDeserialize = new BinaryFormatter();
                forDeserialize.Binder = new AllowAssemblyDeserializationBinder();
                buf = Deserialize<CoOrd>(buffer);

                try
                {
                    #region Get Joints
                    //Get PC1 Joints
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
                    //Get PC2 Joints
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

                    #endregion
                    #region Kalman
                    //Kalman headPC1
                    headPC1Mat[0, 0] = headPC1.x;
                    headPC1Mat[1, 0] = headPC1.y;
                    headPC1Mat[2, 0] = headPC1.z;

                    kalmanheadPC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> headPC1state = headPC1Mat;
                    kalmanheadPC1.CorrectedState = headPC1state;
                    kalmanheadPC1.TransitionMatrix = headSynPC1.transitionMatrix;
                    kalmanheadPC1.MeasurementNoiseCovariance = headSynPC1.measurementNoise;
                    kalmanheadPC1.ProcessNoiseCovariance = headSynPC1.processNoise;
                    kalmanheadPC1.ErrorCovariancePost = headSynPC1.errorCovariancePost;
                    kalmanheadPC1.MeasurementMatrix = headSynPC1.measurementMatrix;

                    Matrix<float> headPC1prediction = new Matrix<float>(3, 1);
                    headPC1prediction = kalmanheadPC1.Predict();
                    MCvPoint3D32f predictPointheadPC1 = new MCvPoint3D32f(headPC1prediction[0, 0], headPC1prediction[1, 0], headPC1prediction[2, 0]);
                    MCvPoint3D32f measurePointheadPC1 = new MCvPoint3D32f(headSynPC1.GetMeasurement()[0, 0],
                        headSynPC1.GetMeasurement()[1, 0], headSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedheadPC1 = kalmanheadPC1.Correct(headSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedheadPC1Point = new MCvPoint3D32f(estimatedheadPC1[0, 0], estimatedheadPC1[1, 0], estimatedheadPC1[2, 0]);
                    headSynPC1.GoToNextState();

                    headPC1.x = estimatedheadPC1Point.x;
                    headPC1.y = estimatedheadPC1Point.y;
                    headPC1.z = estimatedheadPC1Point.z;

                    //Kalman headPC2
                    headPC2Mat[0, 0] = headPC2.x;
                    headPC2Mat[1, 0] = headPC2.y;
                    headPC2Mat[2, 0] = headPC2.z;


                    kalmanheadPC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> headPC2state = headPC2Mat;
                    kalmanheadPC2.CorrectedState = headPC2state;
                    kalmanheadPC2.TransitionMatrix = headSynPC2.transitionMatrix;
                    kalmanheadPC2.MeasurementNoiseCovariance = headSynPC2.measurementNoise;
                    kalmanheadPC2.ProcessNoiseCovariance = headSynPC2.processNoise;
                    kalmanheadPC2.ErrorCovariancePost = headSynPC2.errorCovariancePost;
                    kalmanheadPC2.MeasurementMatrix = headSynPC2.measurementMatrix;

                    Matrix<float> headPC2prediction = new Matrix<float>(3, 1);
                    headPC2prediction = kalmanheadPC2.Predict();
                    MCvPoint3D32f predictPointheadPC2 = new MCvPoint3D32f(headPC2prediction[0, 0], headPC2prediction[1, 0], headPC2prediction[2, 0]);
                    MCvPoint3D32f measurePointheadPC2 = new MCvPoint3D32f(headSynPC2.GetMeasurement()[0, 0],
                        headSynPC2.GetMeasurement()[1, 0], headSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedheadPC2 = kalmanheadPC2.Correct(headSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedheadPC2Point = new MCvPoint3D32f(estimatedheadPC2[0, 0], estimatedheadPC2[1, 0], estimatedheadPC2[2, 0]);
                    headSynPC2.GoToNextState();

                    headPC2.x = estimatedheadPC2Point.x;
                    headPC2.y = estimatedheadPC2Point.y;
                    headPC2.z = estimatedheadPC2Point.z;

                    //Kalman neckPC1
                    neckPC1Mat[0, 0] = neckPC1.x;
                    neckPC1Mat[1, 0] = neckPC1.y;
                    neckPC1Mat[2, 0] = neckPC1.z;

                    kalmanneckPC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> neckPC1state = neckPC1Mat;
                    kalmanneckPC1.CorrectedState = neckPC1state;
                    kalmanneckPC1.TransitionMatrix = neckSynPC1.transitionMatrix;
                    kalmanneckPC1.MeasurementNoiseCovariance = neckSynPC1.measurementNoise;
                    kalmanneckPC1.ProcessNoiseCovariance = neckSynPC1.processNoise;
                    kalmanneckPC1.ErrorCovariancePost = neckSynPC1.errorCovariancePost;
                    kalmanneckPC1.MeasurementMatrix = neckSynPC1.measurementMatrix;

                    Matrix<float> neckPC1prediction = new Matrix<float>(3, 1);
                    neckPC1prediction = kalmanneckPC1.Predict();
                    MCvPoint3D32f predictPointneckPC1 = new MCvPoint3D32f(neckPC1prediction[0, 0], neckPC1prediction[1, 0], neckPC1prediction[2, 0]);
                    MCvPoint3D32f measurePointneckPC1 = new MCvPoint3D32f(neckSynPC1.GetMeasurement()[0, 0],
                        neckSynPC1.GetMeasurement()[1, 0], neckSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedneckPC1 = kalmanneckPC1.Correct(neckSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedneckPC1Point = new MCvPoint3D32f(estimatedneckPC1[0, 0], estimatedneckPC1[1, 0], estimatedneckPC1[2, 0]);
                    neckSynPC1.GoToNextState();

                    neckPC1.x = estimatedneckPC1Point.x;
                    neckPC1.y = estimatedneckPC1Point.y;
                    neckPC1.z = estimatedneckPC1Point.z;

                    //Kalman neckPC2
                    neckPC2Mat[0, 0] = neckPC2.x;
                    neckPC2Mat[1, 0] = neckPC2.y;
                    neckPC2Mat[2, 0] = neckPC2.z;


                    kalmanneckPC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> neckPC2state = neckPC2Mat;
                    kalmanneckPC2.CorrectedState = neckPC2state;
                    kalmanneckPC2.TransitionMatrix = neckSynPC2.transitionMatrix;
                    kalmanneckPC2.MeasurementNoiseCovariance = neckSynPC2.measurementNoise;
                    kalmanneckPC2.ProcessNoiseCovariance = neckSynPC2.processNoise;
                    kalmanneckPC2.ErrorCovariancePost = neckSynPC2.errorCovariancePost;
                    kalmanneckPC2.MeasurementMatrix = neckSynPC2.measurementMatrix;

                    Matrix<float> neckPC2prediction = new Matrix<float>(3, 1);
                    neckPC2prediction = kalmanneckPC2.Predict();
                    MCvPoint3D32f predictPointneckPC2 = new MCvPoint3D32f(neckPC2prediction[0, 0], neckPC2prediction[1, 0], neckPC2prediction[2, 0]);
                    MCvPoint3D32f measurePointneckPC2 = new MCvPoint3D32f(neckSynPC2.GetMeasurement()[0, 0],
                        neckSynPC2.GetMeasurement()[1, 0], neckSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedneckPC2 = kalmanneckPC2.Correct(neckSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedneckPC2Point = new MCvPoint3D32f(estimatedneckPC2[0, 0], estimatedneckPC2[1, 0], estimatedneckPC2[2, 0]);
                    neckSynPC2.GoToNextState();

                    neckPC2.x = estimatedneckPC2Point.x;
                    neckPC2.y = estimatedneckPC2Point.y;
                    neckPC2.z = estimatedneckPC2Point.z;

                    //Kalman spineMidPC1
                    spineMidPC1Mat[0, 0] = spineMidPC1.x;
                    spineMidPC1Mat[1, 0] = spineMidPC1.y;
                    spineMidPC1Mat[2, 0] = spineMidPC1.z;

                    kalmanspineMidPC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> spineMidPC1state = spineMidPC1Mat;
                    kalmanspineMidPC1.CorrectedState = spineMidPC1state;
                    kalmanspineMidPC1.TransitionMatrix = spineMidSynPC1.transitionMatrix;
                    kalmanspineMidPC1.MeasurementNoiseCovariance = spineMidSynPC1.measurementNoise;
                    kalmanspineMidPC1.ProcessNoiseCovariance = spineMidSynPC1.processNoise;
                    kalmanspineMidPC1.ErrorCovariancePost = spineMidSynPC1.errorCovariancePost;
                    kalmanspineMidPC1.MeasurementMatrix = spineMidSynPC1.measurementMatrix;

                    Matrix<float> spineMidPC1prediction = new Matrix<float>(3, 1);
                    spineMidPC1prediction = kalmanspineMidPC1.Predict();
                    MCvPoint3D32f predictPointspineMidPC1 = new MCvPoint3D32f(spineMidPC1prediction[0, 0], spineMidPC1prediction[1, 0], spineMidPC1prediction[2, 0]);
                    MCvPoint3D32f measurePointspineMidPC1 = new MCvPoint3D32f(spineMidSynPC1.GetMeasurement()[0, 0],
                        spineMidSynPC1.GetMeasurement()[1, 0], spineMidSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedspineMidPC1 = kalmanspineMidPC1.Correct(spineMidSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedspineMidPC1Point = new MCvPoint3D32f(estimatedspineMidPC1[0, 0], estimatedspineMidPC1[1, 0], estimatedspineMidPC1[2, 0]);
                    spineMidSynPC1.GoToNextState();

                    spineMidPC1.x = estimatedspineMidPC1Point.x;
                    spineMidPC1.y = estimatedspineMidPC1Point.y;
                    spineMidPC1.z = estimatedspineMidPC1Point.z;

                    //Kalman spineMidPC2
                    spineMidPC2Mat[0, 0] = spineMidPC2.x;
                    spineMidPC2Mat[1, 0] = spineMidPC2.y;
                    spineMidPC2Mat[2, 0] = spineMidPC2.z;


                    kalmanspineMidPC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> spineMidPC2state = spineMidPC2Mat;
                    kalmanspineMidPC2.CorrectedState = spineMidPC2state;
                    kalmanspineMidPC2.TransitionMatrix = spineMidSynPC2.transitionMatrix;
                    kalmanspineMidPC2.MeasurementNoiseCovariance = spineMidSynPC2.measurementNoise;
                    kalmanspineMidPC2.ProcessNoiseCovariance = spineMidSynPC2.processNoise;
                    kalmanspineMidPC2.ErrorCovariancePost = spineMidSynPC2.errorCovariancePost;
                    kalmanspineMidPC2.MeasurementMatrix = spineMidSynPC2.measurementMatrix;

                    Matrix<float> spineMidPC2prediction = new Matrix<float>(3, 1);
                    spineMidPC2prediction = kalmanspineMidPC2.Predict();
                    MCvPoint3D32f predictPointspineMidPC2 = new MCvPoint3D32f(spineMidPC2prediction[0, 0], spineMidPC2prediction[1, 0], spineMidPC2prediction[2, 0]);
                    MCvPoint3D32f measurePointspineMidPC2 = new MCvPoint3D32f(spineMidSynPC2.GetMeasurement()[0, 0],
                        spineMidSynPC2.GetMeasurement()[1, 0], spineMidSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedspineMidPC2 = kalmanspineMidPC2.Correct(spineMidSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedspineMidPC2Point = new MCvPoint3D32f(estimatedspineMidPC2[0, 0], estimatedspineMidPC2[1, 0], estimatedspineMidPC2[2, 0]);
                    spineMidSynPC2.GoToNextState();

                    spineMidPC2.x = estimatedspineMidPC2Point.x;
                    spineMidPC2.y = estimatedspineMidPC2Point.y;
                    spineMidPC2.z = estimatedspineMidPC2Point.z;

                    //Kalman spineBasePC1
                    spineBasePC1Mat[0, 0] = spineBasePC1.x;
                    spineBasePC1Mat[1, 0] = spineBasePC1.y;
                    spineBasePC1Mat[2, 0] = spineBasePC1.z;

                    kalmanspineBasePC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> spineBasePC1state = spineBasePC1Mat;
                    kalmanspineBasePC1.CorrectedState = spineBasePC1state;
                    kalmanspineBasePC1.TransitionMatrix = spineBaseSynPC1.transitionMatrix;
                    kalmanspineBasePC1.MeasurementNoiseCovariance = spineBaseSynPC1.measurementNoise;
                    kalmanspineBasePC1.ProcessNoiseCovariance = spineBaseSynPC1.processNoise;
                    kalmanspineBasePC1.ErrorCovariancePost = spineBaseSynPC1.errorCovariancePost;
                    kalmanspineBasePC1.MeasurementMatrix = spineBaseSynPC1.measurementMatrix;

                    Matrix<float> spineBasePC1prediction = new Matrix<float>(3, 1);
                    spineBasePC1prediction = kalmanspineBasePC1.Predict();
                    MCvPoint3D32f predictPointspineBasePC1 = new MCvPoint3D32f(spineBasePC1prediction[0, 0], spineBasePC1prediction[1, 0], spineBasePC1prediction[2, 0]);
                    MCvPoint3D32f measurePointspineBasePC1 = new MCvPoint3D32f(spineBaseSynPC1.GetMeasurement()[0, 0],
                        spineBaseSynPC1.GetMeasurement()[1, 0], spineBaseSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedspineBasePC1 = kalmanspineBasePC1.Correct(spineBaseSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedspineBasePC1Point = new MCvPoint3D32f(estimatedspineBasePC1[0, 0], estimatedspineBasePC1[1, 0], estimatedspineBasePC1[2, 0]);
                    spineBaseSynPC1.GoToNextState();

                    spineBasePC1.x = estimatedspineBasePC1Point.x;
                    spineBasePC1.y = estimatedspineBasePC1Point.y;
                    spineBasePC1.z = estimatedspineBasePC1Point.z;

                    //Kalman spineBasePC2
                    spineBasePC2Mat[0, 0] = spineBasePC2.x;
                    spineBasePC2Mat[1, 0] = spineBasePC2.y;
                    spineBasePC2Mat[2, 0] = spineBasePC2.z;

                    kalmanspineBasePC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> spineBasePC2state = spineBasePC2Mat;
                    kalmanspineBasePC2.CorrectedState = spineBasePC2state;
                    kalmanspineBasePC2.TransitionMatrix = spineBaseSynPC2.transitionMatrix;
                    kalmanspineBasePC2.MeasurementNoiseCovariance = spineBaseSynPC2.measurementNoise;
                    kalmanspineBasePC2.ProcessNoiseCovariance = spineBaseSynPC2.processNoise;
                    kalmanspineBasePC2.ErrorCovariancePost = spineBaseSynPC2.errorCovariancePost;
                    kalmanspineBasePC2.MeasurementMatrix = spineBaseSynPC2.measurementMatrix;

                    Matrix<float> spineBasePC2prediction = new Matrix<float>(3, 1);
                    spineBasePC2prediction = kalmanspineBasePC2.Predict();
                    MCvPoint3D32f predictPointspineBasePC2 = new MCvPoint3D32f(spineBasePC2prediction[0, 0], spineBasePC2prediction[1, 0], spineBasePC2prediction[2, 0]);
                    MCvPoint3D32f measurePointspineBasePC2 = new MCvPoint3D32f(spineBaseSynPC2.GetMeasurement()[0, 0],
                        spineBaseSynPC2.GetMeasurement()[1, 0], spineBaseSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedspineBasePC2 = kalmanspineBasePC2.Correct(spineBaseSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedspineBasePC2Point = new MCvPoint3D32f(estimatedspineBasePC2[0, 0], estimatedspineBasePC2[1, 0], estimatedspineBasePC2[2, 0]);
                    spineBaseSynPC2.GoToNextState();

                    spineBasePC2.x = estimatedspineBasePC2Point.x;
                    spineBasePC2.y = estimatedspineBasePC2Point.y;
                    spineBasePC2.z = estimatedspineBasePC2Point.z;

                    //Kalman shoulderLeftPC1
                    shoulderLeftPC1Mat[0, 0] = shoulderLeftPC1.x;
                    shoulderLeftPC1Mat[1, 0] = shoulderLeftPC1.y;
                    shoulderLeftPC1Mat[2, 0] = shoulderLeftPC1.z;

                    kalmanshoulderLeftPC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> shoulderLeftPC1state = shoulderLeftPC1Mat;
                    kalmanshoulderLeftPC1.CorrectedState = shoulderLeftPC1state;
                    kalmanshoulderLeftPC1.TransitionMatrix = shoulderLeftSynPC1.transitionMatrix;
                    kalmanshoulderLeftPC1.MeasurementNoiseCovariance = shoulderLeftSynPC1.measurementNoise;
                    kalmanshoulderLeftPC1.ProcessNoiseCovariance = shoulderLeftSynPC1.processNoise;
                    kalmanshoulderLeftPC1.ErrorCovariancePost = shoulderLeftSynPC1.errorCovariancePost;
                    kalmanshoulderLeftPC1.MeasurementMatrix = shoulderLeftSynPC1.measurementMatrix;

                    Matrix<float> shoulderLeftPC1prediction = new Matrix<float>(3, 1);
                    shoulderLeftPC1prediction = kalmanshoulderLeftPC1.Predict();
                    MCvPoint3D32f predictPointshoulderLeftPC1 = new MCvPoint3D32f(shoulderLeftPC1prediction[0, 0], shoulderLeftPC1prediction[1, 0], shoulderLeftPC1prediction[2, 0]);
                    MCvPoint3D32f measurePointshoulderLeftPC1 = new MCvPoint3D32f(shoulderLeftSynPC1.GetMeasurement()[0, 0],
                        shoulderLeftSynPC1.GetMeasurement()[1, 0], shoulderLeftSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedshoulderLeftPC1 = kalmanshoulderLeftPC1.Correct(shoulderLeftSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedshoulderLeftPC1Point = new MCvPoint3D32f(estimatedshoulderLeftPC1[0, 0], estimatedshoulderLeftPC1[1, 0], estimatedshoulderLeftPC1[2, 0]);
                    shoulderLeftSynPC1.GoToNextState();

                    shoulderLeftPC1.x = estimatedshoulderLeftPC1Point.x;
                    shoulderLeftPC1.y = estimatedshoulderLeftPC1Point.y;
                    shoulderLeftPC1.z = estimatedshoulderLeftPC1Point.z;

                    //Kalman shoulderLeftPC2
                    shoulderLeftPC2Mat[0, 0] = shoulderLeftPC2.x;
                    shoulderLeftPC2Mat[1, 0] = shoulderLeftPC2.y;
                    shoulderLeftPC2Mat[2, 0] = shoulderLeftPC2.z;


                    kalmanshoulderLeftPC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> shoulderLeftPC2state = shoulderLeftPC2Mat;
                    kalmanshoulderLeftPC2.CorrectedState = shoulderLeftPC2state;
                    kalmanshoulderLeftPC2.TransitionMatrix = shoulderLeftSynPC2.transitionMatrix;
                    kalmanshoulderLeftPC2.MeasurementNoiseCovariance = shoulderLeftSynPC2.measurementNoise;
                    kalmanshoulderLeftPC2.ProcessNoiseCovariance = shoulderLeftSynPC2.processNoise;
                    kalmanshoulderLeftPC2.ErrorCovariancePost = shoulderLeftSynPC2.errorCovariancePost;
                    kalmanshoulderLeftPC2.MeasurementMatrix = shoulderLeftSynPC2.measurementMatrix;

                    Matrix<float> shoulderLeftPC2prediction = new Matrix<float>(3, 1);
                    shoulderLeftPC2prediction = kalmanshoulderLeftPC2.Predict();
                    MCvPoint3D32f predictPointshoulderLeftPC2 = new MCvPoint3D32f(shoulderLeftPC2prediction[0, 0], shoulderLeftPC2prediction[1, 0], shoulderLeftPC2prediction[2, 0]);
                    MCvPoint3D32f measurePointshoulderLeftPC2 = new MCvPoint3D32f(shoulderLeftSynPC2.GetMeasurement()[0, 0],
                        shoulderLeftSynPC2.GetMeasurement()[1, 0], shoulderLeftSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedshoulderLeftPC2 = kalmanshoulderLeftPC2.Correct(shoulderLeftSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedshoulderLeftPC2Point = new MCvPoint3D32f(estimatedshoulderLeftPC2[0, 0], estimatedshoulderLeftPC2[1, 0], estimatedshoulderLeftPC2[2, 0]);
                    shoulderLeftSynPC2.GoToNextState();

                    shoulderLeftPC2.x = estimatedshoulderLeftPC2Point.x;
                    shoulderLeftPC2.y = estimatedshoulderLeftPC2Point.y;
                    shoulderLeftPC2.z = estimatedshoulderLeftPC2Point.z;

                    //Kalman elbowLeftPC1
                    elbowLeftPC1Mat[0, 0] = elbowLeftPC1.x;
                    elbowLeftPC1Mat[1, 0] = elbowLeftPC1.y;
                    elbowLeftPC1Mat[2, 0] = elbowLeftPC1.z;

                    kalmanelbowLeftPC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> elbowLeftPC1state = elbowLeftPC1Mat;
                    kalmanelbowLeftPC1.CorrectedState = elbowLeftPC1state;
                    kalmanelbowLeftPC1.TransitionMatrix = elbowLeftSynPC1.transitionMatrix;
                    kalmanelbowLeftPC1.MeasurementNoiseCovariance = elbowLeftSynPC1.measurementNoise;
                    kalmanelbowLeftPC1.ProcessNoiseCovariance = elbowLeftSynPC1.processNoise;
                    kalmanelbowLeftPC1.ErrorCovariancePost = elbowLeftSynPC1.errorCovariancePost;
                    kalmanelbowLeftPC1.MeasurementMatrix = elbowLeftSynPC1.measurementMatrix;

                    Matrix<float> elbowLeftPC1prediction = new Matrix<float>(3, 1);
                    elbowLeftPC1prediction = kalmanelbowLeftPC1.Predict();
                    MCvPoint3D32f predictPointelbowLeftPC1 = new MCvPoint3D32f(elbowLeftPC1prediction[0, 0], elbowLeftPC1prediction[1, 0], elbowLeftPC1prediction[2, 0]);
                    MCvPoint3D32f measurePointelbowLeftPC1 = new MCvPoint3D32f(elbowLeftSynPC1.GetMeasurement()[0, 0],
                        elbowLeftSynPC1.GetMeasurement()[1, 0], elbowLeftSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedelbowLeftPC1 = kalmanelbowLeftPC1.Correct(elbowLeftSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedelbowLeftPC1Point = new MCvPoint3D32f(estimatedelbowLeftPC1[0, 0], estimatedelbowLeftPC1[1, 0], estimatedelbowLeftPC1[2, 0]);
                    elbowLeftSynPC1.GoToNextState();

                    elbowLeftPC1.x = estimatedelbowLeftPC1Point.x;
                    elbowLeftPC1.y = estimatedelbowLeftPC1Point.y;
                    elbowLeftPC1.z = estimatedelbowLeftPC1Point.z;

                    //Kalman elbowLeftPC2
                    elbowLeftPC2Mat[0, 0] = elbowLeftPC2.x;
                    elbowLeftPC2Mat[1, 0] = elbowLeftPC2.y;
                    elbowLeftPC2Mat[2, 0] = elbowLeftPC2.z;


                    kalmanelbowLeftPC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> elbowLeftPC2state = elbowLeftPC2Mat;
                    kalmanelbowLeftPC2.CorrectedState = elbowLeftPC2state;
                    kalmanelbowLeftPC2.TransitionMatrix = elbowLeftSynPC2.transitionMatrix;
                    kalmanelbowLeftPC2.MeasurementNoiseCovariance = elbowLeftSynPC2.measurementNoise;
                    kalmanelbowLeftPC2.ProcessNoiseCovariance = elbowLeftSynPC2.processNoise;
                    kalmanelbowLeftPC2.ErrorCovariancePost = elbowLeftSynPC2.errorCovariancePost;
                    kalmanelbowLeftPC2.MeasurementMatrix = elbowLeftSynPC2.measurementMatrix;

                    Matrix<float> elbowLeftPC2prediction = new Matrix<float>(3, 1);
                    elbowLeftPC2prediction = kalmanelbowLeftPC2.Predict();
                    MCvPoint3D32f predictPointelbowLeftPC2 = new MCvPoint3D32f(elbowLeftPC2prediction[0, 0], elbowLeftPC2prediction[1, 0], elbowLeftPC2prediction[2, 0]);
                    MCvPoint3D32f measurePointelbowLeftPC2 = new MCvPoint3D32f(elbowLeftSynPC2.GetMeasurement()[0, 0],
                        elbowLeftSynPC2.GetMeasurement()[1, 0], elbowLeftSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedelbowLeftPC2 = kalmanelbowLeftPC2.Correct(elbowLeftSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedelbowLeftPC2Point = new MCvPoint3D32f(estimatedelbowLeftPC2[0, 0], estimatedelbowLeftPC2[1, 0], estimatedelbowLeftPC2[2, 0]);
                    elbowLeftSynPC2.GoToNextState();

                    elbowLeftPC2.x = estimatedelbowLeftPC2Point.x;
                    elbowLeftPC2.y = estimatedelbowLeftPC2Point.y;
                    elbowLeftPC2.z = estimatedelbowLeftPC2Point.z;

                    //Kalman wristLeftPC1
                    wristLeftPC1Mat[0, 0] = wristLeftPC1.x;
                    wristLeftPC1Mat[1, 0] = wristLeftPC1.y;
                    wristLeftPC1Mat[2, 0] = wristLeftPC1.z;

                    kalmanwristLeftPC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> wristLeftPC1state = wristLeftPC1Mat;
                    kalmanwristLeftPC1.CorrectedState = wristLeftPC1state;
                    kalmanwristLeftPC1.TransitionMatrix = wristLeftSynPC1.transitionMatrix;
                    kalmanwristLeftPC1.MeasurementNoiseCovariance = wristLeftSynPC1.measurementNoise;
                    kalmanwristLeftPC1.ProcessNoiseCovariance = wristLeftSynPC1.processNoise;
                    kalmanwristLeftPC1.ErrorCovariancePost = wristLeftSynPC1.errorCovariancePost;
                    kalmanwristLeftPC1.MeasurementMatrix = wristLeftSynPC1.measurementMatrix;

                    Matrix<float> wristLeftPC1prediction = new Matrix<float>(3, 1);
                    wristLeftPC1prediction = kalmanwristLeftPC1.Predict();
                    MCvPoint3D32f predictPointwristLeftPC1 = new MCvPoint3D32f(wristLeftPC1prediction[0, 0], wristLeftPC1prediction[1, 0], wristLeftPC1prediction[2, 0]);
                    MCvPoint3D32f measurePointwristLeftPC1 = new MCvPoint3D32f(wristLeftSynPC1.GetMeasurement()[0, 0],
                        wristLeftSynPC1.GetMeasurement()[1, 0], wristLeftSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedwristLeftPC1 = kalmanwristLeftPC1.Correct(wristLeftSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedwristLeftPC1Point = new MCvPoint3D32f(estimatedwristLeftPC1[0, 0], estimatedwristLeftPC1[1, 0], estimatedwristLeftPC1[2, 0]);
                    wristLeftSynPC1.GoToNextState();

                    wristLeftPC1.x = estimatedwristLeftPC1Point.x;
                    wristLeftPC1.y = estimatedwristLeftPC1Point.y;
                    wristLeftPC1.z = estimatedwristLeftPC1Point.z;

                    //Kalman wristLeftPC2
                    wristLeftPC2Mat[0, 0] = wristLeftPC2.x;
                    wristLeftPC2Mat[1, 0] = wristLeftPC2.y;
                    wristLeftPC2Mat[2, 0] = wristLeftPC2.z;


                    kalmanwristLeftPC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> wristLeftPC2state = wristLeftPC2Mat;
                    kalmanwristLeftPC2.CorrectedState = wristLeftPC2state;
                    kalmanwristLeftPC2.TransitionMatrix = wristLeftSynPC2.transitionMatrix;
                    kalmanwristLeftPC2.MeasurementNoiseCovariance = wristLeftSynPC2.measurementNoise;
                    kalmanwristLeftPC2.ProcessNoiseCovariance = wristLeftSynPC2.processNoise;
                    kalmanwristLeftPC2.ErrorCovariancePost = wristLeftSynPC2.errorCovariancePost;
                    kalmanwristLeftPC2.MeasurementMatrix = wristLeftSynPC2.measurementMatrix;

                    Matrix<float> wristLeftPC2prediction = new Matrix<float>(3, 1);
                    wristLeftPC2prediction = kalmanwristLeftPC2.Predict();
                    MCvPoint3D32f predictPointwristLeftPC2 = new MCvPoint3D32f(wristLeftPC2prediction[0, 0], wristLeftPC2prediction[1, 0], wristLeftPC2prediction[2, 0]);
                    MCvPoint3D32f measurePointwristLeftPC2 = new MCvPoint3D32f(wristLeftSynPC2.GetMeasurement()[0, 0],
                        wristLeftSynPC2.GetMeasurement()[1, 0], wristLeftSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedwristLeftPC2 = kalmanwristLeftPC2.Correct(wristLeftSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedwristLeftPC2Point = new MCvPoint3D32f(estimatedwristLeftPC2[0, 0], estimatedwristLeftPC2[1, 0], estimatedwristLeftPC2[2, 0]);
                    wristLeftSynPC2.GoToNextState();

                    wristLeftPC2.x = estimatedwristLeftPC2Point.x;
                    wristLeftPC2.y = estimatedwristLeftPC2Point.y;
                    wristLeftPC2.z = estimatedwristLeftPC2Point.z;

                    //Kalman shoulderRightPC1
                    shoulderRightPC1Mat[0, 0] = shoulderRightPC1.x;
                    shoulderRightPC1Mat[1, 0] = shoulderRightPC1.y;
                    shoulderRightPC1Mat[2, 0] = shoulderRightPC1.z;

                    kalmanshoulderRightPC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> shoulderRightPC1state = shoulderRightPC1Mat;
                    kalmanshoulderRightPC1.CorrectedState = shoulderRightPC1state;
                    kalmanshoulderRightPC1.TransitionMatrix = shoulderRightSynPC1.transitionMatrix;
                    kalmanshoulderRightPC1.MeasurementNoiseCovariance = shoulderRightSynPC1.measurementNoise;
                    kalmanshoulderRightPC1.ProcessNoiseCovariance = shoulderRightSynPC1.processNoise;
                    kalmanshoulderRightPC1.ErrorCovariancePost = shoulderRightSynPC1.errorCovariancePost;
                    kalmanshoulderRightPC1.MeasurementMatrix = shoulderRightSynPC1.measurementMatrix;

                    Matrix<float> shoulderRightPC1prediction = new Matrix<float>(3, 1);
                    shoulderRightPC1prediction = kalmanshoulderRightPC1.Predict();
                    MCvPoint3D32f predictPointshoulderRightPC1 = new MCvPoint3D32f(shoulderRightPC1prediction[0, 0], shoulderRightPC1prediction[1, 0], shoulderRightPC1prediction[2, 0]);
                    MCvPoint3D32f measurePointshoulderRightPC1 = new MCvPoint3D32f(shoulderRightSynPC1.GetMeasurement()[0, 0],
                        shoulderRightSynPC1.GetMeasurement()[1, 0], shoulderRightSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedshoulderRightPC1 = kalmanshoulderRightPC1.Correct(shoulderRightSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedshoulderRightPC1Point = new MCvPoint3D32f(estimatedshoulderRightPC1[0, 0], estimatedshoulderRightPC1[1, 0], estimatedshoulderRightPC1[2, 0]);
                    shoulderRightSynPC1.GoToNextState();

                    shoulderRightPC1.x = estimatedshoulderRightPC1Point.x;
                    shoulderRightPC1.y = estimatedshoulderRightPC1Point.y;
                    shoulderRightPC1.z = estimatedshoulderRightPC1Point.z;

                    //Kalman shoulderRightPC2
                    shoulderRightPC2Mat[0, 0] = shoulderRightPC2.x;
                    shoulderRightPC2Mat[1, 0] = shoulderRightPC2.y;
                    shoulderRightPC2Mat[2, 0] = shoulderRightPC2.z;


                    kalmanshoulderRightPC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> shoulderRightPC2state = shoulderRightPC2Mat;
                    kalmanshoulderRightPC2.CorrectedState = shoulderRightPC2state;
                    kalmanshoulderRightPC2.TransitionMatrix = shoulderRightSynPC2.transitionMatrix;
                    kalmanshoulderRightPC2.MeasurementNoiseCovariance = shoulderRightSynPC2.measurementNoise;
                    kalmanshoulderRightPC2.ProcessNoiseCovariance = shoulderRightSynPC2.processNoise;
                    kalmanshoulderRightPC2.ErrorCovariancePost = shoulderRightSynPC2.errorCovariancePost;
                    kalmanshoulderRightPC2.MeasurementMatrix = shoulderRightSynPC2.measurementMatrix;

                    Matrix<float> shoulderRightPC2prediction = new Matrix<float>(3, 1);
                    shoulderRightPC2prediction = kalmanshoulderRightPC2.Predict();
                    MCvPoint3D32f predictPointshoulderRightPC2 = new MCvPoint3D32f(shoulderRightPC2prediction[0, 0], shoulderRightPC2prediction[1, 0], shoulderRightPC2prediction[2, 0]);
                    MCvPoint3D32f measurePointshoulderRightPC2 = new MCvPoint3D32f(shoulderRightSynPC2.GetMeasurement()[0, 0],
                        shoulderRightSynPC2.GetMeasurement()[1, 0], shoulderRightSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedshoulderRightPC2 = kalmanshoulderRightPC2.Correct(shoulderRightSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedshoulderRightPC2Point = new MCvPoint3D32f(estimatedshoulderRightPC2[0, 0], estimatedshoulderRightPC2[1, 0], estimatedshoulderRightPC2[2, 0]);
                    shoulderRightSynPC2.GoToNextState();

                    shoulderRightPC2.x = estimatedshoulderRightPC2Point.x;
                    shoulderRightPC2.y = estimatedshoulderRightPC2Point.y;
                    shoulderRightPC2.z = estimatedshoulderRightPC2Point.z;

                    //Kalman elbowRightPC1
                    elbowRightPC1Mat[0, 0] = elbowRightPC1.x;
                    elbowRightPC1Mat[1, 0] = elbowRightPC1.y;
                    elbowRightPC1Mat[2, 0] = elbowRightPC1.z;

                    kalmanelbowRightPC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> elbowRightPC1state = elbowRightPC1Mat;
                    kalmanelbowRightPC1.CorrectedState = elbowRightPC1state;
                    kalmanelbowRightPC1.TransitionMatrix = elbowRightSynPC1.transitionMatrix;
                    kalmanelbowRightPC1.MeasurementNoiseCovariance = elbowRightSynPC1.measurementNoise;
                    kalmanelbowRightPC1.ProcessNoiseCovariance = elbowRightSynPC1.processNoise;
                    kalmanelbowRightPC1.ErrorCovariancePost = elbowRightSynPC1.errorCovariancePost;
                    kalmanelbowRightPC1.MeasurementMatrix = elbowRightSynPC1.measurementMatrix;

                    Matrix<float> elbowRightPC1prediction = new Matrix<float>(3, 1);
                    elbowRightPC1prediction = kalmanelbowRightPC1.Predict();
                    MCvPoint3D32f predictPointelbowRightPC1 = new MCvPoint3D32f(elbowRightPC1prediction[0, 0], elbowRightPC1prediction[1, 0], elbowRightPC1prediction[2, 0]);
                    MCvPoint3D32f measurePointelbowRightPC1 = new MCvPoint3D32f(elbowRightSynPC1.GetMeasurement()[0, 0],
                        elbowRightSynPC1.GetMeasurement()[1, 0], elbowRightSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedelbowRightPC1 = kalmanelbowRightPC1.Correct(elbowRightSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedelbowRightPC1Point = new MCvPoint3D32f(estimatedelbowRightPC1[0, 0], estimatedelbowRightPC1[1, 0], estimatedelbowRightPC1[2, 0]);
                    elbowRightSynPC1.GoToNextState();

                    elbowRightPC1.x = estimatedelbowRightPC1Point.x;
                    elbowRightPC1.y = estimatedelbowRightPC1Point.y;
                    elbowRightPC1.z = estimatedelbowRightPC1Point.z;

                    //Kalman elbowRightPC2
                    elbowRightPC2Mat[0, 0] = elbowRightPC2.x;
                    elbowRightPC2Mat[1, 0] = elbowRightPC2.y;
                    elbowRightPC2Mat[2, 0] = elbowRightPC2.z;


                    kalmanelbowRightPC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> elbowRightPC2state = elbowRightPC2Mat;
                    kalmanelbowRightPC2.CorrectedState = elbowRightPC2state;
                    kalmanelbowRightPC2.TransitionMatrix = elbowRightSynPC2.transitionMatrix;
                    kalmanelbowRightPC2.MeasurementNoiseCovariance = elbowRightSynPC2.measurementNoise;
                    kalmanelbowRightPC2.ProcessNoiseCovariance = elbowRightSynPC2.processNoise;
                    kalmanelbowRightPC2.ErrorCovariancePost = elbowRightSynPC2.errorCovariancePost;
                    kalmanelbowRightPC2.MeasurementMatrix = elbowRightSynPC2.measurementMatrix;

                    Matrix<float> elbowRightPC2prediction = new Matrix<float>(3, 1);
                    elbowRightPC2prediction = kalmanelbowRightPC2.Predict();
                    MCvPoint3D32f predictPointelbowRightPC2 = new MCvPoint3D32f(elbowRightPC2prediction[0, 0], elbowRightPC2prediction[1, 0], elbowRightPC2prediction[2, 0]);
                    MCvPoint3D32f measurePointelbowRightPC2 = new MCvPoint3D32f(elbowRightSynPC2.GetMeasurement()[0, 0],
                        elbowRightSynPC2.GetMeasurement()[1, 0], elbowRightSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedelbowRightPC2 = kalmanelbowRightPC2.Correct(elbowRightSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedelbowRightPC2Point = new MCvPoint3D32f(estimatedelbowRightPC2[0, 0], estimatedelbowRightPC2[1, 0], estimatedelbowRightPC2[2, 0]);
                    elbowRightSynPC2.GoToNextState();

                    elbowRightPC2.x = estimatedelbowRightPC2Point.x;
                    elbowRightPC2.y = estimatedelbowRightPC2Point.y;
                    elbowRightPC2.z = estimatedelbowRightPC2Point.z;

                    //Kalman wristRightPC1
                    wristRightPC1Mat[0, 0] = wristRightPC1.x;
                    wristRightPC1Mat[1, 0] = wristRightPC1.y;
                    wristRightPC1Mat[2, 0] = wristRightPC1.z;

                    kalmanwristRightPC1 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> wristRightPC1state = wristRightPC1Mat;
                    kalmanwristRightPC1.CorrectedState = wristRightPC1state;
                    kalmanwristRightPC1.TransitionMatrix = wristRightSynPC1.transitionMatrix;
                    kalmanwristRightPC1.MeasurementNoiseCovariance = wristRightSynPC1.measurementNoise;
                    kalmanwristRightPC1.ProcessNoiseCovariance = wristRightSynPC1.processNoise;
                    kalmanwristRightPC1.ErrorCovariancePost = wristRightSynPC1.errorCovariancePost;
                    kalmanwristRightPC1.MeasurementMatrix = wristRightSynPC1.measurementMatrix;

                    Matrix<float> wristRightPC1prediction = new Matrix<float>(3, 1);
                    wristRightPC1prediction = kalmanwristRightPC1.Predict();
                    MCvPoint3D32f predictPointwristRightPC1 = new MCvPoint3D32f(wristRightPC1prediction[0, 0], wristRightPC1prediction[1, 0], wristRightPC1prediction[2, 0]);
                    MCvPoint3D32f measurePointwristRightPC1 = new MCvPoint3D32f(wristRightSynPC1.GetMeasurement()[0, 0],
                        wristRightSynPC1.GetMeasurement()[1, 0], wristRightSynPC1.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedwristRightPC1 = kalmanwristRightPC1.Correct(wristRightSynPC1.GetMeasurement());
                    MCvPoint3D32f estimatedwristRightPC1Point = new MCvPoint3D32f(estimatedwristRightPC1[0, 0], estimatedwristRightPC1[1, 0], estimatedwristRightPC1[2, 0]);
                    wristRightSynPC1.GoToNextState();

                    wristRightPC1.x = estimatedwristRightPC1Point.x;
                    wristRightPC1.y = estimatedwristRightPC1Point.y;
                    wristRightPC1.z = estimatedwristRightPC1Point.z;

                    //Kalman wristRightPC2
                    wristRightPC2Mat[0, 0] = wristRightPC2.x;
                    wristRightPC2Mat[1, 0] = wristRightPC2.y;
                    wristRightPC2Mat[2, 0] = wristRightPC2.z;


                    kalmanwristRightPC2 = new Kalman(6, 3, 0);

                    Emgu.CV.Matrix<float> wristRightPC2state = wristRightPC2Mat;
                    kalmanwristRightPC2.CorrectedState = wristRightPC2state;
                    kalmanwristRightPC2.TransitionMatrix = wristRightSynPC2.transitionMatrix;
                    kalmanwristRightPC2.MeasurementNoiseCovariance = wristRightSynPC2.measurementNoise;
                    kalmanwristRightPC2.ProcessNoiseCovariance = wristRightSynPC2.processNoise;
                    kalmanwristRightPC2.ErrorCovariancePost = wristRightSynPC2.errorCovariancePost;
                    kalmanwristRightPC2.MeasurementMatrix = wristRightSynPC2.measurementMatrix;

                    Matrix<float> wristRightPC2prediction = new Matrix<float>(3, 1);
                    wristRightPC2prediction = kalmanwristRightPC2.Predict();
                    MCvPoint3D32f predictPointwristRightPC2 = new MCvPoint3D32f(wristRightPC2prediction[0, 0], wristRightPC2prediction[1, 0], wristRightPC2prediction[2, 0]);
                    MCvPoint3D32f measurePointwristRightPC2 = new MCvPoint3D32f(wristRightSynPC2.GetMeasurement()[0, 0],
                        wristRightSynPC2.GetMeasurement()[1, 0], wristRightSynPC2.GetMeasurement()[2, 0]);
                    Matrix<float> estimatedwristRightPC2 = kalmanwristRightPC2.Correct(wristRightSynPC2.GetMeasurement());
                    MCvPoint3D32f estimatedwristRightPC2Point = new MCvPoint3D32f(estimatedwristRightPC2[0, 0], estimatedwristRightPC2[1, 0], estimatedwristRightPC2[2, 0]);
                    wristRightSynPC2.GoToNextState();

                    wristRightPC2.x = estimatedwristRightPC2Point.x;
                    wristRightPC2.y = estimatedwristRightPC2Point.y;
                    wristRightPC2.z = estimatedwristRightPC2Point.z;
                    #endregion

                    //Progressing
                    #region Scailing

                    //scailing_vectorPC1 = scale_vector_update(headPC1, neckPC1, shoulderLeftPC1, elbowLeftPC1, wristLeftPC1, shoulderRightPC1, elbowRightPC1, wristRightPC1, spineMidPC1, spineBasePC1, scailing_vectorPC1);
                    //scailing_vectorPC2 = scale_vector_update(headPC2, neckPC2, shoulderLeftPC2, elbowLeftPC2, wristLeftPC2, shoulderRightPC2, elbowRightPC2, wristRightPC2, spineMidPC2, spineBasePC2, scailing_vectorPC2);


                    //Console.WriteLine("Alpha = {0}, Beta = {1}, Gamma = {2}", scailing_vectorPC1[0], scailing_vectorPC1[1], scailing_vectorPC1[2]);

                    //headPC1 = scale_skeleton(headPC1, scailing_vectorPC1);
                    //neckPC1 = scale_skeleton(neckPC1, scailing_vectorPC1);
                    //spineMidPC1 = scale_skeleton(spineMidPC1, scailing_vectorPC1);
                    //spineBasePC1 = scale_skeleton(spineBasePC1, scailing_vectorPC1);
                    //shoulderLeftPC1 = scale_skeleton(shoulderLeftPC1, scailing_vectorPC1);
                    //elbowLeftPC1 = scale_skeleton(elbowLeftPC1, scailing_vectorPC1);
                    //wristLeftPC1 = scale_skeleton(wristLeftPC1, scailing_vectorPC1);
                    //shoulderRightPC1 = scale_skeleton(shoulderRightPC1, scailing_vectorPC1);
                    //elbowRightPC1 = scale_skeleton(elbowRightPC1, scailing_vectorPC1);
                    //wristRightPC1 = scale_skeleton(wristRightPC1, scailing_vectorPC1);


                    //headPC2 = scale_skeleton(headPC2, scailing_vectorPC2);
                    //neckPC2 = scale_skeleton(neckPC2, scailing_vectorPC2);
                    //spineMidPC2 = scale_skeleton(spineMidPC2, scailing_vectorPC2);
                    //spineBasePC2 = scale_skeleton(spineBasePC2, scailing_vectorPC2);
                    //shoulderLeftPC2 = scale_skeleton(shoulderLeftPC2, scailing_vectorPC2);
                    //elbowLeftPC2 = scale_skeleton(elbowLeftPC2, scailing_vectorPC2);
                    //wristLeftPC2 = scale_skeleton(wristLeftPC2, scailing_vectorPC2);
                    //shoulderRightPC2 = scale_skeleton(shoulderRightPC2, scailing_vectorPC2);
                    //elbowRightPC2 = scale_skeleton(elbowRightPC2, scailing_vectorPC2);
                    //wristRightPC2 = scale_skeleton(wristRightPC2, scailing_vectorPC2);


                    #endregion

                    #region Calibration
                    //Gradient Decsent
                    calVar = gradientDecsent(
                                    spineMidPC1.x, spineMidPC1.y, spineMidPC1.z, spineMidPC2.x, spineMidPC2.y, spineMidPC2.z,
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

                    #endregion
                    #region DrawSkeleton <Calibration>
                    //DrawSkeleton of Calibration

                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { canvas.Children.Clear(); }));

                    //Head
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {

                        System.Windows.Shapes.Ellipse drawHead = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Red,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawHead, headCar.x * 300 - drawHead.Width / 2);
                        Canvas.SetTop(drawHead, headCar.y * 300 - drawHead.Height / 2);
                        canvas.Children.Add(drawHead);

                    }));

                    

                    //Neck
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        System.Windows.Shapes.Ellipse drawNeck = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Orange,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawNeck, neckCar.x * 300 - drawNeck.Width / 2);
                        Canvas.SetTop(drawNeck, neckCar.y * 300 - drawNeck.Height / 2);
                        canvas.Children.Add(drawNeck);
                    }));

                    //Left Shoulder
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        System.Windows.Shapes.Ellipse drawLeftShoulder = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Yellow,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawLeftShoulder, shoulderLeftCar.x * 300 - drawLeftShoulder.Width / 2);
                        Canvas.SetTop(drawLeftShoulder, shoulderLeftCar.y * 300 - drawLeftShoulder.Height / 2);
                        canvas.Children.Add(drawLeftShoulder);
                    }));

                    //Left Elbow
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        System.Windows.Shapes.Ellipse drawElbowLeft = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Yellow,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawElbowLeft, elbowLeftCar.x * 300 - drawElbowLeft.Width / 2);
                        Canvas.SetTop(drawElbowLeft, elbowLeftCar.y * 300 - drawElbowLeft.Height / 2);
                        canvas.Children.Add(drawElbowLeft);
                    }));

                    //Left Wrist
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        System.Windows.Shapes.Ellipse drawWristLeft = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Yellow,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawWristLeft, wristLeftCar.x * 300 - drawWristLeft.Width / 2);
                        Canvas.SetTop(drawWristLeft, wristLeftCar.y * 300 - drawWristLeft.Height / 2);
                        canvas.Children.Add(drawWristLeft);
                    }));

                    //Right Shoulder
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        System.Windows.Shapes.Ellipse drawRightShoulder = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Green,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawRightShoulder, shoulderRightCar.x * 300 - drawRightShoulder.Width / 2);
                        Canvas.SetTop(drawRightShoulder, shoulderRightCar.y * 300 - drawRightShoulder.Height / 2);
                        canvas.Children.Add(drawRightShoulder);
                    }));

                    //Right Elbow
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        System.Windows.Shapes.Ellipse drawElbowRight = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Green,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawElbowRight, elbowRightCar.x * 300 - drawElbowRight.Width / 2);
                        Canvas.SetTop(drawElbowRight, elbowRightCar.y * 300 - drawElbowRight.Height / 2);
                        canvas.Children.Add(drawElbowRight);
                    }));

                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        System.Windows.Shapes.Ellipse drawWristRight = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Green,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawWristRight, wristRightCar.x * 300 - drawWristRight.Width / 2);
                        Canvas.SetTop(drawWristRight, wristRightCar.y * 300 - drawWristRight.Height / 2);
                        canvas.Children.Add(drawWristRight);
                    }));

                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        System.Windows.Shapes.Ellipse drawSpineBase = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Orange,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawSpineBase, spineBaseCar.x * 300 - drawSpineBase.Width / 2);
                        Canvas.SetTop(drawSpineBase, spineBaseCar.y * 300 - drawSpineBase.Height / 2);
                        canvas.Children.Add(drawSpineBase);
                    }));

                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        System.Windows.Shapes.Ellipse drawSpineMid = new System.Windows.Shapes.Ellipse
                        {
                            Fill = Brushes.Orange,
                            Width = 20,
                            Height = 20
                        };

                        Canvas.SetLeft(drawSpineMid, spineMidCar.x * 300 - drawSpineMid.Width / 2);
                        Canvas.SetTop(drawSpineMid, spineMidCar.y * 300 - drawSpineMid.Height / 2);
                        canvas.Children.Add(drawSpineMid);
                    }));

                    try
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            //Head to Neck
                            Line lineHeadToNeck = new Line();
                            lineHeadToNeck.Stroke = Brushes.LightSteelBlue;
                            lineHeadToNeck.X1 = (double)headCar.x;
                            lineHeadToNeck.Y1 = (double)headCar.y;
                            lineHeadToNeck.X2 = (double)neckCar.x;
                            lineHeadToNeck.Y2 = (double)neckCar.y;
                            lineHeadToNeck.StrokeThickness = 2;

                            //Neck to LeftShoulder
                            Line lineNeckToLeftShoulder = new Line();
                            lineNeckToLeftShoulder.Stroke = Brushes.LightSteelBlue;
                            lineNeckToLeftShoulder.X1 = (double)shoulderLeftCar.x;
                            lineNeckToLeftShoulder.Y1 = (double)shoulderLeftCar.y;
                            lineNeckToLeftShoulder.X2 = (double)neckCar.x;
                            lineNeckToLeftShoulder.Y2 = (double)neckCar.y;
                            lineNeckToLeftShoulder.StrokeThickness = 2;

                            //LeftShoulder to LeftElbow
                            Line lineLeftShoulderToElbowLeft = new Line();
                            lineLeftShoulderToElbowLeft.Stroke = Brushes.LightSteelBlue;
                            lineLeftShoulderToElbowLeft.X1 = (double)shoulderLeftCar.x;
                            lineLeftShoulderToElbowLeft.Y1 = (double)shoulderLeftCar.y;
                            lineLeftShoulderToElbowLeft.X2 = (double)elbowLeftCar.x;
                            lineLeftShoulderToElbowLeft.Y2 = (double)elbowLeftCar.y;
                            lineLeftShoulderToElbowLeft.StrokeThickness = 2;

                            //LeftElbow to LeftWrist
                            Line lineElbowLeftToWristLeft = new Line();
                            lineElbowLeftToWristLeft.Stroke = Brushes.LightSteelBlue;
                            lineElbowLeftToWristLeft.X1 = (double)elbowLeftCar.x;
                            lineElbowLeftToWristLeft.Y1 = (double)elbowLeftCar.y;
                            lineElbowLeftToWristLeft.X2 = (double)wristLeftCar.x;
                            lineElbowLeftToWristLeft.Y2 = (double)wristLeftCar.y;
                            lineElbowLeftToWristLeft.StrokeThickness = 2;

                            //Neck to RightShoulder
                            Line lineNeckToRightShoulder = new Line();
                            lineNeckToRightShoulder.Stroke = Brushes.LightSteelBlue;
                            lineNeckToRightShoulder.X1 = (double)shoulderRightCar.x;
                            lineNeckToRightShoulder.Y1 = (double)shoulderRightCar.y;
                            lineNeckToRightShoulder.X2 = (double)neckCar.x;
                            lineNeckToRightShoulder.Y2 = (double)neckCar.y;
                            lineNeckToRightShoulder.StrokeThickness = 2;

                            //RightShoulder to RightElbow
                            Line lineRightShoulderToElbowRight = new Line();
                            lineRightShoulderToElbowRight.Stroke = Brushes.LightSteelBlue;
                            lineRightShoulderToElbowRight.X1 = (double)shoulderRightCar.x;
                            lineRightShoulderToElbowRight.Y1 = (double)shoulderRightCar.y;
                            lineRightShoulderToElbowRight.X2 = (double)elbowRightCar.x;
                            lineRightShoulderToElbowRight.Y2 = (double)elbowRightCar.y;
                            lineRightShoulderToElbowRight.StrokeThickness = 2;

                            //RightElbow to RightWrist
                            Line lineElbowRightToWristRight = new Line();
                            lineElbowRightToWristRight.Stroke = Brushes.LightSteelBlue;
                            lineElbowRightToWristRight.X1 = (double)elbowRightCar.x;
                            lineElbowRightToWristRight.Y1 = (double)elbowRightCar.y;
                            lineElbowRightToWristRight.X2 = (double)wristRightCar.x;
                            lineElbowRightToWristRight.Y2 = (double)wristRightCar.y;
                            lineElbowRightToWristRight.StrokeThickness = 2;

                            //Neck to SpineMid
                            Line lineNeckToSpineMid = new Line();
                            lineNeckToSpineMid.Stroke = Brushes.LightSteelBlue;
                            lineNeckToSpineMid.X1 = (double)neckCar.x;
                            lineNeckToSpineMid.Y1 = (double)neckCar.y;
                            lineNeckToSpineMid.X2 = (double)spineMidCar.x;
                            lineNeckToSpineMid.Y2 = (double)spineMidCar.y;
                            lineNeckToSpineMid.StrokeThickness = 2;

                            //SpineMid to SpineBase
                            Line lineSpineMidToSpineBase = new Line();
                            lineSpineMidToSpineBase.Stroke = Brushes.LightSteelBlue;
                            lineSpineMidToSpineBase.X1 = (double)spineMidCar.x;
                            lineSpineMidToSpineBase.Y1 = (double)spineMidCar.y;
                            lineSpineMidToSpineBase.X2 = (double)spineBaseCar.x;
                            lineSpineMidToSpineBase.Y2 = (double)spineBaseCar.y;
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
                        }));
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    //DrawSkeleton of Calibration ----------------> END <----------------
                    #endregion
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }
        public void Sending()
        {
            #region Angular Transform
            TcpClient tc = new TcpClient("192.168.0.100", 5001);                   // 에버 제어 PC 아이피 확인 후 변경
            NetworkStream stream = tc.GetStream();

            byte[] check_sum = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int packet_type = 0x02;
            int tr_no = 0;
            int data_type = 5000;
            string[] stringArray = new string[1492];
            byte[] buff;
            int data_len;

            while (true)
            {
                CoOrd halfShoulder;

                halfShoulder.x = (shoulderRightCar.x - shoulderLeftCar.x) / 2;
                halfShoulder.y = (shoulderRightCar.y - shoulderLeftCar.y) / 2;
                halfShoulder.z = (shoulderRightCar.z - shoulderLeftCar.z) / 2;
                halfShoulder.markerPC = 1;
                halfShoulder.markerTrackingState = 0;
                halfShoulder.markerType = 0;

                defineVectorAngleArm = Vectorize(spineBaseCar, neckCar);
                defineVectorAngleLeftArmRotate = Vectorize(shoulderRightCar, shoulderLeftCar);
                defineVectorAngleRightArmRotate = Vectorize(shoulderLeftCar, shoulderRightCar);
                

                //머리
                angle1 = 0;                                                                                         //허리회전
                angle2 = 90 - AngularTransform(headCar, neckCar, shoulderLeftCar);                                    //고개 좌우
                angle3 = 180 - AngularTransform(headCar, neckCar, spineMidCar);                                       //고개 앞뒤
                angle4 = 0;                                                                                         //고개 회전

                //왼팔
                angle5 = VectorProjAngle(neckCar, spineMidCar, spineBaseCar, shoulderLeftCar, elbowLeftCar, defineVectorAngleArm);

                if (angle5 > 110)
                {
                    angle5 = 100;
                }

                if (angle5 < -30)
                {
                    angle5 = -30;
                }

                angle6 = 180 - AngularTransform3(elbowLeftCar, shoulderLeftCar, neckCar) - 110;

                if (angle6 < 0)
                {
                    angle6 = 0;
                }

                if (angle6 > 80)
                {
                    angle6 = 80;
                }


                angle7 = VectorProjAngle(shoulderLeftCar, halfShoulder, shoulderRightCar, elbowLeftCar, wristLeftCar, defineVectorAngleLeftArmRotate) - 70;
                if (angle7 < -40)
                {
                    angle7 = -40;
                }

                if (angle7 > 35)
                {
                    angle7 = 35;
                }

                angle8 = AngularTransform3(shoulderLeftCar, elbowLeftCar, wristLeftCar);

                if (angle8 > 110)
                {
                    angle8 = 100;
                }

                //오른팔

                angle11 = VectorProjAngle(neckCar, spineMidCar, spineBaseCar, shoulderRightCar, elbowRightCar, defineVectorAngleArm);
                if (angle11 > 110)
                {
                    angle11 = 100;
                }

                if (angle11 < -30)
                {
                    angle11 = -30;
                }

                angle12 = 180 - AngularTransform3(elbowRightCar, shoulderRightCar, neckCar) - 110;
                if (angle12 < 0)
                {
                    angle12 = 0;
                }

                if (angle12 > 80)
                {
                    angle12 = 80;
                }

                angle13 = VectorProjAngle(shoulderLeftCar, halfShoulder, shoulderRightCar, elbowRightCar, wristRightCar, defineVectorAngleRightArmRotate) - 90;
                if (angle13 < -40)
                {
                    angle13 = -40;
                }

                if (angle13 > 35)
                {
                    angle13 = 35;
                }

                angle14 = AngularTransform3(shoulderRightCar, elbowRightCar, wristRightCar);

                if (angle14 > 110)
                {
                    angle14 = 100;
                }

                string data = angle5 + "," + angle6 + "," + angle7 + "," + angle8 + "," + angle11 + "," + angle12 + "," + angle13 + "," + angle14; //+ ","+faceData

                #region Display Data Of Angles
                Console.Clear();
                Console.WriteLine("-------------------");
                if (faceData == null)
                {
                    Console.WriteLine("No FaceData");
                }
                else
                {
                    Console.WriteLine("FaceData = {0}", faceData);
                }
                

                Console.WriteLine("ArmData = {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", angle5, angle6, angle7, angle8, angle11, angle12, angle13, angle14);
                Console.WriteLine("-------------------");
                #endregion

                buff = Encoding.ASCII.GetBytes(data);
                data_len = data.Length;

                check_sum[0] = 0x55;
                check_sum[1] = (byte)(packet_type & 0xff);
                check_sum[2] = (byte)(tr_no & 0xff);
                check_sum[3] = (byte)((tr_no >> 8) & 0xff);
                check_sum[4] = (byte)(data_len & 0xff);
                check_sum[5] = (byte)((data_len >> 8) & 0xff);
                check_sum[6] = (byte)(data_type & 0xff);
                check_sum[7] = (byte)((data_type >> 8) & 0xff);

                int sum = check_sum[0] + check_sum[1] + check_sum[2] + check_sum[3] + check_sum[4] + check_sum[5] +
              check_sum[6] + check_sum[7];

                int data_sum = 0;

                for (int i = 0; i < data_len; i++)
                {
                    data_sum += buff[i];
                }

                check_sum[8] = (byte)(sum & 0xff);
                check_sum[9] = (byte)(data_sum & 0xff);

                byte[] msg = new byte[check_sum.Length + buff.Length];
                Buffer.BlockCopy(check_sum, 0, msg, 0, check_sum.Length);
                Buffer.BlockCopy(buff, 0, msg, check_sum.Length, buff.Length);


                //stream.Write(msg, 0, msg.Length);


            }

            #endregion
        }
    }
}