using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource musicSource2;
    public AudioClip backgroundMusic;
    public AudioClip midMusic;
    public AudioClip stressedMusic;
    public AudioClip backgroundChangeMusic;
    public AudioClip midChangeMusic;
    public AudioClip stressedChangeMusic;
    public AudioClip tutorial;
    // Start is called before the first frame update
    async void Start()
    {
        musicSource.clip = tutorial;
        musicSource.Play();
        await Task.Delay(15000);
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

        if (!musicSource.isPlaying && musicSource.clip != tutorial)
        {
            musicSource.Play();
        }
    }
    public async void changeTrack(int cont)
    {

        if (cont <= 0)
        {
            if (musicSource.clip != backgroundMusic)
            {
                musicSource2.clip = backgroundChangeMusic;
                musicSource2.Play();
                musicSource.Stop();
                await Task.Delay(3000);
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }
        else if (cont == 1)
        {
            musicSource2.clip = midChangeMusic;
            musicSource2.Play();
            await Task.Delay(3500);
            musicSource.clip = midMusic;
            musicSource.Play();
            musicSource2.Stop() ;
        }
        else
        {
            musicSource2.clip = stressedChangeMusic;
            musicSource2.Play();
            await Task.Delay(2500);
            musicSource.clip = stressedMusic;
            musicSource.Play();
            musicSource2.Stop();
        }
    }
}
