/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace Unity.XR.NRSDK
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.XR.Management;

    [System.Serializable]
    [XRConfigurationData("NRSDK", NRConstants.k_SettingsKey)]
    public class NRSettings : ScriptableObject
    {
        public enum StereoRenderingModeAndroid
        {
            /// <summary>
            /// Unity makes two passes across the scene graph, each one entirely indepedent of the other. 
            /// Each pass has its own eye matrices and render target. Unity draws everything twice, which includes setting the graphics state for each pass. 
            /// This is a slow and simple rendering method which doesn't require any special modification to shaders.
            /// </summary>
            MultiPass = 0,
            /// <summary>
            /// Unity uses a single texture array with two elements. 
            /// Multiview is very similar to Single Pass Instanced; however, the graphics driver converts each call into an instanced draw call so it requires less work on Unity's side. 
            /// As with Single Pass Instanced, shaders need to be aware of the Multiview setting. Unity's shader macros handle the situation.
            /// </summary>
            Multiview = 2
        }

        /// <summary>
        /// The current stereo rendering mode selected for Android-based Nreal platforms
        /// </summary>
        [SerializeField, Tooltip("Set the Stereo Rendering Method")]
        public StereoRenderingModeAndroid m_StereoRenderingModeAndroid;
        [HideInInspector]
        public bool m_UseMultiThread = false;

        public ushort GetStereoRenderingMode()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return (ushort)m_StereoRenderingModeAndroid;
# else
            return 0;
#endif
        }

        public bool GetMultiThreadMode()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return m_UseMultiThread;
#else
            return PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android);
#endif
        }

#if !UNITY_EDITOR
		public static NRSettings s_Settings;

		public void Awake()
		{
			s_Settings = this;
		}
#endif
    }
}
