using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Enemy;
using UI.Tokens;
using UnityEngine;

public class PlasmaBulletBehavior : BulletBehavior
{
    [SerializeField]
    private float sphereSize = 1.5f;

    private List<Collider> hitColliders = new();

    protected override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss") || other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Door") || other.gameObject.CompareTag("BulletInteract"))
        {
            StartCoroutine(PlasmaExplosion());
        }
    }

    private IEnumerator PlasmaExplosion()
    {
        allowMovement = false;

        StartCoroutine(PlasmaSphere());

        yield return new WaitForSeconds(destructionEffect.main.duration);

        Destroy(transform.root.gameObject);
    }

    private IEnumerator PlasmaSphere()
    {
        destructionEffect.Play();

        var colliders = Physics.OverlapSphere(transform.position, sphereSize);
        colliders.ToList();

        foreach (var collider in colliders)
        {
            if (!hitColliders.Contains(collider))
            {
                hitColliders.Add(collider);

                if (collider.CompareTag("Enemy"))
                {
                    float damageMod = TokenManager.instance.damageSection.GetModifier();
                    collider.gameObject.GetComponent<EnemyBase>().TakeDamage((int)Mathf.Round(_minDamage * damageMod), (int)Mathf.Round(_maxDamage * damageMod), 0);
                }

                if (collider.CompareTag("Boss"))
                {
                    float damageMod = TokenManager.instance.damageSection.GetModifier();
                    collider.gameObject.GetComponent<Boss>().TakeDamage((int)Mathf.Round(_minDamage * damageMod), (int)Mathf.Round(_maxDamage * damageMod), 0);
                }
            }
        }

        yield return null;
    }
}