using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField]
    private int minDamage = 5;
    [SerializeField]
    private int maxDamage = 10;

    [Header("Slash Color")]
    public Color slashColor;
    public Texture slashTexture;
    public Color smokeColor;
    public Color sparkColor;
    public Color sparkCoreColor;
    public Color fireColor;
    public Color hitColor;

    public int GetMinDamage()
    {
        return minDamage;
    }

    public int GetMaxDamage()
    {
        return maxDamage;
    }
}
