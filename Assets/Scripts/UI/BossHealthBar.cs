using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health bossHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject victoryPanel;

    private int maxHealth;

    private void Start()
    {
        if (bossHealth == null)
        {
            Debug.LogWarning("BossHealthBar: Boss Health is not assigned!");
            return;
        }

        if (healthSlider == null)
        {
            Debug.LogWarning("BossHealthBar: Health Slider is not assigned!");
            return;
        }

        maxHealth = bossHealth.CurrentHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = bossHealth.CurrentHealth;

        bossHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void Update()
    {
        if (victoryPanel != null && victoryPanel.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthSlider == null || bossHealth == null) return;

        healthSlider.value = bossHealth.CurrentHealth;
    }

    private void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateHealthBar;
        }
    }
}