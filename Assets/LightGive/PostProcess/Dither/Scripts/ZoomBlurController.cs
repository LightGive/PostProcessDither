using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class ZoomBlurController : MonoBehaviour
{
    public VolumeProfile volumeProfile;
    [Range(0f, 100f)]
    public float focusPower = 10f;
    [Range(0, 10)]
    public int focusDetail = 5;
    public Vector2 focusScreenPosition = Vector2.zero;
    Dither zoomBlur;

    void Update()
    {
        if (volumeProfile == null) return;
        if (zoomBlur == null) volumeProfile.TryGet<Dither>(out zoomBlur);
        if (zoomBlur == null) return;


    }
}
