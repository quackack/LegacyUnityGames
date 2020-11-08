using UnityEngine;
using System.Collections;
/**
 * Four component Hyper Complex number. Addition
 * is as expected, multiplication comes from HyperRules
 **/
public struct HyperComplex{
	public Matrix values;

	public float h
	{
		get 
		{
			return values.entry[0, 0];
		}
		set
		{
			values.entry[0,0] = value;
		}
	}

	public float i
	{
		get 
		{
			return values.entry[0, 1];
		}
		set
		{
			values.entry[0,1] = value;
		}
	}

	public float j
	{
		get 
		{
			return values.entry[0, 2];
		}
		set
		{
			values.entry[0,2] = value;
		}
	}

	public float k
	{
		get 
		{
			return values.entry[0, 3];
		}
		set
		{
			values.entry[0,3] = value;
		}
	}

	public HyperRules rules;

	public HyperComplex(HyperComplex original) : this(original.values, original.rules) { }
	
	public HyperComplex(float h, float i, float j, float k) : this(h, i, j, k, HyperRules.Quaternionic){}

	public HyperComplex(float h, float i, float j, float k, HyperRules master) : this(new Matrix(new float[1, 4]{{h, i, j, k}}), master){}

	public HyperComplex(Matrix A, HyperRules master)
	{
		values = A;
		this.rules = master;
	}

	public float MagnitudeSquared()
	{
		return Matrix.dot(values, values);
	}

	public float Magnitude()
	{
		return Mathf.Sqrt(MagnitudeSquared());
	}

	public static HyperComplex Normalize(HyperComplex hc)
	{
		return new HyperComplex(hc.values.normalize(), hc.rules);
	}

	public static HyperComplex Pow(HyperComplex hc, int numb)
	{
		HyperComplex toReturn = new HyperComplex(1, 0, 0, 0, hc.rules);
		for (int i = 0; i < numb; i++)
		{
			toReturn *= hc;
		}
		return toReturn;
	}

	/**
	 * Slightly different than ussual Sum(n= 0, infinity)x^n/n!
	 * For non associative systems.
	 */
	public static HyperComplex exp(HyperComplex hc)
	{
		HyperComplex hcToN = hc;
		HyperComplex toReturn = new HyperComplex(1, 0, 0, 0, hc.rules) + hcToN;

		hcToN = 0.5f*hcToN*hc;
		toReturn = toReturn + hcToN;

		hcToN = 0.3333333333333333f*hcToN*hc;
		toReturn = toReturn + hcToN;

		hcToN = 0.25f*hcToN*hc;
		toReturn = toReturn + hcToN;

		hcToN = 0.2f*hcToN*hc;
		toReturn = toReturn + hcToN;

		hcToN = 0.1666666666666666f*hcToN*hc;
		toReturn = toReturn + hcToN;

		hcToN = 0.14285714285714285714f*hcToN*hc;
		toReturn = toReturn + hcToN;

		hcToN = 0.125f*hcToN*hc;
		toReturn = toReturn + hcToN;

		hcToN = 0.125f*hcToN*hc;
		toReturn = toReturn + hcToN;

		hcToN = 0.111111111111111111f*hcToN*hc;
		toReturn = toReturn + hcToN;

		hcToN = 0.1f*hcToN*hc;
		return toReturn + hcToN;
	}

	public static HyperComplex operator +(HyperComplex hc1, HyperComplex hc2)
	{
		return new HyperComplex(hc1.values + hc2.values, hc1.rules);
	}

	public static HyperComplex operator *(HyperComplex hc1, HyperComplex hc2)
	{
		Matrix hC = hc1.values*hc1.rules.hCoefficients;
		Matrix iC = hc1.values*hc1.rules.iCoefficients;
		Matrix jC = hc1.values*hc1.rules.jCoefficients;
		Matrix kC = hc1.values*hc1.rules.kCoefficients;

		Matrix final = new Matrix( new float[4, 4]{	{hC.entry[0,0], hC.entry[0,1], hC.entry[0,2], hC.entry[0,3]},
													{iC.entry[0,0], iC.entry[0,1], iC.entry[0,2], iC.entry[0,3]},
													{jC.entry[0,0], jC.entry[0,1], jC.entry[0,2], jC.entry[0,3]},
													{kC.entry[0,0], kC.entry[0,1], kC.entry[0,2], kC.entry[0,3]}}).Transpose();
		return new HyperComplex(hc2.values*final, hc1.rules);
	}

	public static HyperComplex operator *(float Real, HyperComplex hc1)
	{
		return new HyperComplex(Real*hc1.values, hc1.rules);
	}

	public static HyperComplex operator *(HyperComplex hc1, float Real)
	{
		return new HyperComplex(Real*hc1.values, hc1.rules);
	}

	public override string ToString ()
	{
		return values.ToString();
	}
}
