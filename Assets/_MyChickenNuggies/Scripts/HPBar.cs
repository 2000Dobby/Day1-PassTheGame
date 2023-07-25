using UnityEngine;

public class HpBar : MonoBehaviour, IProgress {
    [SerializeField] private RectTransform fillTransform;
    private float _initialWidth = -1;


    private void Awake() {
        _initialWidth = fillTransform.sizeDelta.x;
    }

    public void SetProgress(float per) {
        if (_initialWidth < 0) return;
        fillTransform.sizeDelta =  new Vector2(_initialWidth * per, fillTransform.sizeDelta.y);
    }
}
