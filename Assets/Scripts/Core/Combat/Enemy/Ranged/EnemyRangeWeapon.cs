using System.Collections;
using Core.Audio;
using UnityEngine;

public class EnemyRangeWeapon : MonoBehaviour
{
    [Header("Shooting")]
    public float fireRate;

    [Header("Reloading")]
    [HideInInspector]
    public bool reloading;
    public float reloadTime;
    public int currentAmmo;
    public int magSize;

    [Header("Bullet")]
    public GameObject bullet;
    public int minDamage;
    public int maxDamage;
    public float bulletSpeed;

    [Header("Bullet styling")]
    [ColorUsageAttribute(true, true)]
    public Color albedo;
    [ColorUsageAttribute(true, true)]
    public Color glow;
    public float glowPower;
    public Gradient trailGradient;

    [Header("Effect styling")]
    public Color standardColor;
    [ColorUsageAttribute(true, true)]
    public Color emission;
    public Color nonEmissive;

    [SerializeField]
    private LayerMask ignoreLayer;

    [SerializeField]
    private Transform bulletOutput;

    [HideInInspector]
    public bool allowRaycast = false;
    [HideInInspector]
    public bool allowShot = false;

    private float timeSinceLastShot;
    private RaycastHit hit;

    private Vector3 bulletOutputWorldPos;
    private Vector3 bulletDirection;

    private PlayerManager playerManager;
    private Transform enemy;

    private void Start()
    {
        playerManager = PlayerManager.Instance;
    }

    private void OnDisable() => reloading = false;

    private void StartReload()
    {
        if (!reloading && this.gameObject.activeSelf)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        reloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magSize;

        reloading = false;
    }

    private bool CanShoot() => !reloading && timeSinceLastShot > 1f / (fireRate / 60f);

    private void Shoot()
    {
        if (currentAmmo > 0)
        {
            if (CanShoot())
            {
                currentAmmo--;
                timeSinceLastShot = 0;
                OnGunShot();
            }
        }
        else
        {
            if (!reloading)
            {
                StartReload();
            }
        }
    }

    private void Update()
    {
        if (allowRaycast)
        {
            timeSinceLastShot += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (allowRaycast)
        {
            bulletOutputWorldPos = bulletOutput.transform.position;
            bulletDirection = enemy.transform.forward;

            if (Physics.Raycast(bulletOutputWorldPos, bulletDirection, out hit, Mathf.Infinity, ~ignoreLayer, QueryTriggerInteraction.Ignore))
            {
                // Debug.DrawRay(bulletOutputWorldPos, bulletDirection * 1000f, Color.green);
                if (allowShot)
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        Shoot();
                    }
                }
            }
        }
    }

    private void OnGunShot()
    {
        bulletDirection.y = 0f;

        GameObject newBullet = Instantiate(bullet, bulletOutputWorldPos, Quaternion.identity);

        newBullet.transform.LookAt(hit.transform.position);
        newBullet.transform.rotation = new Quaternion(0, newBullet.transform.rotation.y, 0, newBullet.transform.rotation.w);

        EnemyBulletBehaviour enemyBulletBehaviour = newBullet.GetComponentInChildren<EnemyBulletBehaviour>();

        enemyBulletBehaviour.SetDamage(minDamage, maxDamage);
        enemyBulletBehaviour.SetSpeed(bulletSpeed);
        enemyBulletBehaviour.SetDirection(bulletDirection);
        enemyBulletBehaviour.SetBulletStyle(albedo, glow, glowPower, trailGradient);
        enemyBulletBehaviour.SetBulletDestructionStyle(standardColor, emission, nonEmissive);

        SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.ENEMY_SHOOTS);
    }

    public void SetEnemy(Transform _enemy)
    {
        enemy = _enemy;
    }
}
