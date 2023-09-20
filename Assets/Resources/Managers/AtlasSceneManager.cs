using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Managers/AtlasSceneManager")]
public class AtlasSceneManager : ScriptableObject
{
    public const int SCREEN_TILES_WIDTH = 16;
    public const int SCREEN_TILES_HEIGHT = 9;
    public const int PIXELS_PER_TILE = 32;

    private static AtlasSceneManager instance;
    public static AtlasSceneManager Instance { get { return instance; } }

    private static string path = "Assets/SuperTiled2Unity/TiledMaps/AltairCanyon/AltairCanyon.world";//Application.streamingAssetsPath + "/TiledWorld/AltairCanyon.world";
    AtlasWorldMap sceneData;
    public AtlasMap currentScene;
    public List<AtlasMap> neighbors;


    public static AtlasWorldMap getWorldMapData()
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        //string json = "{\"scenes\":[{\"scene\":\"UndergroundFountain\",\"position\":{\"x\":4,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":4,\"y\":3}]},{\"scene\":\"LandingSite\",\"position\":{\"x\":-1,\"y\":2},\"size\":{\"x\":2,\"y\":2},\"coords\":[{\"x\":-1,\"y\":2},{\"x\":-1,\"y\":3},{\"x\":0,\"y\":2},{\"x\":0,\"y\":3}]},{\"scene\":\"CaveEntrance\",\"position\":{\"x\":3,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":3,\"y\":2}]},{\"scene\":\"caveOpening\",\"position\":{\"x\":2,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":2,\"y\":2}]},{\"scene\":\"plainsPath1\",\"position\":{\"x\":1,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":1,\"y\":2}]},{\"scene\":\"UndergroundPath\",\"position\":{\"x\":3,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":3,\"y\":3}]},{\"scene\":\"AltairKettle\",\"position\":{\"x\":7,\"y\":6},\"size\":{\"x\":1,\"y\":6},\"coords\":[{\"x\":7,\"y\":6},{\"x\":7,\"y\":7},{\"x\":7,\"y\":8},{\"x\":7,\"y\":9},{\"x\":7,\"y\":10},{\"x\":7,\"y\":11}]},{\"scene\":\"BamBerry Grotto Intro\",\"position\":{\"x\":4,\"y\":6},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":4,\"y\":6}]},{\"scene\":\"ClockworkBombs\",\"position\":{\"x\":5,\"y\":6},\"size\":{\"x\":2,\"y\":1},\"coords\":[{\"x\":5,\"y\":6},{\"x\":6,\"y\":6}]},{\"scene\":\"followTheBomb\",\"position\":{\"x\":2,\"y\":6},\"size\":{\"x\":1,\"y\":2},\"coords\":[{\"x\":2,\"y\":6},{\"x\":2,\"y\":7}]},{\"scene\":\"warpZone\",\"position\":{\"x\":5,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":5,\"y\":3}]},{\"scene\":\"witchCanyon\",\"position\":{\"x\":3,\"y\":7},\"size\":{\"x\":2,\"y\":4},\"coords\":[{\"x\":3,\"y\":7},{\"x\":3,\"y\":8},{\"x\":3,\"y\":9},{\"x\":3,\"y\":10},{\"x\":4,\"y\":7},{\"x\":4,\"y\":8},{\"x\":4,\"y\":9},{\"x\":4,\"y\":10}]},{\"scene\":\"followStarChamber\",\"position\":{\"x\":3,\"y\":6},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":3,\"y\":6}]},{\"scene\":\"gardenEntrance\",\"position\":{\"x\":6,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":6,\"y\":3}]},{\"scene\":\"climbpastbombers\",\"position\":{\"x\":7,\"y\":0},\"size\":{\"x\":1,\"y\":2},\"coords\":[{\"x\":7,\"y\":0},{\"x\":7,\"y\":1}]},{\"scene\":\"gardenhub1\",\"position\":{\"x\":9,\"y\":1},\"size\":{\"x\":1,\"y\":3},\"coords\":[{\"x\":9,\"y\":1},{\"x\":9,\"y\":2},{\"x\":9,\"y\":3}]},{\"scene\":\"bombflowerHallway\",\"position\":{\"x\":8,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":8,\"y\":2}]},{\"scene\":\"chargerHall\",\"position\":{\"x\":10,\"y\":1},\"size\":{\"x\":2,\"y\":1},\"coords\":[{\"x\":10,\"y\":1},{\"x\":11,\"y\":1}]},{\"scene\":\"gardenTemplate\",\"position\":{\"x\":0,\"y\":6},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":0,\"y\":6}]},{\"scene\":\"gardenRampDown\",\"position\":{\"x\":8,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":8,\"y\":3}]},{\"scene\":\"bombberryTree\",\"position\":{\"x\":7,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":7,\"y\":2}]},{\"scene\":\"bombCrossroads\",\"position\":{\"x\":10,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":10,\"y\":3}]},{\"scene\":\"gardenStairs\",\"position\":{\"x\":11,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":11,\"y\":2}]},{\"scene\":\"gardenBombPuzzle1\",\"position\":{\"x\":11,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":11,\"y\":3}]},{\"scene\":\"movingUpToBumpBerry\",\"position\":{\"x\":12,\"y\":2},\"size\":{\"x\":1,\"y\":2},\"coords\":[{\"x\":12,\"y\":2},{\"x\":12,\"y\":3}]},{\"scene\":\"gardenReturn\",\"position\":{\"x\":9,\"y\":4},\"size\":{\"x\":2,\"y\":1},\"coords\":[{\"x\":9,\"y\":4},{\"x\":10,\"y\":4}]},{\"scene\":\"gardenRibbonBridge\",\"position\":{\"x\":7,\"y\":3},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":7,\"y\":3}]},{\"scene\":\"bumpberryTree\",\"position\":{\"x\":10,\"y\":2},\"size\":{\"x\":1,\"y\":1},\"coords\":[{\"x\":10,\"y\":2}]}]}";
        AtlasWorldMapData worldMapData = JsonUtility.FromJson<AtlasWorldMapData>(json);
        AtlasWorldMap atlasWorldMap = worldMapData.build();
        return atlasWorldMap;
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

