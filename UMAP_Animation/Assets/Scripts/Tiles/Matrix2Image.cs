using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Matrix2Image
{
      	public static Texture2D TextureFromColourMap(Color[] colourMap, int width) {
		Texture2D texture = new Texture2D (width,width);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}
    public static Texture2D TextureFromHeightMap(int[] listValue)
    {
        int width = 28; 
        Color[] colourMap = new Color[listValue.Length-1];
        for (int y = 1; y < listValue.Length; y++)
        {
            colourMap[y-1] = Color.Lerp (Color.black, Color.white, listValue[y]);
        }
        return TextureFromColourMap(colourMap, width);
    }
}

   

    