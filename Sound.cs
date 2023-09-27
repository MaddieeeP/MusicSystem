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

    public bool NextSection()
    {
        if (_currentSection >= sections.Count - 1)
        {
            return false;
        }
        _currentSection++;
        return true;
    }

    public void Reset()
    {
        _currentSection = 0;
    }
}