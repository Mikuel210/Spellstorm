using UnityEngine;

public class CameraFollowController : MonoBehaviour {

    [SerializeField] private float movement;
    private Transform _player;
    
    void Start() {
        _player = PlayerController.Instance.transform;
    }
    
    void Update() {
        transform.position =
            Vector3.Lerp(_player.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), movement);
    }
}
