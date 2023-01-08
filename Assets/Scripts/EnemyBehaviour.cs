using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class EnemyBehaviour : MonoBehaviour
{
    public float health;
    public float attackPt;
    public string name;
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyBehaviour)), CanEditMultipleObjects]
public class EnemyBehaviourEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Select all enemies"))
        {
            var allEnemyBehaviour = GameObject.FindObjectsOfType
           <EnemyBehaviour>();
            var allEnemyGameObjects = allEnemyBehaviour.Select(enemy => enemy.gameObject).ToArray();
            Selection.objects = allEnemyGameObjects;
        }
        
        if (GUILayout.Button("Select enemies with same name"))
        {
            var allEnemyBehaviour = GameObject.FindObjectsOfType
           <EnemyBehaviour>();
            var allEnemyGameObjects = allEnemyBehaviour.Where(enemy => enemy.name == serializedObject.FindProperty("name").stringValue).Select(enemy => enemy.gameObject).ToArray();
            Selection.objects = allEnemyGameObjects;
        }
        
        if (GUILayout.Button("Clear selection"))
            Selection.objects = new Object[] { (target as EnemyBehaviour).gameObject };
             var cachedColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.green;
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Disable/Enable all enemy", GUILayout.Height(40)))
        {
            foreach (var enemy in GameObject.FindObjectsOfType<EnemyBehaviour>())
            {
                enemy.gameObject.SetActive(!enemy.gameObject.activeSelf);
            }
            foreach (var enemy in GameObject.FindObjectsOfType<EnemyBehaviour>())
            {
                Undo.RecordObject(enemy.gameObject, "Disable/Enableenemy");
               
                enemy.gameObject.SetActive(!enemy.gameObject.activeSelf);
            }
        }
        if (GUILayout.Button("Set this stats to all enemies", GUILayout.Height(40)))
        {
            foreach (var enemy in GameObject.FindObjectsOfType<EnemyBehaviour>())
            {
                enemy.health = serializedObject.FindProperty("health").floatValue;
                enemy.attackPt = serializedObject.FindProperty("attackPt").floatValue;
            }
        }
        if (GUILayout.Button("Update all names", GUILayout.Height(40)))
        {
            foreach (var enemy in GameObject.FindObjectsOfType<EnemyBehaviour>())
            {
                enemy.gameObject.name = $"EnemyBehaviour_{enemy.name}";
            }
        }
        GUI.backgroundColor = Color.gray;
        if (GUILayout.Button("Set this stats to same name enemies", GUILayout.Height(40)))
        {
            foreach (var enemy in GameObject.FindObjectsOfType<EnemyBehaviour>().Where(enemy => enemy.name == serializedObject.FindProperty("name").stringValue))
            {
                enemy.health = serializedObject.FindProperty("health").floatValue;
                enemy.attackPt = serializedObject.FindProperty("attackPt").floatValue;
            }
        }

        GUI.backgroundColor = cachedColor;
        GUILayout.Width(40);        
        serializedObject.Update();
        var health = serializedObject.FindProperty("health");
        var attackPt = serializedObject.FindProperty("attackPt");
        EditorGUILayout.PropertyField(health);
        EditorGUILayout.PropertyField(attackPt);
        serializedObject.ApplyModifiedProperties();
        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            var temp = EditorGUILayout.Slider("Health", health.floatValue, 0, 10);
            if (changeScope.changed)
            {
                health.floatValue = temp;
            }
        }
        if (health.floatValue < 0)
        {
            EditorGUILayout.HelpBox("Uwaga Kondycja", MessageType.Warning);
        }
    }
}
#endif