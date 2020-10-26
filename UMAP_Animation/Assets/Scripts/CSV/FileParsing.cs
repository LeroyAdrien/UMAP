using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Globalization;

public static class FileParsing
{
    public static Dictionary<(int,float),UnityEngine.Object> GeneratePathDict()
    {
        //Liste des fichiers dans le répertoire
        var txtFiles=Resources.LoadAll("CSVs",typeof(TextAsset));
        //List des paramètres possibles
        List<int> nnList = new List<int>();
        List<float> minDist = new List<float>();
        Dictionary<(int, float), UnityEngine.Object> pathDict = new Dictionary<(int, float), UnityEngine.Object>();

        //remplissage de la liste
        foreach (var objet in txtFiles)
        {
            //Fetching des paramètres par fichier
            float[] parameters=GetNumberOfParameters(objet.name);
            pathDict.Add( ( (int)parameters[0] , parameters[1]) , objet);
        }

        return pathDict;
        
    }

    

    public static Dictionary<(int, float), (List<Vector3>, List<int>)> GenerateFilesOfTiles(Dictionary<(int, float),UnityEngine.Object> files)
    {
        //Dictionnaire de (numberOfNeighbor,min_dist) -> FileOfTiles
        Dictionary<(int, float), (List<Vector3>, List<int>)> filesOfTiles = new Dictionary<(int, float), (List<Vector3>, List<int>)>();
        //Liste des clés du dictionnaire
        List<(int,float)> keyList = new List<(int,float)>(files.Keys);
        for (int i = 0; i < keyList.Count; i++)
        {
            (int, float) key = keyList[i];
            (List<Vector3>,List<int>) listOfPositions = ReadCSV.ReadCSVFilePositions(files[key].ToString());
            filesOfTiles.Add(key, listOfPositions);

        }
        return filesOfTiles;
    }

public static float[] GetNumberOfParameters(string title)
    {
        float[] parameters = new float[2];
        var match = Regex.Matches(title, "NN_([0-9]+)_DISTMIN_([0-9]+.*[0-9]*)");
        string NbOfNeighbors = (match[0].Groups[1].Captures[0].Value);
        string minimumDistance = match[0].Groups[2].Captures[0].Value;
        parameters[0] = int.Parse(NbOfNeighbors);
        parameters[1] = float.Parse(minimumDistance, CultureInfo.InvariantCulture.NumberFormat);

        return parameters;
    }


}
