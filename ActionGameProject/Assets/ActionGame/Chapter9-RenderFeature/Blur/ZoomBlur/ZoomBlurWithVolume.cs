using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ZoomBlurWithVolume : ScriptableRendererFeature
{
    class ZoomBlurWithVolumePass : ScriptableRenderPass
    {   
        public string name;
        public int DownSample;
        public Material zoomBlurMat;
        public RenderTargetIdentifier passSource{get; set;}
        public RenderTargetIdentifier blurTex;
        public RenderTargetIdentifier tempTex;
        ZoomBlurVolume zoomBlurVolume;

        public ZoomBlurWithVolumePass(string Tag)
        {
            name = Tag;
            var shader = Shader.Find("URPShader/Chapter9/ZoomBlurShader");
            if(shader == null)
            {
                Debug.Log("Shader not find");
                return;
            }
            zoomBlurMat = CoreUtils.CreateEngineMaterial(shader);
        }

        public void setup(RenderTargetIdentifier source)
        {
            this.passSource = source;
        }       

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if(zoomBlurMat == null)
            {
                Debug.Log("Material not Found");
                return;
            }
            if (!renderingData.cameraData.postProcessEnabled) return;
            var stack = VolumeManager.instance.stack;
            zoomBlurVolume = stack.GetComponent<ZoomBlurVolume>();
            if(zoomBlurVolume.enableEffect.value)
            {           
                int blurTexID = Shader.PropertyToID("_BlurTex");
                int tempTexID = Shader.PropertyToID("_SourceTex");
                int iterationID = Shader.PropertyToID("_Iteration");
                int blurDistanceID = Shader.PropertyToID("_BlurDistance");
                int blurIntensityID = Shader.PropertyToID("_BlurIntensity");
                int centerPositionID = Shader.PropertyToID("_CenterPos");

                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                int rtWidth = opaqueDesc.width / zoomBlurVolume.downSample.value;
                int rtHeight = opaqueDesc.height / zoomBlurVolume.downSample.value;

                CommandBuffer cmd = CommandBufferPool.Get(name);

                cmd.SetGlobalInt(iterationID, zoomBlurVolume.iteration.value);
                cmd.SetGlobalFloat(blurDistanceID, zoomBlurVolume.blurDistance.value);
                cmd.SetGlobalFloat(blurIntensityID, zoomBlurVolume.blurIntensity.value);
                cmd.SetGlobalVector(centerPositionID, zoomBlurVolume.centerPosition.value);

                cmd.GetTemporaryRT(blurTexID, rtWidth, rtHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
                cmd.GetTemporaryRT(tempTexID, opaqueDesc);

                blurTex = new RenderTargetIdentifier(blurTexID);
                tempTex = new RenderTargetIdentifier(tempTexID);

                cmd.Blit(passSource, blurTex);
                cmd.Blit(passSource, tempTex);
                cmd.Blit(blurTex, passSource, zoomBlurMat, 0);

                context.ExecuteCommandBuffer(cmd);
                cmd.ReleaseTemporaryRT(blurTexID);
                cmd.ReleaseTemporaryRT(tempTexID);
                CommandBufferPool.Release(cmd); 
            }             
        }        
    }

    ZoomBlurWithVolumePass zoomBlurWithVolumePass;

    public override void Create()
    {
        zoomBlurWithVolumePass = new ZoomBlurWithVolumePass(name);
        zoomBlurWithVolumePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        zoomBlurWithVolumePass.setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(zoomBlurWithVolumePass);
    }
}


