using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AllPlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GroundCHK GroundCheck;
    public AudioClip walkingSFX;
    public AudioClip backgroundMusic;
    public AudioClip jumpingSFX;
    public AudioClip damagedSFX;
    public AudioClip optionsSFX;
    public AudioSource PigeonBackgroundS;
    public AudioSource RunningSFXS;
    public AudioSource JumpingSFXS;
    public AudioSource DamagedSFXS;
    public AudioSource OptionsSFXS;
    void Awake()
    {
        PigeonBackgroundS.clip = backgroundMusic;
        PigeonBackgroundS.loop = true;
        PigeonBackgroundS.Play();
    }
    public void Jumping()
    {
        JumpingSFXS.PlayOneShot(jumpingSFX);
    }
    public void OptionsSound()
    {
        OptionsSFXS.PlayOneShot(optionsSFX);
    }
    public void damagedSound()
    {
        DamagedSFXS.PlayOneShot(damagedSFX);
    }
    public void walkingSound()
    {
        if (GroundCheck.Grounded && playerScript.Horizontal != 0f)
        {
            if(!RunningSFXS.isPlaying)
            {
                RunningSFXS.loop = true;
                RunningSFXS.PlayOneShot(walkingSFX);
                Debug.Log("The Audio walking script is now playing");
            }
        }
        else
        {
            if (RunningSFXS.isPlaying)
            {
                RunningSFXS.loop = false;
                RunningSFXS.Stop();
            }
        }
    }
}
