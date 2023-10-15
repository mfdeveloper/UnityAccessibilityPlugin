namespace UAP.Utility
{
    /// <summary>
    /// Unity UI API that you would like to use on <see cref="UnityEditor.Editor"/>
    /// custom or Runtime Engine <see cref="UnityEngine.MonoBehaviour"/> scripts
    /// </summary>
    public enum UIType
    {
        /// <summary>
        /// Newest UI API using <i>UnityEngine.UIElements.VisualElement</i>
        /// as items elements (Label, Button...).
        /// </summary>
        /// <remarks>
        /// With this UI API, you can use a <i>.uxml</i> file in order to create your
        /// UI elements from <a href="https://docs.unity3d.com/Manual/UIB-interface-overview.html"> UI Builder </a>
        /// on Unity Editor.
        /// <br/><br/>
        /// That API requires at least Unity >= 2020.2
        /// <br/>
        /// </remarks>
        UIToolkit,
        
        /// <summary>
        /// The basic and common Immediate Mode GUI (IMGUI) API to create UI items (legacy)
        /// </summary>
        ImGui
    }
}
