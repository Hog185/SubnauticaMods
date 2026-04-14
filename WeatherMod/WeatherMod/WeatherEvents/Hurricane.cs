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

        var smokeClouds = CloudUtils.GetStormCloudEffect();
        
        smokeClouds.transform.parent = effectPrefab.transform;
        smokeClouds.transform.localPosition = Vector3.up * 350;
        
        smokeClouds.gameObject.SetActive(true);
        
        effectPrefab.AddComponent<AlignWithCamera>();
        effectPrefab.AddComponent<LightningSpawner>();
        effectPrefab.AddComponent<LightningSpawner>();
        effectPrefab.AddComponent<LightningSpawner>();
        effectPrefab.AddComponent<LightningSpawner>();
        effectPrefab.AddComponent<DisableWhenCameraUnderwater>();
        
        effectPrefab.AddComponent<WaterSplashVfxController>().affectedTransform =
            effectPrefab.transform.Find("WaterSplashes");
    }

    protected override void OnEventEnd(GameObject effectPrefab)
    {
        foreach (var lightning in effectPrefab.GetComponents<LightningSpawner>())
        {
            Object.Destroy(lightning);
        }
    }
}
