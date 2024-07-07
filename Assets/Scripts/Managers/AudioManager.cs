using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;

    [SerializeField] private string _musicTetris;
    private float _musicVolume;

    public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = value;
            if (_musicSource != null)
            {
                _musicSource.volume = _musicVolume;
            }
        }
    }

    private void Start()
    {
        PlayLevelMusic();
    }
    public void PlayLevelMusic()
    {
        PlayMusic(Resources.Load($"Music/{_musicTetris}") as AudioClip);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", MusicVolume);
    }

    private void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.Play();
    }
    public void StopMusic()
    {
        _musicSource.Stop();
    }
}
