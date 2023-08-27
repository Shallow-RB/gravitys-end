using System.Collections;
using System.Collections.Generic;
using Controllers.Player;
using UnityEngine;


public class PlayerWeaponSlash : MonoBehaviour
{
    public ParticleSystem[] attacks;

    public void PlaySlash(int attackIndex)
    {
        if (attackIndex >= 0 && attackIndex < attacks.Length && !attacks[attackIndex].isPlaying)
        {
            attacks[attackIndex].Play();
        }
    }

    public void SetSlashStyle(Color slash, Texture slashTexture, Color smoke, Color spark, Color hit, Color sparksCore, Color fire)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            var parentParticle = attacks[i].transform;

            EffectsHelper.ChangeShaderColorParticle(parentParticle, "_AddColor", slash);
            EffectsHelper.ChangeShaderTextureParticle(parentParticle, "_EmissionTex", slashTexture);

            if (i > 2)
            {
                EffectsHelper.ChangeShaderColorParticle(parentParticle, "Slash2", "_AddColor", slash);
                EffectsHelper.ChangeShaderTextureParticle(parentParticle, "Slash2", "_EmissionTex", slashTexture);

                EffectsHelper.ChangeShaderColorParticle(parentParticle, "Slash3", "_AddColor", slash);
                EffectsHelper.ChangeShaderTextureParticle(parentParticle, "Slash3", "_EmissionTex", slashTexture);

                EffectsHelper.ChangeParticleColor(parentParticle, "Fire", fire);
            }
            else
            {
                EffectsHelper.ChangeParticleColor(parentParticle, "Smoke", smoke);
            }

            EffectsHelper.ChangeParticleColor(parentParticle, "Sparks", spark);
            EffectsHelper.ChangeParticleColor(parentParticle, "Hit", hit);

            var attackHit = parentParticle.Find("Hit");

            EffectsHelper.ChangeParticleColor(attackHit, "Flare", sparksCore);
            EffectsHelper.ChangeParticleColor(attackHit, "SparksCore", sparksCore);
        }
    }
}
