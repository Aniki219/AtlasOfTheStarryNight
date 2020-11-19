using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Managers/AtlasSceneManager")]
public class AtlasSceneManager : ScriptableObject
{
    public const float SCREEN_WIDTH = 16.0f;
    public const float SCREEN_HEIGHT = 9.0f;

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
        return sceneData;
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<AtlasSceneManager>("Managers")[0];
        SceneManager.sceneLoaded += instance.OnSceneLoaded;
        instance.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        getSceneData();
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        getNeighbors();
    }

    public static void switchScene(Vector2 dir)
    {
        GameObject player = gameManager.Instance.player;
        
        AtlasScene fromScene = getScene();

        AtlasScene toScene = getNeighborWithCoords(getPlayerCoords() + dir);
        if (toScene == null) throw new Exception("Null scene data found at coords " + (getPlayerCoords() + dir));
        if (toScene.scene == "null") return;
        
        Vector2 d = (toScene.size + fromScene.size) * 0.5f;
        Vector2 t = (fromScene.getCenter() - toScene.getCenter()) * -SCREEN_HEIGHT;

        float startx = 0; 
        float starty = 0;

        if (dir.x != 0) {
            startx = (toScene.size.x * SCREEN_WIDTH * 0.5f - 0.3f) * -dir.x;
            starty = player.transform.position.y + t.y;
        } else
        {
            startx = player.transform.position.x + t.x;
            starty = (toScene.size.y * SCREEN_HEIGHT * 0.5f - 0.4f) * dir.y;
        }

        gameManager.Instance.switchScene(toScene.scene, startx, starty);
    }

    public static void printSceneNames()
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        AtlasSceneData sceneData = JsonUtility.FromJson<AtlasSceneData>(json);
        sceneData.printSceneNames();
    }

    public static Vector2 getScreenSize()
    {
        return new Vector2(SCREEN_WIDTH, SCREEN_HEIGHT);
    }

    public static Vector2 getSceneCoords(string sceneName = null)
    {
        if (sceneName == null)
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        AtlasSceneData sceneData = getSceneData();
        AtlasScene scene = sceneData.scenes.Find(s => s.scene == sceneName);
        if (scene != null)
        {
            return new Vector2(scene.position.x, scene.position.y);
        }
        return new Vector2(-1, -1);
    }

    public static List<AtlasScene> getNeighbors(string sceneName = null)
    {
        if (sceneName == null)
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        AtlasSceneData sceneData = getSceneData();
        AtlasScene currentScene = getScene();

        List<AtlasScene> neighbors = new List<AtlasScene>();
        for (int x = 0; x < currentScene.size.x; x++)
        {
            neighbors.Add(findSceneByCoords(currentScene.position + new Vector2(x, -1)));
            neighbors.Add(findSceneByCoords(currentScene.position + new Vector2(x, currentScene.size.y)));
        }
        for (int y = 0; y < currentScene.size.y; y++)
        {
            neighbors.Add(findSceneByCoords(currentScene.position + new Vector2(-1, y)));
            neighbors.Add(findSceneByCoords(currentScene.position + new Vector2(currentScene.size.x, y)));
        }

        return neighbors;
    }

    public static AtlasScene getNeighborWithCoords(Vector2 coords)
    {
        instance.neighbors = getNeighbors();

        return instance.neighbors.Find(s =>
        {
            foreach (Vector2 c in s.coords)
            {
                if (c.x == coords.x && c.y == coords.y) return true;
            }
            return false;
        });
    }

    public static Vector2 getPlayerCoords()
    {
        Vector2 playerCoords = Vector2.zero;
        Vector2 player = gameManager.Instance.player.transform.position;

        AtlasScene scene = getScene();
        Vector2 bottomLeft = -scene.getExtents(true);

        player.x = Mathf.Clamp(player.x, bottomLeft.x, bottomLeft.x + scene.size.x * SCREEN_WIDTH);
        player.y = Mathf.Clamp(player.y, bottomLeft.y, bottomLeft.y + scene.size.y * SCREEN_HEIGHT);

        Vector2 d = player - bottomLeft;
        Vector2 normd = new Vector2(
            (int)Mathf.Clamp(d.x / SCREEN_WIDTH, 0, scene.size.x-1),
            (int)Mathf.Clamp(d.y / SCREEN_HEIGHT, 0, scene.size.y-1));
        playerCoords.x = normd.x + scene.position.x;
        playerCoords.y = scene.size.y - 1 - normd.y + scene.position.y;

        return playerCoords;
    }

    public static AtlasScene findSceneByCoords(Vector2 sceneCoords)
    {
        AtlasSceneData sceneData = getSceneData();
        AtlasScene rtnScene = sceneData.scenes.Find(s => {return
            sceneCoords.x < s.position.x + s.size.x &&
            sceneCoords.x >= s.position.x &&
            sceneCoords.y < s.position.y + s.size.y &&
            sceneCoords.y >= s.position.y;
        });
        if (rtnScene == null)
        {
            rtnScene = new AtlasScene();
        }
        return rtnScene;
    }

    public static AtlasScene getScene(string sceneName = null)
    {
        if (sceneName == null)
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        AtlasSceneData sceneData = getSceneData();
        AtlasScene currentScene = sceneData.scenes.Find(s => s.scene == sceneName);
        return currentScene;
    }
}

[Serializable]
public class AtlasScene
{
    public string scene;
    public Vector2 position;
    public Vector2 size;
    public Vector2[] coords;

    public AtlasScene()
    {
        scene = "null";
        position = Vector2.zero;
        size = Vector2.one;
        coords = new Vector2[0];
    }

    public AtlasScene(string scene, Vector2 position, Vector2 size, Vector2[] coords)
    {
        this.scene = scene;
        this.position = position;
        this.size = size;
        this.coords = coords;
    }

    public Vector2 getCenter(bool abs = false)
    {
        Vector2 center = position + size * 0.5f;
        if (abs) center.Scale(AtlasSceneManager.getScreenSize());
        return center;
    }

    public Vector2 getExtents(bool abs = false)
    {
        Vector2 extents = size * 0.5f;
        if (abs) extents.Scale(AtlasSceneManager.getScreenSize());
        return extents;
    }
}

[Serializable]
public class AtlasSceneData
{
    public List<AtlasScene> scenes;

    public void printSceneNames()
    {
        foreach (AtlasScene s in scenes)
        {
            Debug.Log(s.scene);
        }
    }
}