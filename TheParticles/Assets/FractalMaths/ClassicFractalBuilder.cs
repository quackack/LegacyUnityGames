using UnityEngine;
using System.Collections;

public class ClassicFractalBuilder : MonoBehaviour {
	public Transform blank;
	public int SectionSize = 16;
	public int SectionNumb = 16;
	public int iterations = 10;
	public Vector3 min;
	public Vector3 max;

	public Fractal fractal;

	private geometryBuilder builder;

	private int xSection;
	private int ySection;
	private int zSection;

	//GpGPu Solution
	void Start () 
	{
		fractal.SetUp(SectionSize, HyperRules.Quaternionic, new float[]{1, 0, 1, 0, 0}, iterations, 0, Vector4.zero);
		builder = new geometryBuilder();

		xSection = 0;
		ySection = 0;
		zSection = 0;

		fractal.StartRender(new Vector4(min.x, 0, 0, 0), new Vector4(Mathf.Lerp(min.x, max.x, (xSection + 1)/(float)SectionNumb), 0, 0, 0), 
		                    new Vector4(0, min.y, 0, 0), new Vector4(0, Mathf.Lerp(min.y, max.y, (ySection + 1)/(float)SectionNumb), 0, 0),
		                    new Vector4(0, 0, min.z, 0), new Vector4(0, 0, Mathf.Lerp(min.z, max.z, (zSection + 1)/(float)SectionNumb), 0));
	}

	void Update()
	{
		if (ySection < SectionNumb && xSection < SectionNumb && zSection < SectionNumb)
		{
			if (builder.isFinished)
			{
				Mesh mesh = builder.product();
				if (mesh != null)
				{
					Transform newbie = Instantiate(blank, new Vector3(xSection*SectionSize, ySection*SectionSize, zSection*SectionSize), Quaternion.identity) as Transform;
					newbie.GetComponent<MeshFilter>().mesh = mesh;
				}
				UpdateImage();
				builder.makeMeshFor(fractal.OutputBuffer, fractal.NormalBuffer, fractal.ConstantBuffer, SectionSize);
			}
		}
	}

	void UpdateImage()
	{
		fractal.RetrieveRender();

		xSection++;
		if (xSection == SectionNumb)
		{
			xSection = 0;
			ySection++;
			if (ySection == SectionNumb)
			{
				ySection = 0;
				zSection++;
			}
		}
		if (ySection < SectionNumb || xSection < SectionNumb || zSection < SectionNumb)
		{
			fractal.StartRender(new Vector4(Mathf.Lerp(min.x, max.x, xSection/(float)SectionNumb), 0, 0, 0), 
			                    new Vector4(Mathf.Lerp(min.x, max.x, (xSection + 1)/(float)SectionNumb), 0, 0, 0), 
			                    new Vector4(0, Mathf.Lerp(min.y, max.y, ySection/(float)SectionNumb), 0, 0), 
			                    new Vector4(0, Mathf.Lerp(min.y, max.y, (ySection + 1)/(float)SectionNumb), 0, 0),
			                    new Vector4(0, 0, Mathf.Lerp(min.z, max.z, zSection/(float)SectionNumb), 0), 
			                    new Vector4(0, 0, Mathf.Lerp(min.z, max.z, (zSection + 1)/(float)SectionNumb), 0));
		}
	}
	/**
	 * CPU multiThread Solution: Very Good, but not GPGPU Good.
	public int RowFinished;
	public int RowApplied;


	Color[,] ColorBuffer;
	// Use this for initialization
	void Start () 
	{
		image = new Texture2D(size, size);

		ColorBuffer = new Color[size, size];

		tex.SetTexture("_MainTex", image);

		RowFinished = 0;

		RowApplied = 0;

		System.Threading.Thread newThread =
			new System.Threading.Thread(makeUp);

		newThread.Start();
	}

	void makeUp()
	{
		float[] Z2C = new float[]{1, 0, 1, 0};
		Recursor theStart = new Recursor(new Polynomial(Z2C));

		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				int hasEscaped;
				float Varaition;
				float iInit = j*3f/size - 1.5f;
				float jInit = i*3f/size - 1.5f;
				HyperComplex result = theStart.runIterations(new HyperComplex(jInit, iInit, 0, 0), 8, out hasEscaped, out Varaition);
				ColorBuffer[i, j] = Mathf.Max((float)hasEscaped/(float)8, 0)*(new Color(1, 1, 1, 1));
			}
			RowFinished = i + 1;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (RowApplied < RowFinished)
		{
			for (;RowApplied < RowFinished; RowApplied++)
			{
				for (int j = 0; j < size; j++)
				{
					image.SetPixel(RowApplied, j, ColorBuffer[RowApplied, j]);
				}
			}
			image.Apply();
		}
	}
	*/
}
