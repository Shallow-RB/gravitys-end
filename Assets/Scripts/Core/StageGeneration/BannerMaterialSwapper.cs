using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerMaterialSwapper : MonoBehaviour
{
    [SerializeField]
    private List<Material> redGreenPurpleMat;

    [SerializeField]
    private List<GameObject> banners;

    public void UpdateMaterials(int matIndex)
    {
        foreach(GameObject banner in banners)
        {
            banner.GetComponent<Renderer>().material = redGreenPurpleMat[matIndex];
        }
    }
}
