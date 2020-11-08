using UnityEngine;
using System.Collections;

public class Polynomial : Formula {
	//The 0 is the constant with the coordinate, the rest are the
	//Ever Z^n
	float[] coeficients;

	public Polynomial(float[] Coeficients)
	{
		coeficients = Coeficients;
	}

	public HyperComplex function(HyperComplex Z, HyperComplex C)
	{
		HyperComplex toReturn = coeficients[0]*C;
		HyperComplex ZYet = Z;
		for (int i = 1; i < coeficients.Length; i++)
		{
			toReturn = toReturn + coeficients[i]*ZYet;
			ZYet = ZYet*Z;
		}
		return toReturn;
	}
}
