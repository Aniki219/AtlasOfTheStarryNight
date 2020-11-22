using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class WorldScenes : EditorWindow
{
    public SceneAsset sceneAsset;
    Vector2 scrollpos;

    [MenuItem("MapEditor/_Scenes")]
    public static void ViewWorldScenes()
    {
        GetWindow(typeof(WorldScenes), false, "World Scenes");
    }

    [MenuItem("MapEditor/Update Scene Data")]
    public static void updateSceneData()
    {
        AtlasSceneManager.getSceneData();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        scrollpos = EditorGUILayout.BeginScrollView(scrollpos, GUIStyle.none, GUI.skin.verticalScrollbar);
        List<SceneAsset> m_SceneAssets = new List<SceneAsset>();
        string folderName = Application.dataPath + "/Scenes/WorldMap/";
        var dirInfo = new DirectoryInfo(folderName);
        var allFileInfos = dirInfo.GetFiles("*.unity", SearchOption.AllDirectories);
        foreach (var fileInfo in allFileInfos)
        {
            SceneAsset sa = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/WorldMap/" + fileInfo.Name);
            m_SceneAssets.Add(sa);
        }

        foreach (var sa in m_SceneAssets)
        {
            sceneAsset = EditorGUILayout.ObjectField("", sa, typeof(SceneAsset), true) as SceneAsset;
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Take Screenshot"))
        {
            ScreenshotEditor.takeScreenshot();
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Add Scenes to Build"))
        {
            AddAllWorldScenes();
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Update Scene Data"))
        {
            AtlasSceneManager.getSceneData();
        }
        if (GUILayout.Button("Toggle Neighbors"))
        {
            MapEditor.toggleNeighbors();
            GUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Open Map Editor"))
        {
            Object mapEditor = AssetDatabase.LoadAssetAtPath("Assets/Scenes/MapEditor.lnk", typeof(Object));
            AssetDatabase.OpenAsset(mapEditor);
            GUIUtility.ExitGUI();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

    }

    [MenuItem("MapEditor/Add World Scenes to Build")]
    public static void AddAllWorldScenes()
    {
        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();

        List<SceneAsset> m_SceneAssets = new List<SceneAsset>();
        string folderName = Application.dataPath + "/Scenes/WorldMap/";
        var dirInfo = new DirectoryInfo(folderName);
        var allFileInfos = dirInfo.GetFiles("*.unity", SearchOption.AllDirectories);
        foreach (var fileInfo in allFileInfos)
        {
            SceneAsset sa = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/WorldMap/" + fileInfo.Name);
            m_SceneAssets.Add(sa);
        }

        foreach (var sceneAsset in m_SceneAssets)
        {
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            if (!string.IsNullOrEmpty(scenePath))
                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
    }
}
