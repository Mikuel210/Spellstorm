using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour {

    [SerializeField] private GameObject howToPlay;
    [SerializeField] private GameObject credits;
    
    private bool _tutorialOpen;
    private bool _creditsOpen;
    
    public void Play() => SceneManager.LoadScene(1);

    public void ToggleTutorial() {
        _tutorialOpen = !_tutorialOpen;
        howToPlay.SetActive(_tutorialOpen);
    }

    public void ToggleCredits() {
        _creditsOpen = !_creditsOpen;
        credits.SetActive(_creditsOpen);
    }

}
