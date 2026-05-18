using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundEntry
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitch = 1f;
    }

    public static SoundManager Instance { get; private set; }

    [Header("SFX")]
    [SerializeField] private SoundEntry footsteps;
    [SerializeField] private SoundEntry death;
    [SerializeField] private SoundEntry pickup;
    [SerializeField] private SoundEntry chestOpen;
    [SerializeField] private SoundEntry teleport;
    [SerializeField] private SoundEntry questComplete;
    [SerializeField] private SoundEntry bossShot;
    [SerializeField] private SoundEntry bossLaser;
    [SerializeField] private SoundEntry buttonClick;

    [Header("Music (per scene)")]
    [SerializeField] private SoundEntry lobbyMusic;
    [SerializeField] private SoundEntry gameplayMusic;
    [SerializeField] private SoundEntry bossMusic;

    private AudioSource sfxSource;
    private AudioSource footstepsSource;
    private AudioSource musicSource;
    private readonly HashSet<int> wiredButtons = new HashSet<int>();

    private const string MUSIC_PREF = "SoundManager.MusicMuted";
    private const string SFX_PREF = "SoundManager.SfxMuted";

    private bool musicMuted;
    private bool sfxMuted;

    public static bool IsMusicMuted => Instance != null && Instance.musicMuted;
    public static bool IsSfxMuted => Instance != null && Instance.sfxMuted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicMuted = PlayerPrefs.GetInt(MUSIC_PREF, 0) == 1;
        sfxMuted = PlayerPrefs.GetInt(SFX_PREF, 0) == 1;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        footstepsSource = gameObject.AddComponent<AudioSource>();
        footstepsSource.playOnAwake = false;
        footstepsSource.loop = true;
        ApplyFootstepsSettings();

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.mute = musicMuted;

        SceneManager.sceneLoaded += OnSceneLoaded;
        WireAllButtons();
        UpdateMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (s.name == "AutroScene" || s.name == "Intro")
        {
            StopAllSounds(); 
        }
        else
        {
            WireAllButtons();
            UpdateMusicForScene(s.name);
        }
    }

    public void StopAllSounds()
    {
        if (musicSource != null) musicSource.Stop();
        if (footstepsSource != null) footstepsSource.Stop();
        if (sfxSource != null) sfxSource.Stop();
    }

    private void UpdateMusicForScene(string sceneName)
    {
        if (musicSource == null) return;

        SoundEntry target;
        if (sceneName == "Lobby") target = lobbyMusic;
        else if (sceneName == "Boss") target = bossMusic;
        else target = gameplayMusic;

        if (target == null || target.clip == null)
        {
            musicSource.Stop();
            musicSource.clip = null;
            return;
        }

        if (musicSource.clip == target.clip && musicSource.isPlaying)
        {
            musicSource.volume = target.volume;
            musicSource.pitch = target.pitch;
            return;
        }

        musicSource.Stop();
        musicSource.clip = target.clip;
        musicSource.volume = target.volume;
        musicSource.pitch = target.pitch;
        musicSource.Play();
    }

    private void WireAllButtons()
    {
        var buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < buttons.Length; i++)
        {
            var btn = buttons[i];
            if (btn == null) continue;
            int id = btn.GetInstanceID();
            if (wiredButtons.Contains(id)) continue;
            btn.onClick.AddListener(PlayClick);
            wiredButtons.Add(id);
        }
    }

    private void PlayOneShot(SoundEntry e)
    {
        if (e == null || e.clip == null || sfxSource == null) return;
        // SFX-mute does not silence UI clicks (так пользователь слышит реакцию кнопок)
        if (sfxMuted && e != buttonClick) return;
        sfxSource.pitch = e.pitch;
        sfxSource.PlayOneShot(e.clip, e.volume);
    }

    private void ApplyFootstepsSettings()
    {
        if (footstepsSource == null || footsteps == null) return;
        footstepsSource.clip = footsteps.clip;
        footstepsSource.volume = footsteps.volume;
        footstepsSource.pitch = footsteps.pitch;
    }

    public static void PlayPickup() { if (Instance != null) Instance.PlayOneShot(Instance.pickup); }
    public static void PlayChestOpen() { if (Instance != null) Instance.PlayOneShot(Instance.chestOpen); }
    public static void PlayTeleport() { if (Instance != null) Instance.PlayOneShot(Instance.teleport); }
    public static void PlayQuestComplete() { if (Instance != null) Instance.PlayOneShot(Instance.questComplete); }
    public static void PlayDeath() { if (Instance != null) Instance.PlayOneShot(Instance.death); }
    public static void PlayBossShot() { if (Instance != null) Instance.PlayOneShot(Instance.bossShot); }
    public static void PlayBossLaser() { if (Instance != null) Instance.PlayOneShot(Instance.bossLaser); }
    public static void PlayClick() { if (Instance != null) Instance.PlayOneShot(Instance.buttonClick); }

    public static void SetFootsteps(bool isWalking)
    {
        if (Instance == null || Instance.footstepsSource == null || Instance.footsteps == null) return;
        if (isWalking && !Instance.sfxMuted)
        {
            if (!Instance.footstepsSource.isPlaying && Instance.footsteps.clip != null)
            {
                Instance.ApplyFootstepsSettings();
                Instance.footstepsSource.Play();
            }
        }
        else
        {
            if (Instance.footstepsSource.isPlaying)
            {
                Instance.footstepsSource.Stop();
            }
        }
    }

    public static void SetMusicMuted(bool muted)
    {
        if (Instance == null) return;
        Instance.musicMuted = muted;
        if (Instance.musicSource != null) Instance.musicSource.mute = muted;
        PlayerPrefs.SetInt(MUSIC_PREF, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void SetSfxMuted(bool muted)
    {
        if (Instance == null) return;
        Instance.sfxMuted = muted;
        if (muted) SetFootsteps(false);
        PlayerPrefs.SetInt(SFX_PREF, muted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
