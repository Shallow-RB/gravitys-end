public class BurstRangeWeapon : RangeWeapon
{
    protected override void OnGunShot()
    {
        RegularShotBehavior();
        base.OnGunShot();
    }
}
