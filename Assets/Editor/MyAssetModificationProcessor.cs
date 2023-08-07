using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MyAssetModificationProcessor : UnityEditor.AssetModificationProcessor
{
    public static string[] OnWillSaveAssets(string[] paths)
    {
        // Get the name of the scene to save.
        string scenePath = string.Empty;
        string sceneName = string.Empty;

        foreach (string path in paths)
        {
            if (path.Contains(".unity"))
            {
                scenePath = Path.GetDirectoryName(path);
                sceneName = Path.GetFileNameWithoutExtension(path);

                //If we are here, we are saving a Scene
                Scene scene = SceneManager.GetSceneByName(sceneName);
                GameObject[] roomObjects = scene.GetRootGameObjects();
                GameObject roomBounds = null;
                foreach (GameObject gameObject in roomObjects)
                {
                    if (gameObject.name == "RoomBounds")
                    {
                        roomBounds = gameObject;
                        break;
                    }
                }

                if (roomBounds == null) throw new System.Exception("No RoomBounds in scene: " + sceneName);

                if (SceneManager.GetSceneAt(0).name != sceneName)
                {
                    MapEditor.shiftScene(sceneName, 0, 0);
                } else
                {
                    Vector2 offset = roomBounds.transform.position;
                    if (offset.x == 0 && offset.y == 0) continue;

                    Vector2 normOffset = new Vector2(
                        offset.x / AtlasSceneManager.SCREEN_TILES_WIDTH,
                        offset.y / AtlasSceneManager.SCREEN_TILES_HEIGHT);

                    MapEditor.shiftScene(sceneName, -normOffset.x, -normOffset.y);
                }
            }
        }

        if (sceneName.Length == 0)
        {
            return paths;
        }

        return paths;
    }

}