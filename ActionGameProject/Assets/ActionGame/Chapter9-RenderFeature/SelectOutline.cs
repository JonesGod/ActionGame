using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SelectOutline : ScriptableRendererFeature
{
    public enum TYPE
    {
        InColorOn, InColorOff
    }
    [System.Serializable]public class SelectOutlineSetting
    {
        public Material mymat;
        public Color color = Color.white;
        [Range(1000,5000)]public int QueueMin = 2000;
        [Range(1000,5000)]public int QueueMax = 2500;
        public LayerMask layer;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingSkybox;
        [Range(0.0f, 3.0f)] public float blur = 1.0f;
        [Range(1, 5)]public int passloop = 3;
        public TYPE ColorType = TYPE.InColorOn;
    }
    public SelectOutlineSetting mysetting = new SelectOutlineSetting();
    int solidcolorID;
    //第一個pass，繪製純色圖像
    class DrawSolidColorPass : ScriptableRenderPass
    {
        SelectOutlineSetting mysetting = null;
        SelectOutline SelectOutline = null;
        ShaderTagId shaderTag = new ShaderTagId("DepthOnly");//只有在這個標籤LightMode對應的shader才會被繪製
        FilteringSettings filter;
        public DrawSolidColorPass(SelectOutlineSetting setting, SelectOutline render)
        {
            mysetting = setting;

            SelectOutline = render;
            //過濾設定
            RenderQueueRange queue = new RenderQueueRange();
            queue.lowerBound = Mathf.Min(setting.QueueMax, setting.QueueMin);
            queue.upperBound = Mathf.Max(setting.QueueMax, setting.QueueMin);
            filter = new FilteringSettings(queue, setting.layer);
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            int temp = Shader.PropertyToID("_MyTempColor");
            RenderTextureDescriptor desc = cameraTextureDescriptor;
            cmd.GetTemporaryRT(temp, desc);
            SelectOutline.solidcolorID = temp;

            ConfigureTarget(temp);
            ConfigureClear(ClearFlag.All,Color.black);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            mysetting.mymat.SetColor("_SolidColor", mysetting.color);
            CommandBuffer cmd = CommandBufferPool.Get("提取固有色pass");

            //繪製設定
            var draw = CreateDrawingSettings(shaderTag, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            draw.overrideMaterial = mysetting.mymat;
            draw.overrideMaterialPassIndex = 0;

            //開始繪製(準備好了繪製設定和過濾設定）
            context.DrawRenderers(renderingData.cullResults, ref draw, ref filter);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
    //第二個pass，計算顏色
    class Calculate : ScriptableRenderPass
    {
        SelectOutlineSetting mysetting = null;
        SelectOutline SelectOutline = null;
        struct LEVEL
        {
            public int down;
            public int up;
        };
        LEVEL[] my_level;
        int maxLevel = 16;
        RenderTargetIdentifier sour;
        public Calculate(SelectOutlineSetting setting, SelectOutline render, RenderTargetIdentifier source)
        {
            mysetting = setting;
            SelectOutline = render;
            sour = source;
            my_level = new LEVEL[maxLevel];
            for (int t = 0; t < maxLevel; t++)//申請32個ID，up和down各16個，用這個id去代替臨時RT來使用
            {
                my_level[t] = new LEVEL
                {
                    down = Shader.PropertyToID("_BlurMipDown" + t),
                    up = Shader.PropertyToID("_BlurMipUp" + t)
                };
            }

            if(mysetting.ColorType == TYPE.InColorOn)
            {
                mysetting.mymat.EnableKeyword("_InColorOn");
                mysetting.mymat.DisableKeyword("_InColorOff"); 
            }
            else
            {
                mysetting.mymat.EnableKeyword("_InColorOff");
                mysetting.mymat.DisableKeyword("_InColorOn");
            }
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("顏色計算");
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            
            int SourID = Shader.PropertyToID("_SourTex");
            
            cmd.GetTemporaryRT(SourID, desc);
            cmd.CopyTexture(sour, SourID);
            
            //計算雙重模糊
            int BlurID = Shader.PropertyToID("_BlurTex");
            cmd.GetTemporaryRT(BlurID, desc);
            mysetting.mymat.SetFloat("_Blur", mysetting.blur);

            int width = desc.width / 2;
            int height = desc.height / 2;
            
            int LastDown = SelectOutline.solidcolorID;
            
            for(int t=0; t < mysetting.passloop; t++)
            {
                int midDown = my_level[t].down;//middle down ，間接計算down的ID
                int midUp = my_level[t].up; //middle Up ，間接計算up的ID
                cmd.GetTemporaryRT(midDown, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);//對指定寬高申請RT，每個循環的指定RT都會變小為原來一半
                cmd.GetTemporaryRT(midUp, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);//同上，但是這裡只做申請並未計算，這樣在UP的循環裡就不用申請RT了
                
                cmd.Blit(LastDown, midDown, mysetting.mymat, 1);//計算down的pass
                LastDown = midDown;
                width = Mathf.Max(width / 2, 1);//每次循環都降尺寸
                height = Mathf.Max(height / 2, 1);
            }
            //up
            int lastUp = my_level[mysetting.passloop - 1].down;//把down的最後一次圖像當成up的第一張圖去計算up
            for(int j = mysetting.passloop - 2; j >= 0; j--)//這裡减2是因為第一次已經有了要减去1，但第一次是直接複製的，所以循環完後還得補一次up
            {
                int midUp = my_level[j].up;
                cmd.Blit(lastUp, midUp, mysetting.mymat, 2);
                lastUp = midUp;
            }
            cmd.Blit(lastUp, BlurID, mysetting.mymat, 2);//補一次up，順便模糊
            cmd.Blit(SelectOutline.solidcolorID, sour, mysetting.mymat, 3);//在第4個pass裡面合併所有圖像
            context.ExecuteCommandBuffer(cmd);
            
            //回收
            for(int k = 0; k<mysetting.passloop; k++)
            {
                cmd.ReleaseTemporaryRT(my_level[k].up);
                cmd.ReleaseTemporaryRT(my_level[k].down);
            }

            cmd.ReleaseTemporaryRT(BlurID);
            cmd.ReleaseTemporaryRT(SourID);
            cmd.ReleaseTemporaryRT(SelectOutline.solidcolorID);
            CommandBufferPool.Release(cmd);
        }
    }    
    DrawSolidColorPass m_DrawSoildColorPass;
    Calculate m_Calculate;
    public override void Create()
    {
        m_DrawSoildColorPass = new DrawSolidColorPass(mysetting, this);
        m_DrawSoildColorPass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(mysetting.mymat!=null)
        {
            RenderTargetIdentifier sour = renderer.cameraColorTarget;
            renderer.EnqueuePass(m_DrawSoildColorPass);
            m_Calculate = new Calculate(mysetting, this, sour);
            m_Calculate.renderPassEvent = mysetting.passEvent;
            renderer.EnqueuePass(m_Calculate);
        }
        else
        {
            Debug.LogError("Material not created.");
        }
    }
}