using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChoice : MonoBehaviour
{
    public Texture2D m_texture;
    public Color m_colorOfBall;
    //public GameObject bouboule;
    public int[] m_noiseMap;
    public int m_label;

    //Choix de la texture
    public enum TextureChoiceEnum { tile, ball }
    public TextureChoiceEnum m_tileOrBall;

    public bool m_textureChoicehasChanged = false;

    public void UpdateTextureChoice(bool toggle)
    {

        if (toggle)
        {
            m_tileOrBall = TextureChoiceEnum.tile;
        }
        if (!toggle)
        {
            m_tileOrBall = TextureChoiceEnum.ball;
        }
        m_textureChoicehasChanged = true;
    }


    // Start is called before the first frame update
    void Start()
    {
        m_texture = Matrix2Image.TextureFromHeightMap(m_noiseMap);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.shader = Shader.Find("Sprites/Default");
        renderer.material.mainTexture = m_texture;
        

        GameObject bouboule = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bouboule.transform.position = transform.position;
        bouboule.transform.parent = transform;
        bouboule.transform.localScale = new Vector3(3, 3, 3);
        bouboule.tag = "bouboule";
        bouboule.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
        bouboule.GetComponent<Renderer>().material.color = m_colorOfBall;
        bouboule.SetActive(false);
    }





    // Update is called once per frame
    void Update()
    {

        if (m_textureChoicehasChanged)
        {
            if (m_tileOrBall == TextureChoiceEnum.ball)
            {
                GetComponent<Renderer>().enabled = false;
            }
            if (m_tileOrBall == TextureChoiceEnum.tile)
            {
                GetComponent<Renderer>().enabled = true;
            }

            m_textureChoicehasChanged = false;

        }
    }
}
