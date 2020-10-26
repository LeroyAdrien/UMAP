using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCentroidPosition : MonoBehaviour
{
    public GameObject[] tiles;
    Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        position = new Vector3(0, 0, 0);
        for(int i=0;i<tiles.Length;i++)
        {
            position+=tiles[i].transform.position;
        }
        transform.position = position / tiles.Length;
    }
}
