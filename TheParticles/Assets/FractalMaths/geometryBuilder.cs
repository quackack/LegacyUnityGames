using UnityEngine;
using System.Collections;
/**
 * Builds the geometry for a scene
 **/
public class geometryBuilder
{
	private bool finished = true;
	public bool isFinished
	{
		get
		{
			return finished;
		}
	}
	private int size;

	private float[] vertexRaw;
	private Vector3[] rawNorm;
	private int[] vertIndex;
	private Vector4[] rawColors;

	private Vector3[] normal;

	private Vector3[] Vertices;

	private int[] triangles;
	System.Collections.Generic.List<int> toCollectTris;

	private Color[] colors;

	public Vector2[] UVs;

	System.Collections.Generic.List<Vector3> toCollectNorms;
	System.Collections.Generic.List<Vector3> toCollectVerts;
	System.Collections.Generic.List<Color> toCollectColors;
	System.Collections.Generic.List<Vector2> toCollectUVs;

	public void makeMeshFor(float[] geometry, Vector3[] normals, Vector4[] colors, int size)
	{
		if (isFinished)
		{
			this.size = size;
			finished = false;
			vertexRaw = new float[geometry.Length];
			rawNorm = new Vector3[normals.Length];
			rawColors = new Vector4[colors.Length];
			normals.CopyTo(rawNorm, 0);
			geometry.CopyTo(vertexRaw, 0);
			colors.CopyTo(rawColors, 0);
			System.Threading.Thread generateProcess = new System.Threading.Thread(generateWorld);
			generateProcess.Start();
			//generateWorld();
		}
	}

	void generateWorld()
	{
		makeVertices();
		createTriangles();
		finished = true;
	}

