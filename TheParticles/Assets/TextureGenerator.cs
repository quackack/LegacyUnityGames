using UnityEngine;
using System.Collections;

public class TextureGenerator : MonoBehaviour {

	public static Texture2D getParticle(Color X, Color Y, Color Z, float Roughness, float Quantity, int Detail, int Seed)
	{
		Texture2D toReturn = new Texture2D(Detail, Detail);
		int XRand = Mathf.Max(1 , Seed%(Detail/3));
		int YRand = Mathf.Max (1, (Seed%Detail)/3);
		for (int i = 0; i < Detail; i++)
		{
			float normX = (float)i/(float)Detail;
			float toSinX = normX*XRand*Mathf.PI;
			float YScale = Mathf.Sin(toSinX)*Roughness + 1;
			for (int j = 0; j < Detail; j++)
			{
				float normY = (float)j/(float)Detail;
				float toSinY = normY*YRand*Mathf.PI;
				float XScale = Mathf.Sin(toSinY)*Roughness + 1;
				float OverAllGradient = Mathf.Sin(normX*Mathf.PI)*Mathf.Sin(normY*Mathf.PI);

				float dotPattern = Mathf.Max(0, Mathf.Sin(toSinX*XScale*Quantity)*Mathf.Sin(toSinY*YScale*Quantity));

				Color thisPixel = dotPattern*X;
				toReturn.SetPixel(i, j, thisPixel*OverAllGradient);
			}
		}
		toReturn.Apply(true);
		return toReturn;
	}
}
