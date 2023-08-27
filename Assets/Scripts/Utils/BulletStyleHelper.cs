using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BulletStyleHelper
{
    public static void SetBulletStyle(Transform transform, TrailRenderer trail, Color albedo, Color glow, float glowPower, Gradient trailGradient)
    {
        EffectsHelper.ChangeShaderColorMaterial(transform, "_Albedo", albedo);
        EffectsHelper.ChangeShaderColorMaterial(transform, "_Glow", glow);

        EffectsHelper.ChangeShaderFloatMaterial(transform, "_GlowPower", glowPower);

        trail.colorGradient = trailGradient;
    }

    public static void SetBulletDestructionStyle(ParticleSystem destructionEffect, Color standard, Color emission, Color nonEmissive)
    {
        EffectsHelper.ChangeShaderColorParticle(destructionEffect.transform, "_BaseColor", standard);
        EffectsHelper.ChangeShaderColorParticle(destructionEffect.transform, "_EmissionColor", emission);

        EffectsHelper.ChangeShaderColorParticle(destructionEffect.transform, "HitFeedbackExtra", "_BaseColor", nonEmissive);

        EffectsHelper.ChangeShaderColorParticle(destructionEffect.transform, "HitFeedbackParticleTrail", "_BaseColor", standard);
        EffectsHelper.ChangeShaderColorParticle(destructionEffect.transform, "HitFeedbackParticleTrail", "_EmissionColor", emission);

        EffectsHelper.ChangeShaderColorTrailParticle(destructionEffect.transform, "HitFeedbackParticleTrail", "_BaseColor", standard);
        EffectsHelper.ChangeShaderColorTrailParticle(destructionEffect.transform, "HitFeedbackParticleTrail", "_EmissionColor", emission);
    }
}