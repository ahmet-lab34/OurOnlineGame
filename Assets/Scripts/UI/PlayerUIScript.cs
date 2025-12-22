using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{
    PlayerScript player;
    [SerializeField] private TMP_Text dashCooldown_Display;
    [SerializeField] private Slider dashCooldown_Visual;
    [SerializeField] private Slider playerHealthCondition;

    [SerializeField] private UIScript DeathPanelForPlayer;

    void Awake()
    {
        player = GetComponent<PlayerScript>();
        DeathPanelForPlayer = FindFirstObjectByType<UIScript>();
    }

    void Update()
    {
        dashCooldown_Display.text = "Dash: " + player.dashCoolcounter.ToString("F1");
        dashCooldown_Visual.value = player.dashCoolcounter;
        dashCooldown_Visual.maxValue = player.movingStats.dashCooldown;
        playerHealthCondition.value = player.playerNumbers.playerHealth;
    }

    public void Die()
    {
        DeathPanelForPlayer.UIDie();
    }
}
