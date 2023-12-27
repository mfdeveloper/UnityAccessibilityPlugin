using UnityEngine;

namespace UAP
{
    /// <summary>
    /// Add padding <see cref="RectOffset"/> offset to UI elements
    /// </summary>
    /// <remarks>
    /// For now, this component is used by <see cref="UAP_AccessibilityManager.m_Frame"/>
    /// highlight outline
    /// </remarks>
    public class UAP_PaddingItem : MonoBehaviour
    {
        [SerializeField] 
        protected RectOffset padding = new();

        public RectOffset Padding => padding;
    }
}
