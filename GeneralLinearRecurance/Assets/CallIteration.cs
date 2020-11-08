using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallIteration : MonoBehaviour {
    public ComputeShader Iterator;
    ComputeBuffer Result;
    int kernelIndex;
    const string kernelName = "CSMain";

    public int dimension = 32;

    public Vector4 xUnit = new Vector4(4, 0, 0, 0);
    public Vector4 yUnit = new Vector4(0, 4, 0, 0);
    public Vector4 zUnit = new Vector4(0, 0, 4, 0);
    public Vector4 average = new Vector4(-2, -2, -2, 0);

    public int iterationCount = 32;

    public float cutOffSqr;

    public IterationSetter.RuleSet multRule = IterationSetter.Quaternions;

    public IterationSetter.Polynomial poly;

    public void init()
    {
        kernelIndex = Iterator.FindKernel(kernelName);
        Iterator.SetFloats("xUnit", vector4ToFloat(xUnit));
        Iterator.SetFloats("yUnit", vector4ToFloat(yUnit));
        Iterator.SetFloats("zUnit", vector4ToFloat(zUnit));
        Iterator.SetFloats("Start", vector4ToFloat(average));
        Iterator.SetInt("iterationCount", iterationCount);
        Iterator.SetInt("dimension", dimension);

        Iterator.SetFloats("Hresult", matrix4x4ToFloat(multRule.Hresult));
        Iterator.SetFloats("Iresult", matrix4x4ToFloat(multRule.Iresult));
        Iterator.SetFloats("Jresult", matrix4x4ToFloat(multRule.Jresult));
        Iterator.SetFloats("Kresult", matrix4x4ToFloat(multRule.Kresult));

        Iterator.SetFloats("co0", vector4ToFloat(poly.co0));
        Iterator.SetFloats("co1", vector4ToFloat(poly.co1));
        Iterator.SetFloats("co2", vector4ToFloat(poly.co2));
        Iterator.SetFloats("co3", vector4ToFloat(poly.co3));

        Result = new ComputeBuffer(dimension * dimension * dimension, sizeof(float));
    }

    public void run(Vector3 origin, Vector3 direction, float[,,] results)
    {
        Iterator.SetFloats("xUnit", vector4ToFloat(xUnit*direction.x/dimension));
        Iterator.SetFloats("yUnit", vector4ToFloat(yUnit*direction.y / dimension));
        Iterator.SetFloats("zUnit", vector4ToFloat(zUnit*direction.z / dimension));
        Iterator.SetFloats("Start", vector4ToFloat(average + origin.x*xUnit + 
            origin.y*yUnit + origin.z*zUnit));
        Iterator.SetBuffer(kernelIndex, "Result", Result);
        Iterator.Dispatch(kernelIndex, dimension/4, dimension/4, dimension/4);
        Result.GetData(results);
    }

    public float[] makeResultBuffer()
    {
        return new float[dimension * dimension * dimension];
    }

    float[] matrix4x4ToFloat(Matrix4x4 value)
    {
        return new float[]
        {value.m00, value.m01, value.m02, value.m03,
        value.m10, value.m11, value.m12, value.m13,
        value.m20, value.m21, value.m22, value.m23,
        value.m30, value.m31, value.m32, value.m33};
    }

    float[] vector4ToFloat(Vector4 value)
    {
        return new float[] { value.x, value.y, value.z, value.w };
    }
}
