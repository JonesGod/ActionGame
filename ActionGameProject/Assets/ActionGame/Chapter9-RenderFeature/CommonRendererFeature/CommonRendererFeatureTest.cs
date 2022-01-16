using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CommonRendererFeatureTest : ScriptableRendererFeature
{
    class CommonPass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "Common PostProcessing";
        private Material m_Material;
        RenderTargetIdentifier currentTarget;
        private RenderTargetHandle destination { get; set; }
        public CommonPass(Material material)
        {
            m_Material = material;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {            
            if (m_Material == null)
            {
                return;
            }
            if (!renderingData.cameraData.postProcessEnabled) return;            

            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isSceneViewCamera) return;

            cmd.Blit(currentTarget, destination.Identifier(), m_Material);
        }
        public void Setup(in RenderTargetIdentifier currentTarget, RenderTargetHandle dest)
        {
            this.destination = dest;
            this.currentTarget = currentTarget;
        }
    }

    public Material UsedMaterial;
    public RenderPassEvent PassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

    CommonPass m_ScriptablePass;

    RenderTargetHandle m_CameraColorAttachment;

    public override void Create()
    {
        m_ScriptablePass = new CommonPass(UsedMaterial)
        {
            renderPassEvent = PassEvent
        };

        m_CameraColorAttachment.Init("_CameraColorTexture");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.Setup(renderer.cameraColorTarget, m_CameraColorAttachment);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
