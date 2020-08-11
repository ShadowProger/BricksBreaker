#if UNITY_EDITOR
using System.IO;
using UnityEngine;

public class SaveLoadMenu
{
    [UnityEditor.MenuItem("Tools/Clear Saves")]
    public static void DebugSettingns()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("<color=blue>Saves removed</color>");
        //RemoveSaves("Assets/Data/Saves");
    }

    static void RemoveSaves(string rootPath)
    {
        //if (!Directory.Exists(rootPath)) return;
        //var filesPathes = Directory.GetFiles(rootPath);
        //foreach (var filePath in filesPathes)
        //{
        //    if (Path.GetFileName(filePath) != ".gitignore")
        //    {
        //        File.Delete(filePath);
        //    }
        //}
        //UnityEditor.AssetDatabase.Refresh();
    }
}
#endif