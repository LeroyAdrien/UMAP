using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Globalization;

public static class ReadCSV
{
    public static List<int[]> ReadCSVFileInt()
    {
        UnityEngine.Object fichierMNIST= Resources.Load("MNIST");
        string[] strings = fichierMNIST.ToString().Split("\n"[0]);

        int nbOfLines = strings.Length;

        List<int[]> matrix = new List<int[]>();

        for (int i = 0; i < strings.Length - 1; i++)
        {
            string[] line = strings[i].Split(',');
            int[] listTemp = new int[line.Length];
            for (int j = line.Length - 1; j >= 0; j--)
            {
                listTemp[j] = int.Parse(line[j]);
            }
            matrix.Add(listTemp);
        }
        return matrix;
    }

    public static (List<Vector3>,List<int>) ReadCSVFilePositions(string positions)
    {

        string[] strings = positions.Split("\n"[0]);
        int nbOfLines = strings.Length;

        List<Vector3> matrixPositions = new List<Vector3>();
        List<int> matrixLabels = new List<int>();

        for (int i = 0; i < strings.Length - 1; i++)
        {
            string[] line = strings[i].Split(',');
            float a = float.Parse(line[0],CultureInfo.InvariantCulture.NumberFormat);
            float b = float.Parse(line[1],CultureInfo.InvariantCulture.NumberFormat);
            float c = float.Parse(line[2],CultureInfo.InvariantCulture.NumberFormat);

            int label = int.Parse(line[3]);
            Vector3 vectorTemp = new Vector3(a, b, c);

            matrixPositions.Add(vectorTemp);
            matrixLabels.Add(label);
        }
        return (matrixPositions,matrixLabels);
    }
}
