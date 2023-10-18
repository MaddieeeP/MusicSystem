using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private Sound[] tracks;

    private List<Sound> currentTracks = new List<Sound>();
    public Sound currentTrack 
    { 
        get 
        { 
            if (currentTracks.Count == 0)
                return null;

            return currentTracks[0]; 
        } 
    }

    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;

    private bool sourceIsA = true;

    public AudioSource currentMusicSource
    {
        get
        {
            if (sourceIsA)
            {
                return musicSourceA;
            }
            return musicSourceB;
        }
    }
    public AudioSource previousMusicSource
    {
        get
        {
            if (sourceIsA)
            {
                return musicSourceB;
            }
            return musicSourceA;
        }
    }

    private bool paused = false;

    private float targetVolumeA = 1f;
    private float targetVolumeB = 0f;

    public float currentTargetVolume
    {
        get
        {
            if (sourceIsA)
            {
                return targetVolumeA;
            }
            return targetVolumeB;
        }
        private set 
        {
            if (sourceIsA)
            {
                targetVolumeA = value;
                return;
            }
            targetVolumeB = value;
        }
    }
    public float previousTargetVolume
    {
        get
        {
            if (sourceIsA)
            {
                return targetVolumeB;
            }
            return targetVolumeA;
        }
        private set
        {
            if (sourceIsA)
            {
                targetVolumeB = value;
                return;
            }
            targetVolumeA = value;
        }
    }

    public float masterVolume = 1f;

    [SerializeField] private AnimationCurve fadeIn;
    [SerializeField] private AnimationCurve fadeOut;

    public Sound GetTrack(string name)
    {
        return Array.Find(tracks, x => x.name == name);
    }

    public void Update()
    {
        currentMusicSource.volume = currentTargetVolume * masterVolume;
        previousMusicSource.volume = previousTargetVolume * masterVolume;
    }

    public void Pause()
    {
        paused = true;
    }

    public void Play(string name, bool fadeDown = true, bool fadeUp = true)
    {
        if (currentTracks.Count > 0)
        {
            if (name == currentTrack.name)
            {
                return;
            }
        }

        Sound track = GetTrack(name);

        if (track == null)
        {
            return;
        }

        currentTracks.Insert(0, track); //FIX - find if already in list
        StopAllCoroutines();

        if (fadeDown)
        {
            StartCoroutine(Fade(fadeOut));
        }
        sourceIsA = !sourceIsA;
        if (fadeUp)
        {
            StartCoroutine(Fade(fadeIn));
        }

        StartCoroutine(PlayTrack(currentMusicSource, currentTrack));
    }

    public void ForceSection(int section)
    {
        if (currentTracks.Count == 0)
        {
            return;
        }

        currentTrack.SetSection(section);

        StopAllCoroutines();
        StartCoroutine(PlayTrack(currentMusicSource, currentTrack));
    }

    public void ForceNextSection(bool endIfNone, bool fade = false)
    {
        if (currentTracks.Count == 0)
        {
            return;
        }

        StopAllCoroutines();

        if (!currentTrack.NextSection())
        {
            currentTracks.Remove(currentTrack);
            if (endIfNone)
            {
                if (fade)
                {
                    StartCoroutine(Fade(fadeOut));
                }
                else
                {
                    currentMusicSource.Stop();
                }
            }
            return;
        }

        StartCoroutine(PlayTrack(currentMusicSource, currentTrack));
    }

    public void ForceNextSection() => ForceNextSection(false, false);

    IEnumerator PlayTrack(AudioSource source, Sound track)
    {
        source.clip = track.clip;
        source.loop = track.loop;
        source.Play();

        while (paused || source.isPlaying)
        {
            if (source != currentMusicSource && !paused && previousTargetVolume == 0f)
            {
                source.Stop();
                yield break;
            }
            yield return null;
        }

        if (!track.loop)
        {
            if (!currentTrack.NextSection())
            {
                currentTracks.Remove(track);
                yield break;
            }
        }

        StartCoroutine(PlayTrack(source, track));
    }

    IEnumerator Fade(AnimationCurve fadeCurve, bool sourceA)
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeCurve.length)
        {
            timeElapsed = Math.Clamp(timeElapsed + Time.deltaTime, 0f, fadeCurve.length);

            if (sourceA)
            {
                targetVolumeA = fadeCurve.Evaluate(timeElapsed);
            } 
            else
            {
                targetVolumeB = fadeCurve.Evaluate(timeElapsed);
            }

            yield return null;
        }
        if (fadeCurve.Evaluate(timeElapsed) < 0.1f)
        {
            if (sourceA)
            {
                targetVolumeA = 0f;
            }
            else
            {
                targetVolumeB = 0f;
            }
        }
    }

    IEnumerator Fade(AnimationCurve fadeCurve) => Fade(fadeCurve, sourceIsA);

}
