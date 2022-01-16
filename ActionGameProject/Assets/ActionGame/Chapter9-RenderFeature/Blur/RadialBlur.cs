using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RadialBlur : ScriptableRendererFeature
{
    [System.Serializable]
    public class RadialBlurSetting
    {
        public string PassName = "myRadialBlurPass";
        public Material RadialBlurMat = null;
        [Range(0,1)] public float x = 0.5f;
        [Range(0,1)] public float y = 0.5f;
        [Range(1,8)] public int loop = 5;
        [Range(0,12)] public float blur = 3;
        [Range(1,5)] public int downsample = 2;
        [Range(0,1)] public float instensity = 0.5f;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;
    }
    public RadialBlurSetting setitng = new RadialBlurSetting();
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material mymat;
        public string name;
        public float x;
        public float y;
        public int loop;
        public float instensity;
        public float blur;
        public int downsample;
        public RenderTargetIdentifier Source{get;set;}
        public RenderTargetIdentifier BlurTex;
        public RenderTargetIdentifier Buffer0;
        public RenderTargetIdentifier Buffer1;
        int ssW;
        int ssH;
        public CustomRenderPass(string Tag)//構造函數
        {
            name = Tag;
        }
        public void setup(RenderTargetIdentifier source)
        {
            this.Source = source;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (mymat == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
            int BlurTexID = Shader.PropertyToID("_BlurTex");
            int TempID0 = Shader.PropertyToID("_Temp");
            int TempID1 = Shader.PropertyToID("_SourceTex");
            int loopID = Shader.PropertyToID("_Loop");
            int Xid = Shader.PropertyToID("_X");
            int Yid = Shader.PropertyToID("_Y");
            int BlurID = Shader.PropertyToID("_Blur");
            int instenID = Shader.PropertyToID("_Instensity");

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;

            ssH = opaqueDesc.height / downsample;
            ssW = opaqueDesc.width / downsample;
            
            CommandBuffer cmd = CommandBufferPool.Get(name);
            
            cmd.GetTemporaryRT(TempID0, ssW, ssH, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);//用來儲存降採樣的
            cmd.GetTemporaryRT(BlurTexID, opaqueDesc);//模糊圖
            cmd.GetTemporaryRT(TempID1, opaqueDesc);

            BlurTex = new RenderTargetIdentifier(BlurTexID);
            Buffer0 = new RenderTargetIdentifier(TempID0);
            Buffer1 = new RenderTargetIdentifier(TempID1);

            //下面三種寫法也可以改變loop數值
            //mymat.SetFloat("_Loop", loop);  
            //mymat.SetFloat(loopID, loop);
            //Shader.SetGlobalFloat(loopID, loop);           
            cmd.SetGlobalFloat(loopID, loop);            
            cmd.SetGlobalFloat(Xid, x);
            cmd.SetGlobalFloat(Yid, y);
            cmd.SetGlobalFloat(BlurID, blur);
            cmd.SetGlobalFloat(instenID, instensity);
            
            cmd.Blit(Source, Buffer0);//儲存降採樣的原圖，用於pass0計算模糊圖
            cmd.Blit(Source, Buffer1);//儲存原圖，用於計算pass1的混合
            cmd.Blit(Buffer0, BlurTex, mymat, 0);//pass0的模糊計算
            cmd.Blit(BlurTex, Source, mymat, 1);//pass1的混合

            context.ExecuteCommandBuffer(cmd);
            cmd.ReleaseTemporaryRT(BlurTexID);
            cmd.ReleaseTemporaryRT(TempID0);
            cmd.ReleaseTemporaryRT(TempID1);
            CommandBufferPool.Release(cmd);
        }
    }
    CustomRenderPass m_ScriptablePass;
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(setitng.PassName);
        m_ScriptablePass.renderPassEvent = setitng.passEvent;
        m_ScriptablePass.blur = setitng.blur;
        m_ScriptablePass.x = setitng.x;
        m_ScriptablePass.y = setitng.y;
        m_ScriptablePass.instensity = setitng.instensity;
        m_ScriptablePass.loop = setitng.loop;
        m_ScriptablePass.mymat = setitng.RadialBlurMat;
        m_ScriptablePass.name = setitng.PassName;
        m_ScriptablePass.downsample = setitng.downsample;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {        
        m_ScriptablePass.setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}