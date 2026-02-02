using UnityEngine;
using TMPro;

public class CoinCount : MonoBehaviour
{
    public static CoinCount instance;
    private UIScript uiScript;
    [SerializeField] public static int coins;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }

        uiScript = GetComponent<UIScript>();
    }
    private void Update()
    {
        uiScript.Coinss.text = coins.ToString();
    }
    public void ChangeCoins(int amount)
    {
        coins += amount;
    }
}
