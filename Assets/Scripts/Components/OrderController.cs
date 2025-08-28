using UnityEngine;

public class OrderController : MonoBehaviour {

    [SerializeField] private float yOffset;
    private SpriteRenderer _spriteRenderer;
    
    void Awake() => _spriteRenderer = GetComponent<SpriteRenderer>();
    
    void Update() {
        _spriteRenderer.sortingOrder = Mathf.RoundToInt(100 * (transform.position.y + yOffset)) * -1;
    }
}
