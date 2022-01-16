using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KawaseBlur : ScriptableRendererFeature
{
    [System.Serializable]
    public class KawaseBlurSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material blurMaterial = null;
        [Range(1, 10)] public int downsample = 1;
        [Range(1, 10)] public int loop = 2;
        [Range(0.0f, 5.0f)] public float blur = 0.5f;
        public string passTag = "mypassTag";
    }

    public KawaseBlurSettings settings = new KawaseBlurSettings();    

    class CustomRenderPass : ScriptableRenderPass//自定義Pass，定義參數來對應接受setting的數據
    {
        public Material blurMaterial;
        public int downsample;
        public int loop;
        public float blur;
        public FilterMode passfiltermode{ get; set; }//圖像的模式
        private RenderTargetIdentifier passSource{ get; set; }//源圖像,目標圖像
        RenderTargetIdentifier buffer1;//臨時計算圖像1
        RenderTargetIdentifier buffer2;//臨時計算圖像2
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
            int bufferid1 = Shader.PropertyToID("bufferblur1");
            int bufferid2 = Shader.PropertyToID("bufferblur2");

            CommandBuffer cmd = CommandBufferPool.Get(passTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            int width = opaqueDesc.width / downsample;
            int height = opaqueDesc.height / downsample;
            opaqueDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(bufferid1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(bufferid2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

            buffer1 = new RenderTargetIdentifier(bufferid1);
            buffer2 = new RenderTargetIdentifier(bufferid2);
            blurMaterial.SetFloat("_Blur", blur);
            cmd.SetGlobalFloat("_Blur", 1f);
            cmd.Blit(passSource, buffer1, blurMaterial);

            for(int t=1; t < loop; t++)
            {
                cmd.SetGlobalFloat("_Blur", t * blur + 1);
                cmd.Blit(buffer1, buffer2, blurMaterial);

                var temRT = buffer1;
                buffer1 = buffer2;
                buffer2 = temRT;
            }
            cmd.SetGlobalFloat("_Blur", loop * blur+1);
            cmd.Blit(buffer1, passSource, blurMaterial);

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
        scriptablePass.loop = settings.loop;
        scriptablePass.blurMaterial = settings.blurMaterial;
        scriptablePass.downsample = settings.downsample;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)//傳值到pass裡
    {
        scriptablePass.setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(scriptablePass);
    }
}


