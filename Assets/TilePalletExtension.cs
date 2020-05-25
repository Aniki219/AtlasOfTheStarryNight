using UnityEngine;
using UnityEditor;

public class TilePalletExtension : EditorWindow
{
    [MenuItem("Window/Test")]
    public static void test()
    {
    
        Debug.Log("hi1");
    }

    private void OnGUI()
    {
        //GetWindow<EditorWindow>("Tile Pallet");
        Debug.Log("hi2");
    }
}
