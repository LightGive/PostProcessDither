using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LightGive.PostProcessDither
{
    public class DitherRenderFeature : ScriptableRendererFeature
    {
        DitherPass ditherPass;

        public override void Create()
        {
            ditherPass = new DitherPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            ditherPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(ditherPass);
        }
    }

    public class DitherPass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "Render Dither Effects";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetDither");
        static readonly int WidthId = Shader.PropertyToID("_Width");
        static readonly int HeightId = Shader.PropertyToID("_Height");
        static readonly int DitherMaskId = Shader.PropertyToID("_DitherMask");

        Dither dither;
        Material ditherMat;
        RenderTargetIdentifier currentTarget;

        public DitherPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find("LightGive/PostProcess/Dither");
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            ditherMat = CoreUtils.CreateEngineMaterial(shader);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (ditherMat == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            if (!renderingData.cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            dither = stack.GetComponent<Dither>();
            if (dither == null) { return; }
            if (!dither.IsActive()) { return; }

            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Setup(in RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }

        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var source = currentTarget;
            int destination = TempTargetId;

            var w = cameraData.camera.scaledPixelWidth;
            var h = cameraData.camera.scaledPixelHeight;
            ditherMat.SetInt(WidthId, dither.Width.value);
            ditherMat.SetInt(HeightId, dither.Height.value);
            ditherMat.SetTexture(DitherMaskId, dither.DitherMatrixTexture.value);

            int shaderPass = 0;
            cmd.SetGlobalTexture(MainTexId, source);
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, ditherMat, shaderPass);
        }
    }
}