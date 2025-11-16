using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const string IS_WALKING = "isWalking";
    private const string MOVE_X = "MoveX";
    private const string MOVE_Y = "MoveY";
    private const string ATTACK_X = "AttackX";
    private const string ATTACK_Y = "AttackY";
    private const string IS_ATTACKING = "isAttacking";
    private const string MOVE_MAGNITUDE = "MoveMagnitude";

    [SerializeField] float attackRange = 2f;
    [SerializeField] int attackDamage = 10;
    [SerializeField] private float _movementSpeed = 233f;
    [SerializeField] float collisionOffset = 0.05f;
    [SerializeField] ContactFilter2D contactFilter;

    private Vector2 _movementInput;
    private Rigidbody2D _rb;
    private Camera cam;
    private Animator animator;
    private Vector2 lastMoveDir;
    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;

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
    }

    // Update is called once per frame
    void Update()
    {
        _movementInput.Set(InputManager.Movement.x, InputManager.Movement.y);

        animator.SetFloat(MOVE_X, _movementInput.x);
        animator.SetFloat(MOVE_Y, _movementInput.y);

        animator.SetFloat(MOVE_MAGNITUDE, _movementInput.magnitude);

        _rb.velocity = _movementInput * _movementSpeed * Time.deltaTime;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleMouseClick();
        }
    }

    private void FixedUpdate()
    {
        if (_movementInput != Vector2.zero)
        {
            bool sucess = TryToMove(_movementInput);

            if (!sucess && _movementInput.x > 0)
            {
                sucess = TryToMove(new Vector2(_movementInput.x, 0));
            }

            if (!sucess && _movementInput.y > 0)
            {
                sucess = TryToMove(new Vector2(0, _movementInput.y));
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
        int count = _rb.Cast(
            direction,
            contactFilter,
            castCollisions,
            _movementSpeed * Time.fixedDeltaTime + collisionOffset
        );

        if (count == 0)
        {
            _rb.MovePosition(_rb.position + _movementSpeed * Time.fixedDeltaTime * _movementInput);
            return true;
        }
        else
        {
            return false;
        }
    }

    void HandleMouseClick()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            
            Transform enemyTransform = hit.collider.transform;

            float distance = Vector2.Distance(transform.position, enemyTransform.position);

            if (distance <= attackRange)
            {
                animator.SetBool(IS_ATTACKING, true);

                Vector2 dir = (enemyTransform.position - transform.position).normalized;
                animator.SetFloat(ATTACK_X, dir.x);
                animator.SetFloat(ATTACK_Y, dir.y);

                ApplyDamage(enemyTransform.gameObject);
            }
        }
    }

    void ApplyDamage(GameObject enemyGameObject)
    {
        HealthBar enemyHealth = enemyGameObject.GetComponent<HealthBar>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
        }
    }
    public void ResetAttack()
    {
        animator.SetBool(IS_ATTACKING, false);
    }
} 
