using System.IO;
using UnityEditor;
using UnityEngine;

public class AutoBuild : Editor
{
    [MenuItem("Tools/导出APK", false, 1)]
    static public void ExportAndroid()
    {
        //PlayerSettings.productName = "UnityProject";
        PlayerSettings.applicationIdentifier = string.Format("com.jerry.lai.{0}", PlayerSettings.productName);
        PlayerSettings.keystorePass = "jerrylai@jingfeng*1990";
        PlayerSettings.keyaliasPass = "lai123";

        string exportPath = string.Format("{0}/../{1}_{2}.apk", 
            Application.dataPath, PlayerSettings.productName, System.DateTime.Now.ToString("HHmmss"));
        
        BuildPipeline.BuildPlayer(new string[] 
        {
            "Assets/Main.unity",
        },
        exportPath,
        BuildTarget.Android,
        BuildOptions.None);
    }
}