using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioMixer mixer;

    private AsyncOperationHandle<AudioClip> bgmHandle;
    private Dictionary<string, AudioClip> sfxCache = new Dictionary<string, AudioClip>();

    protected override void Awake()
    {
        base.Awake();

        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    // BGM 로드 및 재생
    public void PlayBGM(string address)
    {
        // 기존 BGM 해제
        StopBGM();

        bgmHandle = Addressables.LoadAssetAsync<AudioClip>(address);
        bgmHandle.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                bgmSource.clip = handle.Result;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        };
    }

    // BGM 정지 및 언로드
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }

        if (bgmHandle.IsValid())
        {
            Addressables.Release(bgmHandle);
            bgmSource.clip = null;
        }
    }

    // SFX 로드 및 재생 (캐싱)
    public void PlaySFX(string address)
    {
        if (sfxCache.TryGetValue(address, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Addressables.LoadAssetAsync<AudioClip>(address).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    sfxCache[address] = handle.Result;
                    sfxSource.PlayOneShot(handle.Result);
                }
            };
        }
    }

    // 볼륨 관리
    public void SetBGMVolume(float volume)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("BGMVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}