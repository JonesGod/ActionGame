using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GaussianBlur : ScriptableRendererFeature
{
    [System.Serializable]
    public class GaussianBlurSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material blurMaterial = null;
        [Range(1, 10)] public int downsample = 1;
        [Range(1, 10)] public int iterations = 2;
        [Range(0.0f, 5.0f)] public float blur = 0.5f;
        public string passTag = "mypassTag";
    }

    public GaussianBlurSettings settings = new GaussianBlurSettings();    

    class CustomRenderPass : ScriptableRenderPass//自定義Pass，定義參數來對應接受setting的數據
    {
        public Material blurMaterial;
        public int downsample;
        public int iterations;
        public float blur;
        public FilterMode passfiltermode{ get; set; }//圖像的模式
        private RenderTargetIdentifier passSource{ get; set; }//源圖像,目標圖像
        RenderTargetIdentifier buffer0;//臨時計算圖像1
        RenderTargetIdentifier buffer1;//臨時計算圖像2
        
        string passTag;
        public CustomRenderPass(string Tag)//構造函數
        {
            passTag = Tag;
        }
         public void setup(RenderTargetIdentifier source)//接收RenderFeature傳的圖
        {
            this.passSource = source;
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)//執行函數，用來計算模糊圖像
        {
            if (blurMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
            if (!renderingData.cameraData.postProcessEnabled) return;
            int bufferid0 = Shader.PropertyToID("bufferblur0");
            int bufferid1 = Shader.PropertyToID("bufferblur1");

            CommandBuffer cmd = CommandBufferPool.Get(passTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            int width = opaqueDesc.width / downsample;
            int height = opaqueDesc.height / downsample;
            opaqueDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(bufferid0, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(bufferid1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

            buffer0 = new RenderTargetIdentifier(bufferid0);
            blurMaterial.SetFloat("_Blur", blur);
            
            cmd.Blit(passSource, buffer0);

            for(int t = 0; t < iterations; t++)
            {
                cmd.SetGlobalFloat("_Blur", t * blur + 1);
                buffer1 = new RenderTargetIdentifier(bufferid1);
                // Render the vertical pass
                cmd.Blit(buffer0, buffer1, blurMaterial, 0);                

                var temRT = buffer0;
                buffer0 = buffer1;
                buffer1 = temRT;
                // Render the horizontal pass
                cmd.Blit(buffer0, buffer1, blurMaterial, 1);

                buffer0 = buffer1;
            }
            cmd.Blit(buffer0, passSource);

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
        scriptablePass.blurMaterial = settings.blurMaterial;
        scriptablePass.downsample = settings.downsample;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)//傳值到pass裡
    {
        scriptablePass.setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(scriptablePass);
    }
}


