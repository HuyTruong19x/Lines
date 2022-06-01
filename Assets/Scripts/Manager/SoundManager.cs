using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private AudioSource _backgroundSound;
    [SerializeField]
    private AudioSource _sfxSound;
    [SerializeField]
    private List<SFXConfig> _configList;

    Dictionary<SFX, AudioClip> _sounds = new Dictionary<SFX, AudioClip>();
    private void Start()
    {
        LoadResources();
    }

    private void LoadResources()
    {
        foreach(var config in _configList)
        {
            var audio = Resources.Load<AudioClip>($"Audio/SFX_{config.audioName}");
            if(audio != null)
            {
                _sounds.Add(config.sfx, audio);
            }    
            else
            {
                Debug.LogWarning($"Can't load SFX {config.audioName}");
            }    
        }    
    }

    private AudioClip GetAudioClip(SFX i_sfx)
    {
        if(_sounds.ContainsKey(i_sfx))
        {
            return _sounds[i_sfx];
        }    
        return null;
    }    

    public void PlaySFX(SFX i_sfx)
    {
        _sfxSound.PlayOneShot(GetAudioClip(i_sfx));
    }    

    public void Mute(bool i_isMute)
    {
        _backgroundSound.mute = i_isMute;
        _sfxSound.mute = i_isMute;
    }    
}
[System.Serializable]
public class SFXConfig
{
    public SFX sfx;
    public string audioName;
}

public enum SFX
{
    SELECTED,
    MOVE,
    CONFETTI,
    GAMEOVER,
    CANNOTMOVE,
}
