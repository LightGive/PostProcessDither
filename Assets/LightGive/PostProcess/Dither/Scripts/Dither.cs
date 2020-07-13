using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Dither : VolumeComponent, IPostProcessComponent
{
    [Tooltip("4の倍数にして下さい")]
    public MinIntParameter Width = new MinIntParameter(420,4, true);
    [Tooltip("4の倍数にして下さい")]
    public MinIntParameter Height = new MinIntParameter(320, 4, true);

    public TextureParameter DitherMatrixTexture = new TextureParameter(null, true);

    public bool IsActive() => DitherMatrixTexture != null;
    public bool IsTileCompatible() => false;

    private void OnValidate()
    {
        if (Width.value % 4 != 0)
            Width.value += Width.value % 4;
        if (Height.value % 4 != 0)
            Height.value += Height.value % 4;
    }
}