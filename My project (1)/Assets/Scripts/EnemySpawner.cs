using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Wave Settings")]
    [SerializeField] private float waveDuration = 30f;           // 웨이브가 넘어가는 시간 (초)
    [SerializeField] private int baseMaxEnemy = 10;              // 1웨이브의 최대 적 수
    [SerializeField] private int maxEnemyIncrease = 5;           // 웨이브마다 증가하는 최대 적 수
    [SerializeField] private float initialSpawnInterval = 3f;    // 초기 스폰 주기
    [SerializeField] private float spawnIntervalDecrease = 0.2f; // 웨이브마다 짧아지는 스폰 주기
    [SerializeField] private float minSpawnInterval = 0.5f;      // 최소 스폰 주기 한계

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

        // 초기 웨이브 세팅
        currentMaxEnemy = baseMaxEnemy;
        currentSpawnInterval = initialSpawnInterval;

        // UI에 초기 웨이브 반영
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateWave(currentWave);
        }

        StartCoroutine(ContinuousSpawnRoutine());
        StartCoroutine(WaveRoutine()); // 시간마다 웨이브를 올리는 코루틴 시작
    }

    // 웨이브를 지속적으로 올리는 코루틴
    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveDuration);

            currentWave++;
            currentMaxEnemy += maxEnemyIncrease;

            // 스폰 주기를 짧게 만들되 최소값 밑으로는 떨어지지 않게 함
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

            // 현재 한계치보다 적이 적을 때만 소환
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