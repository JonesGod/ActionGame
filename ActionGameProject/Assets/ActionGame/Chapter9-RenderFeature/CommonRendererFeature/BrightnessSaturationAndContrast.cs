using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessSaturationAndContrast : ScriptableRendererFeature
{
    [System.Serializable]
    public class BrightnessSaturationAndContrastSetting//定義一個設置的子類Setting，用來定義自己的参數
    {
        public RenderPassEvent PassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material effectMaterial = null;
        public int MaterialPassIndex = -1;
    }

    public BrightnessSaturationAndContrastSetting setting = new BrightnessSaturationAndContrastSetting();

    class CustomRenderPass : ScriptableRenderPass//自定義Pass
    {
        public Material passMaterial;
        public int passMaterialInt = 0;
        public FilterMode passfiltermode { get; set; }//圖像的模式
        private RenderTargetIdentifier passSource{ get; set; }//源圖像,目標圖像
        RenderTargetHandle passTemplecolorTex;//臨時計算圖像
        string passTag;
        public CustomRenderPass(RenderPassEvent passevent, Material material, int passint, string tag)//構造函數
        {
            this.renderPassEvent = passevent;

            this.passMaterial = material;

            this.passMaterialInt = passint;

            passTag = tag;
        }
        public void setup(RenderTargetIdentifier sour)//接收RenderFeature傳的圖
        {
            this.passSource = sour;
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)//類似OnRenderimagePass
        {
            CommandBuffer cmd = CommandBufferPool.Get(passTag);

            RenderTextureDescriptor opaquedesc = renderingData.cameraData.cameraTargetDescriptor;
            opaquedesc.depthBufferBits = 0;
            
            cmd.GetTemporaryRT(passTemplecolorTex.id, opaquedesc, passfiltermode);//申請一個臨時圖像

            Blit(cmd, passSource, passTemplecolorTex.Identifier(), passMaterial, passMaterialInt);//把源貼圖輸入到材質對應的pass裡處理，並把處理結果的圖像儲存到臨時圖像
            Blit(cmd, passTemplecolorTex.Identifier(), passSource);//然後把臨時圖像又存到源圖像裡 

            context.ExecuteCommandBuffer(cmd);//執行命令緩衝區的該命令
            CommandBufferPool.Release(cmd);//釋放該命令
            cmd.ReleaseTemporaryRT(passTemplecolorTex.id);//釋放臨時圖像
        }
    }
    CustomRenderPass mypass;
    public override void Create()//進行初始化，這裡最先開始
    {
        int passint = setting.effectMaterial == null? 1 : setting.effectMaterial.passCount - 1;//計算材質球裡的pass總數，如果沒有則為1
        setting.MaterialPassIndex = Mathf.Clamp(setting.MaterialPassIndex, -1, passint);//把設置裡的pass的id限制在-1到材質的最大pass數
        mypass = new CustomRenderPass(setting.PassEvent, setting.effectMaterial, setting.MaterialPassIndex, name);//實例化一下並傳参數，name就是tag
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)//傳值到pass裡
    {
        var src = renderer.cameraColorTarget;
        mypass.setup(src);
        renderer.EnqueuePass(mypass);
    }
}


