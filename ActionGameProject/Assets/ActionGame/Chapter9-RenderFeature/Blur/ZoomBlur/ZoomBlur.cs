using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ZoomBlur : ScriptableRendererFeature
{
    [System.Serializable]
    public class ZoomBlurSetting
    {
        public string PassName = "ZoomBlurPass";
        public Material zoomBlurMat = null;
        public Vector2 centerPos = new Vector2(0, 0);
        public float xPos = 0.0f;
        public float yPos = 0.0f;
        [Range(1, 16)]public int Iteration = 4;//(loop)迭代採樣次數
        [Range(0.0f, 16f)]public float BlurDistance = 3.0f;//模糊範圍
        [Range(0.0f, 1.0f)]public float BlurIntensity = 0.5f;//模糊強度(0為關閉，1為開啟)
        [Range(1, 8)]public int DownSample = 1;//降採樣次數，效能優化
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;
    }
    public ZoomBlurSetting setting = new ZoomBlurSetting();

    class ZoomBlurRenderPass : ScriptableRenderPass
    {
        public string name;
        public Material zoomBlurMat;
        public float xPos;
        public float yPos;
        public Vector2 centerPos;
        public int iteration;
        public float blurDistance;
        public float blurIntensity;
        public int downSample;
        public RenderTargetIdentifier passSource{get; set;}
        public RenderTargetIdentifier blurTex;
        public RenderTargetIdentifier tempTex;
        public ZoomBlurRenderPass(string Tag)
        {
            name = Tag;
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
            int blurTexID = Shader.PropertyToID("_BlurTex");
            int tempTexID = Shader.PropertyToID("_SourceTex");
            int iterationID = Shader.PropertyToID("_Iteration");
            int blurDistanceID = Shader.PropertyToID("_BlurDistance");
            int blurIntensityID = Shader.PropertyToID("_BlurIntensity");
            int xPosID = Shader.PropertyToID("_XPos");
            int yPosID = Shader.PropertyToID("_YPos");
            int centerPosID = Shader.PropertyToID("_CenterPos");

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            int rtWidth = opaqueDesc.width / downSample;
            int rtHeight = opaqueDesc.height / downSample;

            //要配合後處理volume的話：var stack = VolumeManager.instance.stack;
            CommandBuffer cmd = CommandBufferPool.Get(name);

            cmd.SetGlobalFloat(iterationID, iteration);
            cmd.SetGlobalFloat(blurDistanceID, blurDistance);
            cmd.SetGlobalFloat(blurIntensityID, blurIntensity);
            cmd.SetGlobalFloat(xPosID, xPos);
            cmd.SetGlobalFloat(yPosID, yPos);
            cmd.SetGlobalVector(centerPosID, centerPos);

            cmd.GetTemporaryRT(blurTexID, rtWidth, rtHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(tempTexID, opaqueDesc);
                        
            blurTex = new RenderTargetIdentifier(blurTexID);
            tempTex = new RenderTargetIdentifier(tempTexID);

            //cmd.CopyTexture(passSource, blurTex);這個不能用，應該是因為GetTemporaryRT設定跟passsource 用的camera設定不同
            //cmd.CopyTexture(passSource, tempTex);
            cmd.Blit(passSource, blurTex);
            cmd.Blit(passSource, tempTex);
            cmd.Blit(blurTex, passSource, zoomBlurMat, 0);

            context.ExecuteCommandBuffer(cmd);
            cmd.ReleaseTemporaryRT(blurTexID);
            cmd.ReleaseTemporaryRT(tempTexID);
            CommandBufferPool.Release(cmd);            
        }
    }

    ZoomBlurRenderPass ZoomBlurPass;
    public override void Create()
    {
        ZoomBlurPass = new ZoomBlurRenderPass(setting.PassName);
        ZoomBlurPass.renderPassEvent = setting.passEvent;
        ZoomBlurPass.zoomBlurMat = setting.zoomBlurMat;
        ZoomBlurPass.xPos = setting.xPos;
        ZoomBlurPass.yPos = setting.yPos;
        ZoomBlurPass.centerPos = setting.centerPos;
        ZoomBlurPass.iteration = setting.Iteration;
        ZoomBlurPass.blurDistance = setting.BlurDistance;
        ZoomBlurPass.blurIntensity = setting.BlurIntensity;
        ZoomBlurPass.downSample = setting.DownSample;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        ZoomBlurPass.setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(ZoomBlurPass);
    }
}


