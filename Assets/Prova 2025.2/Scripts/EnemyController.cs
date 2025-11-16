using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private const string IS_DEAD = "isDead";
    private const string IS_WALKING = "isDead";
    private const string IS_ATTACKING = "isDead";

    private HealthBar healthBar;
    private Animator animator;

    private bool isDying = false;

    void Start()
    {
        healthBar = GetComponent<HealthBar>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isDying && healthBar.IsEmpty())
        {
            StartDeath();
        }
    }

    private void StartDeath()
    {
        isDying = true;

        animator.SetBool(IS_DEAD, true);

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }
}
