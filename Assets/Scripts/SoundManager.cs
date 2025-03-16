using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NameAudioClipPair
{
    [SerializeField] private string _name = "";
    [SerializeField] private AudioClip _audioClip = null;

    public string Name
    {
        get => _name;
    }

    public AudioClip AudioClip
    {
        get => _audioClip;
    }
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField] private NameAudioClipPair[] _nameAudioClipPairs;

    private Dictionary<string, AudioClip> _nameToAudioClip = new Dictionary<string, AudioClip>();

    private AudioSource _seAudioSource;
    private AudioSource _bgmAudioSource;

    public bool BGMPlaying
    {
        get => _bgmAudioSource.isPlaying;
    }

    public bool SEMute
    {
        get => _seAudioSource.mute;
        set
        {
            _seAudioSource.mute = value;
        }
    }

    public bool BGMMute
    {
        get => _bgmAudioSource.mute;
        set
        {
            _bgmAudioSource.mute = value;
        }
    }

    protected override void Init()
    {
        _seAudioSource = gameObject.AddComponent<AudioSource>();
        _seAudioSource.playOnAwake = false;

        _bgmAudioSource = gameObject.AddComponent<AudioSource>();
        _bgmAudioSource.playOnAwake = false;
        _bgmAudioSource.loop = true;

        //SEMute = Settings.SEMute;
        //BGMMute = Settings.BGMMute;

        foreach (var pair in _nameAudioClipPairs)
        {
            _nameToAudioClip.Add(pair.Name, pair.AudioClip);
        }
    }

    public void PlaySE(string name)
    {
        var ac = GetAudioClipByName(name);
        if (ac == null)
        {
            return;
        }

        _seAudioSource.PlayOneShot(ac);
    }

    public void PlayBGM(string name)
    {
        var ac = GetAudioClipByName(name);
        if (ac == null)
        {
            return;
        }

        _bgmAudioSource.clip = ac;
        _bgmAudioSource.Play();
    }

    private AudioClip GetAudioClipByName(string name)
    {
        if (_nameToAudioClip.ContainsKey(name))
        {
            return _nameToAudioClip[name];
        }
        return null;
    }
}