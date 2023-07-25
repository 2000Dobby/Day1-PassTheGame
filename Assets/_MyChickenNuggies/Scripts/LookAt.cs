using UnityEngine;

public class LookAt : MonoBehaviour {
    [SerializeField] private float hideDist = 3f;
    [SerializeField] private float offset;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject visuals;


    private void Update() {
        Vector2 toTarget = target.transform.position - transform.position;
        float dist = toTarget.sqrMagnitude;
        
        if (dist <= hideDist * hideDist) {
            if (visuals.activeSelf) visuals.SetActive(false);
            return;
        }
        
        float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg + offset;
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }
}
