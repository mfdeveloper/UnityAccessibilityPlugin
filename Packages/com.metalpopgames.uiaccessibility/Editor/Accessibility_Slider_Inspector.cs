﻿using System.Collections;
using UnityEngine;
using UnityEditor;

namespace UAP
{
	[CustomEditor(typeof(AccessibleSlider)), CanEditMultipleObjects]
	public class Accessibility_Slider_Inspector : Accessibility_InspectorShared
	{
		
		public override void OnInspectorGUI()
		{
			SetupGUIStyles();
			serializedObject.Update();

			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Slider", myHeadingStyle);
			EditorGUILayout.Separator();

			// Name
			DrawNameSection();

			// Slider Setup
			DrawSliderSetupSection();

			// Reference / Target 
			DrawTargetSection("Slider");

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
