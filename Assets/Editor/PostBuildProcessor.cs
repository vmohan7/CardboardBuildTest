﻿using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using System.Linq;

public class PostBuildProcessor : MonoBehaviour
{
#if UNITY_CLOUD_BUILD
    /*
    This methods are per platform post export methods. They can be added additionally to the post process attributes in the Advanced Features Settings on UCB using
    - PostBuildProcessor.OnPostprocessBuildiOS
    - PostBuildProcessor.OnPostprocessBuildAndroid
    depending on the platform they should be executed.
 
    Here is the basic order of operations (e.g. with iOS operation)
    - Unity Cloud Build Pre-export methods run
    - Export process happens
    - Methods marked the built-in PostProcessBuildAttribute are called
    - Unity Cloud Build Post-export methods run
    - [unity ends]
    - (iOS) Xcode processes project
    - Done!
    More information can be found on http://forum.unity3d.com/threads/solved-ios-build-failed-pushwoosh-dependency.293192/
    */
    public static void OnPostprocessBuildiOS (string exportPath)
    {
    }
#endif

    // a normal post process method which is executed by Unity
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        #if UNITY_CLOUD_BUILD

                Debug.Log("OnPostprocessBuildiOS");
                string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

                PBXProject proj = new PBXProject();
                proj.ReadFromString(File.ReadAllText(projPath));

                string target = proj.TargetGuidByName("Unity-iPhone");

                // Set a custom link flag
                proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-all_load");
                proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

                // add frameworks
                proj.AddFrameworkToProject(target, "CoreTelephony.framework", true);
                proj.AddFrameworkToProject(target, "EventKit.framework", true);
                proj.AddFrameworkToProject(target, "EventKitUI.framework", true);
                proj.AddFrameworkToProject(target, "iAd.framework", true);
                proj.AddFrameworkToProject(target, "MessageUI.framework", true);
                proj.AddFrameworkToProject(target, "StoreKit.framework", true);
                proj.AddFrameworkToProject(target, "Security.framework", true);
                proj.AddFrameworkToProject(target, "GameKit.framework", true);

                File.WriteAllText(projPath, proj.WriteToString());

        #endif
    }
}