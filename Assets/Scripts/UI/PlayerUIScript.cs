using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{
    private PlayerScript player;
    private PlayerStats playerStats;

    [Header("Coin UI")]
    [SerializeField] private TMP_Text coinsText;

    [Header("Bird UI")]
    [SerializeField] private TMP_Text birdCountText;

    [Header("Dash UI")]
    [SerializeField] private Slider dashCooldown_Visual;

    [Header("Health UI")]
    [SerializeField] private Slider playerHealthCondition;

    [Header("Death Panel")]
    [SerializeField] private UIScript deathPanel;

    [Header("ESC Menu")]
    [SerializeField] private GameObject escMenu;

    private void Awake()
    {
        player = GetComponent<PlayerScript>();
        playerStats = GetComponent<PlayerStats>();

        if (deathPanel == null)
            deathPanel = FindFirstObjectByType<UIScript>();
    }

    private void Update()
    {
        if (player == null) return;

        // Update Dash UI
        dashCooldown_Visual.maxValue = playerStats.GetDashCooldownMax();
        dashCooldown_Visual.value = playerStats.GetDashCooldown();

        // Update Health UI
        playerHealthCondition.maxValue = playerStats.GetMaxHealth();
        playerHealthCondition.value = playerStats.GetCurrentHealth();

        // Update Coins UI
        if (coinsText != null)
            coinsText.text = playerStats.GetCoins().ToString();

        // Update Bird Carrying UI
        if (birdCountText != null)
            birdCountText.text = playerStats.GetBirdCount().ToString();
    }
    public void UpdateCoinsUI(int coins)
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }

    public void UpdateBirdCountUI(int birdCount)
    {
        if (birdCountText != null)
            birdCountText.text = birdCount.ToString();
    }

    public void UpdateUpsideDownStateUI(bool isUpsideDown)
    {
        // Example: change color, flip an icon, etc.
        // For now just a debug log
        Debug.Log("UpsideDown state: " + isUpsideDown);
    }

    public void Die()
    {
        deathPanel?.UIDie();
    }
}
