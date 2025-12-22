using UnityEngine;
using TMPro;

public class CoinCount : MonoBehaviour
{
    public static CoinCount instance;
    [SerializeField] public static int coins;
    [SerializeField] private TMP_Text coinsDisplay;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }
    private void Update()
    {
        coinsDisplay.text = coins.ToString();
    }
    public void ChangeCoins(int amount)
    {
        coins += amount;
    }
}
