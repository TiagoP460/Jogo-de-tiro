using UnityEngine;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        [Header("Spawn")]
        public Transform position;
        public GameObject targetPrefab;
        public int quantity = 1;

        [Header("Ajustes extras")]
        public Vector3 positionOffset = Vector3.zero;
        public Vector3 rotationOffset = Vector3.zero;
        public Vector3 scaleMultiplier = Vector3.one;

        [Header("Movimento")]
        public bool moveHorizontal = false;
        public bool moveVertical = false;
        public float moveSpeed = 3f;
        public float moveRange = 2f;

        [Header("Status")]
        public int health = 1;
        public int pointsValue = 1;
    }

    public List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    private List<GameObject> spawnedTargets = new List<GameObject>();

    void Start()
    {
        SpawnAllTargets();
    }

    void Update()
    {
        spawnedTargets.RemoveAll(target => target == null);

        foreach (SpawnPoint point in spawnPoints)
        {
            if (point.position == null || point.targetPrefab == null)
            {
                continue;
            }

            int currentCount = 0;

            foreach (GameObject target in spawnedTargets)
            {
                if (target == null)
                {
                    continue;
                }

                Target targetScript = target.GetComponentInChildren<Target>();

                if (targetScript != null && targetScript.spawnPoint == point)
                {
                    currentCount++;
                }
            }

            while (currentCount < point.quantity)
            {
                SpawnTarget(point);
                currentCount++;
            }
        }
    }

    void SpawnAllTargets()
    {
        foreach (SpawnPoint point in spawnPoints)
        {
            for (int i = 0; i < point.quantity; i++)
            {
                SpawnTarget(point);
            }
        }
    }

    void SpawnTarget(SpawnPoint point)
    {
        if (point.position == null)
        {
            Debug.LogWarning("SpawnPoint sem Position.");
            return;
        }

        if (point.targetPrefab == null)
        {
            Debug.LogWarning("SpawnPoint sem Target Prefab.");
            return;
        }

        Vector3 finalPosition = point.position.position + point.position.TransformDirection(point.positionOffset);

        Quaternion finalRotation =
            point.targetPrefab.transform.rotation *
            Quaternion.Euler(point.rotationOffset);

        GameObject target = Instantiate(
            point.targetPrefab,
            finalPosition,
            finalRotation
        );

        target.name = "Target_Spawnado";

        Vector3 prefabScale = point.targetPrefab.transform.localScale;

        target.transform.localScale = new Vector3(
            prefabScale.x * point.scaleMultiplier.x,
            prefabScale.y * point.scaleMultiplier.y,
            prefabScale.z * point.scaleMultiplier.z
        );

        Target targetScript = target.GetComponentInChildren<Target>();

        if (targetScript != null)
        {
            targetScript.spawnPoint = point;
            targetScript.moveHorizontal = point.moveHorizontal;
            targetScript.moveVertical = point.moveVertical;
            targetScript.moveSpeed = point.moveSpeed;
            targetScript.moveRange = point.moveRange;
            targetScript.health = point.health;
            targetScript.pointsValue = point.pointsValue;
        }
        else
        {
            Debug.LogWarning("O prefab do alvo não tem o script Target.");
        }

        spawnedTargets.Add(target);
    }
}