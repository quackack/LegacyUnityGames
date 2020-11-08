using UnityEngine;
using System.Collections;

public class UnitTester : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		QuaternionicMultiplyTest();
	}

	public void QuaternionicMultiplyTest()
	{
		HyperComplex H = new HyperComplex(1, 0, 0, 0);
		HyperComplex I = new HyperComplex(0, 1, 0, 0);
		HyperComplex J = new HyperComplex(0, 0, 1, 0);
		HyperComplex K = new HyperComplex(0, 0, 0, 1);

		HyperComplex NegH = new HyperComplex(-1, 0, 0, 0);
		HyperComplex NegI = new HyperComplex(0, -1, 0, 0);
		HyperComplex NegJ = new HyperComplex(0, 0, -1, 0);
		HyperComplex NegK = new HyperComplex(0, 0, 0, -1);

		assertEquals(H.ToString(), (H*H).ToString(), StackTraceUtility.ExtractStackTrace());

		assertEquals(I.ToString(), (H*I).ToString(), StackTraceUtility.ExtractStackTrace());
		assertEquals(I.ToString(), (I*H).ToString(), StackTraceUtility.ExtractStackTrace());

		assertEquals(J.ToString(), (H*J).ToString(), StackTraceUtility.ExtractStackTrace());
		assertEquals(J.ToString(), (J*H).ToString(), StackTraceUtility.ExtractStackTrace());

		assertEquals(K.ToString(), (H*K).ToString(), StackTraceUtility.ExtractStackTrace());
		assertEquals(K.ToString(), (K*H).ToString(), StackTraceUtility.ExtractStackTrace());

		assertEquals(NegH.ToString(), (I*I).ToString(), StackTraceUtility.ExtractStackTrace());
		assertEquals(NegH.ToString(), (J*J).ToString(), StackTraceUtility.ExtractStackTrace());
		assertEquals(NegH.ToString(), (K*K).ToString(), StackTraceUtility.ExtractStackTrace());

		assertEquals(K.ToString(), (I*J).ToString(), StackTraceUtility.ExtractStackTrace());
		assertEquals(NegK.ToString(), (J*I).ToString(), StackTraceUtility.ExtractStackTrace());

		assertEquals(I.ToString(), (J*K).ToString(), StackTraceUtility.ExtractStackTrace());
		assertEquals(NegI.ToString(), (K*J).ToString(), StackTraceUtility.ExtractStackTrace());

		assertEquals(J.ToString(), (K*I).ToString(), StackTraceUtility.ExtractStackTrace());
		assertEquals(NegJ.ToString(), (I*K).ToString(), StackTraceUtility.ExtractStackTrace());

		assertEquals(NegH.ToString(), (I*J*K).ToString(), StackTraceUtility.ExtractStackTrace());
	}

	public static void assertEquals(string A, string B, string location)
	{
		if (!A.Equals(B))
		{
			print ("Expected: " + A.ToString() + ", but got: " + B.ToString() + ", at " + location);
		}
	}
}
