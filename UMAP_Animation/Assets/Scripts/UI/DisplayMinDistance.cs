using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMinDistance : MonoBehaviour
{
    public Text minDistanceText;
    public TilesGenerator tilegeneratorComponent;


    // Start is called before the first frame update
    void Start()
    {
        GameObject tileGenerator = GameObject.FindGameObjectWithTag("GameController");
        tilegeneratorComponent=tileGenerator.GetComponent<TilesGenerator>();
        
        //Text numberOfNeighbours = tileGeneratorComponent.

    }

    // Update is called once per frame
    void Update()
    {
        minDistanceText.text=tilegeneratorComponent.m_minDistance.ToString();
    }
}
