using System.Collections;
using Controllers.Player;
using UI.Inventory;
using UnityEngine;

public class EnemyBulletBehaviour : MonoBehaviour
{
    [SerializeField]
    private TrailRenderer trail;
    [SerializeField]
    private ParticleSystem destructionEffect;

    private int _minDamage;
    private int _maxDamage;
    private float _speed;
    private Vector3 _direction;

    private float destructiomTime;
    private bool allowMovement = true;

    private GameObject player;

    private void Awake()
    {
        player = PlayerManager.Instance.player;
    }

    private void Update()
    {
        if (allowMovement)
        {
        transform.root.Translate(_direction * _speed * Time.deltaTime, Space.World);
        destructionEffect.gameObject.transform.position = transform.root.position;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var armor = player.GetComponent<EquipmentSystem>()._equippedArmor;
            player.GetComponent<PlayerStatsController>().TakeDamage(_minDamage, _maxDamage, armor != null ? armor.GetComponent<Item>().GetArmorModifier() : 0);

            Destroy(transform.root.gameObject);
        }

        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Door") || other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("BulletInteract"))
        {
            StartCoroutine(DestroyBullet());
        }
    }

    private IEnumerator DestroyBullet()
    {
        allowMovement = false;

        destructionEffect.Play();

        gameObject.GetComponent<MeshRenderer>().enabled = false;

        yield return new WaitForSeconds(destructionEffect.main.duration);

        Destroy(transform.root.gameObject);
    }

    public void SetBulletStyle(Color albedo, Color glow, float glowPower, Gradient trailGradient)
    {
        BulletStyleHelper.SetBulletStyle(transform, trail, albedo, glow, glowPower, trailGradient);
    }

    public void SetBulletDestructionStyle(Color standard, Color emission, Color nonEmissive)
    {
        BulletStyleHelper.SetBulletDestructionStyle(destructionEffect, standard, emission, nonEmissive);
    }

    public void SetDamage(int minDamage, int maxDamage)
    {
        _minDamage = minDamage;
        _maxDamage = maxDamage;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetDirection(Vector3 direction)
    {
        _direction = direction.normalized;
    }
}
