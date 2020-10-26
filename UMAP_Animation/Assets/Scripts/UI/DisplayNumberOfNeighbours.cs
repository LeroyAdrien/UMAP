using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNumberOfNeighbours : MonoBehaviour
{
    public Text numberOfNeighboursText;
    public TilesGenerator tilegeneratorComponent;


    // Start is called before the first frame update
    void Start()
    {
        GameObject tileGenerator = GameObject.FindGameObjectWithTag("GameController");
        tilegeneratorComponent = tileGenerator.GetComponent<TilesGenerator>();

        //Text numberOfNeighbours = tileGeneratorComponent.

    }

    // Update is called once per frame
    void Update()
    {
        numberOfNeighboursText.text = tilegeneratorComponent.m_neighborNumber.ToString();
    }
}