    public static void switchScene(Vector2 dir, bool clearDoorLabel = false)
    {
        GameObject player = gameManager.Instance.player;
        
        AtlasMap fromScene = getMap();

        AtlasMap toScene = findMapByCoords(getPlayerCoords() + dir);
        if (toScene == null || toScene.fileName == "null") return;
        
        Vector2 t = (fromScene.getCenter() - toScene.getCenter()) * -SCREEN_TILES_HEIGHT;

        float startx = 0; 
        float starty = 0;

        if (dir.x != 0) {
            startx = (toScene.width * SCREEN_TILES_WIDTH * 0.5f - 0.3f) * -dir.x;
            starty = player.transform.position.y + t.y;
        } else
        {
            startx = player.transform.position.x + t.x;
            starty = (toScene.height * SCREEN_TILES_HEIGHT * 0.5f - 0.4f) * dir.y;
        }
        if (clearDoorLabel)
        {
            gameManager.Instance.currentDoorLabel = "none";
        }
        gameManager.Instance.switchScene(toScene.fileName, startx, starty);
        //Debug.Log(toScene.fileName + ": " + startx + ", " + starty);
    }

    public static void printSceneNames()
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        AtlasWorldMap worldMapData = getWorldMapData();
        worldMapData.printSceneNames();
    }

    public static Vector2 getScreenSize()
    {
        return new Vector2(SCREEN_TILES_WIDTH, SCREEN_TILES_HEIGHT);
    }

    public static Vector2 getMapPosition(string fileName = null)
    {
        if (fileName == null)
        {
            fileName = SceneManager.GetActiveScene().name;
        }
        AtlasWorldMap worldMapData = getWorldMapData();
        AtlasMap mapObj = worldMapData.maps.Find(m => m.fileName == fileName);
        if (mapObj != null)
        {
            return new Vector2(mapObj.x, mapObj.y);
        }
        return new Vector2(-1, -1);
    }

    public static List<AtlasMap> getNeighbors(string sceneName = null)
    {
        if (sceneName == null)
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        AtlasWorldMap sceneData = getWorldMapData();
        AtlasMap currentScene = getMap();
        if (currentScene == null) throw new Exception("Cannot get neighbors for scene: " + sceneName);

        List<AtlasMap> neighbors = new List<AtlasMap>();
        for (int x = 0; x < currentScene.width; x++)
        {
            neighbors.Add(findMapByCoords(currentScene.getPositionCoords() + new Vector2(x, -1)));
            neighbors.Add(findMapByCoords(currentScene.getPositionCoords() + new Vector2(x, currentScene.height)));
        }
        for (int y = 0; y < currentScene.height; y++)
        {
            neighbors.Add(findMapByCoords(currentScene.getPositionCoords() + new Vector2(-1, y)));
            neighbors.Add(findMapByCoords(currentScene.getPositionCoords() + new Vector2(currentScene.width, y)));
        }

        return neighbors;
    }

    public static Vector2 getPlayerCoords()
    {
        Vector2 playerCoords = Vector2.zero;
        Vector2 playerWorldPosition = gameManager.Instance.player.transform.position;

        AtlasMap currentMap = getMap();
        Vector2 bottomLeft = -currentMap.getExtents(true);

        playerWorldPosition.x = Mathf.Clamp(playerWorldPosition.x, bottomLeft.x, bottomLeft.x + currentMap.width * SCREEN_TILES_WIDTH);
        playerWorldPosition.y = Mathf.Clamp(playerWorldPosition.y, bottomLeft.y, bottomLeft.y + currentMap.height * SCREEN_TILES_HEIGHT);

        Vector2 d = playerWorldPosition - bottomLeft;
        Vector2 normd = new Vector2(
            (int)Mathf.Clamp(d.x / SCREEN_TILES_WIDTH, 0, currentMap.width-1),
            (int)Mathf.Clamp(d.y / SCREEN_TILES_HEIGHT, 0, currentMap.height-1));
        playerCoords.x = normd.x + currentMap.x;
        playerCoords.y = currentMap.height - 1 - normd.y + currentMap.y;

        return playerCoords;
    }

    public static AtlasMap findMapByCoords(Vector2 mapCoords)
    {
        AtlasWorldMap sceneData = getWorldMapData();
        AtlasMap rtnScene = sceneData.maps.Find(s => {return
            mapCoords.x < s.x + s.width &&
            mapCoords.x >= s.x &&
            mapCoords.y < s.y + s.height &&
            mapCoords.y >= s.y;
        });
        if (rtnScene == null)
        {
            rtnScene = new AtlasMap();
        }
        return rtnScene;
    }

    public static AtlasMap getMap(string fileName = null)
    {
        if (fileName == null)
        {
            fileName = SceneManager.GetActiveScene().name;
        }
        if (fileName == "Main Menu") return new AtlasMap();
        AtlasWorldMap worldMapData = getWorldMapData();
        //printSceneNames();
        AtlasMap currentMap = worldMapData.maps.Find(map => map.fileName == fileName) ?? throw new Exception("No WorldMap data found for Map with fileName: " + fileName);
    return currentMap;
    }
}

