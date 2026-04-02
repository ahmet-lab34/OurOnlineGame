using UnityEngine;
using Unity.Netcode;

public class PlayerStats : NetworkBehaviour
{
    private PlayerUIScript ui;
    private AllPlayerAudio audioPlayer;

    #region Serialized Fields
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Coin Settings")]
    [SerializeField] private int coins = 0;

    [Header("Dash Settings")]
    [SerializeField] private float dashCooldownMax = 2f;
    private float dashCooldownTimer = 0f;

    [Header("Bird Settings")]
    [SerializeField] private int birdCount = 0;

    [Header("Gravity Settings")]
    [SerializeField] private bool isUpsideDown = false;
    [SerializeField] private float defaultGravityScale = 1.7f;
    #endregion

    #region Getters
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public int GetCoins() => coins;
    public float GetDashCooldown() => dashCooldownTimer;
    public float GetDashCooldownMax() => dashCooldownMax;
    public int GetBirdCount() => birdCount;
    public bool IsUpsideDown() => isUpsideDown;
    public float GetDefaultGravityScale() => defaultGravityScale;
    #endregion

    private void Awake()
    {
        currentHealth = maxHealth;
        ui = FindFirstObjectByType<PlayerUIScript>();
        audioPlayer = GetComponent<AllPlayerAudio>();
    }

    private void Update()
    {
        if (!IsOwner) return;
        UpdateDashCooldown();
    }

    #region Health Methods
    public void GetHit(int damage = 1)
    {
        if (!IsOwner || damage <= 0) return;

        currentHealth -= damage;
        audioPlayer?.damagedSound();

        if (currentHealth <= 0) Die();
    }

    public void Heal(int amount)
    {
        if (!IsOwner || amount <= 0) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        ui?.Die();
        Debug.Log("Player died");
    }
    #endregion

    #region Coin Methods
    public void AddCoins(int amount)
    {
        if (!IsOwner || amount <= 0) return;

        coins += amount;
        ui?.UpdateCoinsUI(coins);
    }

    public void SpendCoins(int amount)
    {
        if (!IsOwner || amount <= 0 || coins < amount) return;

        coins -= amount;
        ui?.UpdateCoinsUI(coins);
    }
    #endregion

    #region Dash Methods
    public void StartDash()
    {
        if (!IsOwner || dashCooldownTimer > 0f) return;
        dashCooldownTimer = dashCooldownMax;
    }

    private void UpdateDashCooldown()
    {
        if (dashCooldownTimer <= 0f) return;
        dashCooldownTimer -= Time.deltaTime;
    }
    #endregion

    #region Bird Methods
    public void CarryBird()
    {
        if (!IsOwner) return;
        birdCount++;
        ui?.UpdateBirdCountUI(birdCount);
    }

    public void ReleaseBirds()
    {
        if (!IsOwner) return;
        birdCount = 0;
        ui?.UpdateBirdCountUI(birdCount);
    }
    #endregion

    #region Gravity Methods
    public void SetUpsideDownState(bool value)
    {
        if (!IsOwner) return;
        isUpsideDown = value;
        ui?.UpdateUpsideDownStateUI(isUpsideDown);
    }

    public void ToggleUpsideDown()
    {
        if (!IsOwner) return;

        // Change the jump direction by toggling the upside-down state
        // If the player is currently upside down, we want to set it to normal (not upside down), and vice versa.

        isUpsideDown = !isUpsideDown;

        ui?.UpdateUpsideDownStateUI(isUpsideDown);
    }
    #endregion
}
