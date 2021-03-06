using UnityEngine.Audio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//This Classes Awake and play methods taken from Brackeys
public class AudioManager : MonoBehaviour
{
    private int songPlaying;
    public Slider volumeSlider;
    public static AudioManager instance;
    public bool volumeChanged;
    public AudioMixerGroup mixerGroup;
    int uiPressed;

    public Sound[] sounds;
    Sound music;
    float maxVolume;

    private void Start()
    {
        uiPressed = 0;
        songPlaying = 0;
        maxVolume = 0;
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = mixerGroup;
        }
    }

    public void Play(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Play();
    }

    public void Update()
    {
        String sound = "";
        if (songPlaying != 0)
        {
            if (music == null)
                return;
            if (music.isPlaying())
            {
                if (music.source.time < 5)
                {
                    if (maxVolume == 0)
                    {
                        maxVolume = music.source.volume;
                    }
                    music.source.volume = music.source.time * (maxVolume / 5);
                }
                float timeRemaining = music.clip.length - music.source.time;
                if (timeRemaining < 5)
                {
                    maxVolume = music.source.volume;
                    music.source.volume = maxVolume / 5 * timeRemaining;
                }
                //do nothing
            }
            else
            {
                songPlaying++;
                if (songPlaying > 4)
                {
                    songPlaying = 1;
                }
                if (music == null)
                    return;
                if (songPlaying == 1)
                {
                    sound = "Song1";
                }
                else if (songPlaying == 2)
                {
                    sound = "Song2";
                }
                else if (songPlaying == 3)
                {
                    sound = "Song3";
                }
                else if (songPlaying == 4)
                {
                    sound = "Song4";
                }
                music = Array.Find(sounds, item => item.name == sound);
                music.volume = volumeSlider.value;
                music.source.volume = volumeSlider.value;
                Play(music.name);
            }
        }
        else
        {
            songPlaying = 1;
            music = Array.Find(sounds, item => item.name == "Song1");

            Play(music.name);
        }
        
    }

    public void endTurn()
    {
        Sound sound;
        sound = Array.Find(sounds, item => item.name == "endTurnSound");
        Play(sound.name);
    }

    public void uiSelected()
    {
        Sound sound;
        if(uiPressed == 0)
        {
            sound = Array.Find(sounds, item => item.name == "uiButtonSound1");
            uiPressed++;
        }
        else if (uiPressed == 1)
        {
            sound = Array.Find(sounds, item => item.name == "uiButtonSound2");
            uiPressed++;
        }
        else
        {
            sound = Array.Find(sounds, item => item.name == "uiButtonSound3");
            uiPressed = 0;
        }
        Play(sound.name);
    }

    public void OnValueChanged()
    {
        music.volume = volumeSlider.value;
        music.source.volume = volumeSlider.value;
        volumeChanged = true;
    }
    
}