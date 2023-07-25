using System;
using UnityEngine;

public class Timer : MonoBehaviour, IProgress {
    [SerializeField] private RectTransform handTransform;
    
    private float _initialRotation;
    private float _time;
    private float _timeRemaining;
    private bool _set;
    private Action<Timer> _callback;


    private void Awake() {
        _initialRotation = handTransform.eulerAngles.z;
    }

    private void Update() {
        if (!_set) return;
        
        if (_timeRemaining <= 0) {
            Cancel();
            _callback.Invoke(this);
            return;
        }
        
        _timeRemaining -= Time.deltaTime;
        SetProgress(_timeRemaining / _time);
    }

    public void Cancel() {
        _set = false;
    }

    public void Set(float time, Action<Timer> callback) {
        _set = true;
        _time = time;
        _timeRemaining = time;
        _callback = callback;
        SetProgress(1);
    }
    
    public void SetProgress(float per) {
        float rot = _initialRotation + 360 * per;
        handTransform.eulerAngles = new Vector3(0, 0, rot);
    }
}
