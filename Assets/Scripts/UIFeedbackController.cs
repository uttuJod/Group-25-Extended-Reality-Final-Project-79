using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFeedbackController : MonoBehaviour
{
    [Header("Hitmarker")]
    public CanvasGroup hitmarkerGroup;
    public float hitmarkerVisibleTime = 0.08f;
    public float hitmarkerFadeTime = 0.12f;

    [Header("Damage Flash")]
    public Image damageFlashImage;
    public float damageFlashAlpha = 0.35f;
    public float damageFlashFadeSpeed = 0.4f;

    void Start()
    {
        if (hitmarkerGroup != null)
            hitmarkerGroup.alpha = 0f;

        if (damageFlashImage != null)
        {
            Color c = damageFlashImage.color;
            c.a = 0f;
            damageFlashImage.color = c;
        }
    }

    public void ShowHitmarker()
    {
        StopAllCoroutines();
        StartCoroutine(HitmarkerRoutine());
    }

    public void ShowDamageFlash(float strength = 1f)
    {
        if (damageFlashImage == null)
            return;

        Color c = damageFlashImage.color;
        c.a = Mathf.Clamp01(damageFlashAlpha * strength);
        damageFlashImage.color = c;
    }

    void Update()
    {
        if (damageFlashImage != null)
        {
            Color c = damageFlashImage.color;
            c.a = Mathf.MoveTowards(c.a, 0f, damageFlashFadeSpeed * Time.deltaTime);
            damageFlashImage.color = c;
        }
    }

    IEnumerator HitmarkerRoutine()
    {
        if (hitmarkerGroup == null)
            yield break;

        hitmarkerGroup.alpha = 1f;

        yield return new WaitForSeconds(hitmarkerVisibleTime);

        float elapsed = 0f;
        while (elapsed < hitmarkerFadeTime)
        {
            elapsed += Time.deltaTime;
            hitmarkerGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / hitmarkerFadeTime);
            yield return null;
        }

        hitmarkerGroup.alpha = 0f;
    }
}