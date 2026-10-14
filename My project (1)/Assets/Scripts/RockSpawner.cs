using System.Collections;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [Header("Rock")]
    [SerializeField] private GameObject rockPrefab;

    [Header("Spawn Area")]
    [SerializeField] private float spawnWidth = 20f;
    [SerializeField] private float spawnDepth = 20f;
    [SerializeField] private float spawnY = 0.2f;

    [Header("Spawn Settings")]
    [SerializeField] private int initialSpawnCount = 30; // 시작 시 한 번에 생성할 개수
    [SerializeField] private int maxRockCount = 50;      // 맵에 유지할 최대 돌 개수 (렉 방지용)
    [SerializeField] private float spawnInterval = 1.5f; // 돌 추가 생성 주기 (초 단위)

    [Header("Spacing")]
    [SerializeField] private float minimumSpacing = 1f;

    private void Start()
    {
        // 1. 게임 시작 시 초기 개수만큼 먼저 소환
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnSingleRock();
        }

        // 2. 이후 주기적으로 돌을 계속 생성하는 코루틴 시작
        StartCoroutine(ContinuousSpawnRoutine());
    }

    private IEnumerator ContinuousSpawnRoutine()
    {
        // 무한 루프로 게임 끝날 때까지 반복
        while (true)
        {
            // 설정한 시간(spawnInterval)만큼 대기
            yield return new WaitForSeconds(spawnInterval);

            // 현재 맵에 떨어져 있는 "Rock" 태그를 가진 돌의 개수 확인
            // (PlanetController에서 행성에 붙은 돌은 태그를 지우므로 카운트되지 않음)
            GameObject[] currentRocks = GameObject.FindGameObjectsWithTag("Rock");

            // 최대 개수를 넘지 않았을 때만 새로운 돌 소환
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
        // 최대 20번 랜덤 위치를 찾아봄
        while (attempts < 20)
        {
            attempts++;
            Vector3 randomPosition = GetRandomPosition();

            // 생성 가능한 위치라면 소환하고 함수 종료
            if (CanSpawn(randomPosition))
            {
                Instantiate(
                    rockPrefab,
                    randomPosition,
                    Random.rotation // 방향을 무작위로 회전시켜서 자연스럽게 배치
                );
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
        // 물리 엔진을 사용해 생성하려는 위치 주변(minimumSpacing)에 다른 콜라이더가 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(position, minimumSpacing);

        foreach (Collider col in colliders)
        {
            // 주변에 다른 'Rock'이 있다면 겹치므로 생성 불가 판정
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