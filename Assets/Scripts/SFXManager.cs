using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    [SerializeField] AudioSource sourceMusic;
    [SerializeField] AudioSource sourcePlayer;
    [SerializeField] AudioSource sourceDamage;
    [SerializeField] AudioSource sourceWeapon;

    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip loadingMusic;
    [SerializeField] AudioClip mainMusic;
    [SerializeField] AudioClip caveMusic;
    [SerializeField] AudioClip bossMusic;

    [SerializeField] AudioClip larvaeSFX;
    [SerializeField] AudioClip powerupSFX;
    [SerializeField] AudioClip specialSFX;
    [SerializeField] AudioClip shieldSFX;
    [SerializeField] AudioClip damageSFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterBoss()
    {
        if (sourceMusic.clip != bossMusic)
        {
            sourceMusic.clip = bossMusic;
            sourceMusic.Play();
        }
        else
        {
            sourceMusic.clip = mainMusic;
            sourceMusic.Play();
        }

    }

    public void EnterCave()
    {
        if (sourceMusic.clip == caveMusic)
        {
            sourceMusic.clip = mainMusic;
            sourceMusic.Play();
        }
        else if (sourceMusic.clip == mainMusic)
        {
            //AudioFadeOut(sourceMusic, 7f);
            sourceMusic.clip = caveMusic;
            sourceMusic.Play();
        }
    }

    public void CollectLarvae()
    {
        sourcePlayer.clip = larvaeSFX;
        sourcePlayer.Play();
    }

    public void CollectPowerup()
    {
        sourcePlayer.clip = powerupSFX;
        sourcePlayer.Play();
    }

    public void CollectSpecial()
    {
        sourcePlayer.clip = specialSFX;
        sourcePlayer.Play();
    }

    public void TakeDamage()
    {
        sourceDamage.clip = damageSFX;
        sourceDamage.Play();
    }

    public static void AudioFadeOut(AudioSource audioSource, float FadeTime)
    {

        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            Debug.Log(audioSource.volume);
        }

            //audioSource.Stop();
            //audioSource.volume = startVolume;

    }
    

}
