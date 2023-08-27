public class RegularRangeWeapon : RangeWeapon
{
    protected override void OnGunShot()
    {
        RegularShotBehavior();
        base.OnGunShot();
    }
}
