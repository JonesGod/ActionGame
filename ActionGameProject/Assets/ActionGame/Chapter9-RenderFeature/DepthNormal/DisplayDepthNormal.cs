using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DisplayDepthNormal : ScriptableRendererFeature
{    
    public class DisplayNormalTexturePass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "Display Normal Texture";

        private Material normalTextureMat;

        RenderTargetIdentifier currentTarget;
        private RenderTargetHandle destination { get; set; }

        public DisplayNormalTexturePass()
        {
            var shader = Shader.Find("URPShader/Chapter9/DisplayDepthNormalShader");
            normalTextureMat = CoreUtils.CreateEngineMaterial(shader);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
             if (normalTextureMat == null)
            {
                Debug.LogError("Material not created.");
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
            var source = currentTarget;

            Blit(cmd, source, destination.Identifier(), normalTextureMat);
        }

        public void Setup(in RenderTargetIdentifier currentTarget, RenderTargetHandle dest)
        {
            this.destination = dest;
            this.currentTarget = currentTarget;
        }
    }
    DisplayNormalTexturePass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new DisplayNormalTexturePass();
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var dest = RenderTargetHandle.CameraTarget;
        m_ScriptablePass.Setup(renderer.cameraColorTarget, dest);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}