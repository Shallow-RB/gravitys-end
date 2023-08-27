using System.Collections;
using Controllers.Player;
using UnityEngine;

public class RangeWeapon : MonoBehaviour
{
    [Header("Shooting")]
    public float fireRate;
    public int countOfProjectilesShot;
    public float delayBetweenShots;

    [Header("Reloading")]
    public int currentAmmo;
    // public int reserveAmmo;
    // public int magSize;
    // public float reloadTime;

    [HideInInspector]
    public bool reloading;

    [Header("Projectile")]
    public GameObject projectile;
    public int minDamage;
    public int maxDamage;
    public float bulletSpeed;

    [Header("Projectile styling")]
    [ColorUsageAttribute(true, true)]
    public Color albedo;
    [ColorUsageAttribute(true, true)]
    public Color glow;
    public float glowPower;
    public Gradient trailGradient;

    [Header("Destruction effect styling")]
    public Color standardColor;
    [ColorUsageAttribute(true, true)]
    public Color emission;
    public Color nonEmissive;

    [SerializeField]
    protected Transform bulletOutput;

    protected Vector3 bulletDirection;
    protected GameObject newBullet;
    protected BulletBehavior newBulletBehavior;

    protected float timeSinceLastShot;
    protected Character player;

    protected virtual void Start()
    {
        player = PlayerManager.Instance.player.GetComponent<Character>();
    }

    // private void OnDisable() => reloading = false;

    // public void StartReload()
    // {
    //     if (!reloading && this.gameObject.activeSelf)
    //         StartCoroutine(Reload());
    // }

    // private IEnumerator Reload()
    // {
    //     reloading = true;

    //     yield return new WaitForSeconds(reloadTime);

    //     currentAmmo = reserveAmmo - magSize > 0 ? magSize : reserveAmmo;
    //     reserveAmmo -= currentAmmo;

    //     reloading = false;
    // }

    public bool CanShoot() => timeSinceLastShot > fireRate && currentAmmo > 0;

    public virtual void Shoot()
    {
        if (!CanShoot())
            return;

        currentAmmo -= countOfProjectilesShot;
        timeSinceLastShot = 0;

        StartCoroutine(ExecuteBullet());

        // if (currentAmmo <= 0 && reserveAmmo > 0)
        // {
        //     currentAmmo = 0;
        //     StartReload();
        // }
    }

    protected IEnumerator ExecuteBullet()
    {
        for (int i = 0; i < countOfProjectilesShot; i++)
        {
            OnGunShot();
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }

    protected virtual void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    protected virtual void OnGunShot()
    {
        newBulletBehavior = newBullet.GetComponentInChildren<BulletBehavior>();

        newBulletBehavior.SetDamage(minDamage, maxDamage);
        newBulletBehavior.SetSpeed(bulletSpeed);
        newBulletBehavior.SetDirection(bulletDirection);
        newBulletBehavior.SetBulletStyle(albedo, glow, glowPower, trailGradient);
        newBulletBehavior.SetBulletDestructionStyle(standardColor, emission, nonEmissive);
    }

    // In most cases you want the projectile go straight
    protected void RegularShotBehavior()
    {
        Vector3 bulletOutputWorldPos = bulletOutput.transform.position;
        bulletDirection = (player.lookAtPosition - bulletOutputWorldPos);

        bulletDirection.y = 0f;

        newBullet = Instantiate(projectile, bulletOutputWorldPos, Quaternion.identity);

        newBullet.transform.LookAt(player.lookAtPosition);
        newBullet.transform.rotation = new Quaternion(0, newBullet.transform.rotation.y, 0, newBullet.transform.rotation.w);
    }
}
