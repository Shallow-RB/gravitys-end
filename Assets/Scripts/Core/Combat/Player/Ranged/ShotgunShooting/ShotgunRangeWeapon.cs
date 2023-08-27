using UnityEngine;

public class ShotgunRangeWeapon : RangeWeapon
{
    public float angleBetweenBullets;
    public override void Shoot()
    {
        if (!CanShoot())
            return;

        currentAmmo -= 1;
        timeSinceLastShot = 0;

        OnGunShot();

        // if (currentAmmo <= 0 && reserveAmmo > 0)
        // {
        //     currentAmmo = 0;
        //     StartReload();
        // }
    }

    protected override void OnGunShot()
    {
        Vector3 bulletOutputWorldPos = bulletOutput.transform.position;
        player.transform.LookAt(player.lookAtPosition);

        float totalSpreadAngle = (countOfProjectilesShot - 1) * angleBetweenBullets;
        float startOffset = -totalSpreadAngle / 2f;

        for (int i = 0; i < countOfProjectilesShot; i++)
        {
            float angleOffset = startOffset + (i * angleBetweenBullets);

            newBullet = Instantiate(projectile, bulletOutputWorldPos, player.transform.rotation);
            
            newBullet.transform.Rotate(0f, angleOffset, 0f);

            bulletDirection = newBullet.transform.forward;
            base.OnGunShot();
        }
    }
}
