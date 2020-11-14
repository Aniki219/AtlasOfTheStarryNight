using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Screenshot))]
public class ScreenshotEditor : Editor {

    [MenuItem("MapEditor/Screenshot")]
    public static void takeScreenshot()
    {
        GameObject cam = Instantiate(Resources.Load<GameObject>("Prefabs/Editor/ScreenshotCamera"), Vector3.zero - Vector3.forward * 10.0f, Quaternion.identity);
        SerializedObject serialCam = new SerializedObject(cam.GetComponent<Screenshot>());
        ((Screenshot)serialCam.targetObject).TakeScreenshot();
        DestroyImmediate(cam);
    }
}
