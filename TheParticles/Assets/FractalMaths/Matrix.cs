using UnityEngine;
using System.Collections;
/**
 * I found unity's built in Matrices to be lacking,
 * so I hobbled together my own implementation.
 **/
public struct Matrix{
	//The actual values.
	public float[,] entry;

	//The matrix height
	public int m
	{
		get 
		{
			return entry.GetLength(0);
		}
	}
	//The matrix width
	public int n
	{
		get 
		{
			return entry.GetLength(1);
		}
	}
	//Constructor taking just a height and width.
	public Matrix(int m, int n) : this(new float[1,1]{{0}}, m, n){}

	//Constructor that just takes the entries straight into a matrix.
	public Matrix(float[,] entries) : this(entries, entries.GetLength(0), entries.GetLength(1)){}

	//Constructor that makes a matrix with width and height.
	//If a spot has the correspondiong entry, it is filled with it,
	//0 otherwise.
	public Matrix(float[,] entries, int width, int height)
	{
		entry = new float[width, height];
		int toFillX = Mathf.Min(width, entries.GetLength(0));
		int toFillY = Mathf.Min(height, entries.GetLength(1));
		for (int i = 0; i < toFillX; i++)
		{
			for (int j = 0; j < toFillY; j++)
			{
				entry[i,j] = entries[i,j];
			}
		}
	}
	//Makes a new matrix where entryNew[i,j] = entryOld[j, i].
	//Obviously mNew = nOld and nOld = mNew.
	public Matrix Transpose()
	{
		float[,] forCopy = new float[n,m];
		for (int i = 0; i < m; i++)
		{
			for (int j = 0; j < n; j++)
			{
				forCopy[j,i] = entry[i,j];
			}
		}
		return new Matrix(forCopy);
	}

	//Makes into Unities Built in Matrix4x4. good for Interfacing with
	//Compute shaders and built in unity.
	public Matrix4x4 toBuiltIn()
	{
		if (m < 4 || n < 4) throw new UnityException("Cannot turn Matrix into 4x4");
		Matrix4x4 toReturn = new Matrix4x4();
		toReturn.m00 = entry[0, 0];
		toReturn.m01 = entry[0, 1];
		toReturn.m02 = entry[0, 2];
		toReturn.m03 = entry[0, 3];
		toReturn.m10 = entry[1, 0];
		toReturn.m11 = entry[1, 1];
		toReturn.m12 = entry[1, 2];
		toReturn.m13 = entry[1, 3];
		toReturn.m20 = entry[2, 0];
		toReturn.m21 = entry[2, 1];
		toReturn.m22 = entry[2, 2];
		toReturn.m23 = entry[2, 3];
		toReturn.m30 = entry[3, 0];
		toReturn.m31 = entry[3, 1];
		toReturn.m32 = entry[3, 2];
		toReturn.m33 = entry[3, 3];
		return toReturn;
	}

	//Makes a new matrix missing a specific row and column.
	//Good for finding determinants and inversions.
	public Matrix complimentMatrix(int skipX, int skipY)
	{
		float[,] forCopy = new float[m,n];
		for (int i = 0; i < skipX; i++)
		{
			for (int j = 0; j < skipY; j++)
			{
				forCopy[i,j] = entry[i,j];
			}
			for (int j = skipY + 1; j < n; j++)
			{
				forCopy[i,j] = entry[i,j];
			}
		}
		for (int i = skipX + 1; i < m; i++)
		{
			for (int j = 0; j < skipY; j++)
			{
				forCopy[i,j] = entry[i,j];
			}
			for (int j = skipY + 1; j < n; j++)
			{
				forCopy[i,j] = entry[i,j];
			}
		}
		return new Matrix(forCopy);
	}

