using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer _instance;
    
    [SerializeField] private List<AudioClip> _audioClips;
    private AudioSource _audioSource;
    private int _index = -1;

    void Awake() {
        if (_instance == null) {
            DontDestroyOnLoad(this);
            _instance = this;
            _audioSource = GetComponent<AudioSource>();
        }
        else {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (_audioSource.isPlaying) return;

        _index++;
        if (_index >= _audioClips.Count) _index = 0;
        
        _audioSource.clip = _audioClips[_index];
        _audioSource.Play();
    }
    
    
}
