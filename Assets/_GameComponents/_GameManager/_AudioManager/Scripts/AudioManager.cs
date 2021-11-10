using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
    public Sound[] themes;
    public Sound[] sounds;
    private void Awake()
    {
        foreach (Sound theme in themes)
        {
            theme.source = gameObject.AddComponent<AudioSource>();
            theme.source.clip = theme.clip;
            theme.source.pitch = theme.pitch;
            theme.source.volume = theme.volume;
            theme.source.loop = true;
            theme.isTheme = true;
        }
        
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.pitch = sound.pitch;
            sound.source.volume = sound.volume;
            sound.source.loop = false;
            sound.isTheme = false;
        }

    }

    public void PlaySound(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " was not found");
            return;
        }
        else if (s.source.isPlaying)
        {
            Debug.Log("Sound: " + name + "is already Playing");
            s.source.Stop();
        }
        // s.source.pitch = pitch;
        s.source.Play();
    }

    internal bool isPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogError("Sound: " + name + " was not found");
            return false;
        }

        return s.source.isPlaying;
    }

    public void PlayTheme(string name)
    {
        Sound t = Array.Find(themes, theme => theme.name == name);
        if (t == null)
        {
            Debug.LogError("Theme: " + name + " was not found");
            return;
        }
        else if (t.source.isPlaying)
        {
            Debug.LogWarning("Theme: " + name + "is already Playing");
            return;
        }
        FadeOutOtherThemes();
        t.source.volume = t.volume;
        t.source.Play();
    }

    public void StopTheme()
    {
        Sound t = Array.Find(themes, theme => theme.source.isPlaying);
        if (t != null) 
            t.source.Stop();
    }

    private void FadeOutOtherThemes()
    {
        Sound currentTheme = Array.Find(themes, theme => theme.source.isPlaying);
        if (currentTheme == null)
        {
            Debug.LogWarning("No theme was played before, therfore no fading out");
            return;
        }
        if (!currentTheme.isFadingOut)
        {
            StartCoroutine(currentTheme.FadeOut());
        }
    }

    public void StopSound(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);
        if (sound == null)
        {
            Debug.LogError("Sound: " + name + " was not found");
            return;
        }
        sound.source.Stop();
    }

    public void StopAllSounds()
    {
        foreach (Sound sound in sounds)
        {
            if (!sound.isFadingOut)
            {
                StartCoroutine(sound.FadeOut());
            }
        }
    }


    public void OnLevelFinishedLoading(Scene scene)
    {
        StopAllSounds();
        HandleAudioThemeFor(scene);
    }


    private void HandleAudioThemeFor(Scene scene)
    {
        if (scene.name == Scenes.TITLE || scene.name == Scenes.CONTROLS || scene.name == Scenes.INTRO || scene.name == Scenes.CREDITS)
            PlayTheme("intro");
        else if (scene.name == Scenes.STAGES[0] || scene.name == Scenes.STAGES[1])
            PlayTheme("theme1");
        else if (scene.name == Scenes.STAGES[2] || scene.name == Scenes.STAGES[3] || scene.name == Scenes.CREDITS)
            PlayTheme("theme2");
        else
            StopTheme();
    }
}
