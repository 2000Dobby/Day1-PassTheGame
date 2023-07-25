using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable {
   private Rigidbody2D _rb;

   [Header("Movement")]
   [SerializeField] private float speed = 0.2f;
   [SerializeField] private float grabRadius = 1f;

   [Header("Combat")]
   [SerializeField] private float grabChance = 0.1f;
   [SerializeField] private float staggerDuration = 0.5f;
   [SerializeField] private float staggerKnockback = 1f;
   [SerializeField] private LayerMask grabMask;
   [SerializeField] private SpriteRenderer gfxRenderer;

   [Header("Health")]
   [SerializeField] private int maxHealth = 10;
   [SerializeField] private GameObject hpBarObject;

   private int _health;
   private IProgress _hpBar;
   private Vector2 _movementVector;
   private float _currentStaggerTime;
   public bool GrabbingPlayer { get; private set; }
   private bool _inGrabRange;


   private void Awake() {
      _rb = GetComponent<Rigidbody2D>();
      _hpBar = hpBarObject.GetComponent<IProgress>();
   }

   private void OnEnable() {
      _health = maxHealth;
      _hpBar.SetProgress(1.0f);
   }

   private void Update() {
      if (_currentStaggerTime > 0) {
         _currentStaggerTime -= Time.deltaTime;
         return;
      }
      
      DetectPlayer();
      if (_inGrabRange) {
         GrabPlayer();
         _movementVector = Vector2.zero;
         return;
      }
      
      _movementVector =  GetVectorToPlayer().normalized;
   }
   
   private void FixedUpdate() {
      if (_currentStaggerTime > 0) return;
      if (GrabbingPlayer) {
         _rb.velocity = Vector2.zero;
         return;
      }
      
      float modifier = Mathf.Max(1 - _rb.velocity.sqrMagnitude / (speed * speed), 0);
      _rb.AddForce(((Vector2) transform.forward + _movementVector) * (speed * modifier), ForceMode2D.Force);
   }


   private void GrabPlayer() {
      if (PlayerController.Instance.IsBeingGrabbed 
          || Random.Range(0, 1) > grabChance 
          || !PlayerController.Instance.Grab()) return;

      GrabbingPlayer = true;
      gfxRenderer.color = Color.red;
   }
   
   public void Damage(int amount, GameObject damager) {
      _currentStaggerTime = staggerDuration;
      if (GrabbingPlayer) {
         PlayerController.Instance.Release();
         GrabbingPlayer = false;
         gfxRenderer.color = Color.white;
      }

      _health -= amount;
      if (_health <= 0) OnDeath();
      _hpBar.SetProgress((float) _health / maxHealth);
      
      Vector2 toPlayer = GetVectorToPlayer().normalized;
      _rb.AddForce(toPlayer * (-1 * staggerKnockback), ForceMode2D.Impulse);
   }

   private void OnDeath() {
      GameDirector.Instance.RemoveEnemy(this);
      Destroy(gameObject);
   }

   private Vector2 GetVectorToPlayer() {
      Vector2 playerPos = PlayerController.Instance.gameObject.transform.position;
      return playerPos - (Vector2) transform.position;
   }

   private void DetectPlayer() {
      Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, grabRadius, grabMask);

      if (colliders.Any(col => col.tag.Equals("Player"))) {
         _inGrabRange = true;
         return;
      }

      _inGrabRange = false;
   }
   

   private void OnDrawGizmosSelected() {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, grabRadius);
   }
}
