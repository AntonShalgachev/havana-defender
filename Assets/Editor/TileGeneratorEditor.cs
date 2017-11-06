using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileGenerator))]
public class TileGeneratorEditor : Editor
{
	//public override void OnInspectorGUI()
	//{
	//	var gen = (TileGenerator)target;

	//	if (DrawDefaultInspector())
	//		gen.SpawnTiles();

	//	if (GUILayout.Button("Generate"))
	//		gen.SpawnTiles();
	//}
}
