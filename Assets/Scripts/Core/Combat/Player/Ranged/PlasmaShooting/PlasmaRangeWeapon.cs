public class PlasmaRangeWeapon : RangeWeapon
{
    protected override void OnGunShot()
    {
        RegularShotBehavior();
        base.OnGunShot();
    }
}
