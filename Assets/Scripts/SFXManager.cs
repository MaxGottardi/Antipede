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
    [SerializeField] AudioSource sourceWeapon2;
    [SerializeField] AudioSource sourceShield;
    [SerializeField] AudioSource sourceLazer;
    [SerializeField] AudioSource sourceAnt;
    [SerializeField] AudioSource sourceSpider;
    [SerializeField] AudioSource sourceFlame;

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
    [SerializeField] AudioClip shootWebSFX;
    [SerializeField] AudioClip fireSFX;

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
        Debug.Log("1");
        if (sourceMusic.clip == caveMusic)
        {
            Debug.Log("2");
            sourceMusic.clip = mainMusic;
            sourceMusic.Play();
        }
        else if (sourceMusic.clip == mainMusic)
        {
            Debug.Log("3");
            //AudioFadeOut(sourceMusic, 7f);
            sourceMusic.clip = caveMusic;
            sourceMusic.Play();
        }
    }

    public void CollectLarvae()
    {
        if (sourcePlayer != null)
        {
            sourcePlayer.clip = larvaeSFX;
            sourcePlayer.Play();
        }
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

    public void ShootGun(GameObject Source)
    {
        //sourceWeapon.clip = gunShootSFX;
        //sourceWeapon.Play();

        // Play sounds with overlap.
        AudioSource AS = Source.AddComponent<AudioSource>();
        AS.clip = gunShootSFX;
        AS.Play();
    }

    public void ShootLauncher(GameObject Source)
    {
        sourceWeapon2.clip = launcherShootSFX;
        sourceWeapon2.Play();

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Environment Test")
        {
                // Play sounds with overlap, but only when in the actual game.
                AudioSource AS = Source.AddComponent<AudioSource>();
                AS.clip = launcherShootSFX;
                AS.Play();
        }
    }

    public void ActivateLazer(GameObject Source)
    {
        //sourceLazer.clip = lazerSFX;
        //if (!sourceLazer.isPlaying)
        //{
        //    sourceLazer.Play();
        //}

        // Play sounds with overlap. Also fixes looping issue.
        AudioSource AS = Source.AddComponent<AudioSource>();
        AS.clip = lazerSFX;
        AS.Play();
    }

    public void DeactivateLaser()
    {
        // Sometimes this isn't called when there are two or more Lasers attached;
        // the Laser just keeps looping and playing.
        if (sourceLazer.isPlaying)
        {
            sourceLazer.Stop();
        }
    }

    public void AntAttack()
    {
        sourceAnt.clip = antAttackSFX;
        sourceAnt.Play();
    }

    public void SpiderAttack()
    {
        sourceSpider.clip = spiderAttackSFX;
        sourceSpider.Play();
    }

    public void SpiderWeb()
    {
        sourceSpider.clip = shootWebSFX;
        sourceSpider.Play();
    }

    public void ShootFlame()
    {
        sourceFlame.clip = fireSFX;
        sourceFlame.Play();
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
