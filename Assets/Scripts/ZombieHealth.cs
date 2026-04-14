using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public bool isBoss = false;

    [Header("Damage Multipliers")]
    public float torsoMultiplier = 1f;
    public float limbMultiplier = 0.5f;
    public float bossHeadMultiplier = 2.5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hurtClip;

    [Header("World Health Bar")]
    public GameObject worldHealthBarPrefab;
    public Transform healthBarAnchor;
    [Header("Coin Drop")]
    public GameObject coinPickupPrefab;
    public Transform coinDropPoint;
    public int coinScoreValue = 10;
    int currentHealth;
    bool dead = false;
    Animator animator;
    GameObject spawnedHealthBar;

    public int CurrentHealth => currentHealth;
    public float HealthPercent => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    public bool IsDead => dead;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();

        if (worldHealthBarPrefab != null)
        {
            spawnedHealthBar = Instantiate(worldHealthBarPrefab);

            ZombieWorldHealthBar bar = spawnedHealthBar.GetComponent<ZombieWorldHealthBar>();
            if (bar != null)
            {
                bar.target = this;
                bar.followTarget = healthBarAnchor != null ? healthBarAnchor : transform;
            }
        }
    }

    public void TakeBodyPartDamage(BodyPartZone zone, int baseDamage, int headshotAmmoReward)
    {
        if (dead) return;

        int finalDamage = baseDamage;

        switch (zone)
        {
            case BodyPartZone.Head:
                if (!isBoss)
                {
                    finalDamage = currentHealth;

                    if (GameManager.Instance != null && headshotAmmoReward > 0)
                        GameManager.Instance.AddAmmo(headshotAmmoReward);
                }
                else
                {
                    finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * bossHeadMultiplier));

                    if (GameManager.Instance != null && headshotAmmoReward > 0)
                        GameManager.Instance.AddAmmo(headshotAmmoReward);
                }
                break;

            case BodyPartZone.Torso:
                finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * torsoMultiplier));
                break;

            case BodyPartZone.Limb:
                finalDamage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * limbMultiplier));
                break;
        }

        if (audioSource != null && hurtClip != null)
            audioSource.PlayOneShot(hurtClip);

        currentHealth -= finalDamage;

        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        dead = true;

        if (spawnedHealthBar != null)
            Destroy(spawnedHealthBar);

        if (coinPickupPrefab != null)
{
    Vector3 dropPosition = coinDropPoint != null ? coinDropPoint.position : transform.position;

    GameObject coin = Instantiate(coinPickupPrefab, dropPosition, Quaternion.identity);

    CoinPickup coinPickup = coin.GetComponent<CoinPickup>();
    if (coinPickup != null)
        coinPickup.scoreAmount = isBoss ? 50 : coinScoreValue;
}

        ZombieAI ai = GetComponent<ZombieAI>();
        if (ai != null)
            ai.enabled = false;

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null)
            col.enabled = false;

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
            animator.SetTrigger("Die");
            Destroy(gameObject, 2.0f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}