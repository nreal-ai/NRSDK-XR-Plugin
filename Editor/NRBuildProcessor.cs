/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace Unity.XR.NRSDK.Editor
{
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;

    public class NRBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <summary>Override of <see cref="IPreprocessBuildWithReport"/> and <see cref="IPostprocessBuildWithReport"/></summary>
        public int callbackOrder
        {
            get { return 0; }
        }

        void CleanOldSettings()
        {
            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (preloadedAssets == null)
                return;

            var oldSettings = from s in preloadedAssets
                              where s != null && s.GetType() == typeof(NRSettings)
                              select s;

            if (oldSettings != null && oldSettings.Any())
            {
                var assets = preloadedAssets.ToList();
                foreach (var s in oldSettings)
                {
                    assets.Remove(s);
                }

                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        /// <summary>Override of <see cref="IPreprocessBuildWithReport"/></summary>
        /// <param name="report">Build report.</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            // Always remember to cleanup preloaded assets after build to make sure we don't
            // dirty later builds with assets that may not be needed or are out of date.
            CleanOldSettings();

            NRSettings settings = null;
            EditorBuildSettings.TryGetConfigObject(NRConstants.k_SettingsKey, out settings);
            if (settings == null)
            {
                return;
            }

            settings.m_UseMultiThread = PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android);
            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();

            if (!preloadedAssets.Contains(settings))
            {
                var assets = preloadedAssets.ToList();
                assets.Add(settings);
                PlayerSettings.SetPreloadedAssets(assets.ToArray());
            }
        }

        /// <summary>Override of <see cref="IPostprocessBuildWithReport"/></summary>
        /// <param name="report">Build report.</param>
        public void OnPostprocessBuild(BuildReport report)
        {
            // Always remember to cleanup preloaded assets after build to make sure we don't
            // dirty later builds with assets that may not be needed or are out of date.
            CleanOldSettings();
        }
    }
}
