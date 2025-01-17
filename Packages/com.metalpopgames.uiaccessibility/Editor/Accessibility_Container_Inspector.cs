﻿using UnityEditor;

namespace UAP
{
	[CustomEditor(typeof(AccessibleUIGroupRoot)), CanEditMultipleObjects]
	public class Accessibility_Container_Inspector : Accessibility_InspectorShared
	{
		
		public override void OnInspectorGUI()
		{
			SetupGUIStyles();
			serializedObject.Update();

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Accessible UI Group", myHeadingStyle);
			EditorGUILayout.Separator();

			// General Settings
			DrawContainerSettings();

			// Name
			DrawContainerNameSection();

			// Navigation
			DrawContainerNavigationSettings();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

			serializedObject.ApplyModifiedProperties();
			DrawDefaultInspectorSection();
		}

	}
}
