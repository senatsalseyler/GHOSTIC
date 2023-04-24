using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SettingsMenu : MonoBehaviour
{
    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;
    public void SetVolumeMusic(float volume)
    {
        musicMixer.SetFloat("music_volume", volume);
    }
    public void SetVolumeSFX(float volume)
    {
        sfxMixer.SetFloat("sfx_volume", volume);
    }
}
