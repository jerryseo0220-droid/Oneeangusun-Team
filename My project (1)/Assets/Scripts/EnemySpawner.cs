using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs; // 근거리, 원거리 적 프리팹을 모두 넣을 수 있는 배열

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f;  // 생성 주기 (초)
    [SerializeField] private int maxEnemyCount = 15;    // 맵에 유지할 최대 적 개수 (렉 방지용)
    [SerializeField] private float spawnHeight = 1f;    // 적이 생성될 높이 (바닥에 파묻히면 이 값을 늘리세요)

    [Header("Spawn Area (Player Centered)")]
    [SerializeField] private float minSpawnRadius = 15f; // 플레이어로부터 최소 거리 (이 거리 밖에서 생성됨)
    [SerializeField] private float maxSpawnRadius = 30f; // 플레이어로부터 최대 거리

    private Transform player;

    private void Start()
    {
        // 플레이어 찾기
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }

        // 스폰 코루틴 시작
        StartCoroutine(ContinuousSpawnRoutine());
    }

    private IEnumerator ContinuousSpawnRoutine()
    {
        // 무한 루프로 게임 끝날 때까지 반복
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (player == null) continue;

            // 현재 맵에 있는 "Enemy" 태그를 가진 적의 개수 확인
            // (주의: 적 프리팹들의 Tag를 꼭 "Enemy"로 설정해주세요!)
            GameObject[] currentEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            // 최대 개수를 넘지 않았을 때만 새로운 적 소환
            if (currentEnemies.Length < maxEnemyCount)
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

        // 배열에 넣은 프리팹 중 하나를 무작위로 선택 (근거리 or 원거리)
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[randomIndex];

        // 플레이어 주변 도넛 모양 범위 내의 랜덤 위치 계산
        Vector3 randomPos = GetRandomSpawnPosition();

        // 적 생성
        Instantiate(selectedPrefab, randomPos, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // 반지름이 1인 원 안의 랜덤한 2D 좌표를 구한 뒤, 방향(길이가 1인 벡터)만 가져옴
        Vector2 randomDir = Random.insideUnitCircle.normalized;

        // 설정한 최소 거리와 최대 거리 사이의 랜덤한 거리를 뽑음
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);

        // 플레이어 위치를 기준으로 '방향 * 거리'를 더해 최종 스폰 위치 결정
        Vector3 spawnPos = player.position + new Vector3(randomDir.x, 0, randomDir.y) * randomDistance;

        // 높이를 0이 아닌 설정한 높이(spawnHeight)로 고정 (추가/수정된 부분)
        spawnPos.y = spawnHeight;

        return spawnPos;
    }
}