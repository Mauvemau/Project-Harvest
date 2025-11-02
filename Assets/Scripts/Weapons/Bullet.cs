using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour, IBullet {
    [Header("Preset")]
    [SerializeField] private BulletPresetSO preset;
    
    [Header("Current Behaviour")]
    [SerializeReference, SubclassSelector] private IBulletStrategy currentBehaviour = new LinearShotStrategy();

    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider;
    private Rigidbody2D _rb;
    private Vector2 _aimDirection;
    private float _damage = 0f;
    private AK.Wwise.Event _hitSoundEffect;

    private BulletStats _currentBulletStats;

    private bool _shot = false;
    private float _timeOfDeath = 0f;
    private int _targetsPenetratedCount = 0;

    private readonly HashSet<Collider2D> _currentOverlaps = new HashSet<Collider2D>();

    public void Shoot(BulletPresetSO presetToSet, Vector2 direction, LayerMask targetLayer, float damage, BulletStats stats, Transform weaponTransform = null, bool mirrored = false) {
        if (presetToSet) {
            preset = presetToSet;
            SetUpPreset();
        }

        SetUpCollision(targetLayer);
        _aimDirection = direction;
        _damage = damage;

        _currentBulletStats = stats;

        _hitSoundEffect = preset.HitAudioEvent;

        _timeOfDeath = Time.time + _currentBulletStats.lifeTime;
        currentBehaviour.Init(transform, _rb, _aimDirection, _currentBulletStats.speed, weaponTransform);
        _spriteRenderer.flipX = mirrored;
        _spriteRenderer.enabled = true;
        _shot = true;
    }

    private void SetUpPreset() {
        IBulletStrategy strategy = preset.Behaviour;
        Sprite sprite = preset.Sprite;

        if (strategy != null) {
            currentBehaviour = (IBulletStrategy)strategy.Clone();
        }
        if (sprite && _spriteRenderer) {
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color = preset.Tint;
        }
    }

    private void SetUpCollision(LayerMask targetLayer) {
        if (!_collider) return;

        int inverted = ~0 & ~targetLayer;
        _collider.includeLayers = 0;
        _collider.excludeLayers = inverted;
    }

    private void HandleCollision(Collider2D other) {
        if (other.TryGetComponent(out IDamageable damageable)) {
            damageable.TakeDamage(_damage);
        }

        _hitSoundEffect?.Post(gameObject);
        
        _targetsPenetratedCount++;
        if (_targetsPenetratedCount > _currentBulletStats.penetrationCount) {
            gameObject.SetActive(false); // If layered correctly it will disappear upon hitting walls
        }

        if (other.TryGetComponent<IPushable>(out var pushable)) {
            if (_currentBulletStats.pushForce < 0.1f) return;

            Vector2 toTarget = (other.transform.position - transform.position);
            Vector2 pushDir = toTarget.normalized;

            pushable.RequestPush(pushDir, _currentBulletStats.pushForce);
        }
    }

    private void FixedUpdate() {
        if (!_shot && currentBehaviour == null) return;
        currentBehaviour.HandleMovement();
    }

    private void Update() {
        if (!_shot) return;
        if(Time.time >= _timeOfDeath) {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!_shot) return;
        if (_currentOverlaps.Add(collision)) { // Will return false if target is already inside
            HandleCollision(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!_shot) return;
        _currentOverlaps.Remove(collision);
    }

    private void Awake() {
        if (!TryGetComponent(out _collider)) {
            Debug.LogError($"{name}: missing required component {nameof(CircleCollider2D)}");
        }
        if (!TryGetComponent(out _rb)) {
            Debug.LogError($"{name}: missing required component {nameof(CircleCollider2D)}");
        }
        if (!TryGetComponent(out _spriteRenderer)) {
            Debug.LogWarning($"{name}: missing optional component {nameof(CircleCollider2D)}");
        }
    }

    private void OnEnable() {
        _shot = false;
        _targetsPenetratedCount = 0;
        _currentOverlaps.Clear();
        _hitSoundEffect = null;
        if (_collider) {
            _collider.isTrigger = true;
        }
        if (_rb) {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
        if (_spriteRenderer) {
            _spriteRenderer.enabled = false;
        }
    }

    private void OnDisable() {
        _shot = false;
        _targetsPenetratedCount = 0;
        _currentOverlaps.Clear();
    }

    private void OnValidate() {
        if (!TryGetComponent(out _spriteRenderer)) {
            Debug.LogWarning($"{name}: missing optional component {nameof(CircleCollider2D)}");
        }
        SetUpPreset();
    }
}
