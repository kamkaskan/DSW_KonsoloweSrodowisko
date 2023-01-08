using UnityEngine;
namespace JK.ObjectSpawner
{
    [CreateAssetMenu(fileName = "Spawner", menuName = "ObjectSpawner/Spawner")]
    public class Spawner : ScriptableObject {
        public int spawnCount = 10;
        public GameObject[] prefabs;
        [Header("Auto Ground")]
        public bool autoGround = true;
        public float raycastHeight = 10;
        public float groundOffset;
        public void SpawnObjects(Vector3 position, float radius, bool onlyEdge = false, int overrideSpawnCount = 0) {
            var stampParentObject = new GameObject("Stamp - " + name);
            stampParentObject.transform.position = position;
            #if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(stampParentObject, "Create " + stampParentObject.name);
            #endif
            int prefabSpawnCount = (overrideSpawnCount == 0) ? spawnCount : overrideSpawnCount;
            for (int i = 0; i < prefabSpawnCount; i++)
            {
                var randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
                var randomPosition2D = onlyEdge ? Random.insideUnitCircle.normalized : Random.insideUnitCircle;
                randomPosition2D *= radius;
                var targetPositionX = position.x + randomPosition2D.x;
                var targetPositionZ = position.z + randomPosition2D.y;
                var targetPositionY = position.y;
                var targetPosition = new Vector3(targetPositionX, targetPositionY, targetPositionZ);
                if (autoGround)
                {
                var raycastOrigin = targetPosition;
                raycastOrigin.y += raycastHeight;
                if (Physics.Raycast(raycastOrigin, Vector3.down, out var result, raycastHeight))
                targetPosition.y = result.point.y + groundOffset;
                }
                #if UNITY_EDITOR
                var target = UnityEditor.PrefabUtility.
                InstantiatePrefab(randomPrefab, stampParentObject.transform) as GameObject;
                target.transform.position = targetPosition;
                #else
                Instantiate(randomPrefab, targetPosition, Quaternion.identity, stampParentObject.transform);
                #endif
            } 
        }
    }
}