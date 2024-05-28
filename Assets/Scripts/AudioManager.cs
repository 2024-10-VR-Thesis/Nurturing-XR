using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    public AudioClip backgroundMusic;
    public AudioClip midMusic;
    public AudioClip stressedMusic;
    // Start is called before the first frame update
    void Start()
    {
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
    public void changeTrack(int cont)
    {

        if (cont <= 0)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
        else if (cont == 1)
        {
            musicSource.clip = midMusic;
            musicSource.Play();
        }
        else
        {
            musicSource.clip = stressedMusic;
            musicSource.Play();
        }
    }
}
