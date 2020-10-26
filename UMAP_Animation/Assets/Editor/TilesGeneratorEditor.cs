/*
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (TilesGenerator))]
public class TilesGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		TilesGenerator tilesGen = (TilesGenerator)target;

		if (DrawDefaultInspector ()) {

		}

		if (GUILayout.Button ("Generate")) {
			tilesGen.DrawTilesInEditor ();
        }

        if(GUILayout.Button ("Move points"))
        {
            tilesGen.MoveTiles(tilesGen.m_neighborNumber,tilesGen.m_minDistance);
        }


    }
}
*/
