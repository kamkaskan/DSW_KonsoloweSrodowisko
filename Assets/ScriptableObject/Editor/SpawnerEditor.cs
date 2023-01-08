using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
namespace JK.ObjectSpawner.Editor
{
    [EditorTool("Object Spawner")]
    public class ObjectSpawnerEditorTool : EditorTool
    {
        [NonSerialized] private Rect toolWindowRect = new Rect(10, 0,
        260f, 0f);
        private Spawner selectedSpawner;
        [NonSerialized] private Spawner selectedSpawnerMemory; //If changed, than update spawn count
        private float spawnRadius = 2;
        private bool onlyEdge = false; //New Spawn method, spawn only on circle edge
        private bool overrideSpawnCount = false; //An option to change spawn count
        private int overridenSpawnCount = 0;
        [NonSerialized] private bool mouseDown;
        [NonSerialized] private Vector2 mouseDownPosition;
        [NonSerialized] private GUIContent toolIcon;
        public override GUIContent toolbarIcon
        {
            get
            {
                if (toolIcon == null) toolIcon = EditorGUIUtility.IconContent("GameObject On Icon", "Object Spawner");
                return toolIcon;
            }
        }
        public void DrawToolWindow(int id)
        {
            selectedSpawner = EditorGUILayout.
            ObjectField(selectedSpawner, typeof(Spawner), false) as Spawner;
            spawnRadius = EditorGUILayout.FloatField("Radius", spawnRadius);
            onlyEdge = EditorGUILayout.Toggle("Only Edge", onlyEdge);
            overrideSpawnCount = EditorGUILayout.Toggle("Override Spawn Count", overrideSpawnCount);
            if (overrideSpawnCount) 
            {
                overridenSpawnCount = EditorGUILayout.IntField("Spawn Count", overridenSpawnCount);
            }
        }
        public override void OnToolGUI(EditorWindow window)
        {
            if (selectedSpawnerMemory != selectedSpawner) 
            {
                selectedSpawnerMemory = selectedSpawner;
                if (selectedSpawner != null) overridenSpawnCount = selectedSpawner.spawnCount;
            }
            toolWindowRect.y = window.position.height - toolWindowRect.
            height - 10;
            toolWindowRect = GUILayout.Window(45, toolWindowRect,
            DrawToolWindow, "Object Spawner");
            // Event logic
            var ray = HandleUtility.GUIPointToWorldRay(mouseDown ?
            mouseDownPosition : Event.current.mousePosition);
            bool hitGround = Physics.Raycast(ray, out var result, 100);
            if (hitGround)
            {
                Handles.DrawWireDisc(result.point, Vector3.up, spawnRadius);
            }

            var controlId = EditorGUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                if (Event.current.button == 0 && Event.current.modifiers == EventModifiers.None)
                {
                    GUIUtility.hotControl = controlId;
                    mouseDown = true;
                    mouseDownPosition = Event.current.mousePosition;
                    Event.current.Use();
                }
                break;
                case EventType.MouseDrag:
                if (mouseDown)
                {
                    spawnRadius += EditorGUIUtility.PixelsToPoints(Event.current.delta).x / 100;
                    window.Repaint();
                }
                break;
                case EventType.MouseMove:
                window.Repaint();
                break;
                case EventType.MouseLeaveWindow:
                case EventType.MouseUp:
                if (mouseDown && hitGround && selectedSpawner)
                {
                    selectedSpawner.SpawnObjects(result.point, spawnRadius, onlyEdge, overrideSpawnCount ? overridenSpawnCount : 0);
                }
                mouseDown = false;
                break;
            }
        }
    }
}