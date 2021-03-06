﻿using System.Collections;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [HideInInspector]
    public AudioSource source;
    public AudioClip clip;

    public string name;

    [HideInInspector]
    public bool loop;
    [HideInInspector]
    public bool isTheme;

    public float pitch = 1F;
    public float volume = .5f;

    [HideInInspector]
    public bool isFadingOut = false;
    private float fadeOutTargetVolume = .1f;
    private float fadeOutSpeed = 2F;

    public IEnumerator FadeOut()
    {
        isFadingOut = true;
        while (source.volume > fadeOutTargetVolume)
        {
            source.volume -= fadeOutSpeed * Time.deltaTime;
            yield return null;
        }
        source.Stop();
        source.volume = volume;
        yield return new WaitForSeconds(0F);

        isFadingOut = false;
    }
}