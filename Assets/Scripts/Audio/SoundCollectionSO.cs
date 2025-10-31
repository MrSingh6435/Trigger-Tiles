using UnityEngine;

[CreateAssetMenu]
public class SoundCollectionSO : ScriptableObject
{
    [Header("Music")]
    public SoundSO[] FightMusic;
    public SoundSO[] DiscoParty;


    [Header("SFX")]
    public SoundSO[] GunShoot;
    public SoundSO[] Jump;
    public SoundSO[] Splat;
    public SoundSO[] Jetpack;
    public SoundSO[] GrenadeExplode;
    public SoundSO[] GrenadeShoot;
    public SoundSO[] GrenadeBeep;
    public SoundSO[] PlayerHit;
    public SoundSO[] Megakill;
}
