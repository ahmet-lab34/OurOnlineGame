using UnityEngine;

public class AllPlayerAudio : MonoBehaviour
{
    [SerializeField] private GroundCHK groundCheck;
    [SerializeField] private Rigidbody2D rb;

    public AudioClip walkingSFX;
    public AudioClip damagedSFX;

    public AudioSource runningSFXS;
    public AudioSource jumpingSFXS;
    public AudioSource damagedSFXS;

    public void Jumping()
    {
        jumpingSFXS.Play();
    }

    public void damagedSound()
    {
        damagedSFXS.PlayOneShot(damagedSFX);
    }

    public void walkingSound()
    {
        if (groundCheck.Grounded && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            if (!runningSFXS.isPlaying)
            {
                runningSFXS.loop = true;
                runningSFXS.clip = walkingSFX;
                runningSFXS.Play();
            }
        }
        else
        {
            if (runningSFXS.isPlaying)
            {
                runningSFXS.Stop();
            }
        }
    }
}