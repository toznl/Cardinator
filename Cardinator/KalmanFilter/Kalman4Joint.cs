using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;


namespace Coordinator
{
    public class Kalman4Joint


    {
        public MCvPoint3D32f Kalman4Joints(float x, float y, float z, Kalman kalman, SyntheticData syntheticData)
        {
            //Kalman headPC1
            Emgu.CV.Matrix<float> MatrixGet=new Emgu.CV.Matrix<float>(6,1);
            MatrixGet[0, 0] = x;
            MatrixGet[1, 0] =y;
            MatrixGet[2, 0] =z;

            kalman = new Kalman(6, 3, 0);

            Emgu.CV.Matrix<float> state = MatrixGet;
            kalman.CorrectedState = state;
            kalman.TransitionMatrix = syntheticData.transitionMatrix;
            kalman.MeasurementNoiseCovariance = syntheticData.measurementNoise;
            kalman.ProcessNoiseCovariance = syntheticData.processNoise;
            kalman.ErrorCovariancePost = syntheticData.errorCovariancePost;
            kalman.MeasurementMatrix = syntheticData.measurementMatrix;

            Matrix<float> prediction = new Matrix<float>(3, 1);
            prediction = kalman.Predict();
            MCvPoint3D32f predictPointheadPC1 = new MCvPoint3D32f(prediction[0, 0], prediction[1, 0], prediction[2, 0]);
            MCvPoint3D32f measurePointheadPC1 = new MCvPoint3D32f(syntheticData.GetMeasurement()[0, 0],
                syntheticData.GetMeasurement()[1, 0], syntheticData.GetMeasurement()[2, 0]);
            Matrix<float> estimated = kalman.Correct(syntheticData.GetMeasurement());
            MCvPoint3D32f estimatedPoint = new MCvPoint3D32f(estimated[0, 0], estimated[1, 0], estimated[2, 0]);
            syntheticData.GoToNextState();

            return estimatedPoint;
        }

        




        
    


        


    }
}      