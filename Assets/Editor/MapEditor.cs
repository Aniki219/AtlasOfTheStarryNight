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
    [MenuItem("MapEditor/Display Neighbors")]
    public static void displayNeighbors()
    {
        loadScene();
    }

    [MenuItem("MapEditor/Hide Neighbors")]
    public static void hideNeighbors()
    {
        for (int i = 1; i < SceneManager.sceneCount; i++)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
        }
    }

    static void loadScene()
    {
        List<AtlasScene> neighbors = AtlasSceneManager.getNeighbors();

        for (int i = 0; i < 4; i++)
        {
            AtlasScene scene = neighbors[i];
            if (scene.name != "null")
            {
                EditorSceneManager.OpenScene("Assets/Scenes/WorldMap/" + scene.name + ".unity", OpenSceneMode.Additive);

                if (i == 0) shiftScene(scene.name, 0, 1);
                if (i == 1) shiftScene(scene.name, -1, 0);
                if (i == 2) shiftScene(scene.name, 1, 0);
                if (i == 3) shiftScene(scene.name, 0, -1);
            }
        }
    }

    static void shiftScene(string name, int xdir, int ydir)
    {
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
