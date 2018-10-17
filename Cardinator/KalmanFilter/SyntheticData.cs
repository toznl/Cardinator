using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

using System.Runtime.InteropServices;



namespace KalmanFilter.SyntheticData
{
    // unscented kalman filter ported from Yi Cao matlab implementation. same outputs as MyKalman class from emgu
    public class SyntheticData
    {
        public Matrix<float> state;
        public Matrix<float> transitionMatrix;
        public Matrix<float> measurementMatrix;
        public Matrix<float> processNoise;
        public Matrix<float> measurementNoise;
        public Matrix<float> errorCovariancePost;

        public SyntheticData()
        {
            state = new Matrix<float>(6, 1);
            state[0, 0] = 0f; // x-pos
            state[1, 0] = 0f; // y-pos
            state[2, 0] = 0f; // z-pos
            state[3, 0] = 0f; // x-velocity
            state[4, 0] = 0f; // y-velocity
            state[5, 0] = 0f; // z-velocity

            transitionMatrix = new Matrix<float>(new float[,]
                    {
                        {1, 0, 0, 0.00001f, 0, 0},  // x-pos, y-pos, x-velocity, y-velocity
                        {0, 1, 0, 0, 0.00001f, 0},
                        {0, 0, 1, 0, 0, 0.00001f},
                        {0, 0, 0, 1, 0, 0},
                        {0, 0, 0, 0, 1, 0},
                        {0, 0, 0, 0, 0, 1}
                    });

            measurementMatrix = new Matrix<float>(new float[,]
                    {
                        { 1, 0, 0, 0, 0, 0 },
                        { 0, 1, 0, 0, 0, 0 },
                        { 0, 0, 1, 0, 0, 0 }
                    });
            measurementMatrix.SetIdentity();
            processNoise = new Matrix<float>(6, 6); //Linked to the size of the transition matrix
            processNoise.SetIdentity(new MCvScalar(1.0e-10)); //The smaller the value the more resistance to noise 
            measurementNoise = new Matrix<float>(3, 3); //Fixed accordiong to input data 
            measurementNoise.SetIdentity(new MCvScalar(1.0f));
            errorCovariancePost = new Matrix<float>(6, 6); //Linked to the size of the transition matrix
            errorCovariancePost.SetIdentity();
        }

        public Matrix<float> GetMeasurement()
        {
            Matrix<float> measurementNoise = new Matrix<float>(3, 1);
            measurementNoise.SetRandNormal(new MCvScalar(), new MCvScalar(Math.Sqrt(measurementNoise[0, 0])));
            return measurementMatrix * state + measurementNoise;
        }

        public void GoToNextState()
        {
            Matrix<float> processNoise = new Matrix<float>(6, 1);
            processNoise.SetRandNormal(new MCvScalar(), new MCvScalar(processNoise[0, 0]));
            state = transitionMatrix * state + processNoise;
        }
    }

}