using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    private float damage;
    private LayerMask enemyLayer;
    private LayerMask groundLayer;
    private Rigidbody2D rb;

    public void Setup(float dmg, LayerMask enemyMask, LayerMask groundMask, Vector2 velocity)
    {
        damage = dmg;
        enemyLayer = enemyMask;
        groundLayer = groundMask;

        if (TryGetComponent(out rb))
        {
            rb.gravityScale = 0;
            rb.linearVelocity = velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            if (collision.TryGetComponent<HealthComponent>(out var targetHealth))
            {
                targetHealth.TakeDamage(damage);
            }
        }

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
