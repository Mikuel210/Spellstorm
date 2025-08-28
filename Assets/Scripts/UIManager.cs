using System.Collections.Generic;
using CodeMonkey.Utils;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    [SerializeField] private GameObject _popupPrefab;
    private static GameObject popupPrefab;
    
    [Space, SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject spellsPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI waveText;
    [Space, SerializeField] private Sprite buttonSprite;
    [SerializeField] private Sprite closeButtonSprite;
    [SerializeField] private Image upgradeButtonImage;
    [SerializeField] private GameObject upgradeButtonIcon;

    private bool _gameOver;
    
    private GameObject _fireSpellButton;
    private GameObject _iceSpellButton;
    private GameObject _lightningSpellButton;

    private Image _fireSpellImage;
    private Image _iceSpellImage;
    private Image _lightningSpellImage;
    
    private GameObject _fireButton;
    private GameObject _iceButton;
    private GameObject _lightningButton;
    Dictionary<GameObject, Spell> _buttonSpells;

    void Awake() {
        popupPrefab = _popupPrefab;
        _fireSpellButton = spellsPanel.transform.Find("FireSpell").gameObject;
        _iceSpellButton = spellsPanel.transform.Find("IceSpell").gameObject;
        _lightningSpellButton = spellsPanel.transform.Find("LightningSpell").gameObject;
        
        _fireSpellImage = _fireSpellButton.transform.Find("Image").GetComponent<Image>();
        _iceSpellImage = _iceSpellButton.transform.Find("Image").GetComponent<Image>();
        _lightningSpellImage = _lightningSpellButton.transform.Find("Image").GetComponent<Image>();
        
        _fireButton = upgradePanel.transform.Find("FireSpell").gameObject;
        _iceButton = upgradePanel.transform.Find("IceSpell").gameObject;
        _lightningButton = upgradePanel.transform.Find("LightningSpell").gameObject;

        _buttonSpells = new() {
            { _fireButton, PlayerController.Instance.FireSpell },
            { _iceButton, PlayerController.Instance.IceSpell },
            { _lightningButton, PlayerController.Instance.LightningSpell }
        };
    }

    void Start() => UpdateSpellsPanel();

    void Update() {
        UpdateTimeVisuals();
        UpdatePointsText();
        UpdateWavesPanel();
        
        // Update upgrade panel
        if (!_isUpgradePanelOpen) return;

        foreach (KeyValuePair<GameObject, Spell> buttonSpellPair in _buttonSpells) {
            GameObject button = buttonSpellPair.Key;
            Spell spell = buttonSpellPair.Value;

            if (GameManager.Instance.SpellUpgrades[spell].Count < spell.Level) {
                button.GetComponent<Image>().color = new(0.5f, 0.5f, 0.5f);
                continue;
            }
            
            UpgradeSO upgrade = GameManager.Instance.SpellUpgrades[spell][spell.Level - 1];
            button.GetComponent<Image>().color = GameManager.Instance.Points >= upgrade.cost ? Color.white : new(0.5f, 0.5f, 0.5f);
        }
    }

    private void UpdateTimeVisuals() {
        // Update time visuals
        _fireSpellImage.fillAmount = PlayerController.Instance.FireSpell.Debounce / PlayerController.Instance.FireSpell.FireRate;
        _iceSpellImage.fillAmount = PlayerController.Instance.IceSpell.Debounce / PlayerController.Instance.IceSpell.FireRate;
        _lightningSpellImage.fillAmount = PlayerController.Instance.LightningSpell.Debounce / PlayerController.Instance.LightningSpell.FireRate;
    }

    private void UpdatePointsText() {
        // Update points text
        pointsText.text = GameManager.Instance.Points.ToString();
        
        Vector2 preferredValues = pointsText.GetPreferredValues();
        pointsText.rectTransform.sizeDelta = new(preferredValues.x, pointsText.rectTransform.sizeDelta.y);
    }

    private void UpdateWavesPanel() {
        if (_gameOver) return;
        
        GameManager gameManager = GameManager.Instance;
        string prefix = gameManager.Spawning ? "<mspace=0.45em>T</mspace>+" : "<mspace=0.45em>T</mspace>-";

        float time = gameManager.Spawning
            ? gameManager.Time
            : gameManager.Interval - (gameManager.Time - gameManager.CurrentWaveDuration) + 1;

        int minutes = Mathf.FloorToInt(time) / 60;
        int seconds = Mathf.FloorToInt(time) % 60;
        
        string formattedTime = $"<mspace=0.45em>{minutes:00}</mspace>:<mspace=0.45em>{seconds:00}</mspace>";
        timeText.text = prefix + formattedTime;

        waveText.text = "Wave " + (gameManager.Spawning ? gameManager.CurrentWave : gameManager.CurrentWave + 1);
    }
    
    public static void UpdateSlider(Slider slider, float progress, bool flipColor) {
        slider.value = progress;
        
        Image fill = slider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        fill.color = flipColor
            ? TriColorLerp(Color.green, Color.yellow, Color.red, slider.value)
            : TriColorLerp(Color.red, Color.yellow, Color.green, slider.value);
    }

    private static Color TriColorLerp(Color a, Color b, Color c, float t) {
        if (t > 0.5f)
            return Color.Lerp(b, c, t * 2 - 1f);
        
        return Color.Lerp(a, b, t * 2);
    }

    private void UpdateUpgradePanel() {
        upgradePanel.SetActive(true);
        
        foreach (KeyValuePair<GameObject, Spell> buttonSpellPair in _buttonSpells) {
            GameObject button = buttonSpellPair.Key;
            Spell spell = buttonSpellPair.Value;
            
            TextMeshProUGUI damageFrom = button.transform.Find("DamageFrom").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI damageTo = button.transform.Find("DamageTo").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI fireRateFrom = button.transform.Find("FireRateFrom").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI fireRateTo = button.transform.Find("FireRateTo").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI points = button.transform.Find("PointsText").GetComponent<TextMeshProUGUI>();
            
            damageFrom.text = spell.Damage.ToString();
            fireRateFrom.text = spell.FireRate.ToString();

            if (GameManager.Instance.SpellUpgrades[spell].Count < spell.Level) {
                // Max level
                points.text = "MAX LEVEL";
                damageTo.gameObject.SetActive(false);
                fireRateTo.gameObject.SetActive(false);
                button.transform.Find("DamageArrow").gameObject.SetActive(false);
                button.transform.Find("FireRateArrow").gameObject.SetActive(false);
            }
            else {
                UpgradeSO upgradeSO = GameManager.Instance.SpellUpgrades[spell][spell.Level - 1];
                
                damageTo.text = upgradeSO.damage.ToString();
                fireRateTo.text = upgradeSO.fireRate.ToString();
                points.text = upgradeSO.cost.ToString();   
            }
        }
    }

    private bool _isUpgradePanelOpen;

    public void ToggleUpgradePanel() {
        _isUpgradePanelOpen = !_isUpgradePanelOpen;

        if (_isUpgradePanelOpen) {
            UpdateUpgradePanel();
            upgradePanel.SetActive(true);
            upgradeButtonImage.sprite = closeButtonSprite;
            upgradeButtonIcon.SetActive(false);
        }
        else {
            upgradePanel.SetActive(false);
            upgradeButtonImage.sprite = buttonSprite;
            upgradeButtonIcon.SetActive(true);
        }
    }

    private void UpgradeSpell(Spell spell) {
        if (GameManager.Instance.SpellUpgrades[spell].Count < spell.Level) return;
        
        UpgradeSO upgrade = GameManager.Instance.SpellUpgrades[spell][spell.Level - 1];

        if (GameManager.Instance.Points < upgrade.cost) return;
        
        GameManager.Instance.SpendPoints(upgrade.cost);
        PlayerController.Instance.UpgradeSpell(spell);

        ToggleUpgradePanel();
    }
    
    public void UpgradeFireSpell() => UpgradeSpell(PlayerController.Instance.FireSpell);
    public void UpgradeIceSpell() => UpgradeSpell(PlayerController.Instance.IceSpell);
    public void UpgradeLightningSpell() => UpgradeSpell(PlayerController.Instance.LightningSpell);

    private void UpdateSpellsPanel() {
        PlayerController player = PlayerController.Instance;
        Color activeColor = new(0.5f, 0.5f, 0.5f);
        Color inactiveColor = new(1f, 1f, 1f);
        
        _fireSpellButton.GetComponent<Image>().color = player.CurrentSpell == player.FireSpell ? activeColor : inactiveColor;
        _iceSpellButton.GetComponent<Image>().color = player.CurrentSpell == player.IceSpell ? activeColor : inactiveColor;
        _lightningSpellButton.GetComponent<Image>().color = player.CurrentSpell == player.LightningSpell ? activeColor : inactiveColor;
    }

    public void SelectSpell(Spell spell) {
        PlayerController.Instance.SetCurrentSpell(spell);
        UpdateSpellsPanel();
    }
    
    public void SelectFireSpell() => SelectSpell(PlayerController.Instance.FireSpell);
    public void SelectIceSpell() => SelectSpell(PlayerController.Instance.IceSpell);
    public void SelectLightningSpell() => SelectSpell(PlayerController.Instance.LightningSpell);

    public static void Popup(string text, Vector3 position, Color? color = null, float size = 1f)
    {
        if (color == null) color = Color.white;
        
        GameObject canvas = Instantiate(popupPrefab, position + Vector3.up, Quaternion.identity);
        RectTransform rectTransform = canvas.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one * 0.002f * size;
        
        TextMeshProUGUI textMesh = canvas.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.color = color.Value;

        float startingTime = 0.75f;
        float time = startingTime;
        
        FunctionUpdater.Create(() => {
            rectTransform.position += new Vector3(0, Time.unscaledDeltaTime / startingTime);
            textMesh.color = new(textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a - Time.unscaledDeltaTime / startingTime);
            
            time -= Time.unscaledDeltaTime;
            if (time <= 0f)
            {
                Destroy(canvas);
                return true;
            }

            return false;
        }, "WorldTextPopup");
    }

    [SerializeField] private GameObject pauseMenu;
    private bool _isPaused;

    public void TogglePause() {
        if (_gameOver) return;
        
        _isPaused = !_isPaused;
        pauseMenu.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0 : 1;
    }

    public void Restart() {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void Quit() {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void Win() {
        _gameOver = true;
        winPanel.SetActive(true);
    }

    public void Lose() {
        _gameOver = true;
        losePanel.SetActive(true);
    }
    
}