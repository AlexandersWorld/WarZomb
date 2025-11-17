using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private const string IS_DEAD = "isDead";
    private const string IS_WALKING = "isWalking";
    private const string IS_ATTACKING = "isAttacking";

    private HealthBar healthBar;
    private Animator animator;
    private Transform target;

    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] int attackDamage = 2;

    private float lastAttackTime;

    private bool isDying = false;

    void Start()
    {
        CacheComponents();
        FindTarget();
    }

    void Update()
    {
        if (ShouldDie())
        {
            StartDeath();
            return;
        }

        if (isDying || target == null)
            return;

        HandleBehavior();
    }

    private void CacheComponents()
    {
        healthBar = GetComponent<HealthBar>();
        animator = GetComponent<Animator>();
    }

    private void FindTarget()
    {
        GameObject t = GameObject.FindGameObjectWithTag("Target");
        if (t != null)
            target = t.transform;
    }

    private void HandleBehavior()
    {
        float distance = GetDistanceToTarget();

        if (distance > attackRange)
            MoveTowardTarget();
        else
            TryAttack();
    }

    private bool ShouldDie()
    {
        return !isDying && healthBar.IsEmpty();
    }

    private void StartDeath()
    {
        isDying = true;
        animator.SetBool(IS_DEAD, true);

        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 2f);
    }

    private void MoveTowardTarget()
    {
        Vector2 direction = GetDirectionToTarget();
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        FlipSprite(direction);
        animator.SetBool(IS_WALKING, true);
        animator.SetBool(IS_ATTACKING, false);
    }

    private Vector2 GetDirectionToTarget()
    {
        return (target.position - transform.position).normalized;
    }

    private float GetDistanceToTarget()
    {
        return Vector2.Distance(transform.position, target.position);
    }

    private void FlipSprite(Vector2 direction)
    {
        if (direction.x == 0) return;

        transform.localScale = new Vector3(
            direction.x > 0 ? 1 : -1,
            1,
            1
        );
    }
    private void TryAttack()
    {
        animator.SetBool(IS_WALKING, false);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetBool(IS_ATTACKING, true);
            lastAttackTime = Time.time;

            DealDamage();
        }
    }

    private void DealDamage()
    {
        if (target == null) return;

        if (GetDistanceToTarget() <= attackRange + 0.2f)
        {
            target.GetComponent<HealthBar>().TakeDamage(attackDamage);
        }
    }
}
