using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Managers/AtlasSceneManager")]
public class AtlasSceneManager : ScriptableObject
{
    private static AtlasSceneManager instance;
    public static AtlasSceneManager Instance { get { return instance; } }
    private static string path = "MapEditor/data/test.json";
    AtlasSceneData sceneData;
    public AtlasScene currentScene;
    public List<AtlasScene> neighbors;

    public static AtlasSceneData getSceneData(string sceneName = null)
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        AtlasSceneData sceneData = JsonUtility.FromJson<AtlasSceneData>(json);
        if (Instance)
        {
            instance.sceneData = sceneData;
        }
        return sceneData;
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<AtlasSceneManager>("Managers")[0];
        SceneManager.sceneLoaded += instance.OnSceneLoaded;
        instance.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        getNeighbors();
    }

    public static void printSceneNames()
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        AtlasSceneData sceneData = JsonUtility.FromJson<AtlasSceneData>(json);
        sceneData.printSceneNames();
    }

    public static Vector2 getSceneCoords(string sceneName = null)
    {
        if (sceneName == null)
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        AtlasSceneData sceneData = getSceneData();
        AtlasScene scene = sceneData.scenes.Find(s => s.name == sceneName);
        if (scene != null)
        {
            return new Vector2(scene.x, scene.y);
        }
        return new Vector2(-1, -1);
    }

    public static List<AtlasScene> getNeighbors(string sceneName = null)
    {
        if (sceneName == null)
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        AtlasSceneData sceneData;
        List<AtlasScene> neighbors = new List<AtlasScene>();
        if (instance)
        {
            sceneData = instance.sceneData;
            instance.neighbors = neighbors;
        } else
        {
            sceneData = getSceneData();
        }
        Vector2 sceneCoords = getSceneCoords(sceneName);

        neighbors.Add(findSceneByCoords(sceneCoords + new Vector2(0, -1)));
        neighbors.Add(findSceneByCoords(sceneCoords + new Vector2(-1, 0)));
        neighbors.Add(findSceneByCoords(sceneCoords + new Vector2(1, 0)));
        neighbors.Add(findSceneByCoords(sceneCoords + new Vector2(0, 1)));

        foreach(AtlasScene n in neighbors) {
            Debug.Log(n.name);
        }

        return neighbors;
    }

    public static AtlasScene findSceneByCoords(Vector2 sceneCoords)
    {
        AtlasSceneData sceneData;
        if (instance != null && instance.sceneData != null)
        {
            sceneData = instance.sceneData;
        } else
        {
            sceneData = getSceneData();
        }
        AtlasScene rtnScene = sceneData.scenes.Find(s => s.x == sceneCoords.x && s.y == sceneCoords.y);
        if (rtnScene == null)
        {
            rtnScene = new AtlasScene("null", -1, -1);
        }
        return rtnScene;
    }

    public static AtlasScene getScene(string sceneName)
    {
        if (sceneName == null)
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        AtlasSceneData sceneData;
        if (Instance.sceneData != null)
        {
            sceneData = instance.sceneData;
        }
        else
        {
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            reader.Close();
            sceneData = JsonUtility.FromJson<AtlasSceneData>(json);
        }
        AtlasScene currentScene = sceneData.scenes.Find(s => s.name == sceneName);
        instance.currentScene = currentScene;
        return currentScene;
    }
}


[Serializable]
public class AtlasScene
{
    public string name;
    public int x;
    public int y;

    public AtlasScene()
    {
        this.name = "null";
        this.x = -1;
        this.y = -1;
    }

    public AtlasScene(string name, int x, int y)
    {
        this.name = name;
        this.x = x;
        this.y = y;
    }
}

[Serializable]
public class AtlasSceneData
{
    public int test;
    public List<AtlasScene> scenes;

    public void printSceneNames()
    {
        foreach (AtlasScene s in scenes)
        {
            Debug.Log(s.name);
        }
    }
}