using UnityEngine;
using System.Collections;
/**
 * The rules that a hypercomplex number system will follow.
 * 
 * The basic premise of a system is simple,
 * a complex number is of the form al + bi +cj + dk.
 * 
 * Addition follows the obvious rules, but the multiplication rules are
 * less clear. The main idea is to try to make distributive rules
 * for the multiplication of the complex components.
 * 
 * That is, for specific x, y element of {l, i, j, k}, there is a specific
 * a, b, c, d such that x*y = al + bi + cj + dk. Note, y*x != x*y.
 * The real components by x and y are multiplied together.
 * 
 * This specific implemnentation is meant to be offloaded to the GPU,
 * this is just a CPU representation.
 * 
 * Let us consider a vector space, as for all intents an purposes this is,
 * with m orthonormal basis vectors, in this case h, i, j, and k.
 * Lets call these basis ri where 0<i<=m
 * 
 * Then for all u, all v in this vector space:
 * u = Sum(k from 1 to m)(Ak*rk)
 * v = Sum(l from 1 to m)(Bl*rl)
 * 
 * Then u*v = Sum(k from 1 to m)(Ak*rk)*Sum(l from 1 to m)(Bl*rl)
 * 
 * Because the space is distributive,
 * u*v = Sum(k from 1 to m)(Ak*rk*Sum(l from 1 to m)(Bl*rl))
 * u*v = Sum(k from 1 to m)Sum(l from 1 to m)(Ak*rk*(Bl*rl))
 * u*v = Sum(k from 1 to m)Sum(l from 1 to m)(Ak*Bl*rk*rl))
 * 
 * Because the vector space is spanned by m basis vectors,
 * each rk*rl = Sum(n from 1 to m)C(k, l, n)*rn
 * 
 * Therefore:
 * u*v = Sum(k from 1 to m)Sum(l from 1 to m)(Ak*Bl*Sum(n from 1 to m)C(k, l, n)*rn)
 * u*v = Sum(k from 1 to m)Sum(l from 1 to m)Sum(n from 1 to m)(C(k, l, n)*Ak*Bl*rn)
 * u*v = Sum(n from 1 to m)[  Sum(k from 1 to m)Sum(l from 1 to m)(C(k, l, n)*Ak*Bl)  ]*rn
 * 
 * Notice that C is indexed by k, l, and n, so C has m^3 possible values.
 * This can be conveniently be visualized as a cube of values,
 * similar to a third order tensor.
 * Notice that each nth entry in the resulting vector comes from
 * one plane in the cube.
 * 
 * We can visualize another matrix of values created by the
 * products of Ak and Bl, such that M(k, l) = Ak*Bl.
 * 
 * Then un is just the dot product of M and C indexed at n.
 * 
 * Graphics cards are not built to do this sort of matrix dot,
 * so the actual code is different, but the representation the same.
 * 
 * We do matrix muliplications to multiply by the Ak first.
 * THen form a new matrix to do the Bl matrix.
 **/
public class HyperRules{
	/**
	 * The matrix to multiply the first complex number by to
	 * get the first row of matrix elements of the matrix
	 * to multiply c2 by.
	 * 
	 * This should be a four by four matrix.
	 **/
	public Matrix hCoefficients;
	/**
	 * The matrix to multiply the first complex number by to
	 * get the second row of matrix elements of the matrix
	 * to multiply c2 by.
	 * 
	 * This should be a four by four matrix.
	 **/
	public Matrix iCoefficients;
	/**
	 * The matrix to multiply the first complex number by to
	 * get the third row of matrix elements of the matrix
	 * to multiply c2 by.
	 * 
	 * This should be a four by four matrix.
	 **/
	public Matrix jCoefficients;
	/**
	 * The matrix to multiply the first complex number by to
	 * get the fourth row of matrix elements of the matrix
	 * to multiply c2 by.
	 * 
	 * This should be a four by four matrix.
	 **/
	public Matrix kCoefficients;

	/**
	 * this is a struct generated to allow offloading to GPU
	 **/
	//Number of bytes = 2^2(bytes floats) * 2^4(floats in matrix) * 2^2 (number of matrices) = 2^8 = 256.
	public struct ruleMatrices
	{
		/**
		 * same as hCoefficients
		 **/
		public Matrix4x4 hC;
		/**
		 * same as iCoefficients
		 **/
		public Matrix4x4 iC;
		/**
		 * same as jCoefficients
		 **/
		public Matrix4x4 jC;
		/**
		 * same as kCoefficients
		 **/
		public Matrix4x4 kC;

