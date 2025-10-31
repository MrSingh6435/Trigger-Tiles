using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Range(0f, 2f)]
    [SerializeField] private float _masterVolume = 1f;
    [SerializeField] private SoundCollectionSO _soundCollectionSO;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    private AudioSource _currentMusic;

    #region Unity Methods

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    private void Start()
    {
        FightMusic();
    }

    private void OnEnable()
    {
        Gun.OnShoot += Gun_OnShoot;
        Gun.OnGrenadeShoot += Gun_OnGrenadeShoot;
        PlayerController.OnJump += PlayerController_OnJump;
        PlayerController.OnJetpack += PlayerController_OnJetpack;
        Health.OnDeath += HandleDeath;
        DiscoBallManager.OnDiscoBallHitEvent += DiscoBallMusic;
    }

    private void OnDisable()
    {
        Gun.OnShoot -= Gun_OnShoot;
        Gun.OnGrenadeShoot -= Gun_OnGrenadeShoot;
        PlayerController.OnJump -= PlayerController_OnJump;
        PlayerController.OnJetpack -= PlayerController_OnJetpack;
        Health.OnDeath -= HandleDeath;
        DiscoBallManager.OnDiscoBallHitEvent -= DiscoBallMusic;
    }

    #endregion

    #region Sound Methods 

    private void PlayRandomSound(SoundSO[] sounds)
    {
        if (sounds != null && sounds.Length > 0)
        {
            SoundSO soundSO = sounds[Random.RandomRange(0, sounds.Length)];
            SoundToPlay(soundSO);
        }
    }

    private void SoundToPlay(SoundSO soundSO)
    {
        AudioClip clip = soundSO.Clip;
        float pitch = soundSO.Pitch;
        float volume = soundSO.Volume;
        bool loop = soundSO.Loop;
        AudioMixerGroup audioMixerGroup;

        pitch = RendomizePitch(soundSO, pitch);

        audioMixerGroup = DetermineAudioMixerGroup(soundSO);

        PlaySound(clip, pitch, volume, loop, audioMixerGroup);
    }

    private AudioMixerGroup DetermineAudioMixerGroup(SoundSO soundSO)
    {
        AudioMixerGroup audioMixerGroup;
        switch (soundSO.AudioType)
        {
            case SoundSO.AudioTypes.SFX:
                audioMixerGroup = _sfxMixerGroup;
                break;
            case SoundSO.AudioTypes.Music:
                audioMixerGroup = _musicMixerGroup;
                break;
            default:
                audioMixerGroup = null;
                break;
        }

        return audioMixerGroup;
    }

    private static float RendomizePitch(SoundSO soundSO, float pitch)
    {
        if (soundSO.RandomizePitch)
        {
            float randomizePitchModifier = Random.Range(-soundSO.RandomizePitchRangeModifier, soundSO.RandomizePitchRangeModifier);
            pitch = soundSO.Pitch + randomizePitchModifier;
        }

        return pitch;
    }

    private void PlaySound(AudioClip clip, float pitch, float volume, bool loop, AudioMixerGroup audioMixerGroup)
    {
        GameObject soundObject = new GameObject("Temp Audio Source");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume * _masterVolume;
        audioSource.loop = loop;
        audioSource.outputAudioMixerGroup = audioMixerGroup;

        audioSource.Play();

        if (!loop) { Destroy(soundObject, clip.length); }

        DetermineMusic(audioMixerGroup, audioSource);
    }

    private void DetermineMusic(AudioMixerGroup audioMixerGroup, AudioSource audioSource)
    {
        if (audioMixerGroup == _musicMixerGroup)
        {
            if (_currentMusic != null)
            {
                _currentMusic.Stop();
            }

            _currentMusic = audioSource;
        }
    }

    #endregion

    #region SFX
    private void Gun_OnShoot()
    {
        PlayRandomSound(_soundCollectionSO.GunShoot);
    }

    private void PlayerController_OnJump()
    {
        PlayRandomSound(_soundCollectionSO.Jump);
    }

    private void Health_OnDeath(Health health)
    {
        PlayRandomSound(_soundCollectionSO.Splat);
    }

    // Method is overloaded
    private void Health_OnDeath()
    {
        PlayRandomSound(_soundCollectionSO.Splat);
    }

    private void PlayerController_OnJetpack()
    {
        PlayRandomSound(_soundCollectionSO.Jetpack);
    }

    private void Gun_OnGrenadeShoot()
    {
        PlayRandomSound(_soundCollectionSO.GrenadeShoot);
    }

    public void Grenade_OnBeep()
    {
        PlayRandomSound(_soundCollectionSO.GrenadeBeep);
    }

    public void Grenade_OnExplode()
    {
        PlayRandomSound(_soundCollectionSO.GrenadeExplode);
    }

    public void Enemy_OnPlayerHit()
    {
        PlayRandomSound(_soundCollectionSO.PlayerHit);
    }

    public void AudioManager_Megakill()
    {
        PlayRandomSound(_soundCollectionSO.Megakill);
    }

    #endregion

    #region Music   
    private void FightMusic()
    {
        PlayRandomSound(_soundCollectionSO.FightMusic);
    }

    private void DiscoBallMusic()
    {
        PlayRandomSound(_soundCollectionSO.DiscoParty);
        float soundLength = _soundCollectionSO.DiscoParty[0].Clip.length;
        Utils.RunAfterDelay(this, soundLength, FightMusic);
    }

    #endregion

    #region Custom SFX Logic

    private List<Health> _deathList = new List<Health>();
    private Coroutine _deathCoroutine;
    private void HandleDeath(Health health)
    {
        bool isEnemy = health.GetComponent<Enemy>();

        if (isEnemy)
        {
            _deathList.Add(health);
        }

        if (_deathCoroutine == null)
        {
            _deathCoroutine = StartCoroutine(DeathWindowRoutine());
        }
    }

    private IEnumerator DeathWindowRoutine() // red line under IEnumerator
    {
        yield return null;

        int megakillAmount = 3;

        if (_deathList.Count >= megakillAmount)
        {
            AudioManager_Megakill();
        }

        Health_OnDeath();

        _deathList.Clear();
        _deathCoroutine = null;
    }


    #endregion
}
