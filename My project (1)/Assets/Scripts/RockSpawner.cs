using System.Collections;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [Header("Rock")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private float uniformRockScale = 1f;

    [Header("Spawn Area")]
    [SerializeField] private float spawnWidth = 20f;
    [SerializeField] private float spawnDepth = 20f;
    [SerializeField] private float spawnY = 0.2f;

    [Header("Spawn Settings")]
    [SerializeField] private int initialSpawnCount = 30;
    [SerializeField] private int maxRockCount = 50;
    [SerializeField] private float spawnInterval = 1.5f;

    [Header("Spacing")]
    [SerializeField] private float minimumSpacing = 1f;

    private void Start()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnSingleRock();
        }

        StartCoroutine(ContinuousSpawnRoutine());
    }

    private IEnumerator ContinuousSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            GameObject[] currentRocks = GameObject.FindGameObjectsWithTag("Rock");

            if (currentRocks.Length < maxRockCount)
            {
                SpawnSingleRock();
            }
        }
    }

    private void SpawnSingleRock()
    {
        if (rockPrefab == null)
        {
            Debug.LogError("Rock Prefab이 연결되지 않았습니다.");
            return;
        }

        int attempts = 0;
        while (attempts < 20)
        {
            attempts++;
            Vector3 randomPosition = GetRandomPosition();

            if (CanSpawn(randomPosition))
            {
                GameObject newRock = Instantiate(
                    rockPrefab,
                    randomPosition,
                    Random.rotation
                );

                newRock.transform.localScale = Vector3.one * uniformRockScale;

                return;
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        float randomX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        float randomZ = Random.Range(-spawnDepth / 2f, spawnDepth / 2f);

        return transform.position + new Vector3(randomX, spawnY, randomZ);
    }

    private bool CanSpawn(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, minimumSpacing);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Rock"))
            {
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(spawnWidth, 0.1f, spawnDepth)
        );
    }
}