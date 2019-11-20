using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SpritePaletteLookupGenerator : MonoBehaviour
{
    public Texture2D spriteTex;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateLookupTexture()
    {
        var lookup = new Texture2D(256, 1, TextureFormat.ARGB32, false);
        Dictionary<int, Color> colorsBeenSet = new Dictionary<int, Color>();
        Dictionary<Color, int> indexedSprite = new Dictionary<Color, int>();
        var pal = new Texture2D(256, 1, TextureFormat.ARGB32, false);

        Color[] pix = spriteTex.GetPixels(0, 0, spriteTex.width, spriteTex.height);
        int colorIndex = 0;
        for (int col = 0; col < spriteTex.width; col++) {
            for (int row = 0; row < spriteTex.height; row++)
            {
                Color c = spriteTex.GetPixel(col, row);
                
                if (false) { 
                //float hash = (c.r + c.g / 2f + c.b / 3f + c.a / 4f);
                int hash = (int)(c.r * 255);//((c.r * 255 + c.g * 255 + c.b * 255 + c.a * 255));
                if (!colorsBeenSet.ContainsKey(hash))
                {
                    colorsBeenSet[hash] = c;
                    lookup.SetPixel(col, row, c);
                }
                else if (colorsBeenSet[hash] != c)
                {
                    Debug.Log("Color with r value already added: " + c);
                }
                } else
                {
                    if (!indexedSprite.ContainsKey(c))
                    {
                        pal.SetPixel(colorIndex, 0, c);
                        indexedSprite[c] = colorIndex++;
                    }
                }
            }
        }

        if (false)
        {
            lookup.Apply();
            byte[] bytes = lookup.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/Resources/Lookups/" + spriteTex.name + "_lookup.png", bytes);
        } else
        {
            
            var grayscaleIndex = new Texture2D(spriteTex.width, spriteTex.height, TextureFormat.R16, false);
            for (int col = 0; col < spriteTex.width; col++)
            {
                for (int row = 0; row < spriteTex.height; row++)
                {
                    grayscaleIndex.SetPixel(col, row, new Color(indexedSprite[spriteTex.GetPixel(col, row)] / 255f, 0, 0));
                }
            }
            grayscaleIndex.Apply();
            byte[] bytes = grayscaleIndex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/Resources/Lookups/" + spriteTex.name + "_lookup.png", bytes);
            pal.Apply();
            bytes = pal.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/Resources/Lookups/" + spriteTex.name + "_pal.png", bytes);
        }
        Debug.Log("Generated lookup tex!");
    }
}