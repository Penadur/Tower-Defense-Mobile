using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public int piercing;
    public int maxHitsAllowed = 1;
    private int hitCount = 0;
    [SerializeField] private float explosionRadius;
    [SerializeField] private bool explosive;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitCount >= maxHitsAllowed)
        {
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null) // Check for null before using the enemy reference
            {
                hitCount++;

                if (explosive)
                {
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(collision.transform.position, explosionRadius);

                    foreach (Collider2D collider in colliders)
                    {
                        Enemy otherEnemy = collider.GetComponent<Enemy>();
                        if (otherEnemy != null)
                        {
                            hitCount++;

                            otherEnemy.TakeDamage(damage);
                            otherEnemy.ShowDamage(damage);
                            otherEnemy.CheckHealthStatus();

                            if (hitCount >= maxHitsAllowed)
                            {
                                Destroy(gameObject);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    enemy.TakeDamage(damage);
                    enemy.ShowDamage(damage);
                    enemy.CheckHealthStatus();
                }
            }
            
            if (hitCount >= maxHitsAllowed)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
}
