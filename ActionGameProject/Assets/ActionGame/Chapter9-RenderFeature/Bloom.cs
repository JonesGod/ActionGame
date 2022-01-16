using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Bloom : ScriptableRendererFeature
{
    [System.Serializable]
    public class BloomSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material bloomMaterial = null;
        [Range(1, 10)] public int downsample = 1;
        [Range(1, 10)] public int iterations = 2;
        [Range(0.0f, 5.0f)] public float blur = 0.5f;
        [Range(0.0f, 4.0f)] public float luminanceThreshold = 0.6f;
        public string passTag = "mypassTag";
    }

    public BloomSettings settings = new BloomSettings();    

    class CustomRenderPass : ScriptableRenderPass//自定義Pass，定義參數來對應接受setting的數據
    {
        public Material bloomMaterial;
        public int downsample;
        public int iterations;
        public float blur;
        public float luminanceThreshold;
        public FilterMode passfiltermode{ get; set; }//圖像的模式        
        RenderTargetIdentifier buffer0;//臨時計算圖像1
        RenderTargetIdentifier buffer1;//臨時計算圖像2  
        RenderTargetIdentifier currentTarget{ get; set; }//源圖像,目標圖像
        private RenderTargetHandle destination { get; set; }      
        
        string passTag;
        public CustomRenderPass(string Tag)//構造函數
        {
            passTag = Tag;
        }
         public void setup(in RenderTargetIdentifier currentTarget, RenderTargetHandle dest)//接收RenderFeature傳的圖
        {            
            this.destination = dest;
            this.currentTarget = currentTarget;
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)//執行函數，用來計算模糊圖像
        {
            if (bloomMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }      
            if (!renderingData.cameraData.postProcessEnabled) return;      

            var passSource = currentTarget;
            
            int bufferid0 = Shader.PropertyToID("bufferblur0");
            int bufferid1 = Shader.PropertyToID("bufferblur1");

            CommandBuffer cmd = CommandBufferPool.Get(passTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            int width = opaqueDesc .width / downsample;
            int height = opaqueDesc .height / downsample;
            opaqueDesc.depthBufferBits = 0;            

            cmd.GetTemporaryRT(bufferid0, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(bufferid1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            bloomMaterial.SetFloat("_LuminanceThreshold", luminanceThreshold);

            buffer0 = new RenderTargetIdentifier(bufferid0);
            bloomMaterial.SetFloat("_Blur", blur);

            cmd.Blit(passSource, buffer0, bloomMaterial, 0);

            for(int t = 0; t < iterations; t++)
            {
                cmd.SetGlobalFloat("_Blur", t * blur + 1);
                buffer1 = new RenderTargetIdentifier(bufferid1);
                // Render the vertical pass
                cmd.Blit(buffer0, buffer1, bloomMaterial, 1);                

                var temRT = buffer0;
                buffer0 = buffer1;
                buffer1 = temRT;
                // Render the horizontal pass
                cmd.Blit(buffer0, buffer1, bloomMaterial, 2);

                buffer0 = buffer1;
            }
            buffer1 = new RenderTargetIdentifier(bufferid1);
            
            cmd.SetGlobalTexture ("_BloomTex", buffer0);  
			cmd.Blit (passSource, buffer1, bloomMaterial, 3); 
            cmd.Blit(buffer1, passSource);

            context.ExecuteCommandBuffer(cmd);//執行命令緩衝區的該命令
            CommandBufferPool.Release(cmd);//釋放該命令
        }        
    }
    CustomRenderPass scriptablePass;
    public override void Create()
    { 
        scriptablePass = new CustomRenderPass(settings.passTag);//實例化並傳参數,name就是tag
        scriptablePass.renderPassEvent = settings.renderPassEvent;
        scriptablePass.blur = settings.blur;
        scriptablePass.iterations = settings.iterations;
        scriptablePass.bloomMaterial = settings.bloomMaterial;
        scriptablePass.downsample = settings.downsample;
        scriptablePass.luminanceThreshold = settings.luminanceThreshold;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)//傳值到pass裡
    {
        var dest = RenderTargetHandle.CameraTarget;
        scriptablePass.setup(renderer.cameraColorTarget, dest);
        renderer.EnqueuePass(scriptablePass);
    }
}


