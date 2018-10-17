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
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Windows.Threading;

using CooOrdStructure;
using Cardinator.Run;


namespace Coordinator
{
    public partial class MainWindow : Window
    {
        /*
        Region for all Functions and Variables
        */
                
        //MainWindow
        public MainWindow()
        {
            //Alloc Console Window
            AllocConsole();
            InitializeComponent();

            //Calibration Thread
            new Thread(new ThreadStart(Calibration)).Start();
            new Thread(new ThreadStart(Sending)).Start();
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
           
            
        }
        public void Sending()
        {
            #region Angular Transform

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
                angle9 = 0; //x 
                angle10 = 0;//x

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

                angle15 = 0; //x
                angle16 = 0; //x

                string data = angle1 + "," + angle2 + "," + angle3 + "," + angle4 + "," + angle5 + "," + angle6 + "," + angle7 + "," + angle8 + "," + angle9 + "," + angle10 + "," + angle11 + "," + angle12 + "," + angle13 + "," + angle14 + "," + angle15 + "," + angle16;

                //Console.WriteLine("Angle11 : {0} ", angle13);

                SendBuffer(data);
            }

            #endregion
        }
    }
}




