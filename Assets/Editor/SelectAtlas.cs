using UnityEngine;
using UnityEditor;
using UnityEditorInternal; 
using System.Reflection;

//Tells the Editor class that this will be the Editor for the AnimMapper
[CreateAssetMenu(menuName = "Select Atlas")]
[ExecuteInEditMode]
public class SelectAtlas : Editor
{
    static EditorWindow inspectorInstance;
    
    [MenuItem("Select Atlas/Atlas")]
    public static void selectAtlas() {
        selectGameObject("Atlas");
    }

    [MenuItem("Select Atlas/Atlas Sprite")]
    public static void selectAtlasSprite() {
        selectGameObject("Atlas/AtlasSprite");
    }

    public static void selectGameObject(string name) {
        GameObject obj = GameObject.Find(name);
        if (obj) {
            Selection.activeGameObject = obj;
        } else {
            Debug.LogWarning("Can't find " + name);
        }
    }
}