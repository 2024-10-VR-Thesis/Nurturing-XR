using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
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

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
    public async void changeTrack(int cont)
    {

        if (cont <= 0)
        {
            musicSource.clip = backgroundChangeMusic;
            musicSource.Play();
            await Task.Delay(3000);
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
        else if (cont == 1)
        {
            musicSource.clip = midChangeMusic;
            musicSource.Play();
            await Task.Delay(3000);
            musicSource.clip = midMusic;
            musicSource.Play();
        }
        else
        {
            musicSource.clip = stressedChangeMusic;
            musicSource.Play();
            await Task.Delay(3000);
            musicSource.clip = stressedMusic;
            musicSource.Play();
        }
    }
}
