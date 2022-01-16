using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BokehBlur : ScriptableRendererFeature
{
    [System.Serializable]public class BokehBlursetting
    {
        public Material BokehBlurMat;
        [Range(1, 10)]public int downsample = 2;
        [Range(3, 500)] public int loop = 50;
        [Range(0.1f, 10)]  public float radius = 1; //採樣半徑，越大圓斑越大但採樣點越分散"
        [Range(0,0.5f)]public float BlurSmoothness = 0.1f;//模糊過度的平滑度
        public float NearDist = 5; //近處模糊结束距離
        public float FarDist = 9; //遠處模糊结束距離
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
        public string PassName = "BokehBlurPass";
    }
    public BokehBlursetting setting = new BokehBlursetting();
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material BokehBlurMat;
        public int loop;
        public float BlurSmoothness;
        public int downsample;
        public float radius;
        public float NearDist;
        public float FarDist;
        RenderTargetIdentifier source;
        public string name;
        int width;
        int height;
        readonly static int BlurID = Shader.PropertyToID("blur");//申請之後就不再變化
        readonly static int SourceBakedID = Shader.PropertyToID("_SourceTex");
        public CustomRenderPass(string Tag)//構造函數
        {
            name = Tag;
        }
        public void setup(RenderTargetIdentifier Sour)
        {
            this.source = Sour;            
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (BokehBlurMat == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
            BokehBlurMat.SetFloat("_loop", loop);
            BokehBlurMat.SetFloat("_radius", radius);
            BokehBlurMat.SetFloat("_NearDist", NearDist);
            BokehBlurMat.SetFloat("_FarDist", FarDist);
            BokehBlurMat.SetFloat("_BlurSmoothness", BlurSmoothness);

            CommandBuffer cmd = CommandBufferPool.Get(name);
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            
            width = desc.width / downsample;
            height = desc.height / downsample;

            cmd.GetTemporaryRT(BlurID, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            cmd.GetTemporaryRT(SourceBakedID, desc); 
            cmd.Blit(source, SourceBakedID);//把相機圖像複製到備份RT圖，並自動發送到shader裡，無須手動指定發送
            cmd.Blit(source, BlurID, BokehBlurMat, 0);//第一個pass: 把屏幕圖像計算後存到一個降採樣的模糊圖裡
            cmd.Blit(BlurID, source, BokehBlurMat, 1);//第二個pass: 發送模糊圖到shader的maintex,然後混合輸出

            context.ExecuteCommandBuffer(cmd);
            cmd.ReleaseTemporaryRT(BlurID);
            cmd.ReleaseTemporaryRT(SourceBakedID);
            CommandBufferPool.Release(cmd);
        }
    }
    CustomRenderPass m_ScriptablePass;
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(setting.PassName);
        m_ScriptablePass.BokehBlurMat = setting.BokehBlurMat;
        m_ScriptablePass.loop = setting.loop;
        m_ScriptablePass.BlurSmoothness = setting.BlurSmoothness;
        m_ScriptablePass.radius = setting.radius;
        m_ScriptablePass.renderPassEvent = setting.Event;
        m_ScriptablePass.name = setting.PassName;
        m_ScriptablePass.downsample = setting.downsample;
        m_ScriptablePass.NearDist = setting.NearDist;
        m_ScriptablePass.FarDist = setting.FarDist;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}