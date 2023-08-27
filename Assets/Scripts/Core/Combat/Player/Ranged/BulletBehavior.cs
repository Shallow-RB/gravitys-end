using System.Collections;
using Core.Enemy;
using UI.Tokens;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField]
    protected TrailRenderer trail;
    [SerializeField]
    protected ParticleSystem destructionEffect;

    protected int _minDamage;
    protected int _maxDamage;
    protected float _speed;
    protected Vector3 _direction;

    protected float destructiomTime;
    protected bool allowMovement = true;

    protected virtual void Update()
    {
        if (allowMovement)
        {
            transform.root.Translate(_direction * _speed * Time.deltaTime, Space.World);
            destructionEffect.gameObject.transform.position = transform.root.position;
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<EnemyBase>())
            {
                float damageMod = TokenManager.instance.damageSection.GetModifier();
                other.gameObject.GetComponent<EnemyBase>().TakeDamage((int)Mathf.Round(_minDamage * damageMod), (int)Mathf.Round(_maxDamage * damageMod), 0);
            }

            Destroy(transform.root.gameObject);
        }

        if (other.gameObject.CompareTag("Boss"))
        {
            if (other.gameObject.GetComponent<Boss>())
            {
                float damageMod = TokenManager.instance.damageSection.GetModifier();
                other.gameObject.GetComponent<Boss>().TakeDamage((int)Mathf.Round(_minDamage * damageMod), (int)Mathf.Round(_maxDamage * damageMod), 0);
            }

            Destroy(transform.root.gameObject);
        }

        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Door") || other.gameObject.CompareTag("BulletInteract"))
        {
            StartCoroutine(DestroyBullet());
        }
    }

    protected IEnumerator DestroyBullet()
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