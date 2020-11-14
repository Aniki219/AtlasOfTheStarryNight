using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScreenShotter : Editor
{
    [MenuItem("MapEditor/Take Screenshot")]
    public static void takeScreenShot() {
        GameObject roomBounds = GameObject.Find("RoomBounds");
        if (!roomBounds) throw new Exception("No Room Bounds for Screenshotter");
        BoxCollider2D bounds = roomBounds.GetComponent<BoxCollider2D>();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        DestroyImmediate(tex);

        File.WriteAllBytes(Application.dataPath + "/../MapEditor/screenshots/SavedScreen.png", bytes);
    }

}
