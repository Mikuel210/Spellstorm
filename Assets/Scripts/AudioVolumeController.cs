using UnityEngine;

public class AudioVolumeController : MonoBehaviour
{
    private AudioSource _audioSource;
    private GameObject _player;
    
    void Start() {
        _audioSource = GetComponent<AudioSource>();
        _player = PlayerController.Instance.gameObject;
    }
    
    void Update()
    {
        _audioSource.volume =  0.3f / (Vector2.Distance(_player.transform.position, transform.position) * 0.1f + 1);
    }
}
