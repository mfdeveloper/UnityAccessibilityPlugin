﻿using UnityEditor;

namespace UAP
{
	[CustomEditor(typeof(AccessibleDropdown)), CanEditMultipleObjects]
	public class Accessibility_Dropdown_Inspector : Accessibility_InspectorShared
	{
		
		public override void OnInspectorGUI()
		{
			SetupGUIStyles();
			serializedObject.Update();

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Dropdown", myHeadingStyle);
			EditorGUILayout.Separator();

			// Name
			DrawNameSection();

			// Reference / Target 
			DrawTargetSection("Dropdown");

			// Positioning / Traversal
			DrawPositionOrderSection();

			// Speech Output
			DrawSpeechOutputSection();

			// Callbacks
			DrawCallbackSection(true);


			serializedObject.ApplyModifiedProperties();
			DrawDefaultInspectorSection();
		}

	}
}
