/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

#if XR_MANAGEMENT_320
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;

namespace Unity.XR.NRSDK.Editor
{
    internal class NRMetadata : IXRPackage
    {
        private class NRPackageMetadata : IXRPackageMetadata
        {
            public string packageName => "NRSDK XR Plugin";
            public string packageId => "com.nreal.xr";
            public string settingsType => "Unity.XR.NRSDK.NRSettings";
            public List<IXRLoaderMetadata> loaderMetadata => s_LoaderMetadata;

            private readonly static List<IXRLoaderMetadata> s_LoaderMetadata = new List<IXRLoaderMetadata>() { new NRLoaderMetadata() };
        }

        private class NRLoaderMetadata : IXRLoaderMetadata
        {
            public string loaderName => "NRSDK";
            public string loaderType => "Unity.XR.NRSDK.NRXRLoader";
            public List<BuildTargetGroup> supportedBuildTargets => s_SupportedBuildTargets;

            private readonly static List<BuildTargetGroup> s_SupportedBuildTargets = new List<BuildTargetGroup>()
            {
                BuildTargetGroup.Standalone,
                BuildTargetGroup.Android
            };
        }

        private static IXRPackageMetadata s_Metadata = new NRPackageMetadata();
        public IXRPackageMetadata metadata => s_Metadata;

        public bool PopulateNewSettingsInstance(ScriptableObject obj)
        {
            var settings = obj as NRSettings;
            if (settings != null)
            {
                settings.m_StereoRenderingModeAndroid = NRSettings.StereoRenderingModeAndroid.MultiPass;
                return true;
            }

            return false;
        }
    }
}
#endif