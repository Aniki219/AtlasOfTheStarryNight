using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Screenshot))]
public class ScreenshotEditor : Editor {

    [MenuItem("MapEditor/Screenshot")]
    public static void takeScreenshot()
    {
        //Either use the ScreenshotCamera already in the room, or make a new one
        GameObject cam = GameObject.Find("ScreenshotCamera");
        bool destroyCam = false;
        if (cam ==  null)
        {
            //If we make a new one, we'll destroy it right after
            destroyCam = true;
            cam = Instantiate(Resources.Load<GameObject>("Prefabs/Editor/ScreenshotCamera"), Vector3.zero - Vector3.forward * 10.0f, Quaternion.identity);
        }

        //Make sure there are RoomBounds to use as reference
        GameObject roomBounds = GameObject.Find("RoomBounds");
        if (roomBounds == null) throw new System.Exception("No Room Bounds!");

        //Set the resolution based on the size of the room
        Vector3 boundScale = roomBounds.transform.localScale;
        cam.GetComponent<Camera>().orthographicSize = 4.5f * boundScale.y;
        cam.GetComponent<Screenshot>().setResolutionScale(boundScale.x, boundScale.y);

        //Take the screenshot. We serialize so that the resolution info is correct
        SerializedObject serialCam = new SerializedObject(cam.GetComponent<Screenshot>());
        ((Screenshot)serialCam.targetObject).TakeScreenshot();

        //Destory the camera if we created it just now
        if (destroyCam) DestroyImmediate(cam);
    }
}
