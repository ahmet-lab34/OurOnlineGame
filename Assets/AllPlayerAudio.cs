using UnityEngine;

public class AllPlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GroundCHK GroundCheck;
    public AudioClip walkingSFX;
    public AudioClip backgroundMusic;
    public AudioClip jumpingSFX;
    public AudioClip damagedSFX;
    public AudioClip optionsSFX;
    private AudioSource walkingSource;
    private AudioSource backgroundSource;
    private AudioSource jumpingSource;
    private AudioSource damagedSource;
    private AudioSource optionsSource;
    void Awake()
    {
        //playerScript = FindFirstObjectByType<PlayerScript>();
        //GroundCheck = FindFirstObjectByType<GroundCHK>();

        walkingSource = gameObject.AddComponent<AudioSource>();
        backgroundSource = gameObject.AddComponent<AudioSource>();
        jumpingSource = gameObject.AddComponent<AudioSource>();
        damagedSource = gameObject.AddComponent<AudioSource>();
        optionsSource = gameObject.AddComponent<AudioSource>();
    }
    public void Jumping()
    {
        jumpingSource.PlayOneShot(jumpingSFX);
    }
    public void OptionsSound()
    {
        optionsSource.PlayOneShot(optionsSFX);
    }
    public void damagedSound()
    {
        damagedSource.PlayOneShot(damagedSFX);
    }
    public void walkingSound()
    {
        if (GroundCheck.Grounded && playerScript.Horizontal != 0f)
        {
            if(!walkingSource.isPlaying)
            {
                walkingSource.loop = true;
                walkingSource.PlayOneShot(walkingSFX);
                Debug.Log("The Audio walking script is now playing");
            }
        }
        else
        {
            if (walkingSource.isPlaying)
            {
                walkingSource.loop = false;
                walkingSource.Stop();
            }
        }
    }
}
