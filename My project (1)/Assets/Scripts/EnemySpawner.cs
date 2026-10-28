using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Wave Settings")]
    [SerializeField] private float waveDuration = 30f;
    [SerializeField] private int baseMaxEnemy = 10;
    [SerializeField] private int maxEnemyIncrease = 5;
    [SerializeField] private float initialSpawnInterval = 3f;
    [SerializeField] private float spawnIntervalDecrease = 0.2f;
    [SerializeField] private float minSpawnInterval = 0.5f;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnHeight = 1f;

    [Header("Spawn Area (Player Centered)")]
    [SerializeField] private float minSpawnRadius = 15f;
    [SerializeField] private float maxSpawnRadius = 30f;

    private Transform player;
    private int currentWave = 1;
    private int currentMaxEnemy;
    private float currentSpawnInterval;

    private void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }

        currentMaxEnemy = baseMaxEnemy;
        currentSpawnInterval = initialSpawnInterval;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateWave(currentWave);
        }

        StartCoroutine(ContinuousSpawnRoutine());
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveDuration);

            currentWave++;
            currentMaxEnemy += maxEnemyIncrease;

            currentSpawnInterval -= spawnIntervalDecrease;
            if (currentSpawnInterval < minSpawnInterval)
            {
                currentSpawnInterval = minSpawnInterval;
            }

            Debug.Log($"--- 웨이브 {currentWave} 시작! 최대 적 수: {currentMaxEnemy}, 스폰주기: {currentSpawnInterval}초 ---");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateWave(currentWave);
            }
        }
    }

    private IEnumerator ContinuousSpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);

            if (player == null) continue;

            GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (currentEnemies.Length < currentMaxEnemy)
            {
                SpawnSingleEnemy();
            }
        }
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("Enemy Prefab이 배열에 없습니다.");
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[randomIndex];

        Vector3 randomPos = GetRandomSpawnPosition();

        Instantiate(selectedPrefab, randomPos, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector3 spawnPos = player.position + new Vector3(randomDir.x, 0, randomDir.y) * randomDistance;
        spawnPos.y = spawnHeight;

        return spawnPos;
    }
}