using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cardinator.Old_Version
{

    ////Partial Differential // ErrorFunciton
    //public float errorFunc0(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
    //{
    //    float result;

    //    result = 2 * square((x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) - y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))))) - 2 * (x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) - y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) * ((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) - y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + 2 * square((y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + z * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))))) - 2 * (y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) - x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + z * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) * ((Convert.ToSingle(errorVar.transY)) - y1 - x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))));



    //    ;

    //    return result;
    //}

    //public float errorFunc1(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
    //{
    //    float result;
    //    result = 2 * (z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) * ((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) - y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) - 2 * (z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + y * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) * ((Convert.ToSingle(errorVar.transX)) - x1 - z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + 2 * (z * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) * ((Convert.ToSingle(errorVar.transY)) - y1 - x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))));

    //    return result;
    //}
    //public float errorFunc2(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
    //{
    //    float result;
    //    result = 2 * (y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) - x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) * ((Convert.ToSingle(errorVar.transX)) - x1 - z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + 2 * (x * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + y * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))))) * ((Convert.ToSingle(errorVar.transZ)) - z1 + x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) - y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) - 2 * (x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))))) * ((Convert.ToSingle(errorVar.transY)) - y1 - x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + z * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))));

    //    return result;
    //}
    //public float errorFunc3(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
    //{
    //    float result;
    //    result = 2 * (Convert.ToSingle(errorVar.transX)) - 2 * x1 - 2 * z * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) + 2 * x * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + 2 * y * (Convert.ToSingle(Math.Cos(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)));
    //    return result;
    //}
    //public float errorFunc4(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
    //{
    //    float result;
    //    result = 2 * (Convert.ToSingle(errorVar.transY)) - 2 * y1 - 2 * x * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) - (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) + 2 * y * ((Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) + (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + 2 * z * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)));
    //    return result;
    //}
    //public float errorFunc5(float x, float y, float z, float x1, float y1, float z1, ErrorVar errorVar)
    //{
    //    float result;
    //    result = 2 * (Convert.ToSingle(errorVar.transZ)) - 2 * z1 + 2 * x * ((Convert.ToSingle(Math.Sin(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ))) + (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)))) - 2 * y * ((Convert.ToSingle(Math.Cos(errorVar.thetaZ))) * (Convert.ToSingle(Math.Sin(errorVar.thetaX))) - (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY))) * (Convert.ToSingle(Math.Sin(errorVar.thetaZ)))) + 2 * z * (Convert.ToSingle(Math.Cos(errorVar.thetaX))) * (Convert.ToSingle(Math.Sin(errorVar.thetaY)));
    //    return result;
    //}

    ////GradientDecsent Diffrential 
    //public ErrorVar gradientDecsent(
    //    float x0, float y0, float z0, float x1, float y1, float z1,
    //    float x2, float y2, float z2, float x3, float y3, float z3,
    //    float x4, float y4, float z4, float x5, float y5, float z5, ErrorVar errorVar)
    //{

    //    float eta = 0.005f;


    //    ErrorVar resultVar;

    //    float grad0 = errorFunc0(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc0(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc0(x4, y4, z4, x5, y5, z5, errorVar);
    //    float grad1 = errorFunc1(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc1(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc1(x4, y4, z4, x5, y5, z5, errorVar);
    //    float grad2 = errorFunc2(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc2(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc2(x4, y4, z4, x5, y5, z5, errorVar);
    //    float grad3 = errorFunc3(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc3(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc3(x4, y4, z4, x5, y5, z5, errorVar);
    //    float grad4 = errorFunc4(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc4(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc4(x4, y4, z4, x5, y5, z5, errorVar);
    //    float grad5 = errorFunc5(x0, y0, z0, x1, y1, z1, errorVar) + errorFunc5(x2, y2, z2, x3, y3, z3, errorVar) + errorFunc5(x4, y4, z4, x5, y5, z5, errorVar);

    //    // ROTATION VAR
    //    //errorVar.thetaX = errorVar.thetaX - eta * grad0;
    //    //errorVar.thetaY = errorVar.thetaY - eta * grad1;
    //    //errorVar.thetaZ = errorVar.thetaZ - eta * grad2;

    //    float den = 1.0f * Convert.ToSingle(Math.PI);
    //    errorVar.thetaX = (errorVar.thetaX % den) - eta * (grad0 % den);
    //    errorVar.thetaY = (errorVar.thetaY % den) - eta * (grad1 % den);
    //    errorVar.thetaZ = (errorVar.thetaZ % den) - eta * (grad2 % den);

    //    // TRANS VAR
    //    errorVar.transX = errorVar.transX - eta * grad3;
    //    errorVar.transY = errorVar.transY - eta * grad4;
    //    errorVar.transZ = errorVar.transZ - eta * grad5;

    //    // 종료조건 //데이터양이 무한으로 들어오므로 for문 및 종료조건 필요 x
    //    //float tol = 0.001f;
    //    //if ((grad0 < tol) && (grad1 < tol) && (grad2 < tol) && (grad3 < tol) && (grad4 < tol) && (grad5 < tol)) break;

    //    resultVar = errorVar;

    //    return resultVar;

    //}
    //Call Variables of Rotation and Translation and initialize
}