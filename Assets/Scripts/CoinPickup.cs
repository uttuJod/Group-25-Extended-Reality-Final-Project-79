using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int scoreAmount = 10;
    public float pickupDistance = 1.5f;
    public float pickupDelay = 0.5f;

    Transform playerTarget;
    float spawnTime;

    void Start()
    {
        spawnTime = Time.time;

        if (Camera.main != null)
            playerTarget = Camera.main.transform;
    }

    void Update()
    {
        if (Time.time < spawnTime + pickupDelay)
            return;

        if (playerTarget == null)
        {
            if (Camera.main != null)
                playerTarget = Camera.main.transform;

            return;
        }

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance <= pickupDistance)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(scoreAmount);

            Destroy(gameObject);
        }
    }
}