using UnityEngine;
using UnityEditor;
using UnityEditorInternal; 

//Tells the Editor class that this will be the Editor for the AnimMapper
[CustomEditor(typeof(AnimMapper))]
public class AnimMapperEditor : Editor
{
    //The array property we will edit
    SerializedProperty stateAnims;

    //The Reorderable list we will be working with
    ReorderableList list;

    private void OnEnable()
    {
        //Gets the stateAnims property in AnimMapper so we can access it. 
        stateAnims = serializedObject.FindProperty("stateAnims");

        //Initialises the ReorderableList. We are creating a Reorderable List from the "stateAnims" property. 
        //In this, we want a ReorderableList that is draggable, with a display header, with add and remove buttons        
        list = new ReorderableList(serializedObject, stateAnims, true, true, true, true);

        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawHeader;

    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {        
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); //The element in the list

        // Create a property field and label field for each property. 

        // The 'mobs' property. Since the enum is self-evident, I am not making a label field for it. 
        // The property field for mobs (width 100, height of a single line)
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), 
            element.FindPropertyRelative("state"),
            GUIContent.none
        ); 

        // The 'level' property
        // The label field for level (width 100, height of a single line)
        //EditorGUI.LabelField(new Rect(rect.x + 120, rect.y, 100, EditorGUIUtility.singleLineHeight), "Start");

        //The property field for level. Since we do not need so much space in an int, width is set to 20, height of a single line.
        EditorGUI.PropertyField(
            new Rect(rect.x + 110, rect.y, 120, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("startAnim"),
            GUIContent.none
        ); 


        // The 'quantity' property
        // The label field for quantity (width 100, height of a single line)
        //EditorGUI.LabelField(new Rect(rect.x + 200, rect.y, 100, EditorGUIUtility.singleLineHeight), "Update");

        //The property field for quantity (width 20, height of a single line)
        EditorGUI.PropertyField(
            new Rect(rect.x + 240, rect.y, 120, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("updateAnim"),
            GUIContent.none
        );        

    }

    void DrawHeader(Rect rect)
    {
        string name = "stateAnims";
        EditorGUI.LabelField(rect, name);
    }

    //This is the function that makes the custom editor work
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}