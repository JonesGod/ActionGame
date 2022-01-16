using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DoubleBlur : ScriptableRendererFeature
{
    [System.Serializable]
    public class mysetting//定義一個設置的子類
    {
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material mymat;
        [Range(1, 8)]public int downsample = 2;
        [Range(2, 8)]public int loop = 2;
        [Range(0.0f, 5f)]public float blur = 0.0f;
        public string RenderFeatureName = "mydoubleblur";//render feature的名字
    }
    public mysetting setting = new mysetting();
    class CustomRenderPass : ScriptableRenderPass//自定義pass
    {
        public Material passMat = null;
        public int passdownsample = 2;
        public int passloop = 2;//模糊的迭代次數
        public float passblur = 4;
        private RenderTargetIdentifier passSource{ get; set; }
        RenderTargetIdentifier buffer1;//RT1的ID
        RenderTargetIdentifier buffer2;//RT2的ID
        string RenderFeatureName;//feature名
        struct LEVEL
        {
            public int down;
            public int up;
        };
        LEVEL[] my_level;
        int maxLevel = 16;//指定一個最大值限制申請的ID數量
        public CustomRenderPass(string name)//構造函數
        {
            RenderFeatureName = name;
        }
        

        public void setup(RenderTargetIdentifier source)//初始化，接收RenderFeature傳的圖
        {
            this.passSource = source;
            my_level = new LEVEL[maxLevel];
            for (int t = 0; t < maxLevel; t++)//申請32個ID，up和down各16個，用這個id去代替臨時RT來使用
            {
                my_level[t] = new LEVEL
                {
                    down = Shader.PropertyToID("_BlurMipDown" + t),
                    up = Shader.PropertyToID("_BlurMipUp" + t)
                };
            }
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (passMat == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
            CommandBuffer cmd = CommandBufferPool.Get(RenderFeatureName);//定義cmd
            passMat.SetFloat("_Blur", passblur);//指定材質參數，設置模糊
            //cmd.SetGlobalFloat("_Blur", passblur);//設置模糊
            RenderTextureDescriptor opaquedesc = renderingData.cameraData.cameraTargetDescriptor;//定義螢幕圖像參數結構體
            int width = opaquedesc.width / passdownsample;//第一次降採樣是使用此参数，後面就是除2去降採樣了
            int height = opaquedesc.height / passdownsample;
            opaquedesc.depthBufferBits = 0;
            //down
            RenderTargetIdentifier LastDown = passSource;//把初始圖像作為lastdown的起始圖像去计算
            for(int t = 0; t < passloop; t++)
            {
                int midDown = my_level[t].down;//middle down ，間接計算down的ID
                int midUp = my_level[t].up; //middle Up ，間接計算up的ID
                cmd.GetTemporaryRT(midDown, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);//對指定寬高申請RT，每個循環的指定RT都會變小為原來一半
                cmd.GetTemporaryRT(midUp, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);//同上，但是這裡只做申請並未計算，這樣在UP的循環裡就不用申請RT了
                cmd.Blit(LastDown, midDown, passMat, 0);//計算down的pass
                LastDown = midDown;
                width = Mathf.Max(width/2, 1);//每次循環都降尺寸
                height = Mathf.Max(height/2, 1);
            }
            //up
            int lastUp = my_level[passloop - 1].down;//把down的最後一次圖像當成up的第一張圖去計算up
            for(int j = passloop - 2; j >= 0; j--)//這裡减2是因為第一次已經有了要减去1，但第一次是直接複製的，所以循環完後還得補一次up
            {
                int midUp = my_level[j].up;
                cmd.Blit(lastUp, midUp, passMat, 1);//在down過程中已經把RT的位置儲存好了，這裡可以直接使用
                lastUp = midUp;
            }
            cmd.Blit(lastUp, passSource, passMat,1);//補一次up，順便输出
            context.ExecuteCommandBuffer(cmd);//執行命令緩衝區的該命令
            CommandBufferPool.Release(cmd);//釋放cmd
            for(int k = 0; k < passloop; k++)//清空RT，防止内存洩漏
            {
                cmd.ReleaseTemporaryRT(my_level[k].up);
                cmd.ReleaseTemporaryRT(my_level[k].down);
            }
        }
    }
    CustomRenderPass mypass;
    public override void Create()//進行初始化，這裡最新開始
    {
        mypass = new CustomRenderPass(setting.RenderFeatureName);//實力化並傳參數,name就是tag
        mypass.renderPassEvent = setting.passEvent;
        mypass.passblur = setting.blur;
        mypass.passloop = setting.loop;
        mypass.passMat = setting.mymat;
        mypass.passdownsample = setting.downsample;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)//傳值到pass裡
    {
        mypass.setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(mypass);
    }
}


