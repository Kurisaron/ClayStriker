using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{
    private Transform soundParent { get => transform.Find("Sound"); }
    private Transform sfxParent;

    private AudioSource musicPlayer;
    private List<AudioSource> sfxPlayers;

    [SerializeField] private List<Sound> soundEffects;

    public override void Awake()
    {
        base.Awake();

        if (SceneManager.GetActiveScene().name != GameManager.Instance.sceneLoader.loadingSceneName) return;

        AddMusicPlayer();
        SetupSFXPlayers();
    }

    private void AddMusicPlayer()
    {
        GameObject mPlayer = new GameObject("MusicPlayer");
        mPlayer.transform.SetPositionRotationAndParent(soundParent);
        musicPlayer = mPlayer.AddComponent<AudioSource>();
        musicPlayer.loop = true;
        //Debug.Log("Music player added to sound manager");
    }

    private void SetupSFXPlayers()
    {
        sfxParent = new GameObject("SFX").transform;
        sfxParent.SetPositionRotationAndParent(soundParent);

        sfxPlayers = new List<AudioSource>();
    }


    public void PlaySFX(SoundContext context)
    {
        Sound sound = soundEffects.Find(effect => effect.context == context);
        if (sound == null)
        {
            Debug.Log("No sound matching context");
            return;
        }

        PlaySFX(sound.clip);
        Debug.Log("SFX played: " + context.ToString());
    }

    private async void PlaySFX(AudioClip clip)
    {
        AudioSource source;
        if (sfxPlayers.Count <= 0 || sfxPlayers.All(source => source.isPlaying))
        {
            // Add a new sfx player. Either no sfx players or all in use
            GameObject newPlayer = new GameObject("SFX_Player");
            newPlayer.transform.SetPositionRotationAndParent(sfxParent);
            sfxPlayers.Add(source = newPlayer.AddComponent<AudioSource>());
        }
        else
        {
            source = sfxPlayers.Find(source => !source.isPlaying);
            source.gameObject.SetActive(true);
        }

        source.clip = clip;
        source.Play();

        await WaitForPlayComplete(source);

        source.gameObject.SetActive(false);
    }

    private async Task WaitForPlayComplete(AudioSource source)
    {
        while (source != null && source.isPlaying) await Task.Yield();
    }
}

[Serializable]
public class Sound
{
    public SoundContext context;
    public AudioClip clip;
}

public enum SoundContext
{
    BulletShoot,
    TargetBreak,
    PatRegular,
    PatDisappointed
}
