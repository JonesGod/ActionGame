using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ToonHairShadowRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class ToonHairShadowSetting//定義一個設置的子類Setting，用來定義自己的参數
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        public LayerMask hairLayer;
        public LayerMask faceLayer;
        [Range(1000,5000)]public int QueueMin = 2000;
        [Range(1000,5000)]public int QueueMax = 2500;
        public Material passMaterial = null;  
        //public string[] PassNames;
    }
    public ToonHairShadowSetting setting = new ToonHairShadowSetting();    
    class CustomRenderPass : ScriptableRenderPass
    {        
        public int solidColorID = 0;
        public ShaderTagId shaderTag = new ShaderTagId("UniversalForward");
        public ToonHairShadowSetting setting;
        FilteringSettings filtering;
        FilteringSettings filtering2;        
        public CustomRenderPass(ToonHairShadowSetting setting)//建構函式
        {
            this.setting = setting;

            //創建queue以用於兩個FilteringSettings的賦值
            RenderQueueRange queue = new RenderQueueRange();
            queue.lowerBound = Mathf.Min(setting.QueueMax, setting.QueueMin);    
            queue.upperBound = Mathf.Max(setting.QueueMax, setting.QueueMin);
            filtering = new FilteringSettings(queue, setting.hairLayer);  
            filtering2 = new FilteringSettings(queue, setting.faceLayer);  
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            int temp = Shader.PropertyToID("_HairSolidColor");
            //use the same settings as the camera texture
            RenderTextureDescriptor desc = cameraTextureDescriptor;
            cmd.GetTemporaryRT(temp, desc);
            solidColorID = temp;            
            //set the RT as Render Target
            ConfigureTarget(temp);
            //clear the RT
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //第一個Pass,頭髮純色
            //繪製設定
            var draw = CreateDrawingSettings(shaderTag, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            draw.overrideMaterial = setting.passMaterial;
            draw.overrideMaterialPassIndex = 0;
            //開始繪製
            context.DrawRenderers(renderingData.cullResults, ref draw, ref filtering);

            // //第二個Pass,臉部深度
            // //繪製設定
            // var draw2 = CreateDrawingSettings(shaderTag, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            // draw.overrideMaterial = setting.passMaterial;
            // draw.overrideMaterialPassIndex = 1;
            // //開始繪製
            // context.DrawRenderers(renderingData.cullResults, ref draw, ref filtering);
        }        
    }

    CustomRenderPass m_ScriptablePass;
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(setting);
        m_ScriptablePass.renderPassEvent = setting.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


