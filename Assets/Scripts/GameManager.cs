using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Stats")]
    public int playerHealth = 100;
    public int score = 0;

    [Header("Ammo")]
    public int startingAmmo = 30;
    public int currentAmmo;

    [Header("UI")]
    public TMP_Text healthText;
    public TMP_Text scoreText;
    public TMP_Text ammoText;
    public GameObject gameOverPanel;

    [Header("UI Feedback")]
    public UIFeedbackController uiFeedback;
    public bool IsGameOver { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        IsGameOver = false;
        currentAmmo = startingAmmo;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;

        score += amount;
        UpdateUI();
    }

    public void TakeDamage(int amount)
{
    if (IsGameOver) return;

    playerHealth -= amount;

    if (playerHealth < 0)
        playerHealth = 0;

    if (uiFeedback != null)
    {
        float lowHealthStrength = 1f - ((float)playerHealth / 100f);
        lowHealthStrength = Mathf.Clamp01(lowHealthStrength);
        uiFeedback.ShowDamageFlash(Mathf.Lerp(0.35f, 1f, lowHealthStrength));
    }

    if (playerHealth <= 0)
    {
        GameOver();
    }

    UpdateUI();
}

    public bool TryUseAmmo(int amount)
    {
        if (IsGameOver) return false;
        if (currentAmmo < amount) return false;

        currentAmmo -= amount;
        UpdateUI();
        return true;
    }

    public void AddAmmo(int amount)
    {
        if (IsGameOver) return;

        currentAmmo += amount;
        if (currentAmmo < 0)
            currentAmmo = 0;

        UpdateUI();
    }
    public void AddHealth(int amount)
{
    if (IsGameOver) return;

    playerHealth += amount;

    if (playerHealth > 100)
        playerHealth = 100;

    UpdateUI();
}
    void GameOver()
    {
        IsGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + playerHealth;

        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (ammoText != null)
            ammoText.text = "Ammo: " + currentAmmo;
    }
}