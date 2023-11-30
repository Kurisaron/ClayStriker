using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class VFXManager : Singleton<VFXManager>
{
    [SerializeField] private VisualEffectAsset targetBreakVFX;
    private Transform vfxParent { get => transform.Find("VFX"); }
    private List<VisualEffect> visualEffects;
    
    public void TargetBreak(Vector3 position, Vector3 hitDirection)
    {
        PlayVFX(targetBreakVFX, position, null, effect => effect.SetVector3("hitDirection", hitDirection));
    }

    private async void PlayVFX(VisualEffectAsset effectAsset, Vector3? position = null, Quaternion? rotation = null, Action<VisualEffect> setParameters = null)
    {
        if (visualEffects == null) visualEffects = new List<VisualEffect>();
        VisualEffect effectToUse;
        if (visualEffects.Count <= 0 || visualEffects.TrueForAll(effect => effect.gameObject.activeInHierarchy || effect.HasAnySystemAwake()))
        {
            GameObject effectObject = new GameObject("VFXPlayer");
            effectObject.transform.parent = vfxParent;
            visualEffects.Add(effectToUse = effectObject.AddComponent<VisualEffect>());
        }
        else
        {
            effectToUse = visualEffects.Find(effect => !effect.gameObject.activeInHierarchy || effect.aliveParticleCount <= 0);
            if (!effectToUse.gameObject.activeInHierarchy) effectToUse.gameObject.SetActive(true);
        }
        effectToUse.transform.SetPositionAndRotation(position.HasValue ? position.Value : vfxParent.position, rotation.HasValue ? rotation.Value : vfxParent.rotation);
        effectToUse.visualEffectAsset = effectAsset;
        if (setParameters != null) setParameters(effectToUse);
        effectToUse.Play();

        await WaitForEffectDuration(effectToUse);

        if (effectToUse == null) return;
        effectToUse.Stop();
        effectToUse.gameObject.SetActive(false);
    }

    private async Task WaitForEffectDuration(VisualEffect effect)
    {
        while (effect != null && effect.aliveParticleCount <= 0)
        {
            await Task.Yield();
            if (effect == null) return;
        }

        while (effect != null && effect.HasAnySystemAwake())
        {
            await Task.Yield();
            if (effect == null) return;
        }
    }
}

