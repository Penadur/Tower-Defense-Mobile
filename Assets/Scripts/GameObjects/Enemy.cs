using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform[] targetCheckpoints;
    [SerializeField] private int checkpoint = 0;
    [SerializeField] private int prize;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Spawner spawner;
    private float maxHealth;
    public float health;
    private float currentValue = 0;
    [SerializeField] private GameObject takenDamage;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        healthBar.maxValue = health;
        CheckForSpawner();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPoints();
    }
    private void CheckForSpawner()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.01f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.tag == "Spawner")
            {
                spawner = collider.GetComponent<Spawner>();
                targetCheckpoints = spawner.checkpoints;
                break;
            }
        }
    }
    private void CheckPoints()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetCheckpoints[checkpoint].transform.position, Time.deltaTime * moveSpeed);

        if (transform.position == targetCheckpoints[checkpoint].transform.position)
        {
            checkpoint++;
            if (checkpoint == targetCheckpoints.Length)
            {
                GameManager.Instance.health--;
                Destroy(gameObject);
            }
        }
    }

    public void CheckHealthStatus()
    {
        healthBar.value = health;

        if (health <= 0)
        {
            GameManager.Instance.money += prize;
            Destroy(gameObject);
        }

        if (health < maxHealth)
        {
            healthBar.gameObject.SetActive(true);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void ShowDamage(int damage)
    {
        GameObject damageTaken = Instantiate(takenDamage, transform.position, Quaternion.identity);
        TextMeshPro text = damageTaken.GetComponent<TextMeshPro>();

        text.text = damage.ToString();
    }

    
}
