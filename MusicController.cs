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

    public AudioSource musicSourceA;
    public AudioSource musicSourceB;

    public float volume;

    private float trackChangeCooldownLength = 0.5f;
    private float trackChangeCooldown = 10f;
    private bool sourceB = false;

    public Sound GetTrack(string name)
    {
        return Array.Find(tracks, x => x.name == name);
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

        currentTracks.Insert(0, track); //FIX
        trackChangeCooldown = trackChangeCooldownLength;

        StopAllCoroutines();
        StartCoroutine(PlayTrack());
    }

    public void ForceNextSection()
    {
        if (currentTracks.Count == 0)
        {
            return;
        }

        StopAllCoroutines();

        if (!currentTrack.NextSection())
        {
            currentTracks.Remove(currentTrack);
            musicSourceA.Stop();
            return;
        }

        StartCoroutine(PlayTrack());
    }

    IEnumerator PlayTrack()
    {
        musicSourceA.clip = currentTrack.clip;
        musicSourceA.loop = currentTrack.loop;
        musicSourceA.Play();

        if (currentTrack.loop)
        {
            yield break;
        } else
        {
            yield return new WaitForSeconds(currentTrack.clip.length);
            if (!currentTrack.NextSection())
            {
                currentTracks.Remove(currentTrack);
                yield break;
            }
        }

        StartCoroutine(PlayTrack());
    }
}
