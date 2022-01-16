using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CommonRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class CommonPassSetting//定義一個設置的子類Setting，用來定義自己的参數
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material passMaterial = null;
        public string passTag = "mypassTag";
        public FilterMode passfiltermode;
    }
    public CommonPassSetting setting = new CommonPassSetting();    

    class CustomRenderPass : ScriptableRenderPass
    {
        public Material passMaterial;
        public FilterMode passfiltermode{ get; set; }//圖像的模式
        private RenderTargetIdentifier passSource{ get; set; }//源圖像,目標圖像
        RenderTargetHandle passTemplecolorTex;//臨時計算圖像1      
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
            CommandBuffer cmd = CommandBufferPool.Get(passTag);

            RenderTextureDescriptor opaquedesc = renderingData.cameraData.cameraTargetDescriptor;
            opaquedesc.depthBufferBits = 0;
            //passTemplecolorTex.id = 2;
            cmd.GetTemporaryRT(passTemplecolorTex.id, opaquedesc, passfiltermode);//申請一個臨時圖像            

            cmd.Blit(passSource, passTemplecolorTex.Identifier(), passMaterial);
            cmd.Blit(passTemplecolorTex.Identifier(), passSource);

            context.ExecuteCommandBuffer(cmd);//執行命令緩衝區的該命令
            CommandBufferPool.Release(cmd);//釋放該命令
            cmd.ReleaseTemporaryRT(passTemplecolorTex.id);//釋放臨時圖像
        }    
    }
    
    CustomRenderPass myCommonPass;
    public override void Create()
    {
        myCommonPass = new CustomRenderPass(setting.passTag);//實例化並傳参數,name就是tag
        myCommonPass.renderPassEvent = setting.renderPassEvent;
        myCommonPass.passMaterial = setting.passMaterial;
        myCommonPass.passfiltermode = setting.passfiltermode;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)//傳值到pass裡
    {
        myCommonPass.setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(myCommonPass);
    }
}