	public Mesh product()
	{
		if (isFinished)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = Vertices;
			mesh.triangles = triangles;
			mesh.normals = normal;
			mesh.colors = colors;
			mesh.uv = UVs;
			return mesh;
		}
		else
		{
			return null;
		}
	}

	void addVertex(int index, int i, int j, int k, float boundary) {
		float normFactor = (1f/(size-3));
		if (vertexRaw[index] > boundary)
		{
			vertIndex[index] = toCollectVerts.Count;
			if (k == 1 || k == size - 2 || j == 1 || j == size - 2 || i == 1 || i == size - 2)
			{
				toCollectVerts.Add(new Vector3(i-1, j-1, k-1)*normFactor - new Vector3(0.5f, 0.5f, 0.5f));
			}
			else
			{
				toCollectVerts.Add(new Vector3(i-1, j-1, k-1)*normFactor - new Vector3(0.5f, 0.5f, 0.5f) - rawNorm[index]*0.25f*normFactor);
			}
			toCollectUVs.Add(new Vector2(toCollectVerts[toCollectVerts.Count-1].x,toCollectVerts[toCollectVerts.Count-1].y));
			toCollectNorms.Add(rawNorm[index]);
			toCollectColors.Add (new Color(rawColors[index].x, rawColors[index].y, rawColors[index].z, rawColors[index].w));
		} else {
			vertIndex[index] = -1;
		}
	}

	void makeVertices()
	{
		toCollectNorms = new System.Collections.Generic.List<Vector3>();
		toCollectVerts = new System.Collections.Generic.List<Vector3>();
		toCollectColors = new System.Collections.Generic.List<Color>();
		toCollectUVs = new System.Collections.Generic.List<Vector2>();
		vertIndex = new int[vertexRaw.Length];
		for (int i = 1; i < size-1;i++)
		{
			int row = i;
			for (int j = 1; j < size-1; j++)
			{
				int column = j*size;
				for (int k = 1; k < size-1; k++)
				{
					int depth = k*size*size;
					int index = row+column+depth;
					addVertex(index, i, j, k, 0.5f);
				}
			}
		}
		//Adds sides to hide LOD problems
		for (int n = 1; n < size-1;n++)
		{
			int i = n; int j = 0; int k = 0;
			int index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			i = n; j = size-1; k = 0;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			i = n; j = 0; k = size-1;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			i = n; j = size-1; k = size-1;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;


			i = 0; j = n; k = 0;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			i = size-1; j = n; k = 0;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			i = 0; j = n; k = size-1;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			i = size-1; j = n; k = size-1;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			
			i = 0; j = 0; k = n;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			i = size-1; j = 0; k = n;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			i = 0; j = size-1; k = n;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;
			
			i = size-1; j = size-1; k = n;
			index = i + j*size + k*size*size;
			vertIndex[index] = -1;

			for (int m = 1; m < size-1; m++)
			{
				i = n;
				j = m;
				k = 1;
				index = i + j*size + k*size*size;
				vertIndex[index - size*size] = -1;
				if ( vertexRaw[index] < 0.375f) addVertex(index, i, j, k, 0.125f);

				i = n;
				j = m;
				k = size-2;
				index = i + j*size + k*size*size;
				vertIndex[index + size*size] = -1;
				if ( vertexRaw[index] < 0.375f) addVertex(index, i, j, k, 0.125f);

				i = n;
				j = 1;
				k = m;
				index = i + j*size + k*size*size;
				vertIndex[index - size] = -1;
				if ( vertexRaw[index] < 0.375f) addVertex(index, i, j, k, 0.125f);

				i = n;
				j = size - 2;
				k = m;
				index = i + j*size + k*size*size;
				vertIndex[index + size] = -1;
				if ( vertexRaw[index] < 0.375f) addVertex(index, i, j, k, 0.125f);

				i = 1;
				j = n;
				k = m;
				index = i + j*size + k*size*size;
				vertIndex[index - 1] = -1;
				if ( vertexRaw[index] < 0.375f) addVertex(index, i, j, k, 0.125f);

				i = size-2;
				j = n;
				k = m;
				index = i + j*size + k*size*size;
				vertIndex[index + 1] = -1;
				if ( vertexRaw[index] < 0.375f) addVertex(index, i, j, k, 0.125f);

			}
		}
		normal = toCollectNorms.ToArray();
		Vertices = toCollectVerts.ToArray();
		colors = toCollectColors.ToArray();
		UVs = toCollectUVs.ToArray();
		toCollectNorms.Clear();
		toCollectVerts.Clear();
		toCollectColors.Clear();
		toCollectUVs.Clear();
	}

	void createTriangles()
	{
		toCollectTris = new System.Collections.Generic.List<int>();
		/**
		 * Esenentially make the world into a bunch of discreet cubes, like this.
		 * *---*---*..
		 * | A | B | 
		 * *---*---*..
		 * | C | D |
		 * *---*---*...
		 * :   :   :
		 * 
		 * Than, for each cube, A, make all the reasonable faces.
		 *   
		 **/
		for (int i = 0; i < size - 1;i++)
		{
			int row = i;
			for (int j = 0; j < size - 1; j++)
			{
				int column = j*size;
				for (int k = 0; k < size - 1; k++)
				{
					int depth = k*size*size;
					int a = row+column+depth;
					int x = a + 1;
					int y = a + size;
					int xy = a + size + 1;
					int z = a + size*size;
					int xz = z + 1;
					int yz = z + size;
					int xyz = z + size + 1;

					processQuad(a,	x, 	xy, y);
					processQuad(a,	x, 	xz, z);
					processQuad(a,	y,	yz, z);

					processQuad(a,	xy,	xz,	yz); processQuad(xy,	xz,	yz,	a);

					processQuad(x, xyz,	z,	y); processQuad(y, x, xyz,	z);

					processQuad(a,	x, 	xyz, yz);
					processQuad(y,	z,	xz, xy);

					processQuad(a,	y,	xyz, xz);
					processQuad(x,	xy,	yz,	z);

					processQuad(a,	z,	xyz,xy);
					processQuad(x,	y,	yz,	xz);


					/**
					 * Handles some faces in bad order so they may still have holes
					processQuad(a,	x, 	y,		xy);
					processQuad(a,	x, 		z,		xz);
					processQuad(a,	 	y,	z,			yz);

					processQuad(a,	 			xy,	xz,	yz); //Need a yz xz
					processQuad(	x, 	y,	z,				xyz);

					processQuad(a,	x, 					yz,	xyz);
					processQuad(		y,	z,	xy,	xz);

					processQuad(a,		y,			xz,		xyz);
					processQuad(	x,		z,	xy,		yz);

					processQuad(a,			z,	xy,			xyz);
					processQuad(	x,	y,			xz,	yz);

					processQuad(	x,	y,	z,				xyz);
					**/

					/** Original
					 * Has some redundant geometry the next cube sweep will handle.
					processQuad(a,	x, 	y,		xy);
					processQuad(a,	x,					yz,	xyz);
					processQuad(a,	x,		z,		xz);
					processQuad(a,		y,			xz,		xyz);

					processQuad(a,		y,	z,			yz);
					processQuad(a,			z,	xy,			xyz);
					processQuad(	x,	y,			xz,	yz);
					processQuad(	x,		z,	xy,		yz);

					processQuad(	x,			xy, xz, 	xyz);
					processQuad(		y,	z,	xy,	xz);
					processQuad(		y,		xy,		yz, xyz);
					processQuad(			z,		xz,	yz, xyz);


					processQuad(a,				xy, xz, yz);
					processQuad(	x,	y,	z,				xyz);
					**/
				}
			}
		}

		triangles = toCollectTris.ToArray();
	}

	void addTriangle(int a, int b, int c)
	{
		Vector3 aTob = Vertices[vertIndex[b]] - Vertices[vertIndex[a]];
		Vector3 aToc = Vertices[vertIndex[c]] - Vertices[vertIndex[a]];
		Vector3 norm = normal[vertIndex[a]] + normal[vertIndex[b]] + normal[vertIndex[c]];
		float inDir = Vector3.Dot(Vector3.Cross(aTob, aToc), norm);
		if (inDir == 0) {
			toCollectTris.Add(vertIndex[a]); toCollectTris.Add(vertIndex[b]);toCollectTris.Add(vertIndex[c]);
			toCollectTris.Add(vertIndex[a]); toCollectTris.Add(vertIndex[c]);toCollectTris.Add(vertIndex[b]);
		}else if (inDir > 0)
		{
			toCollectTris.Add(vertIndex[a]); toCollectTris.Add(vertIndex[b]);toCollectTris.Add(vertIndex[c]);
		}
		else
		{
			toCollectTris.Add(vertIndex[a]); toCollectTris.Add(vertIndex[c]);toCollectTris.Add(vertIndex[b]);
		}

		//toCollectTris.Add(vertIndex[a]); toCollectTris.Add(vertIndex[b]);toCollectTris.Add(vertIndex[c]);
		//toCollectTris.Add(vertIndex[a]); toCollectTris.Add(vertIndex[c]);toCollectTris.Add(vertIndex[b]);
	}

	void processQuad(int a, int b, int c, int d) //Need a c d
	{
		if (vertIndex[a] >= 0)
		{
			if (vertIndex[b] >= 0)
			{
				if (vertIndex[c] >= 0)
				{
					if (vertIndex[d] >= 0)
					{
						addTriangle(a, b, c);
						addTriangle(a, c, d);

						//addTriangle(a, c, d);
						//addTriangle(a, b, d);
					}
					else
					{
						addTriangle(a, b, c);
					}
				}
				else if (vertIndex[d] >= 0)
				{
					addTriangle(a, b, d);
				}
			}
			else if (vertIndex[c] >= 0 && vertIndex[d] >= 0)
			{
				addTriangle(a, c, d);
			}
		}
		else if(vertIndex[b] >= 0 && vertIndex[c] >= 0 && vertIndex[d] >= 0)
		{
			addTriangle(b, c, d);
		}
	}
}
