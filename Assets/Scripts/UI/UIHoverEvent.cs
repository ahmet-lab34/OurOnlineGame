using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHoverEvent : MonoBehaviour
{
    public List<Button> buttons;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        foreach (var btn in buttons)
        {
            if (btn == null) continue;

            var listener = btn.gameObject.AddComponent<PointerEnterListener>();
            listener.Setup(audioSource);
        }
    }

    private class PointerEnterListener : MonoBehaviour, IPointerEnterHandler
    {
        private AudioSource source;

        public void Setup(AudioSource audioSource)
        {
            source = audioSource;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (source == null) return;

            source.PlayOneShot(source.clip);
        }
    }
}