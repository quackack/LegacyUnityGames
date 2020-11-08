using UnityEngine;
using System.Collections;

/**
 * This is the formula that will be used to render the fractal.
 **/
public class Recursor {

	public Formula algorithm;

	public Recursor(Formula me)
	{
		algorithm = me;
	}

	//Runs a pass and returns some useful Data.
	public HyperComplex runIterations (HyperComplex C, int iterations, out int escapeTime, out float variation)
	{
		HyperComplex Z = C;
		variation = 0;
		escapeTime = -1;
		for (int i = 0; i < iterations; i++)
		{
			HyperComplex newValue = algorithm.function(Z, C);
			if (escapeTime >= 0)
			{
				if (newValue.MagnitudeSquared() < 2)
				{
					escapeTime = -1;
				}
			}
			else
			{
				if (newValue.MagnitudeSquared() > 2)
				{
					escapeTime = i;
				}
			}
			variation += (Z + -1*newValue).MagnitudeSquared();

			Z = newValue;
		}
		return Z;
	}
}
