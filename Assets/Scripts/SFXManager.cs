using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    //http://freesoundeffect.net/sound/evil-androids-sound-effect
    //http://freesoundeffect.net/sound/insect-voice-chirp-bu01-385-sound-effect
    //http://freesoundeffect.net/sound/insect-voice-chirp-bu01-386-sound-effect
    //http://freesoundeffect.net/sound/millipedde-walk-bu01-538-sound-effect

    [SerializeField] AudioSource sourceMusic;
    [SerializeField] AudioSource sourcePlayer;
    [SerializeField] AudioSource sourceDamage;
    [SerializeField] AudioSource sourceWeapon;
    [SerializeField] AudioSource sourceShield;
    [SerializeField] AudioSource sourceLazer;
    [SerializeField] AudioSource sourceAnt;

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
    [SerializeField] AudioClip walkSFX;

    [SerializeField] AudioClip gunShootSFX;
    [SerializeField] AudioClip launcherShootSFX;
    [SerializeField] AudioClip lazerSFX;
    [SerializeField] AudioClip antAttackSFX;
    [SerializeField] AudioClip spiderAttackSFX;

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
    public void Walk()
    {
        if (sourcePlayer.clip != walkSFX && sourcePlayer.isPlaying == false)
        {
            sourcePlayer.clip = walkSFX;
        }
        if (sourcePlayer.isPlaying == false)
        {
            sourcePlayer.time = 0.09f;
            sourcePlayer.Play();
        }
        if (sourcePlayer.clip == walkSFX && sourcePlayer.time > 0.35f)
        {
            sourcePlayer.Stop();
        }

    }

    public void ActivateShield()
    {
        sourceShield.clip = shieldSFX;
        sourceShield.Play();
    }

    public void DeactivateShield()
    {
        sourceShield.Stop();
    }

    public void ShootGun()
    {
        sourceWeapon.clip = gunShootSFX;
        sourceWeapon.Play();
    }

    public void ShootLauncher()
    {
        sourceWeapon.clip = launcherShootSFX;
        sourceWeapon.Play();
    }

    public void ActivateLazer()
    {
        sourceLazer.clip = lazerSFX;
        sourceLazer.Play();
    }

    public void DeactivateLaser()
    {
        sourceLazer.Stop();
    }

    public void AntAttack()
    {
        sourceAnt.clip = antAttackSFX;
        sourceAnt.Play();
    }

    public void SpiderAttack()
    {

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
