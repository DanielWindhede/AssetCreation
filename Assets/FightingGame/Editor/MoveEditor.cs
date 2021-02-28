using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Fami.FightingGame
{
	[CustomEditor(typeof(Move))]
	public class MoveEditor : Editor
	{
		private bool showInfo = true;
		public override void OnInspectorGUI()
		{

			if (GUILayout.Button("Open Move Editor"))
				MoveEditorWindow.ShowWindow();

			EditorGUILayout.Space();

			showInfo = GUILayout.Toggle(showInfo, "Show info");

			if (showInfo)
				base.OnInspectorGUI();
		}
	}
}