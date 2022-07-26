using UnityEngine;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
public class AudioManager : Singleton<AudioManager> {

    #region Fields
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundsSource;
    #endregion
    public void PlayMusic(AudioClip clip, bool loop = true) {
        if (_musicSource.clip == clip) return;
        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }
    public void PauseMusic(AudioClip clip, bool loop = true)
    {
        _musicSource.Pause();
    }
    public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1) {
        _soundsSource.transform.position = pos;
        PlaySound(clip, vol);
    }
    public void PlaySound(AudioClip clip, float vol = 1) {
        _soundsSource.PlayOneShot(clip, vol);
    }
    public void SetMusicVolume(float volume) => _musicSource.volume = volume;
    public void SetSoundsVolume(float volume) => _soundsSource.volume = volume;
}