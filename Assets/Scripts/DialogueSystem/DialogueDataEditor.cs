namespace DefaultNamespace.DialogueSystem
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(DialogueData))]
    public class DialogueDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            EditorGUILayout.PropertyField(serializedObject.FindProperty("entryPointID"));
        
            var branches = serializedObject.FindProperty("branches");
            EditorGUILayout.PropertyField(branches, true);
        
            if (GUILayout.Button("Add New Branch"))
            {
                branches.arraySize++;
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}