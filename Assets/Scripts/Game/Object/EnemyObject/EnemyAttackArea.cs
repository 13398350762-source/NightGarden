using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    private BaseEnemyObject enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<BaseEnemyObject>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemy != null) enemy.AttackPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (enemy != null) enemy.AttackPlayer(other);
    }
}
