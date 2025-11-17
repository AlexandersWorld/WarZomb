using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private const string IS_WALKING = "isWalking";
    private const string MOVE_X = "MoveX";
    private const string MOVE_Y = "MoveY";
    private const string ATTACK_X = "AttackX";
    private const string ATTACK_Y = "AttackY";
    private const string IS_ATTACKING = "isAttacking";
    private const string MOVE_MAGNITUDE = "MoveMagnitude";
    private const string LAST_MOVE_X = "LastMoveX";
    private const string LAST_MOVE_Y = "LastMoveY";
    private const string IS_SUPER = "isSuper";

    [SerializeField] float attackRange = .5f;
    [SerializeField] int attackDamage = 10;
    [SerializeField] private float movementSpeed = 1.8f;
    [SerializeField] float collisionOffset = 0.05f;
    [SerializeField] float attackCooldown = 0.4f;
    [SerializeField] TextMeshProUGUI killCountText;
    [SerializeField] private float killXP = 10f;
    [SerializeField] Slider xpSlider;
    [SerializeField] ContactFilter2D contactFilter;


    private bool isAttacking = false;
    private float attackTimer = 0f;
    private Vector2 _movementInput;
    private Rigidbody2D _rb;
    private Camera cam;
    private Animator animator;
    private Vector2 lastMoveDir;
    private bool facingLeft = true;
    private SpriteRenderer spriteRenderer;
    private int killCount = 0;

    private bool isTransformed = false;
    private float transformDuration = 30f;
    private float transformTimer = 0f;

    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        killCountText.text = killCount.ToString();
        xpSlider.maxValue = 100;
    }

    void Update()
    {
        killCountText.text = killCount.ToString();

        _movementInput = InputManager.Movement;

        HandleAnimation(_movementInput);

        if (_movementInput.sqrMagnitude > 0.01f)
        {
            lastMoveDir = _movementInput.normalized;
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                animator.SetBool(IS_ATTACKING, false);
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }

        Flip();

        if (isTransformed)
        {
            transformTimer -= Time.deltaTime;

            if (transformTimer <= 0)
            {
                EndTransformation();
            }
        }

        if (!isTransformed && xpSlider.value >= 100)
        {
            StartTransformation();
        }
    }

    void StartTransformation()
    {
        isTransformed = true;
        transformTimer = transformDuration;

        xpSlider.value = 0;
        attackDamage = 30;
        attackRange = 1f;
        movementSpeed = 1.2f;

        Debug.Log("TRANSFORMED!");
        animator.SetBool(IS_SUPER, true);
    }

    void EndTransformation()
    {
        isTransformed = false;
        attackDamage = 10;
        attackRange = 0.5f;
        movementSpeed = 1.8f;

        Debug.Log("BACK TO NORMAL!");
        animator.SetBool(IS_SUPER, false);
    }

    private void HandleAnimation(Vector2 _movementInput)
    {

        animator.SetFloat(MOVE_X, _movementInput.x);
        animator.SetFloat(MOVE_Y, _movementInput.y);

        animator.SetFloat(MOVE_MAGNITUDE, _movementInput.magnitude);

        animator.SetFloat(LAST_MOVE_X, lastMoveDir.x);
        animator.SetFloat(LAST_MOVE_Y, lastMoveDir.y);
    }

    private void FixedUpdate()
    {
        if (!isAttacking && _movementInput != Vector2.zero)
        {
            bool success = TryToMove(_movementInput);

            if (!success)
            {
                success = TryToMove(new Vector2(_movementInput.x, 0));

                if (!success)
                    success = TryToMove(new Vector2(0, _movementInput.y));
            }

            animator.SetBool(IS_WALKING, _movementInput.magnitude > 0.1f);
        }
        else
        {
            animator.SetBool(IS_WALKING, false);
        }

        if (_movementInput.x < 0)
        {
            spriteRenderer.flipX = true;

        } else if (_movementInput.x > 0)
        {
           spriteRenderer.flipX = false;
        }
    }

    bool TryToMove(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return false;

        float distance = movementSpeed * Time.fixedDeltaTime + collisionOffset;

        int count = _rb.Cast(
            direction,
            contactFilter,
            castCollisions,
            distance
        );

        if (count == 0)
        {
            _rb.MovePosition(_rb.position + direction.normalized * movementSpeed * Time.fixedDeltaTime);
            return true;
        }

        return false;
    }

    void Flip()
    {
        if (_movementInput.x < 0 && !facingLeft || _movementInput.x > 0 && facingLeft)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingLeft = !facingLeft;
        }
    }

    void TryAttack()
    {
        if (isAttacking) return;

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider == null || !hit.collider.CompareTag("Enemy"))
            return;

        Transform enemy = hit.collider.transform;

        if (!InRange(enemy.gameObject))
            return;

        Vector2 dir = (enemy.position - transform.position).normalized;

        animator.SetFloat(ATTACK_X, dir.x);
        animator.SetFloat(ATTACK_Y, dir.y);

        StartAttack(enemy.gameObject);
    }

    void StartAttack(GameObject enemy)
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        animator.SetBool(IS_ATTACKING, true);

        ApplyDamage(enemy);
    }

    void ApplyDamage(GameObject enemyGameObject)
    {
        HealthBar enemyHealth = enemyGameObject.GetComponent<HealthBar>();

        if (enemyHealth != null && InRange(enemyGameObject))
        {
            enemyHealth.TakeDamage(attackDamage);

            if (enemyHealth.IsEmpty())
            {
                killCount++;

                if (!isTransformed)
                    xpSlider.value += killXP;
            }
        }
    }

    bool InRange(GameObject enemy)
    {
        float distance = Vector2.Distance(transform.position, enemy.transform.position);

        return distance < attackRange;
    }
} 
