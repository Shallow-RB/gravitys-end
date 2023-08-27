using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SceneLoadFadeIn : MonoBehaviour
{
    void Start()
    {
        Navigation.instance.FadeIn();
    }
}
