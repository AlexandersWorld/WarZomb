using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    [SerializeField] private HealthBarCanvas healthBar;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (healthBar)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public bool IsEmpty()
    {
        return isDead;
    }
}
