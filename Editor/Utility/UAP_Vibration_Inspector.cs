using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UAP.Utility
{
    // TODO: [Improvement] Create an Editor base abstract class for Inspector UI using either Toolkit and IMGUI
    [CustomEditor(typeof(UAP_Vibration), true)]
    public class UAP_Vibration_Inspector : Editor
    {
        #region Constants

        private const string WARNING_MESSAGE = @"[Accessibility] Vibration is only supported on <b>{0}</b> and <b>{1}</b> platforms.\n" +
                                               "BuildTarget is: \"<b>{2}</b>\"";

        #endregion

        #region Fields
        
        [SerializeField]
        protected VisualTreeAsset inspectorUxml;
        
        protected UIType uiType = UIType.UIToolkit;
        protected object[] availablePlatforms = { "Mobile", "WebGL" };
        protected string formattedMessage = string.Empty;
        protected BuildTarget buildTarget;

        #endregion

        #region Unity Events

        protected virtual void OnEnable()
        {
            InitializeFields();
        }
        
        /// <summary>
        /// <b> References </b>
        /// <ul>
        ///     <li>
        ///         <a href="https://forum.unity.com/threads/is-ui-toolkit-supposed-to-be-retrocompatible-with-unity-2019-4.1038430">
        ///             Is UI Toolkit supposed to be retro-compatible with Unity 2019.4?
        ///         </a>
        ///     </li>
        /// </ul>
        /// </summary>
        /// <returns></returns>
        #if UNITY_2020_2_OR_NEWER
        
        public override VisualElement CreateInspectorGUI()
        {
            return uiType == UIType.UIToolkit ? WarnInvalidPlatform(forceDefaultInspector: true) : base.CreateInspectorGUI();
        }
        
        #endif
        
        /// <summary>
        /// Override <see cref="Editor.OnInspectorGUI"/> here as a legacy compatibility code
        /// in order to use Immediate Mode GUI (IMGUI)
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (uiType == UIType.ImGui)
            {
                WarnInvalidPlatform();
            }
        }
        
        #endregion

        #region Methods

        protected virtual void InitializeFields()
        {
            // PS: Unity UI Toolkit is stable available only on
            //     Editor version >= 2020.2
            #if !UNITY_2020_2_OR_NEWER
                        
            uiType = UIType.ImgGui;

            #endif

            buildTarget = EditorUserBuildSettings.activeBuildTarget;

            formattedMessage = string.Format(
                WARNING_MESSAGE,
                availablePlatforms[0],
                availablePlatforms[1],
                buildTarget
            );
        }
        
        /// <summary>
        /// <b> References </b>
        /// <ul>
        ///     <li>
        ///         <a href="https://stackoverflow.com/a/58753455">
        ///             How to achieve `EditorGUI.HelpBox` look with `GUIStyle`?
        ///         </a>
        ///     </li>
        /// </ul>
        /// </summary>
        /// <param name="forceDefaultInspector">True for always draw the default inspector fields (from MonoBehaviour serialized fields) ? </param>
        /// <param name="afterDrawInspector">Callback function with a custom <see cref="Func{T1,TResult}"/> to execute after draw the inspector</param>
        /// <returns></returns>
        protected virtual VisualElement WarnInvalidPlatform(
            bool forceDefaultInspector = false, 
            Func<VisualElement, VisualElement> afterDrawInspector = null
        )
        {
            var inspector = new VisualElement();
            
            if (buildTarget != BuildTarget.Android
                && buildTarget != BuildTarget.iOS
                && buildTarget != BuildTarget.WebGL)
            {
                
                if (uiType == UIType.UIToolkit)
                {
                    if (inspectorUxml != null)
                    {
                        inspectorUxml.CloneTree(inspector);
                    }
                    else
                    {
                        var messageBox = new HelpBox(formattedMessage, HelpBoxMessageType.Warning);
                        inspector.Add(messageBox);
                    }

                    if (forceDefaultInspector)
                    {
                        // Attach a default inspector to a "root" VisualElement
                        InspectorElement.FillDefaultInspector(inspector, serializedObject, this);
                    }
                }
                else
                {
                    EditorStyles.helpBox.richText = true;
                    EditorGUILayout.HelpBox(formattedMessage, MessageType.Warning);
                    
                    if (forceDefaultInspector)
                    {
                        // Draw default inspector (MonoBehaviour serialized fields)
                        base.OnInspectorGUI();
                    }

                    inspector = null;
                }
            }
            else
            {
                if (uiType == UIType.UIToolkit)
                {
                    // Attach a default inspector to a "root" VisualElement
                    InspectorElement.FillDefaultInspector(inspector, serializedObject, this);
                }
                else
                {
                    // Draw default inspector (MonoBehaviour serialized fields)
                    base.OnInspectorGUI();
                    EditorStyles.helpBox.richText = false;
                }

                if (afterDrawInspector != null)
                {
                    inspector = afterDrawInspector(inspector);
                }
            }
            
            return inspector;
        }
        
        #endregion
    }
}
