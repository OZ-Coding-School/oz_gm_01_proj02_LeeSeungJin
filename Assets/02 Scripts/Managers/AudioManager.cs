using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioMixer mixer;

    protected override void Awake()
    {
        base.Awake();

        // 실행 시 저장된 볼륨 불러오기
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    // Addressables로 BGM 로드 및 재생
    public void PlayBGM(string address)
    {
        Addressables.LoadAssetAsync<AudioClip>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                bgmSource.clip = handle.Result;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        };
    }

    // BGM 정지
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    // Addressables로 SFX 로드 및 재생
    public void PlaySFX(string address)
    {
        Addressables.LoadAssetAsync<AudioClip>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                sfxSource.PlayOneShot(handle.Result);
            }
        };
    }

    // BGM 볼륨 조절 + 저장
    public void SetBGMVolume(float volume)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    // SFX 볼륨 조절 + 저장
    public void SetSFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}