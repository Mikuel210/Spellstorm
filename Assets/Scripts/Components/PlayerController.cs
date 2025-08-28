using System.Collections.Generic;
using Helpers;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Singleton<PlayerController> {

    [SerializeField] private float movementSpeed;
    private float _initialSpeed;
    
    [Space, SerializeField] private List<GameObject> fireSpellPrefabs;
    [SerializeField] private List<GameObject> iceSpellPrefabs;
    [SerializeField] private List<GameObject> lightningSpellPrefabs;

    [Space, SerializeField] private AudioClip throwFire;
    [SerializeField] private AudioClip throwIce;
    [SerializeField] private AudioClip throwLightning;
    
    [Space, SerializeField] private float fireSpeedReduction;
    [SerializeField] private float fireSpeedDuration;
    [SerializeField] private float iceSpeedReduction;
    [SerializeField] private float iceSpeedDuration;
    [SerializeField] private float lightningSpeedReduction;
    [SerializeField] private float lightningSpeedDuration;
    
    public Spell CurrentSpell { get; private set; }
    public FireSpell FireSpell { get; } = new();
    public IceSpell IceSpell { get; } = new();
    public LightningSpell LightningSpell { get; } = new();
    
    private Animator _animator;

    void Awake() {
        _initialSpeed = movementSpeed;
        
        FireSpell.Initialize(fireSpellPrefabs);
        IceSpell.Initialize(iceSpellPrefabs);
        LightningSpell.Initialize(lightningSpellPrefabs);
        
        CurrentSpell = FireSpell;
        _animator = GetComponent<Animator>();  
    }
    
    void Update()
    {
        UpdateMovement();
        UpdateSwitchSpell();
        UpdateSpell();
    }

    private void UpdateMovement() {
        movementSpeed = _initialSpeed;

        if (FireSpell.Debounce < fireSpeedDuration) movementSpeed -= fireSpeedReduction;
        if (IceSpell.Debounce < iceSpeedDuration) movementSpeed -= iceSpeedReduction;
        if (LightningSpell.Debounce < lightningSpeedDuration) movementSpeed -= lightningSpeedReduction;
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(horizontal, vertical).normalized * movementSpeed;
        
        transform.Translate(movement * Time.deltaTime);
        
        if (horizontal > 0)
            _animator.SetInteger("X", 1);
        else if (horizontal < 0)
            _animator.SetInteger("X", -1);
        
        _animator.SetBool("Moving", horizontal != 0 || vertical != 0);
    }
    
    private void UpdateSwitchSpell() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UIManager.Instance.SelectSpell(FireSpell);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UIManager.Instance.SelectSpell(IceSpell);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UIManager.Instance.SelectSpell(LightningSpell);
    }

    private void UpdateSpell() {
        FireSpell.Debounce += Time.deltaTime;
        IceSpell.Debounce += Time.deltaTime;
        LightningSpell.Debounce += Time.deltaTime;
        
        if (!Input.GetMouseButton(0)) return;
        if (CurrentSpell.Debounce < CurrentSpell.FireRate) return;
        if (MouseOverUI()) return;
        
        // Get direction pointing towards the mouse
        Vector2 fromPosition = transform.position;
        Vector2 toPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = toPosition - fromPosition;
        Quaternion rotation = Quaternion.LookRotation(direction);
        
        // Invoke the spell
        CurrentSpell.Invoke(fromPosition, rotation);
        CurrentSpell.Debounce = 0;
        
        // SFX
        if (CurrentSpell == FireSpell && throwFire) AudioSource.PlayClipAtPoint(throwFire, transform.position);
        if (CurrentSpell == IceSpell && throwIce) AudioSource.PlayClipAtPoint(throwIce, transform.position);
        if (CurrentSpell == LightningSpell && throwLightning) AudioSource.PlayClipAtPoint(throwLightning, transform.position);
    }
    
    public bool MouseOverUI() {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results) {
            if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }

        return false;
    }

    public void UpgradeSpell(Spell spell) {
        UpgradeSO upgrade = GameManager.Instance.SpellUpgrades[spell][spell.Level - 1];

        spell.Damage = upgrade.damage;
        spell.FireRate = upgrade.fireRate;
        spell.Level++;
    }
    
    public void SetCurrentSpell(Spell spell) => CurrentSpell = spell;
}