[Serializable]
public class AtlasMap
{
    public string fileName;
    public float x;
    public float y;
    public int height;
    public int width;

    public AtlasMap()
    {

    }

    public Vector2 getExtents(bool abs = false)
    {
        Vector2 extents = new Vector2(width, height) * 0.5f;
        if (abs) extents.Scale(AtlasSceneManager.getScreenSize());
        return extents;
    }

    public Vector2 getCenter(bool abs = false)
    {
        Vector2 center = getPositionCoords() + getSize() * 0.5f;
        if (abs) center.Scale(AtlasSceneManager.getScreenSize());
        return center;
    }

    public Vector2 getPositionCoords() {
        return new Vector2(x, y);
    }

    public Vector2 getSize() {
        return new Vector2(width, height);
    }
}

[Serializable]
public class AtlasWorldMap
{
    public List<AtlasMap> maps;

    public void printSceneNames()
    {
        foreach (AtlasMap m in maps)
        {
            Debug.Log(m.fileName + ": " + m.x + ", " + m.y);
        }
    }
}

[Serializable]
public struct AtlasWorldMapData
{
    public List<AtlasMapData> maps;

    public AtlasWorldMap build() {
        AtlasWorldMap worldMap = new AtlasWorldMap();
        List<AtlasMap> atlasMaps = maps.Select(map => {
            AtlasMap atlasMap = new AtlasMap();

            string pattern = @"(\/|\.)";
            Regex regex = new Regex(pattern);   
            string[] arr = Regex.Split(map.fileName, pattern);
            atlasMap.fileName = arr[arr.Length-3];

            int xfactor = AtlasSceneManager.SCREEN_TILES_WIDTH * AtlasSceneManager.PIXELS_PER_TILE;
            int yfactor = AtlasSceneManager.SCREEN_TILES_HEIGHT * AtlasSceneManager.PIXELS_PER_TILE;

            atlasMap.x = (map.x / xfactor);
            atlasMap.y = (map.y / yfactor);
            atlasMap.height = (map.height / yfactor);
            atlasMap.width = (map.width / xfactor);

            return atlasMap;
        }).ToList<AtlasMap>();
        worldMap.maps = atlasMaps;
        return worldMap;
    }
}

[Serializable]
public struct AtlasMapData
{
    public string fileName;
    public int x;
    public int y;
    public int height;
    public int width;
}