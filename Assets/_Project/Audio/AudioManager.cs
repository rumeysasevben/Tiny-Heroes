using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private AudioClip xpPickupSfx;
    [SerializeField] private AudioClip levelUpSfx;
    [SerializeField] private AudioClip heartPickupSfx;
    [SerializeField] private AudioClip chanceBoxSfx;

    [Header("Shoot Sound Limiter")]
    [Tooltip("İki shoot sesi arası minimum süre (saniye). 0.08 = saniyede max ~12 ses")]
    [SerializeField] private float minShootInterval = 0.08f;
    private float lastShootTime = -999f;

    [Header("Volumes")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.8f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayHit()          => PlaySfx(hitSfx);
    public void PlayXpPickup()     => PlaySfx(xpPickupSfx);
    public void PlayLevelUp()      => PlaySfx(levelUpSfx);
    public void PlayHeartPickup()  => PlaySfx(heartPickupSfx);
    public void PlayChanceBox()    => PlaySfx(chanceBoxSfx);

    public void PlayShoot()
    {
        if (shootSfx == null) return;
        if (Time.time - lastShootTime < minShootInterval) return;
        lastShootTime = Time.time;
        sfxSource.PlayOneShot(shootSfx);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            ToggleMute();
    }

    public void ToggleMute()
    {
        AudioListener.volume = AudioListener.volume > 0f ? 0f : 1f;
    }

    public bool IsMuted() => AudioListener.volume <= 0f;
}