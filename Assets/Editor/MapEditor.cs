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
        EditorSceneManager.OpenScene("Assets/Scenes/pinkFruitTree.unity", OpenSceneMode.Additive);

        Scene currentScene = SceneManager.GetSceneAt(0);
        Scene rightScene = SceneManager.GetSceneByName("pinkFruitTree");

        GameObject[] gameObjectsCurrent = currentScene.GetRootGameObjects();
        GameObject[] gameObjectsRight = rightScene.GetRootGameObjects();

        BoxCollider2D roomBoundsCurrent = getRoomBounds(gameObjectsCurrent);
        BoxCollider2D roomBoundsRight = getRoomBounds(gameObjectsRight);

        float rb1x = roomBoundsCurrent.offset.x + roomBoundsCurrent.transform.position.x;
        float rb1w = roomBoundsCurrent.size.x;
        float rb2x = roomBoundsRight.offset.x + roomBoundsRight.transform.position.x;
        float rb2w = roomBoundsRight.size.x;
        float dx = rb1x + (rb1w + rb2w) / 2.0f - rb2x;

        foreach (GameObject g in gameObjectsRight)
        {
            g.transform.position += Vector3.right * dx;
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
