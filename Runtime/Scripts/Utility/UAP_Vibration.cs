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
            #if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            
            if (vibration == null)
            {
                vibration = GetComponent<VibrationComponent>();
            }

            #endif
        }
        private void OnSwipe(UAP_AccessibilityManager.ESDirection? direction, float fingerCount)
        {
            #if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            
            vibration.Vibrate();

            #endif
        }
    
        private bool OnTouchExploreStart(float fingerCount)
        {
            #if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            
            vibration.Vibrate(vibrationType: VibrationType.DoubleTap);
            return true;

            #else

            return false;

            #endif
        }

        #endregion
    }
}
