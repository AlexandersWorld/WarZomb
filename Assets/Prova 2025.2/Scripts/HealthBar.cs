using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public int maxHealth = 30;
    private int health;

    private bool isDead = false;

    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {health}");

        if (health <= 0)
        {
            isDead = true;
        }
    }

    public bool IsEmpty()
    {
        return isDead;
    }
}
