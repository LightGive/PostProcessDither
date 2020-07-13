Shader "PostEffect/ZoomBlur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Tags { "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            CGPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag

                sampler2D _DitherMaskLOD2D;
                sampler2D _MainTex;
                sampler2D _DitherMask;
                int _Width;
                int _Height;
            
                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f Vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }


                float4 Frag(v2f i) : SV_Target
                {
                    float4 outColor = float4(0, 0, 0, 1);
                    float2 res = float2(_Width,_Height);
                    
                    //表示するUV
                    float2 uv = floor(i.uv*res)/res;
                    
                    //UVの0~1で4pxelで分けた時のブロックの1つ辺りの長さ
                    float widColm = 1.0 / float(_Width);
                    float heiColm = 1.0 / float(_Height);
 
                    //色を取得するUVを計算
                    float2 getPixelUV = float2(
                        floor(i.uv.x / widColm) * widColm + (widColm * 0.5),
                        floor(i.uv.y / heiColm) * heiColm + (heiColm * 0.5));

                    //モノクロ（0～1で取得）
                    float3 c = tex2D(_MainTex, getPixelUV).rgb;
                    float grayScale = (c.r + c.g + c.b) / 3.0;

                    //ディザーテクスチャ抜き出し
                    float2 ditherUV = float2(frac(i.uv.x /widColm),frac(i.uv.y/heiColm));
                    float value = tex2D(_DitherMask, ditherUV).rgb;
                    outColor = step(value, grayScale);

                    //outColor = grayScale;
                    return outColor;
                }
            ENDCG
        }
    }
}