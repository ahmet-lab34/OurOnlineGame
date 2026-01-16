using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AllPlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GroundCHK GroundCheck;
    public AudioClip walkingSFX;
    public AudioClip damagedSFX;
    public AudioSource RunningSFXS;
    public AudioSource JumpingSFXS;
    public AudioSource DamagedSFXS;
    public void Jumping()
    {
        JumpingSFXS.Play();
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