	//Used to find solvability, inversions,
	//and other nifty information about the matrix.
	public float determinant()
	{
		if (m == 1 || n == 1)
		{
			return entry[0, 0];
		}
		else
		{
			float sumSoFar = 0;
			float neg1ToN = -1;
			for (int i = 0; i < m; i++)
			{
				neg1ToN *= -1;
				sumSoFar += neg1ToN*complimentMatrix(i, 0).determinant();
			}
			return sumSoFar;
		}
	}
	//Makes the matrix orthoNormal, unless a Row is all Zeros.
	public Matrix normalize()
	{
		float[,] tempMatrix = new float[m,n];
		for (int j = 0; j < n; j++)
		{
			for (int i = 0; i < m; i++)
			{
				tempMatrix[i, j] = entry[i,j];
			}

			for (int k = 0; k < j; k++)
			{
				float dot = 0;

				for (int i = 0; i < m; i++)
				{
					dot += tempMatrix[i, k]*tempMatrix[i, j];
				}
				for (int i = 0; i < m; i++)
				{
					tempMatrix[i, j] += -tempMatrix[i, k]*dot;
				}
			}

			float columnMag = 0;
			for (int i = 0; i < m; i++)
			{
				columnMag += tempMatrix[i, j]*tempMatrix[i, j];
			}
			if (columnMag == 0)
			{
				tempMatrix[0, j] = 1;
			}
			else
			{
				columnMag = Mathf.Sqrt(columnMag);
				for (int i = 0; i < m; i++)
				{
					tempMatrix[i, j] = tempMatrix[i,j]/columnMag;
				}
			}
		}
		return new Matrix(tempMatrix);
	}

	//Multiplies and then sums all entries[i, j] of the two matrices together.
	public static float dot( Matrix A, Matrix B)
	{
		int minY = Mathf.Min(A.m, B.m);
		int minX = Mathf.Min(A.n, B.n);
		float product = 0;
		for (int i = 0; i < minY; i++)
		{
			for (int j = 0; j < minX; j++)
			{
				product += A.entry[i, j]*B.entry[i, j];
			}
		}
		return product;
	}

	//Matrix addition, every entry is summed.
	public static Matrix operator +(Matrix A, Matrix B)
	{
		int maxN = Mathf.Max (A.n, B.n);

		int maxM = Mathf.Max (A.m, B.m);

		float[,] forNewMatrix = new float[maxM, maxN];

		for (int i = 0; i < A.m; i++)
		{
			for (int j = 0; j < A.n; j++)
			{
				forNewMatrix[i, j] = A.entry[i, j];
			}
		}

		for (int i = 0; i < B.m; i++)
		{
			for (int j = 0; j < B.n; j++)
			{
				forNewMatrix[i, j] += B.entry[i, j];
			}
		}

		return new Matrix(forNewMatrix);
	}

	//Matrix multiplication, Look it up. Hard to explain Quickly.
	public static Matrix operator *(Matrix A, Matrix B)
	{	
		float[,] forNewMatrix = new float[A.m, B.n];
		
		for (int i = 0; i < A.m; i++)
		{
			for (int j = 0; j < B.n; j++)
			{
				int min = Mathf.Min(A.n, B.m);
				float result = 0;
				for (int k = 0; k < min; k++)
				{
					result += A.entry[i, k]*B.entry[k, j];
				}
				forNewMatrix[i, j] = result;
			}
		}
		
		return new Matrix(forNewMatrix);
	}

	//Scaler multiplication, multiplys every entry by the scaler, k.
	public static Matrix operator *(float k, Matrix A)
	{	
		float[,] forNewMatrix = new float[A.m, A.n];
		
		for (int i = 0; i < A.m; i++)
		{
			for (int j = 0; j < A.n; j++)
			{
				forNewMatrix[i, j] = A.entry[i, j]*k;
			}
		}
		
		return new Matrix(forNewMatrix);
	}
	//Scaler multiplication, multiplys every entry by the scaler, k.
	public static Matrix operator *(Matrix A, float k)
	{	
		float[,] forNewMatrix = new float[A.m, A.n];
		
		for (int i = 0; i < A.m; i++)
		{
			for (int j = 0; j < A.n; j++)
			{
				forNewMatrix[i, j] = A.entry[i, j]*k;
			}
		}
		
		return new Matrix(forNewMatrix);
	}

	//Prints a badly formatted version of the matrix. Used mostly for unit testing.
	public override string ToString ()
	{
		string output = "{";
		for (int i = 0; i < m; i++)
		{
			output += "(";
			for (int j = 0; j < n; j++)
			{
				output += entry[i, j].ToString() + ", ";
			}
			output += "), ";
		}
		output += "}";
		return output;
	}
}