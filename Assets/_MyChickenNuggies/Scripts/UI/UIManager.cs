using UnityEngine;

// Sorry, didn't have enough time for clean code
public class UIManager : MonoBehaviour {
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject looseScreen;


    public void Begin() {
        titleScreen.SetActive(false);
        winScreen.SetActive(false);
        looseScreen.SetActive(false);
    }

    public void ShowTitleScreen() {
        titleScreen.SetActive(true);
        winScreen.SetActive(false);
        looseScreen.SetActive(false);
    }

    public void ShowWinScreen() {
        winScreen.SetActive(true);
        titleScreen.SetActive(false);
        looseScreen.SetActive(false);
    }

    public void ShowLooseScreen() {
        looseScreen.SetActive(true);
        titleScreen.SetActive(false);
        winScreen.SetActive(false);
    }
}
