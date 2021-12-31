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

    private static string path = Application.streamingAssetsPath + "/WorldMapData.json";
    AtlasSceneData sceneData;
    public AtlasScene currentScene;
    public List<AtlasScene> neighbors;

    public static AtlasSceneData getSceneData(string sceneName = null)
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        //string json = "{\"scenes\":[{\"scene\":\"UndergroundFountain\",\"position\":{\"x\":4,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":4,\"y\":3}]},{\"scene\":\"LandingSite\",\"position\":{\"x\":-1,\"y\":2},\"size\":{\"x\":2,\"y\":2},\"coords\":[{\"x\":-1,\"y\":2},{\"x\":-1,\"y\":3},{\"x\":0,\"y\":2},{\"x\":0,\"y\":3}]},{\"scene\":\"CaveEntrance\",\"position\":{\"x\":3,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":3,\"y\":2}]},{\"scene\":\"caveOpening\",\"position\":{\"x\":2,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":2,\"y\":2}]},{\"scene\":\"plainsPath1\",\"position\":{\"x\":1,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":1,\"y\":2}]},{\"scene\":\"UndergroundPath\",\"position\":{\"x\":3,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":3,\"y\":3}]},{\"scene\":\"AltairKettle\",\"position\":{\"x\":7,\"y\":6},\"size\":{\"x\":1,\"y\":6},\"coords\":[{\"x\":7,\"y\":6},{\"x\":7,\"y\":7},{\"x\":7,\"y\":8},{\"x\":7,\"y\":9},{\"x\":7,\"y\":10},{\"x\":7,\"y\":11}]},{\"scene\":\"BamBerry Grotto Intro\",\"position\":{\"x\":4,\"y\":6},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":4,\"y\":6}]},{\"scene\":\"ClockworkBombs\",\"position\":{\"x\":5,\"y\":6},\"size\":{\"x\":2,\"y\":1},\"coords\":[{\"x\":5,\"y\":6},{\"x\":6,\"y\":6}]},{\"scene\":\"followTheBomb\",\"position\":{\"x\":2,\"y\":6},\"size\":{\"x\":1,\"y\":2},\"coords\":[{\"x\":2,\"y\":6},{\"x\":2,\"y\":7}]},{\"scene\":\"warpZone\",\"position\":{\"x\":5,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":5,\"y\":3}]},{\"scene\":\"witchCanyon\",\"position\":{\"x\":3,\"y\":7},\"size\":{\"x\":2,\"y\":4},\"coords\":[{\"x\":3,\"y\":7},{\"x\":3,\"y\":8},{\"x\":3,\"y\":9},{\"x\":3,\"y\":10},{\"x\":4,\"y\":7},{\"x\":4,\"y\":8},{\"x\":4,\"y\":9},{\"x\":4,\"y\":10}]},{\"scene\":\"followStarChamber\",\"position\":{\"x\":3,\"y\":6},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":3,\"y\":6}]},{\"scene\":\"gardenEntrance\",\"position\":{\"x\":6,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":6,\"y\":3}]},{\"scene\":\"climbpastbombers\",\"position\":{\"x\":7,\"y\":0},\"size\":{\"x\":1,\"y\":2},\"coords\":[{\"x\":7,\"y\":0},{\"x\":7,\"y\":1}]},{\"scene\":\"gardenhub1\",\"position\":{\"x\":9,\"y\":1},\"size\":{\"x\":1,\"y\":3},\"coords\":[{\"x\":9,\"y\":1},{\"x\":9,\"y\":2},{\"x\":9,\"y\":3}]},{\"scene\":\"bombflowerHallway\",\"position\":{\"x\":8,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":8,\"y\":2}]},{\"scene\":\"chargerHall\",\"position\":{\"x\":10,\"y\":1},\"size\":{\"x\":2,\"y\":1},\"coords\":[{\"x\":10,\"y\":1},{\"x\":11,\"y\":1}]},{\"scene\":\"gardenTemplate\",\"position\":{\"x\":0,\"y\":6},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":0,\"y\":6}]},{\"scene\":\"gardenRampDown\",\"position\":{\"x\":8,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":8,\"y\":3}]},{\"scene\":\"bombberryTree\",\"position\":{\"x\":7,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":7,\"y\":2}]},{\"scene\":\"bombCrossroads\",\"position\":{\"x\":10,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":10,\"y\":3}]},{\"scene\":\"gardenStairs\",\"position\":{\"x\":11,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":11,\"y\":2}]},{\"scene\":\"gardenBombPuzzle1\",\"position\":{\"x\":11,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":11,\"y\":3}]},{\"scene\":\"movingUpToBumpBerry\",\"position\":{\"x\":12,\"y\":2},\"size\":{\"x\":1,\"y\":2},\"coords\":[{\"x\":12,\"y\":2},{\"x\":12,\"y\":3}]},{\"scene\":\"gardenReturn\",\"position\":{\"x\":9,\"y\":4},\"size\":{\"x\":2,\"y\":1},\"coords\":[{\"x\":9,\"y\":4},{\"x\":10,\"y\":4}]},{\"scene\":\"gardenRibbonBridge\",\"position\":{\"x\":7,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":7,\"y\":3}]},{\"scene\":\"bumpberryTree\",\"position\":{\"x\":10,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":10,\"y\":2}]}]}";
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

    public static void switchScene(Vector2 dir, bool clearDoorLabel = false)
    {
        GameObject player = gameManager.Instance.player;
        
        AtlasScene fromScene = getScene();

        AtlasScene toScene = getNeighborWithCoords(getPlayerCoords() + dir);
        if (toScene == null || toScene.scene == "null") return;
        
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
        if (clearDoorLabel)
        {
            gameManager.Instance.currentDoorLabel = "none";
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
        if (currentScene == null) throw new Exception("Cannot get neighbors for scene: " + sceneName);

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
        if (sceneName == "Main Menu") return new AtlasScene();
        AtlasSceneData sceneData = getSceneData();
        AtlasScene currentScene = sceneData.scenes.Find(s => s.scene == sceneName);

        if (currentScene == null) throw new Exception("No Scene data found for scene: " + sceneName);
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