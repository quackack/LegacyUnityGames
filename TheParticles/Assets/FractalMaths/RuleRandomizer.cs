using UnityEngine;
using System.Collections;

/**
 * Takes a WorldManager as an argument, and randomizes the world it will create
 * 
 **/
public class RuleRandomizer : MonoBehaviour {

	/**
	 * The weight on the odd terms
	 **/
	public float oddness = 1;

	/**
	 * The weight on the even terms
	 **/
	public float evenness = 1;

	/**
	 * The weight on the low order terms, put in range from 0 to 1.
	 * 0.5 no effect.
	 **/
	public float lowness = 1;

	/**
	 * The weight on the high order terms, put in range from 0 to 1.
	 * 0.5 no effect.
	 **/
	public float hiness = 1;

	/**
	 * The amount of the constant.
	 **/
	public float constantness = 1;

	/**
	 * The upper cap of polynomial term randomweights.
	 **/
	public float polyTop = 2;

	/**
	 * The lower cap of polynomial term randomweights
	 **/
	public float polyBottom = -2;

	/**
	 * How square the fractal should be, that is, how much it is like the
	 * bi_complexes it should be. BiComplexes make square fractals.
	 * BiComplexes are commutative.
	 **/
	public float squareness = 0;

	/**
	 * How much to use the ruleset of the shifter
	 * Shiters are commutative and make square shapes. Little else is known.
	 **/
	public float shifter = 0;

	/**
	 * How round the fractal should be, that is, how much it should 
	 * be like the quaternions
	 * 
	 * Quaternions are associative.
	 **/
	public float roundness = 0;



	/**
	 * Boolean that will optionally enforce commutative
	 * properties. That is, each matrix will be symetric.
	 **/
	public bool bCommutative = false;

	/**
	 * How chaotic it is while preserving the unity of one.
	 * (That is 1*i =i, i*1 = i, 1*j = j, ect).
	 **/
	public float varience = 1;

	/**
	 * Total craziness just put on top. That is,
	 * complete randomness.
	 **/
	public float randomness = 0;

	/**
	 * Takes world and randomizes its rule set according
	 * to this objects parameters.
	 **/
	public void randomizeWorld(WorldManager world) {
		world.polynomial = new float[5]{Random.Range(polyBottom, polyTop) * constantness,
										Random.Range(polyBottom, polyTop) * oddness * lowness,
										Random.Range(polyBottom, polyTop) * evenness * lowness,
										Random.Range(polyBottom, polyTop) * oddness * hiness,
										Random.Range(polyBottom, polyTop) * evenness * hiness};
		//Now, we need to normalize world.polynomial

		float magSqr = 0;
		for (int i = 0; i < 5; i++) {magSqr += world.polynomial[i]*world.polynomial[i];}

		if (magSqr < 0.5f) {
			if (magSqr > 0) {
				magSqr = 1 / Mathf.Sqrt (magSqr);
				for (int i = 0; i < 5; i++) {
					world.polynomial [i] = world.polynomial [i] * magSqr;
				}
			} else {
				world.polynomial = new float[5]{0.5f, 0, 0.5f, 0, 0};
			}
		}

		Matrix randH = new Matrix(new float[4,4]{	{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)},
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}, 
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}, 
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}});
		Matrix randI = new Matrix(new float[4,4]{	{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)},
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}, 
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}, 
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}});
		Matrix randJ = new Matrix(new float[4,4]{	{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)},
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}, 
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}, 
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}});
		Matrix randK = new Matrix(new float[4,4]{	{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)},
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}, 
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}, 
													{Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}});
		Matrix varH = new Matrix(new float[4,4]{	{1,						0,					0,						0},
													{0, Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)},
													{0, Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)},
													{0, Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)}});
		Matrix varI = new Matrix(new float[4,4]{	{0,						1,					0,						0},
													{1, 					0, 					0, 						0},
													{0, 					0, Random.Range(-1f, 1f), Random.Range(-1f, 1f)},
													{0, 					0, Random.Range(-1f, 1f), Random.Range(-1f, 1f)}});
		Matrix varJ = new Matrix(new float[4,4]{	{0,						0,					1,						0},
													{0, Random.Range(-1f, 1f), 					0, Random.Range(-1f, 1f)},
													{1, 					0, 					0, 						0},
													{0, Random.Range(-1f, 1f), 					0, Random.Range(-1f, 1f)}});
		Matrix varK = new Matrix(new float[4,4]{	
			{0,						0,					0,						1},
			{0, Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0},
			{0, Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0},
			{1, 					0, 					0, 0}});
		Matrix resH = randH * randomness + varH * varience + HyperRules.Quaternionic.hCoefficients * roundness + 
						HyperRules.BiComplex.hCoefficients * squareness + HyperRules.Shift.hCoefficients * shifter;
		Matrix resI = randI * randomness + varI * varience + HyperRules.Quaternionic.iCoefficients * roundness + 
						HyperRules.BiComplex.iCoefficients * squareness + HyperRules.Shift.iCoefficients * shifter;
		Matrix resJ = randJ * randomness + varJ * varience + HyperRules.Quaternionic.jCoefficients * roundness + 
						HyperRules.BiComplex.jCoefficients * squareness  + HyperRules.Shift.jCoefficients * shifter;
		Matrix resK = randK * randomness + varK * varience + HyperRules.Quaternionic.kCoefficients * roundness + 
						HyperRules.BiComplex.kCoefficients * squareness  + HyperRules.Shift.kCoefficients * shifter;
		if (bCommutative) {
			resH = resH + resH.Transpose();
			resI = resI + resI.Transpose();
			resJ = resJ + resJ.Transpose();
			resK = resK + resK.Transpose();
		}

		world.rules = new HyperRules(resH, resI, resJ, resK);
	}
}
