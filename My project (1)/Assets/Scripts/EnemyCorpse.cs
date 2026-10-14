using UnityEngine;

public class EnemyCorpse : MonoBehaviour
{
    public bool IsCollected { get; private set; }

    public void Collect()
    {
        if (IsCollected)
            return;

        IsCollected = true;

        Debug.Log(
            gameObject.name +
            " 시체가 행성에 붙음"
        );
    }
}