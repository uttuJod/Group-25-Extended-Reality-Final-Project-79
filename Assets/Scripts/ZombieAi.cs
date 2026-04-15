using UnityEngine;

public class ZombieAI : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float damageDistance = 1.5f;
    public int damageAmount = 10;
    public float damageCooldown = 1f;
    public float targetRefreshInterval = 0.25f;
    public float girlDamageDistance = 1.0f;
    public float playerDamageDistance = 1.5f;

    Transform playerTarget;
    GirlHealth girlHealth;
    Transform currentTarget;
    bool attackingGirl = false;

    float lastDamageTime;
    float nextTargetRefreshTime;

    Animator animator;

    void Start()
    {
        if (Camera.main != null)
            playerTarget = Camera.main.transform;

        girlHealth = FindFirstObjectByType<GirlHealth>();
        animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }

        RefreshTarget();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (Time.time >= nextTargetRefreshTime)
        {
            nextTargetRefreshTime = Time.time + targetRefreshInterval;
            RefreshTarget();
        }

        if (currentTarget == null)
            return;

        Vector3 targetPos = currentTarget.position;
        targetPos.y = transform.position.y;

        float dist = Vector3.Distance(transform.position, targetPos);

        float currentDamageDistance = attackingGirl ? girlDamageDistance : playerDamageDistance;

        if (dist > currentDamageDistance)
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;

            if (dir != Vector3.zero)
                transform.forward = dir;

            if (animator != null)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
            }
        }
        else
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            if (dir != Vector3.zero)
                transform.forward = dir;

            if (animator != null)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
            }

            if (Time.time >= lastDamageTime + damageCooldown)
            {
                DealDamageToCurrentTarget();
                lastDamageTime = Time.time;
            }
        }
    }

    void RefreshTarget()
    {
        if (playerTarget == null && Camera.main != null)
            playerTarget = Camera.main.transform;

        if (girlHealth == null || girlHealth.IsDead)
            girlHealth = FindFirstObjectByType<GirlHealth>();

        float playerDistance = float.MaxValue;
        float girlDistance = float.MaxValue;

        if (playerTarget != null)
            playerDistance = Vector3.Distance(transform.position, playerTarget.position);

        if (girlHealth != null && !girlHealth.IsDead && girlHealth.AttackTarget != null)
            girlDistance = Vector3.Distance(transform.position, girlHealth.AttackTarget.position);

        if (girlDistance < playerDistance)
        {
            currentTarget = girlHealth.AttackTarget;
            attackingGirl = true;
        }
        else if (playerTarget != null)
        {
            currentTarget = playerTarget;
            attackingGirl = false;
        }
        else
        {
            currentTarget = null;
            attackingGirl = false;
        }
    }

    void DealDamageToCurrentTarget()
    {
        if (attackingGirl)
        {
            if (girlHealth != null && !girlHealth.IsDead)
                girlHealth.TakeDamage(damageAmount);
        }
        else
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TakeDamage(damageAmount);
        }
    }

    void OnDisable()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }
}