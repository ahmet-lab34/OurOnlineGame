using System;
using UnityEngine;
using TMPro;

public class UI_InputWindow : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private string title = "Title";
    [SerializeField] private string defaultText = "Enter text...";
    [SerializeField] private int characterLimit = 100;

    private Action onCancelAction;
    private Action<string> onOkAction;

    private void Awake() {
        Hide();
    }

    private void Update() {
        if (!gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            onOkAction?.Invoke(inputField.text);
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            onCancelAction?.Invoke();
        }
    }

    public void Show(Action<string> onOk, Action onCancel = null) {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        titleText.text = title;
        inputField.characterLimit = characterLimit;
        inputField.text = defaultText;
        inputField.Select();

        onOkAction = (s) => {
            onOk?.Invoke(s);
            Hide();
        };

        onCancelAction = () => {
            onCancel?.Invoke();
            Hide();
        };
    } 


    private void Hide() {
        gameObject.SetActive(false);
    }
}
