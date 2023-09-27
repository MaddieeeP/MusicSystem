using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private Sound[] tracks;

    public Sound currentTrack;

    public AudioSource musicSource;

    private float trackChangeCooldownLength = 0.5f;
    private float trackChangeCooldown = 10f;

    void Start()
    {
        Play("boss1");
    }

    public Sound GetTrack(string name)
    {
        return Array.Find(tracks, x => x.name == name);
    }

    public void Play(string name)
    {
        if (name == currentTrack.name)
        {
            return;
        }

        Sound track = GetTrack(name);

        if (track == null)
        {
            return;
        }

        track.Reset();
        currentTrack = track;
        trackChangeCooldown = trackChangeCooldownLength;

        StopAllCoroutines();
        StartCoroutine(PlayTrack());
    }

    public void ForceNextSection()
    {
        if (currentTrack == null)
        {
            return;
        }

        StopAllCoroutines();

        if (!currentTrack.NextSection())
        {
            currentTrack = null;
            return;
        }

        StartCoroutine(PlayTrack());
    }

    IEnumerator PlayTrack()
    {
        musicSource.clip = currentTrack.clip;
        musicSource.loop = currentTrack.loop;
        musicSource.Play();

        if (currentTrack.loop)
        {
            yield break;
        } else
        {
            yield return new WaitForSeconds(currentTrack.clip.length);
            if (!currentTrack.NextSection())
            {
                currentTrack = null;
                yield break;
            }
        }

        StartCoroutine(PlayTrack());
    }
}
