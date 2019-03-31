using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnNode))]
public class SpawnNodeInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		SpawnNode t = (SpawnNode) target;
		if (GUILayout.Button("Get Nodes"))
		{
			t.GetNodes();
		}
	}
}
