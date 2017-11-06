using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IslandGenerator))]
public class DungeonGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var changed = DrawDefaultInspector();

		var gen = (IslandGenerator)target;
		var tileGen = gen.gameObject.GetComponent<TileGenerator>();

		if (changed)
			tileGen.GenerateIsland();

		if (GUILayout.Button("Generate"))
			tileGen.GenerateIsland();
	}
}
