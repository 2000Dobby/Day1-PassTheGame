using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerArm : MonoBehaviour {
    private static readonly int PunchTrigger = Animator.StringToHash("Punch");
    
    [SerializeField] private int damage = 1;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private int maxEnemiesHit = 2;
    [SerializeField] private Vector2 hitDimensions;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private Camera followCamera;

    private Animator _animator;
    private float _currentCooldown;


    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        LookAtMouse();
        if (_currentCooldown > 0) _currentCooldown -= Time.deltaTime;
    }

    private void LookAtMouse() {
        Vector3 mouse = followCamera.ScreenToWorldPoint(Mouse.current.position.value);
        Vector2 toMouse = (mouse - transform.position).normalized;

        float angle = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg + 90f;
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }

    public void Punch(InputAction.CallbackContext ctx) {
        if (_currentCooldown > 0) return;

        _currentCooldown = cooldown;
        _animator.SetTrigger(PunchTrigger);
        HurtEnemies();
    }

    private void HurtEnemies() {
        Collider2D[] cols  = Physics2D.OverlapBoxAll(hitPoint.position, hitDimensions, 0, hitMask);
        if (cols == null) return;

        List<Collider2D> colliders = new List<Collider2D>(cols);
        
        for (int i = 0; i < maxEnemiesHit; i++) {
            float minDist = 100f;
            Collider2D minCol = null;
            
            foreach (Collider2D col in colliders) {
                float dist = (col.transform.position - transform.parent.position).sqrMagnitude;
                if (!IsGrabbingMe(col) && !(dist < minDist)) continue;
                
                minDist = dist;
                minCol = col;
            }
            
            if (minCol == null) break;

            colliders.Remove(minCol);
            minCol.GetComponent<IDamageable>().Damage(damage, PlayerController.Instance.gameObject);
        }
    }

    private bool IsGrabbingMe(Collider2D col) {
        Enemy enemy = col.GetComponent<Enemy>();
        return enemy != null && enemy.GrabbingPlayer;
    }

    private void OnDrawGizmosSelected() {
        if (hitPoint == null) return;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(hitPoint.position, hitDimensions);
    }
}
