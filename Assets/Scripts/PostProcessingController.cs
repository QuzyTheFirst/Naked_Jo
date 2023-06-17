using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingController : MonoBehaviour
{
    //SlowMo
    private PostProcessVolume _slowMoPostProcessingVolume;
    private Vignette _slowMoVignette;
    private LensDistortion _slowMoLensDistortion;

    //Kill
    private PostProcessVolume _killPostProcessingVolume;
    private Vignette _killVignette;

    public void SetUpPostProcessingController(PostProcessVolume slowMoPostProcessVolume, PostProcessVolume killPostProcessVolume)
    {
        _slowMoPostProcessingVolume = slowMoPostProcessVolume;
        _slowMoPostProcessingVolume.profile.TryGetSettings(out _slowMoVignette);
        _slowMoPostProcessingVolume.profile.TryGetSettings(out _slowMoLensDistortion);

        _killPostProcessingVolume = killPostProcessVolume;
        _killPostProcessingVolume.profile.TryGetSettings(out _killVignette);
    }

    public void ToggleSlowMoPostProcessing(bool value)
    {
        GameObject go = _slowMoPostProcessingVolume.gameObject;

        if (value)
        {
            LeanTween.cancel(go);
            LeanTween.value(go, (val) => { _slowMoVignette.intensity.value = val; }, _slowMoVignette.intensity.value, 0.25f, .5f).setEase(LeanTweenType.easeOutElastic);
            LeanTween.value(go, (val) => { _slowMoLensDistortion.intensity.value = val; }, _slowMoLensDistortion.intensity.value, -10f, .5f).setEase(LeanTweenType.easeOutElastic);
        }
        else
        {
            LeanTween.cancel(go);
            LeanTween.value(go, (val) => { _slowMoVignette.intensity.value = val; }, _slowMoVignette.intensity.value, 0f, .5f).setEase(LeanTweenType.easeOutElastic);
            LeanTween.value(go, (val) => { _slowMoLensDistortion.intensity.value = val; }, _slowMoLensDistortion.intensity.value, 0f, .5f).setEase(LeanTweenType.easeOutElastic);
        }
    }

    public void PlayKillPostProcessAnim()
    {
        //Debug.Log("Kill Post Process");
        GameObject go = _killPostProcessingVolume.gameObject;

        LeanTween.cancel(go);
        LeanTween.value(go, (val) => { _killVignette.intensity.value = val; }, .25f, 0f, .5f);
    }
}
