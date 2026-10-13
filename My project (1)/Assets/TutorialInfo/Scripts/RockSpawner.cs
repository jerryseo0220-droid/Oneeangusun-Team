using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [Header("Rock")]
    [SerializeField] private GameObject rockPrefab;

    [Header("Spawn")]
    [SerializeField] private int spawnCount = 30;

    [SerializeField] private float spawnWidth = 20f;
    [SerializeField] private float spawnDepth = 20f;

    [SerializeField] private float spawnY = 0.2f;

    [Header("Spacing")]
    [SerializeField] private float minimumSpacing = 1f;

    private List<Vector3> spawnedPositions = new List<Vector3>();

    private void Start()
    {
        SpawnRocks();
    }

    private void SpawnRocks()
    {
        if (rockPrefab == null)
        {
            Debug.LogError("Rock Prefab이 연결되지 않았습니다.");
            return;
        }

        int spawnedCount = 0;
        int attempts = 0;

        while (spawnedCount < spawnCount && attempts < spawnCount * 20)
        {
            attempts++;

            Vector3 randomPosition = GetRandomPosition();

            if (!CanSpawn(randomPosition))
                continue;

            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(0f, 360f),
                Random.Range(0f, 360f),
                Random.Range(0f, 360f)
            );

            Instantiate(
                rockPrefab,
                randomPosition,
                randomRotation
            );

            spawnedPositions.Add(randomPosition);

            spawnedCount++;
        }

        Debug.Log("돌 생성 완료 : " + spawnedCount);
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(
            -spawnWidth / 2f,
            spawnWidth / 2f
        );

        float randomZ = Random.Range(
            -spawnDepth / 2f,
            spawnDepth / 2f
        );

        return transform.position +
               new Vector3(randomX, spawnY, randomZ);
    }

    private bool CanSpawn(Vector3 position)
    {
        foreach (Vector3 otherPosition in spawnedPositions)
        {
            float distance = Vector3.Distance(
                position,
                otherPosition
            );

            if (distance < minimumSpacing)
                return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(
                spawnWidth,
                0.1f,
                spawnDepth
            )
        );
    }
}