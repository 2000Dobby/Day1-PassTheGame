using UnityEngine;

public class Target : MonoBehaviour, IInteractable {
    public static Target Instance;


    private void Awake() {
        if (Instance == null) Instance = this;
    }


    public void Interact(GameObject interactor) {
        GameDirector.Instance.Win();
    }
}
