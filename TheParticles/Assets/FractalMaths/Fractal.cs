using UnityEngine;
using System.Collections;
/**
 * Handles calling and getting from the compute buffer for all fractal generation.
 * 
 * @author Joshua alan Cook (jacook7@ncsu.edu)
 **/
public class Fractal : MonoBehaviour {
	
	public ComputeShader BufferSet;
	public ComputeShader Polynomial;
	public ComputeShader FormEdge;
	public ComputeShader GetNormal;
	public ComputeShader BlurNorms;

	public Vector4[] ConstantBuffer;
	public float[] OutputBuffer;
	public Vector3[] NormalBuffer;

	private ComputeBuffer constantBuffer;
	private ComputeBuffer workingBuffer;
	private ComputeBuffer outputBuffer;
	private ComputeBuffer normalBuffer;
	private ComputeBuffer tempBuffer;
	private ComputeBuffer ruleSet;
	private int CSPolynomial;
	private int CSBufferSet;
	private int CSFormEdge;
	private int CSGetNormal;
	private int CSBlurNorms;

	void Start()
	{
		//finds the kernel and give the data for the rules.
		CSBufferSet = BufferSet.FindKernel("CSBufferSet");
		CSPolynomial = Polynomial.FindKernel("CSPolynomial");
		CSFormEdge = FormEdge.FindKernel("CSFormEdge");
		CSGetNormal = GetNormal.FindKernel("CSGetNormal");
		CSBlurNorms = BlurNorms.FindKernel("CSBlurNorms");
	}

	/**
	 * Use this to set up for a fractal with new rules.
	 **/
	public void SetUp(int size, HyperRules ruleset, float[] coeficients, int iterations, float isJulia, Vector4 juliaC)
	{
		CleanUp();
		OutputBuffer = new float[size*size*size];
		ConstantBuffer = new Vector4[OutputBuffer.Length];
		NormalBuffer = new Vector3[OutputBuffer.Length];
		
		outputBuffer = new ComputeBuffer(OutputBuffer.Length, 4);
		workingBuffer = new ComputeBuffer(OutputBuffer.Length, 4);
		constantBuffer = new ComputeBuffer(OutputBuffer.Length, 16);
		normalBuffer = new ComputeBuffer(OutputBuffer.Length, 12);
		tempBuffer = new ComputeBuffer(OutputBuffer.Length, 12);

		ruleSet = new ComputeBuffer(1, 256);
		ruleSet.SetData(ruleset.myRules);
		
		Polynomial.SetBuffer(CSPolynomial, "ruleSet", ruleSet);
		
		//Sets the Polynomial Coefficients
		Polynomial.SetFloat("constantCo", coeficients[0]);
		Polynomial.SetFloat("Zto1Co", coeficients[1]);
		Polynomial.SetFloat("Zto2Co", coeficients[2]);
		Polynomial.SetFloat("Zto3Co", coeficients[3]);
		Polynomial.SetFloat("Zto4Co", coeficients[4]);

		//Sets Polynomials Julia set parameters
		Polynomial.SetFloat ("isJulia", isJulia);
		Polynomial.SetVector ("juliaC", juliaC);

		Polynomial.SetInt("iterations", iterations);

		BufferSet.SetInt("size", size);
		FormEdge.SetInt("size", size);
		GetNormal.SetInt("size", size);
		BlurNorms.SetInt("size", size);
		//puts the buffers in
		BufferSet.SetBuffer(CSBufferSet, "Constant", constantBuffer);
		
		Polynomial.SetBuffer(CSPolynomial, "Constant", constantBuffer);
		Polynomial.SetBuffer(CSPolynomial, "Working", workingBuffer);
		
		FormEdge.SetBuffer(CSFormEdge, "Working", workingBuffer);
		FormEdge.SetBuffer(CSFormEdge, "Output", outputBuffer);

		GetNormal.SetBuffer(CSGetNormal, "Working", workingBuffer);
		GetNormal.SetBuffer(CSGetNormal, "Output", outputBuffer);
		GetNormal.SetBuffer(CSGetNormal, "vertNorm", normalBuffer);

		BlurNorms.SetBuffer(CSBlurNorms, "Output", outputBuffer);
		BlurNorms.SetBuffer(CSBlurNorms, "vertNormStart", normalBuffer);
		BlurNorms.SetBuffer(CSBlurNorms, "vertNormResult", tempBuffer);
	}
	
	/**
	 * Renders with last set settings from Initials to finals.
	 **/
	public void StartRender(Vector4 Xinitial, Vector4 Xfinal, Vector4 Yinitial, Vector4 Yfinal, Vector4 Zinitial, Vector4 Zfinal)
	{
		BufferSet.SetFloats("XInit", Xinitial.x, Xinitial.y, Xinitial.z, Xinitial.w);
		BufferSet.SetFloats("XFin", Xfinal.x, Xfinal.y, Xfinal.z, Xfinal.w);
		BufferSet.SetFloats("YInit", Yinitial.x, Yinitial.y, Yinitial.z, Yinitial.w);
		BufferSet.SetFloats("YFin", Yfinal.x, Yfinal.y, Yfinal.z, Yfinal.w);
		BufferSet.SetFloats("ZInit", Zinitial.x, Zinitial.y, Zinitial.z, Zinitial.w);
		BufferSet.SetFloats("ZFin", Zfinal.x, Zfinal.y, Zfinal.z, Zfinal.w);

		BufferSet.Dispatch(CSBufferSet, OutputBuffer.Length, 1, 1);
		Polynomial.Dispatch(CSPolynomial, OutputBuffer.Length, 1,1);
		FormEdge.Dispatch(CSFormEdge, OutputBuffer.Length, 1, 1);
		GetNormal.Dispatch(CSGetNormal, OutputBuffer.Length, 1, 1);
		BlurNorms.Dispatch(CSBlurNorms, OutputBuffer.Length, 1, 1);
		BlurNorms.SetBuffer(CSBlurNorms, "vertNormStart", tempBuffer);
		BlurNorms.SetBuffer(CSBlurNorms, "vertNormResult", normalBuffer);
		BlurNorms.Dispatch(CSBlurNorms, OutputBuffer.Length, 1, 1);
		BlurNorms.SetBuffer(CSBlurNorms, "vertNormStart", normalBuffer);
		BlurNorms.SetBuffer(CSBlurNorms, "vertNormResult", tempBuffer);
	}
	
	public void RetrieveRender()
	{
		outputBuffer.GetData(OutputBuffer);
		constantBuffer.GetData(ConstantBuffer);
		tempBuffer.GetData(NormalBuffer);
	}
	
	void CleanUp()
	{
		if (constantBuffer != null)
		{
			constantBuffer.Release();
			outputBuffer.Release();
			workingBuffer.Release();
			normalBuffer.Release();
			tempBuffer.Release();
			ruleSet.Release();
		}
	}
	
	
	void OnDestroy()
	{
		CleanUp();
	}

}
