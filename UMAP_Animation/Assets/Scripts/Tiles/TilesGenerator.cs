using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilesGenerator : MonoBehaviour
{
    
    //Prefab d'une tuile
    public GameObject m_tile;
    //Couleurs des boules
    public Color[] m_ballColors;

    //Attributs de la répartition & Paramètres de UMAP
    public float m_scale;
    //Vitesse de déplacement des points
    public float m_speed = 3f;

    public float m_minDistance;
    public int m_neighborNumber;


    //Compteurs
    private int tileCount = 0;
    private bool tilesAreGenerated = false;
    private bool tilesAreMoving = false;

    //Liste de paramètre Nombre de voisins
    public List<int> numberOfNeighborsList = new List<int>();
    ///iste de paramètre Distance minimum
    public List<float> minimumDistanceList = new List<float>();
    //Dictionnaire des chemin en fonction des paramètre
    public Dictionary<(int, float), UnityEngine.Object> m_files;
    //Dictionnaire des FichiersdeTiles en fonction des paramètres
    public Dictionary<(int, float), (List<Vector3>, List<int>)> m_dictOfTileCoordinates;
    //Liste des tuiles
    public List<Tile> m_listOfTiles;
    //Liste des Gameobjects créés
    public List<GameObject> m_listOfGameObjectsTiles;


    //Texte des scrollbars
    public string m_numberofNeighborTextUI;
    public string m_minDistanceTextUI;



    //References aux scrollbars
    public void NewNumberOfNeighbors(float newNN)
    {
        m_neighborNumber = numberOfNeighborsList[(int)newNN];
        m_numberofNeighborTextUI = numberOfNeighborsList[(int)newNN].ToString();
    }

    public void NewMinimumDistance(float newDistance)
    {
        m_minDistance = minimumDistanceList[(int)newDistance];
        m_minDistanceTextUI = minimumDistanceList[(int)newDistance].ToString();
    }
    
     public void DisableChildObjectsWithTag(Transform parent, string tag, bool shouldBeActive)
     {
         for (int x = 0; x < parent.childCount; x++)
         {
             Transform child = parent.GetChild(x);
             if(child.tag == tag)
             {
                 child.gameObject.SetActive(shouldBeActive);
             }
             DisableChildObjectsWithTag(child, tag, shouldBeActive);
         }
     }
     

    //Reference au toggle mode d'affichage
    public void DisplayChoice(bool toggle)
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");

        if (toggle)
        {
            foreach (GameObject node in nodes)
            {
                node.GetComponent<TextureChoice>().UpdateTextureChoice(toggle);
                DisableChildObjectsWithTag(node.transform, "bouboule", !toggle);
            }

        }
        if (!toggle)
        {
            foreach (GameObject node in nodes)
            {
                node.GetComponent<TextureChoice>().UpdateTextureChoice(toggle);
                DisableChildObjectsWithTag(node.transform, "bouboule", !toggle);
            }
        }
    }

    public void Update()
    {
        MoveTiles(m_neighborNumber, m_minDistance);
    }

    public void Start()
    {
        //Destruction des instances précédentes
        ResetPreviousNodes();

        //Lecture des CSV
        m_files = FileParsing.GeneratePathDict();
        List<(int, float)> keylist = new List<(int, float)>(m_files.Keys);


        //Creation des Dictionnaires de positions des Tiles
        m_dictOfTileCoordinates = FileParsing.GenerateFilesOfTiles(m_files);

        //Initialisation des tiles à des positions aléatoires 
        List<GameObject> m_listOfGameObjectsTiles = GenerateTiles();
        GenerateFirstCenterOfMass();


        //Remplissage des deux listes
        List<(int, float)> keysToParse = new List<(int, float)>(m_dictOfTileCoordinates.Keys);
        foreach ((int, float) tuple in keysToParse)
        {
            //Remplissage du nombre de voisins possibles
            if (!numberOfNeighborsList.Contains(tuple.Item1))
            {
                numberOfNeighborsList.Add(tuple.Item1);
            }
            //Remplissage de la distance minimale
            if (!minimumDistanceList.Contains(tuple.Item2))
            {
                minimumDistanceList.Add(tuple.Item2);
            }


        }
        numberOfNeighborsList.Sort();
        minimumDistanceList.Sort();
        tilesAreGenerated = true;

    }
    //Creation des possibilités pour les variables de l'editeur





    //Fait bouger les tuiles à leur nouvelle position jusqu'à ce qu'elles aient terminé

    public void MoveTiles(List<Vector3> coordinates)
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");
        //Listes de booleens, pour savoir si chacun est arrivé
        bool[] hasArrived = new bool[nodes.Length];
        //Si tout le monde est arrivé
        bool everyoneHasArrived = false;
        //Chaque noeud se déplace un peu
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].transform.position = Vector3.MoveTowards(nodes[i].transform.position, coordinates[i] * m_scale, m_speed * Time.deltaTime);
        }
        //On vérifie pour chaque noeud si il a atteint sa position
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].transform.position == coordinates[i] * m_scale)
            {
                hasArrived[i] = true;
            }
        }
        //On vérifie si un seul noeud n'est pas arrivé
        everyoneHasArrived = true;
        for (int i = 0; i < nodes.Length; i++)
        {
            if (!hasArrived[i])
            {
                everyoneHasArrived = false;
            }
        }

        //Si chaque noeud est arrivé, on arrête d'appeler la fonction
        tilesAreMoving = false;

        //Debug.Log("Everyone has reached its destination");
    }





    //Fait bouger les noeuds en fonction des paramètres spécifiés
    public void MoveTiles(int numberOfNeighbors, float minDistance)
    {
        //récupération des positions pour les paramètres donnés 
        (List<Vector3>, List<int>) file = m_dictOfTileCoordinates[(numberOfNeighbors, minDistance)];
        List<Vector3> coordinates = file.Item1;
        //Mouvement des tuiles
        MoveTiles(coordinates);

    }



    public void ResetPreviousNodes()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");
        foreach (GameObject node in nodes)
        {
            DestroyImmediate(node);
        }
        tileCount = 0;
        GameObject center = GameObject.FindGameObjectWithTag("center");
        DestroyImmediate(center);
    }

    public GameObject GenerateTile(Tile tile)
    {
        //Position Aléatoire au moment de la génération
        Vector3 RandomPos = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)) * m_scale;

        //Creation de l'objet et set du parent
        GameObject tempTile = Instantiate(m_tile);
        tempTile.name = $"Tile_{tileCount}";
        TextureChoice textureChoice = tempTile.GetComponent<TextureChoice>();
        textureChoice.m_noiseMap = tile.noiseMap;
        textureChoice.m_colorOfBall = m_ballColors[tile.label];
        tempTile.transform.parent = transform;
        tempTile.transform.position = RandomPos;
        tempTile.transform.rotation = Quaternion.identity;
        tempTile.tag = "node";
        tileCount++;

        //Creation et assignation de la texture
        /*
        Texture2D texture = Matrix2Image.TextureFromHeightMap(tile.noiseMap);
        Renderer renderer = tempTile.GetComponent<Renderer>();
        renderer.material.shader = Shader.Find("Sprites/Default");
        renderer.material.mainTexture = texture;
        */

        return tempTile;
    }

    public List<GameObject> GenerateTiles()
    {
        //Récuperation des valeurs de chaque tile:
        List<int[]> listOfvalues = ReadCSV.ReadCSVFileInt();
        //Récuperation des labels de chaque tile:
        List<(int, float)> keyList = new List<(int, float)>(m_dictOfTileCoordinates.Keys);
        (List<Vector3>, List<int>) ValueList = m_dictOfTileCoordinates[keyList[0]];
        List<int> labels = ValueList.Item2;

        //Creation de la liste des tiles
        List<Tile> listOfTiles = new List<Tile>();
        //Remplissage de la liste
        for (int i = 0; i < listOfvalues.Count; i++)
        {
            listOfTiles.Add(new Tile(labels[i], listOfvalues[i]));
        }
        //list des tiles en Gameobject
        List<GameObject> gameObjectsList = new List<GameObject>();

        foreach (Tile tile in listOfTiles)
        {
            gameObjectsList.Add(GenerateTile(tile));
        }
        return gameObjectsList;
    }

    /*
     public void GenerateCenterOfMass(List<Tile> tiles)
     {
         //Creation de la liste des positions à partir de la liste de tiles
         List<Vector3> positions = new List<Vector3>();
         foreach (Tile tile in tiles)
         {
             positions.Add(tile.position);
         }

         Vector3 centerPosition = Vector3.zero;
         for (int i = 0; i < positions.Count; i++) {
             centerPosition += positions[i];
         }
         centerPosition /= positions.Count;
         GameObject centroid = new GameObject("Center");
         centroid.transform.position = centerPosition;
         centroid.tag = "center";
     }
     */

    public void GenerateFirstCenterOfMass()
    {
        GameObject centroid = new GameObject("Center");
        centroid.transform.position = Vector3.zero;
        centroid.tag = "center";
    }
}


public struct Tile
{
    public int[] noiseMap;
    public int label;
    public List<Vector3> position;

    public Tile(int label, int[] noiseMap)
    {
        this.noiseMap = noiseMap;
        this.label = label;
        this.position = new List<Vector3>();
    }

    public void AddPosition(Vector3 position)
    {
        this.position.Add(position);
    }
}


