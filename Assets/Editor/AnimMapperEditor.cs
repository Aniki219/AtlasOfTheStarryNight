using UnityEngine;
using UnityEditor;
using UnityEditorInternal; 
using System.Reflection;

//Tells the Editor class that this will be the Editor for the AnimMapper
[CreateAssetMenu(menuName = "AnimMapper")]
[ExecuteInEditMode]
[CustomEditor(typeof(AnimMapper))]
public class AnimMapperEditor : Editor
{
    static EditorWindow inspectorInstance;
    
    [MenuItem("AnimMapper/Open Mapper Window")]
    public static void toggleMapperWhilePlaying() {
        toggleMapper(false);
    }

    [MenuItem("AnimMapper/Open Mapper Window _F5")]
    public static void toggleMapper(bool test = true) {
        if(!test && EditorApplication.isPlaying ) return;
        if (inspectorInstance != null) {
            inspectorInstance.Close();
            inspectorInstance = null;
            return;
        }
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        inspectorInstance = ScriptableObject.CreateInstance(inspectorType) as EditorWindow;
        inspectorInstance.Show();

        var prevSelection = Selection.activeGameObject;

        Selection.activeObject = Resources.LoadAll<AnimMapper>("Managers")[0];
        var isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
        isLocked.GetSetMethod().Invoke(inspectorInstance, new object[] { true });

        Selection.activeGameObject = prevSelection;
    }


    //The array property we will edit
    SerializedProperty stateAnims;

    //The Reorderable list we will be working with
    ReorderableList list;

    int fieldWidth = 150;
    int fieldSpacing = 10;

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

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), 
            element.FindPropertyRelative("state"),
            GUIContent.none
        ); 

        EditorGUI.PropertyField(
            new Rect(rect.x + fieldWidth + fieldSpacing, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("startAnim"),
            GUIContent.none
        ); 

        EditorGUI.PropertyField(
            new Rect(rect.x + 2*(fieldWidth + fieldSpacing), rect.y, fieldWidth, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("updateAnim"),
            GUIContent.none
        );  

        EditorGUI.PropertyField(
            new Rect(rect.x + 3*(fieldWidth + fieldSpacing), rect.y, fieldWidth, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("exitAnim"),
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