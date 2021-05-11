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
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.XR;
    using UnityEngine.XR.Management;
    using System.Runtime.InteropServices;

    public class NRXRLoader : XRLoaderHelper
    {
        private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors =
            new List<XRDisplaySubsystemDescriptor>();
        private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors =
            new List<XRInputSubsystemDescriptor>();

        public XRDisplaySubsystem displaySubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRDisplaySubsystem>();
            }
        }

        public XRInputSubsystem inputSubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRInputSubsystem>();
            }
        }

        public override bool Initialize()
        {
            Debug.Log("[XR][NRXRLoader] Initialize");
            UserDefinedSettings userDefinedSettings = new UserDefinedSettings();
#if UNITY_ANDROID && !UNITY_EDITOR
            NRSettings settings = GetSettings();
            if (settings != null)
            {
                userDefinedSettings.stereoRenderingMode = (ushort)settings.GetStereoRenderingMode();
                userDefinedSettings.colorSpace = (ushort)((QualitySettings.activeColorSpace == ColorSpace.Linear) ? 1 : 0);
                userDefinedSettings.useMultiThread = settings.GetMultiThreadMode();
                SetUserDefinedSettings(userDefinedSettings);
            }
            else
            {
                Debug.Log("[XR][NRXRLoader] Settings is null!");
                userDefinedSettings.stereoRenderingMode = (ushort)NRSettings.StereoRenderingModeAndroid.Multiview;
                userDefinedSettings.useMultiThread = false;
                SetUserDefinedSettings(userDefinedSettings);
            }
#endif
            Debug.Log("[XR][NRXRLoader] Settings:" + userDefinedSettings.ToString());
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, "NRSDK Display");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "NRSDK Head Tracking");

            if (displaySubsystem == null || inputSubsystem == null)
            {
                Debug.Log("[XR][NRXRLoader] Unable to start NRSDK XR Plugin.");
                return false;
            }

            return true;
        }

        public override bool Start()
        {
            Debug.Log("[XR][NRXRLoader] Start");
            StartSubsystem<XRDisplaySubsystem>();
            StartSubsystem<XRInputSubsystem>();
            return true;
        }

        public override bool Stop()
        {
            Debug.Log("[XR][NRXRLoader] Stop");
            StopSubsystem<XRDisplaySubsystem>();
            StopSubsystem<XRInputSubsystem>();
            return true;
        }

        public override bool Deinitialize()
        {
            Debug.Log("[XR][NRXRLoader] Deinitialize");
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct UserDefinedSettings
        {
            public ushort stereoRenderingMode;
            public ushort colorSpace;
            public bool useMultiThread;

            public override string ToString()
            {
                return string.Format("stereoRenderingMode:{0} colorSpace:{1} useMultiThread:{2}",
                    stereoRenderingMode, colorSpace, useMultiThread);
            }
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        [DllImport("NrealXRPlugin", CharSet = CharSet.Auto)]
        static extern void SetUserDefinedSettings(UserDefinedSettings settings);
#endif

        public NRSettings GetSettings()
        {
            NRSettings settings = null;
#if UNITY_EDITOR
            EditorBuildSettings.TryGetConfigObject<NRSettings>(NRConstants.k_SettingsKey, out settings);
#else
            settings = NRSettings.s_Settings;
#endif
            return settings;
        }
    }
}
