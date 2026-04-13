using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 20;
    public float pickupDistance = 1.5f;

    Transform playerTarget;

    void Start()
    {
        if (Camera.main != null)
            playerTarget = Camera.main.transform;
    }

    void Update()
    {
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
                GameManager.Instance.AddHealth(healthAmount);

            Destroy(gameObject);
        }
    }
}