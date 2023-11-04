using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    [SerializeField] private string _name;
    public string name { get { return _name; } }

    [SerializeField] private List<AudioClip> sections;

    [SerializeField] private List<bool> _loopSection;
    public List<bool> loopSection { get { return _loopSection; } }
    public bool loop { get { return loopSection[currentSection]; } }

    public AudioClip clip { get { return sections[currentSection]; } }

    private int _currentSection = 0;
    public int currentSection { get { return Math.Clamp(_currentSection, 0, sections.Count - 1); } }

    private float _time = 0f;
    public float time { get { return _time; } }

    public float length { get { return clip.length; } }

    public bool NextSection()
    {
        if (_currentSection >= sections.Count - 1)
        {
            _currentSection = sections.Count - 1;
            return false;
        }
        _currentSection++;
        _time = 0f;
        return true;
    }

    public void AddTime(float deltaTime)
    {
        _time += deltaTime;
        while (_time > clip.length)
        {
            _time -= clip.length;
        }
    }

    public void SetTime(float timeValue)
    {
        _time = Math.Clamp(timeValue, 0f, clip.length);
    }

    public void SetSection(int section)
    {
        _currentSection = Math.Clamp(section, 0, sections.Count - 1);
        _time = 0f;
    }

    public void Reset()
    {
        _currentSection = 0;
        _time = 0f;
    }

    public void CopyPlaybackData(Sound sound) 
    {
        SetSection(sound.currentSection);
        SetTime(sound.time);
    }
}