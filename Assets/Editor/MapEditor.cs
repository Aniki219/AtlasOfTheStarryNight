using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CreateAssetMenu(menuName = "MapEditor")]
[ExecuteInEditMode]
public class MapEditor : Editor
{
    public static bool displayingNeighbors = false;

    [MenuItem("MapEditor/Display Neighbors")]
    public static void displayNeighbors()
    {
        AtlasSceneManager.getWorldMapData();
        AtlasSceneManager.getNeighbors();
        loadScene();
        displayingNeighbors = true;
    }

    [MenuItem("MapEditor/Hide Neighbors")]
    public static void hideNeighbors()
    {
        for (int i = 1; i < SceneManager.sceneCount; i++)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
        }
        displayingNeighbors = false;
    }

    [MenuItem("MapEditor/Toggle Neighbors _F6")]
    public static void toggleNeighbors()
    {
        if (displayingNeighbors)
        {
            hideNeighbors();
        } else
        {
            displayNeighbors();
        }
    }

    static void loadScene()
    {
        List<AtlasMap> neighbors = AtlasSceneManager.getNeighbors();
        AtlasMap currentScene = AtlasSceneManager.getMap();

        Debug.LogWarning("Scene loaded with " + neighbors.Count + " nieghbors");

        foreach (AtlasMap n in neighbors)
        {
            if (n.fileName != "null")
            {
                EditorSceneManager.OpenScene("Assets/Scenes/WorldMap/" + n.fileName + ".unity", OpenSceneMode.Additive);
                Vector2 d = (n.getSize() + currentScene.getSize())*0.5f;
                Vector2 t = (n.getCenter() - currentScene.getCenter());
                shiftScene(n.fileName, t.x, -t.y);
            }
        }
    }

    public static void shiftScene(string name, float xdir, float ydir)
    {
        Debug.LogWarning("Shift Scene");
        Scene currentScene = SceneManager.GetSceneAt(0);
        Scene rightScene = SceneManager.GetSceneByName(name);

        GameObject[] gameObjectsCurrent = currentScene.GetRootGameObjects();
        GameObject[] gameObjectsRight = rightScene.GetRootGameObjects();

        BoxCollider2D roomBoundsCurrent = getRoomBounds(gameObjectsCurrent);
        BoxCollider2D roomBoundsRight = getRoomBounds(gameObjectsRight);

        float rb1x = roomBoundsCurrent.offset.x + roomBoundsCurrent.transform.position.x;
        float rb1w = roomBoundsCurrent.size.x;
        float rb2x = roomBoundsRight.offset.x + roomBoundsRight.transform.position.x;
        float rb2w = roomBoundsRight.size.x;
        float dx = rb1x + xdir*(rb1w + rb2w) / 2.0f - rb2x;

        float rb1y = roomBoundsCurrent.offset.y + roomBoundsCurrent.transform.position.y;
        float rb1h = roomBoundsCurrent.size.y;
        float rb2y = roomBoundsRight.offset.y + roomBoundsRight.transform.position.y;
        float rb2h = roomBoundsRight.size.y;
        float dy = rb1y + ydir*(rb1h + rb2h) / 2.0f - rb2y;

        foreach (GameObject g in gameObjectsRight)
        {
            g.transform.position += Vector3.right * dx + Vector3.up * dy;
        }
    }

    static BoxCollider2D getRoomBounds(GameObject[] gameObjects)
    {
        foreach(GameObject g in gameObjects)
        {
            if (g.name == "RoomBounds")
            {
                return g.GetComponent<BoxCollider2D>();
            }
        }
        throw new System.Exception("No RoomBounds for room: " + gameObjects[0].scene.name);
    }
}
