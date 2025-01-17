﻿using UnityEditor;

namespace UAP
{
	[CustomEditor(typeof(AccessibleLabel)), CanEditMultipleObjects]
	public class Accessibility_Label_Inspector : Accessibility_InspectorShared
	{
		
		public override void OnInspectorGUI()
		{
			SetupGUIStyles();
			serializedObject.Update();

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Label", myHeadingStyle);
			EditorGUILayout.Separator();
			
			// Name
			DrawNameSection();

			// Positioning / Traversal
			DrawPositionOrderSection();

			// Speech Output
			DrawSpeechOutputSection();

			// Callbacks
			DrawCallbackSection(false);


			serializedObject.ApplyModifiedProperties();
			DrawDefaultInspectorSection();
		}

	}
}
