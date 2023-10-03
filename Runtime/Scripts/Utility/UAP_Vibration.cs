using UnityEngine;
using VibrationPlugin;
using VibrationPlugin.Enums;

namespace UAP
{
    [DisallowMultipleComponent]
    public class UAP_Vibration : MonoBehaviour
    {

        #region Unity Events

        [SerializeField]
        protected VibrationComponent vibration;

        protected void Awake() 
        {
            InitComponents();
        }

        protected void OnValidate()
        {
            InitComponents();
        }

        protected virtual void OnEnable()
        {
            UAP_AccessibilityManager.RegisterOnMobileChangeElementCallback(OnSwipe);
            UAP_AccessibilityManager.RegisterOnTouchExploreActiveCallback(OnTouchExploreStart);
        }

        protected virtual void OnDisable()
        {
            UAP_AccessibilityManager.UnregisterOnMobileChangeElementCallback(OnSwipe);
            UAP_AccessibilityManager.UnregisterOnTouchExploreActiveCallback(OnTouchExploreStart);
        }

        #endregion

        #region Methods

        protected virtual void InitComponents()
        {
            if (vibration == null)
            {
                vibration = GetComponent<VibrationComponent>();
            }
        }
        private void OnSwipe(UAP_AccessibilityManager.ESDirection? direction, float fingerCount)
        {
            vibration.Vibrate();
        }
    
        private bool OnTouchExploreStart(float fingerCount)
        {
            vibration.Vibrate(vibrationType: VibrationType.DoubleTap);
            return true;
        }

        #endregion
    }
}
