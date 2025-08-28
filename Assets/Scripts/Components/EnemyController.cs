using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyController : MonoBehaviour {

    [SerializeField] private GameObject hitEffectFire;
    [SerializeField] private GameObject hitEffectIce;
    [SerializeField] private GameObject hitEffectLightning;
    
    [Space, SerializeField] private AudioClip damageFire;
    [SerializeField] private AudioClip damageIce;
    [SerializeField] private AudioClip damageLightning;
    
    [Space, SerializeField] private float speed;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float damageToTower;

    private List<SpeedModifier> _modifiers = new();
    
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Health _health;
    
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
    }
    
    void Update() {
        // Update modifiers
        float finalSpeed = speed;
        Color finalColor = Color.white;
        List<SpeedModifier> modifiersToRemove = new();

        foreach (SpeedModifier modifier in _modifiers) {
            modifier.timeElapsed += Time.deltaTime;

            if (modifier.timeElapsed >= modifier.duration) {
                modifiersToRemove.Add(modifier);
                break;
            }
            
            finalSpeed *= modifier.speedMultiplier;
            finalColor *= modifier.color;
        }
        
        _modifiers = _modifiers.Except(modifiersToRemove).ToList();
        
        // Update color
        _spriteRenderer.color = finalColor;

        // Update movement
        Vector2 direction = (Vector2.zero - (Vector2)transform.position).normalized;
        transform.Translate(direction * (finalSpeed * speedMultiplier * Time.deltaTime));
        
        _animator.SetInteger("X", Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? (direction.x > 0 ? 1 : -1) : 0);
        _animator.SetInteger("Y", Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? (direction.y > 0 ? 1 : -1) : 0);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.gameObject.CompareTag("Projectile")) return;
        if (_health.CurrentHealth <= 0) return;
        
        ProjectileController projectile = other.gameObject.GetComponent<ProjectileController>();
        GameObject prefab = null;

        if (projectile.isFire) prefab = hitEffectFire;
        if (projectile.isIce) prefab = hitEffectIce;
        if (projectile.isLightning) prefab = hitEffectLightning;
        
        GameObject effect = Instantiate(prefab, transform.position, projectile.transform.rotation);
        effect.transform.localScale = transform.localScale;
        Destroy(effect, 2f);

        GameManager.Instance.EarnPoints(Mathf.RoundToInt(Mathf.Min(projectile.damage, _health.CurrentHealth)));
        _health.TakeDamage(projectile.damage);
        
        UIManager.Popup(projectile.damage.ToString(), transform.position + Vector3.up, Color.red);
        
        if (projectile.isFire) AudioSource.PlayClipAtPoint(damageFire, transform.position, 1);
        if (projectile.isIce) AudioSource.PlayClipAtPoint(damageIce, transform.position, 1);
        if (projectile.isLightning) AudioSource.PlayClipAtPoint(damageLightning, transform.position, 1);

        if (projectile.GetComponentInChildren<ParticleSystem>() && projectile.bounces == 0) {
            ParticleSystem particleSystem = projectile.GetComponentInChildren<ParticleSystem>();
            particleSystem.Stop();
            particleSystem.transform.position = particleSystem.transform.parent.position;
            particleSystem.transform.rotation = particleSystem.transform.parent.rotation;
            particleSystem.transform.parent = null;
            particleSystem.transform.localScale = Vector3.one;
            
            Destroy(particleSystem, 5f);
        }
        
        if (projectile.cancelModifiers) _modifiers.Clear();
        if (projectile.useModifier) _modifiers.Add(projectile.modifier);
        if (projectile.bounces > 0) projectile.Bounce();
        else {
            if (projectile.isLightning) Destroy(effect);
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (!other.gameObject.CompareTag("Tower")) return;
        other.gameObject.GetComponent<Health>().TakeDamage(damageToTower * Time.fixedDeltaTime);
    }

}

[Serializable]
public class SpeedModifier {

    public float speedMultiplier;
    public float duration;
    public float timeElapsed;
    public Color color;

}
