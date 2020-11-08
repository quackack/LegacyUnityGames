using UnityEngine;
using System.Collections;

public class ShareComputeBuffer : MonoBehaviour {
	public ComputeShader Add1;
	public ComputeShader Make2;

	private ComputeBuffer Data;
	public float[] result;

	// Use this for initialization
	void Start () {
		result = new float[16];
		Data = new ComputeBuffer(result.Length, sizeof(float));

		int CSMain_Make2 = Make2.FindKernel("CSMain");
		int CSMain_Add1 = Add1.FindKernel("CSMain");

		Make2.SetBuffer(CSMain_Make2, "inOut", Data);
		Add1.SetBuffer(CSMain_Add1, "inOut", Data);

		Make2.Dispatch(CSMain_Make2, result.Length, 1, 1);
		Add1.Dispatch(CSMain_Make2, result.Length, 1, 1);
		Make2.Dispatch(CSMain_Make2, result.Length, 1, 1);
		Add1.Dispatch(CSMain_Make2, result.Length, 1, 1);
		Add1.Dispatch(CSMain_Make2, result.Length, 1, 1);

		Data.GetData(result);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
