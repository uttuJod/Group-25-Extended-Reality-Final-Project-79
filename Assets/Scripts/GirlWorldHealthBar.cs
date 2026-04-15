using UnityEngine;
using UnityEngine.UI;

public class GirlWorldHealthBar : MonoBehaviour
{
    public GirlHealth target;
    public Transform followTarget;
    public Image fillImage;
    public Vector3 worldOffset = new Vector3(0f, 0.35f, 0f);

    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null || target.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        if (mainCam == null)
            mainCam = Camera.main;

        if (followTarget != null)
            transform.position = followTarget.position + worldOffset;

        if (mainCam != null)
            transform.forward = mainCam.transform.forward;

        if (fillImage != null)
            fillImage.fillAmount = target.HealthPercent;
    }
}