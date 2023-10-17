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

    private bool paused = false;

    public float volume;

    private float trackChangeCooldownLength = 0.5f;
    private float trackChangeCooldown = 10f;

    public Sound GetTrack(string name)
    {
        return Array.Find(tracks, x => x.name == name);
    }

    public void Pause()
    {
        paused = true;
    }

    public void Play(string name)
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
        trackChangeCooldown = trackChangeCooldownLength;

        StopAllCoroutines();
        StartCoroutine(PlayTrack(currentMusicSource, currentTrack));
    }

    public void ForceNextSection()
    {
        if (currentTracks.Count == 0)
        {
            return;
        }

        if (!currentTrack.NextSection())
        {
            currentTracks.Remove(currentTrack);
            currentMusicSource.Stop(); //Fade
            sourceIsA = !sourceIsA;
            return;
        }

        StartCoroutine(PlayTrack(currentMusicSource, currentTrack));
    }

    IEnumerator PlayTrack(AudioSource source, Sound track)
    {
        source.clip = track.clip;
        source.loop = track.loop;
        source.Play();

        //yield return new WaitForSeconds(track.clip.length);
        while (paused || source.isPlaying)
        {
            if (source != currentMusicSource && !paused && source.volume == 0f) //Fade transition ended
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
}