		public ruleMatrices(Matrix hCoefficients, Matrix iCoefficients, Matrix jCoefficients, Matrix kCoefficients)
		{
			hC = hCoefficients.toBuiltIn();
			iC = iCoefficients.toBuiltIn();
			jC = jCoefficients.toBuiltIn();
			kC = kCoefficients.toBuiltIn();
		}
	}

	public ruleMatrices[] myRules
	{
		get
		{
			return new ruleMatrices[]{new ruleMatrices(hCoefficients, iCoefficients, jCoefficients, kCoefficients)};
		}
	}

	private static HyperRules Standard;

	private static HyperRules Alternate;

	private static HyperRules Shifter;

	/**
	 * Quaternionic rules are that i*i = j*j = k*k = i*j*k = -1
	 * 								j*k = -k*j = i
	 * 								k*i = -i*k = j
	 * 								i*j = -j*i = k
	 **/
	public static HyperRules Quaternionic
	{
		get
		{
			if (Standard == null)
			{
				Standard = new HyperRules(
					new Matrix(new float[4,4]{	{1, 0, 0, 0},
												{0, -1, 0, 0}, 
												{0, 0, -1, 0}, 
												{0, 0, 0, -1}}),
					new Matrix(new float[4,4]{	{0, 1, 0, 0},
												{1, 0, 0, 0}, 
												{0, 0, 0, 1}, 
												{0, 0, -1, 0}}),
					new Matrix(new float[4,4]{	{0, 0, 1, 0},
												{0, 0, 0, -1}, 
												{1, 0, 0, 0}, 
												{0, 1, 0, 0}}),
					new Matrix(new float[4,4]{	{0, 0, 0, 1},
												{0, 0, 1, 0}, 
												{0, -1, 0, 0}, 
												{1, 0, 0, 0}}));
			}
			return Standard;
		}
	}

	public static HyperRules BiComplex
	{
		get
		{
			if (Alternate == null)
			{
				Alternate = new HyperRules(
					new Matrix(new float[4,4]{	{1, 0, 0, 0},
												{0, -1, 0, 0}, 
												{0, 0, -1, 0}, 
												{0, 0, 0, 1}}),
					new Matrix(new float[4,4]{	{0, 1, 0, 0},
												{1, 0, 0, 0}, 
												{0, 0, 0, -1}, 
												{0, 0, -1, 0}}),
					new Matrix(new float[4,4]{	{0, 0, 1, 0},
												{0, 0, 0, -1}, 
												{1, 0, 0, 0}, 
												{0, -1, 0, 0}}),
					new Matrix(new float[4,4]{	{0, 0, 0, 1},
												{0, 0, 1, 0}, 
												{0, 1, 0, 0}, 
												{1, 0, 0, 0}}));
			}
			return Alternate;
		}
	}

	public static HyperRules Shift
	{
		get
		{
			if (Shifter == null)
			{
				Shifter = new HyperRules(
					new Matrix(new float[4,4]{	{1, 0, 0, 0},
												{0, 0, 0, 1}, 
												{0, 0, 1, 0}, 
												{0, 1, 0, 0}}),
					new Matrix(new float[4,4]{	{0, 1, 0, 0},
												{1, 0, 0, 0}, 
												{0, 0, 0, 1}, 
												{0, 0, 1, 0}}),
					new Matrix(new float[4,4]{	{0, 0, 1, 0},
												{0, 1, 0, 0}, 
												{1, 0, 0, 0}, 
												{0, 0, 0, 1}}),
					new Matrix(new float[4,4]{	{0, 0, 0, 1},
												{0, 0, 1, 0}, 
												{0, 1, 0, 0}, 
												{1, 0, 0, 0}}));
			}
			return Shifter;
		}
	}

	public HyperRules(Matrix hC, Matrix iC, Matrix jC, Matrix kC)
	{
		hCoefficients = hC;//.normalize();
		iCoefficients = iC;//.normalize();
		jCoefficients = jC;//.normalize();
		kCoefficients = kC;//.normalize();
	}
}