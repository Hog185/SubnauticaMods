using UnityEngine;
using WeatherMod.Mono;

namespace WeatherMod.WeatherEvents;

public class Hurricane : WeatherEvent
{
    protected override GameObject EffectPrefab { get; } = Plugin.AssetBundle.LoadAsset<GameObject>("Weather_Thunderstorm");
    protected override float DestroyDelay { get; } = 22;
    protected override FogSettings Fog { get; } = new FogSettings(0.0035f, new Color(0.03f, 0.03f, 0.04f), 0.15f, 0.25f, 0.2f);
    public override float MinDuration { get; } = 180;
    public override float MaxDuration { get; } = 300;
    public override WeatherEventAudio AmbientSound { get; } = new WeatherEventAudio(WeatherAudio.ThunderstormLoop, WeatherAudio.ThunderstormLoopInside, null);
    public override int RainDropVfxEmission { get; } = 45;

    protected override void OnEventBegin(GameObject effectPrefab)
    {
        foreach (var renderer in effectPrefab.GetComponentsInChildren<Renderer>())
        {
            WeatherMaterialUtils.ApplyRainMaterial(renderer);
        }

        var stormClouds = CloudUtils.GetStormCloudEffect();
        if (stormClouds != null)
        {
            stormClouds.transform.SetParent(effectPrefab.transform, worldPositionStays: false);
            stormClouds.localPosition = Vector3.up * 350;
            stormClouds.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[Hurricane] Storm cloud effect not found. Skipping cloud setup.");
        }

        effectPrefab.AddComponent<AlignWithCamera>();
        
        for (int i = 0; i < 4; i++)
        {
            effectPrefab.AddComponent<LightningSpawner>();
        }
        
        effectPrefab.AddComponent<DisableWhenCameraUnderwater>();

        var waterSplashes = effectPrefab.transform.Find("WaterSplashes");
        if (waterSplashes != null)
        {
            effectPrefab.AddComponent<WaterSplashVfxController>().affectedTransform = waterSplashes;
        }
        else
        {
            Debug.LogWarning("[Hurricane] 'WaterSplashes' transform not found. Skipping splash VFX controller.");
        }
    }

    protected override void OnEventEnd(GameObject effectPrefab)
    {
        foreach (var lightning in effectPrefab.GetComponents<LightningSpawner>())
        {
            Object.Destroy(lightning);
        }
    }
}
