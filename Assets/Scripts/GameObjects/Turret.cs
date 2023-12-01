using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject turretHead;
    //[SerializeField] private GameObject projectileSpawn;
    public int level = 1;
    public int[] damage;
    public int[] sellingPrice;
    public int[] upgradePrice;
    //public GameObject range;
    public float detectionRadius;
    public float[] fireRate;
    public float projectileSpeed;

    public bool isAssisted;
    private int assistLevel;

    private Quaternion targetRotation;

    [SerializeField] private bool restrictRotation = false;

    [SerializeField] float animationExitTime;
    [SerializeField] private Transform targetEnemy;
    private Transform[] targetEnemies;

    [SerializeField] private bool attackAll;
    [SerializeField] private bool attackClosest;
    [SerializeField] private bool attackHighestHealth;
    private Animator animator;

    private Vector3 lastKnownEnemyPosition;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(ShootInterval());
        detectionRadius += 0.15f;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyRotation();
        LookForHelperTurrets();
    }
    IEnumerator ShootInterval()
    {
        while (true)
        {
            if (attackAll)
            {
                targetEnemies = FindAllEnemiesInRange();
                if (targetEnemies.Length > 0)
                {
                    ShootAtAllEnemies(targetEnemies);
                }
            }
            else
            {
                targetEnemy = FindTargetEnemy();
                if (targetEnemy != null)
                {
                    ShootAtEnemy(targetEnemy);
                }
            }

            yield return new WaitForSeconds(fireRate[level - 1]);
        }
    }
    private Transform FindTargetEnemy()
    {
        if (restrictRotation)
        {
            return FindDirectionEnemy();
        }
        else if (attackClosest)
        {
            return FindClosestEnemy();
        }
        else if (attackHighestHealth)
        {
            return FindHighestHealthEnemy();
        }
        else
        {
            // Check if the current target is in range
            if (targetEnemy != null && IsEnemyInRange(targetEnemy))
            {
                return targetEnemy;
            }
            else
            {
                // If the current target is out of range or null, search for a new one
                return LockOntoEnemy();
            }
        }
    }

    private bool IsEnemyInRange(Transform enemyTransform)
    {
        if (enemyTransform == null)
            return false;

        float distance = Vector3.Distance(transform.position, enemyTransform.position);
        return distance <= detectionRadius;
    }
    private Transform LockOntoEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        float closestDistanceSqr = Mathf.Infinity;
        Transform nearestEnemy = null;
        Vector3 currentPosition = transform.position;

        foreach (Collider2D collider in colliders)
        {
            if (collider != null && collider.gameObject.CompareTag("Enemy"))
            {
                Transform enemyTransform = collider.transform;
                Vector3 directionToEnemy = enemyTransform.position - currentPosition;
                float distanceSqrToEnemy = directionToEnemy.sqrMagnitude;

                if (distanceSqrToEnemy < closestDistanceSqr && IsEnemyInRange(enemyTransform))
                {
                    nearestEnemy = enemyTransform;
                    closestDistanceSqr = distanceSqrToEnemy;
                }
            }
        }

        return nearestEnemy;
    }
    private Transform[] FindAllEnemiesInRange()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        List<Transform> enemies = new List<Transform>();

        foreach (Collider2D collider in colliders)
        {
            if (collider != null && collider.gameObject.CompareTag("Enemy"))
            {
                Transform enemyTransform = collider.transform;
                enemies.Add(enemyTransform);
            }
        }

        return enemies.ToArray();
    }

    private Transform FindClosestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        float closestDistanceSqr = Mathf.Infinity;
        Transform closestEnemy = null;
        Vector3 currentPosition = transform.position;

        foreach (Collider2D collider in colliders)
        {
            if (collider != null && collider.gameObject.CompareTag("Enemy"))
            {
                Transform enemyTransform = collider.transform;
                Vector3 directionToEnemy = enemyTransform.position - currentPosition;
                float distanceSqrToEnemy = directionToEnemy.sqrMagnitude;

                if (distanceSqrToEnemy < closestDistanceSqr)
                {
                    closestEnemy = enemyTransform;
                    closestDistanceSqr = distanceSqrToEnemy;
                }
            }
        }

        return closestEnemy;
    }

    private Transform FindHighestHealthEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        float highestHealth = 0;
        Transform highestHealthEnemy = null;

        foreach (Collider2D collider in colliders)
        {
            if (collider != null && collider.gameObject.CompareTag("Enemy"))
            {
                Transform enemyTransform = collider.transform;
                Enemy enemyHealth = enemyTransform.GetComponent<Enemy>();

                if (enemyHealth != null && enemyHealth.health > highestHealth)
                {
                    highestHealth = enemyHealth.health;
                    highestHealthEnemy = enemyTransform;
                }
            }
        }

        return highestHealthEnemy;
    }
    private Transform FindDirectionEnemy()
    {
        RaycastHit2D[] enemyColliders = new RaycastHit2D[4];

        // Perform raycasts in all specified directions
        enemyColliders[0] = Physics2D.Raycast(transform.position, Vector2.left, detectionRadius);
        enemyColliders[1] = Physics2D.Raycast(transform.position, Vector2.right, detectionRadius);
        enemyColliders[2] = Physics2D.Raycast(transform.position, Vector2.up, detectionRadius);
        enemyColliders[3] = Physics2D.Raycast(transform.position, Vector2.down, detectionRadius);

        Transform targetEnemy = null;

        // Check for collisions in each direction and find the first enemy encountered
        foreach (RaycastHit2D collider in enemyColliders)
        {
            if (collider.collider != null)
            {
                if (collider.collider.CompareTag("Enemy"))
                {
                    targetEnemy = collider.transform;
                    break; // Exit the loop after finding the first enemy
                }
            }
        }

        return targetEnemy;
    }
    private Quaternion CalculateTargetRotation(Vector3 targetPosition)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle - 90f, Vector3.forward);
    }
    private void ShootAtEnemy(Transform enemyTransform)
    {
        if (enemyTransform == null)
            return;

        StartCoroutine(FiringProcess(enemyTransform.gameObject));
    }

    private void ShootAtAllEnemies(Transform[] enemies)
    {
        foreach (Transform enemyTransform in enemies)
        {
            ShootAtEnemy(enemyTransform);
        }
    }

    private IEnumerator FiringProcess(GameObject currentEnemy)
    {
        animator.SetTrigger("Fire");

        yield return new WaitForSeconds(animationExitTime + 0.01f);

        if (currentEnemy == null || !IsEnemyInRange(currentEnemy.transform))
        {
            // If the current enemy is null or out of range, find a new target
            Transform newEnemyTransform = FindTargetEnemy();
            if (newEnemyTransform != null)
            {
                currentEnemy = newEnemyTransform.gameObject;
            }
        }

        // Check if there's a valid enemy to shoot at
        if (currentEnemy != null)
        {
            GameObject round = Instantiate(projectile, transform.position, Quaternion.identity);
            Projectile projectileScript = round.GetComponent<Projectile>();

            if (isAssisted)
            {
                projectileScript.SetDamage(damage[level - 1] + (damage[level - 1] * assistLevel / 5));
            }
            else
            {
                projectileScript.SetDamage(damage[level - 1]);
            }

            // Calculate the direction to the enemy
            round.transform.rotation = CalculateTargetRotation(currentEnemy.transform.position);

            // Set the arrow's initial velocity towards the enemy
            Rigidbody2D arrowRigidbody = round.GetComponent<Rigidbody2D>();
            arrowRigidbody.velocity = (currentEnemy.transform.position - transform.position).normalized * projectileSpeed;

            Destroy(round, (detectionRadius + 5) / projectileSpeed);
        }
    }

    private void LookForHelperTurrets()
    {
        Collider2D[] turrets = Physics2D.OverlapCircleAll(transform.position, 2.5f);

        int highestLevel = 0;
        Transform highestLevelTurret = null;

        bool foundHelper = false; // Track if any helper turret was found

        foreach (Collider2D turret in turrets)
        {
            if (turret != null && turret.gameObject.CompareTag("Helper"))
            {
                Helper helper = turret.GetComponent<Helper>();

                if (helper != null && helper.level > highestLevel)
                {
                    highestLevel = helper.level;
                    highestLevelTurret = turret.transform;
                }

                foundHelper = true; // Set foundHelper to true if at least one helper turret is found
            }
        }

        if (foundHelper)
        {
            // Set the assistLevel and isAssisted only if a helper turret was found
            assistLevel = highestLevel;
            isAssisted = true;
        }
        else
        {
            // If no helper turret was found, reset the values
            assistLevel = 0;
            isAssisted = false;
        }
    }
    private void ApplyRotation()
    {
        if (targetEnemy != null) targetRotation = CalculateTargetRotation(targetEnemy.position);

        if (targetRotation != Quaternion.identity)
        {
            turretHead.transform.rotation = Quaternion.Lerp(turretHead.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
