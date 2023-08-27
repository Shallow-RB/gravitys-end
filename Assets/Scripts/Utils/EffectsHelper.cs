using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectsHelper
{
    // Change the shader texture of the particle.
    public static void ChangeShaderTextureParticle(Transform particle, string nameField, Texture texture)
    {
        var renderer = particle.GetComponent<ParticleSystemRenderer>();
        var originalMat = renderer.material;
        var newMat = new Material(originalMat);

        newMat.SetTexture(nameField, texture);

        renderer.material = newMat;
    }


    // Change the shader texture of the particle of a child.
    public static void ChangeShaderTextureParticle(Transform particle, string name, string nameField, Texture texture)
    {
        var renderer = particle.transform.Find(name).GetComponent<ParticleSystemRenderer>();
        var originalMat = renderer.material;
        var newMat = new Material(originalMat);

        newMat.SetTexture(nameField, texture);

        renderer.material = newMat;
    }


    // Change the shader color of the particle.
    public static void ChangeShaderColorParticle(Transform particle, string nameField, Color color)
    {
        var renderer = particle.GetComponent<ParticleSystemRenderer>();
        var originalMat = renderer.material;
        var newMat = new Material(originalMat);

        newMat.SetColor(nameField, color);

        renderer.material = newMat;
    }

    // Change the shader color of the particle of a child.
    public static void ChangeShaderColorParticle(Transform particle, string name, string nameField, Color color)
    {
        var renderer = particle.transform.Find(name).GetComponent<ParticleSystemRenderer>();
        var originalMat = renderer.material;
        var newMat = new Material(originalMat);

        newMat.SetColor(nameField, color);

        renderer.material = newMat;
    }

    // Change the shader color of the particle trail of a child.
    public static void ChangeShaderColorTrailParticle(Transform particle, string name, string nameField, Color color)
    {
        var renderer = particle.transform.Find(name).GetComponent<ParticleSystemRenderer>();
        var originalMat = renderer.trailMaterial;
        var newMat = new Material(originalMat);

        newMat.SetColor(nameField, color);

        renderer.trailMaterial = newMat;
    }

    // Change the shader color of the material.
    public static void ChangeShaderColorMaterial(Transform material, string nameField, Color color)
    {
        var renderer = material.GetComponent<Renderer>();
        var originalMat = renderer.sharedMaterial;
        var newMat = new Material(originalMat);

        newMat.SetColor(nameField, color);

        renderer.material = newMat;
    }

    // Change the shader color of the material.
    public static void ChangeShaderColorMaterial(Transform material, string nameField, string name, Color color)
    {
        var renderer = material.transform.Find(name).GetComponent<Renderer>();
        var originalMat = renderer.sharedMaterial;
        var newMat = new Material(originalMat);

        newMat.SetColor(nameField, color);

        renderer.material = newMat;
    }

    // Change the shader float of the material.
    public static void ChangeShaderFloatMaterial(Transform material, string nameField, float newValue)
    {
        var renderer = material.GetComponent<Renderer>();
        var originalMat = renderer.sharedMaterial;
        var newMat = new Material(originalMat);

        newMat.SetFloat(nameField, newValue);

        renderer.material = newMat;
    }

    // Change the particle color.
    public static void ChangeParticleColor(Transform particle, Color color)
    {
        var mainModuleSparks = particle.GetComponent<ParticleSystem>().main;
        mainModuleSparks.startColor = color;
    }

    // Change the particle color of a child.
    public static void ChangeParticleColor(Transform particle, string name, Color color)
    {
        var mainModuleSparks = particle.Find(name).GetComponent<ParticleSystem>().main;
        mainModuleSparks.startColor = color;
    }
}
