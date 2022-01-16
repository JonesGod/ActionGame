using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthNormalScan : ScriptableRendererFeature
{
    public enum ax
    {
        X, Y, Z
    }

    [System.Serializable]
    public class setting
    {
        public Material Scanmat = null;
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingTransparents;
        [ColorUsage(true,true)]public Color ColorX = Color.white;
        [ColorUsage(true,true)]public Color ColorY = Color.white;
        [ColorUsage(true,true)]public Color ColorZ = Color.white;
        [ColorUsage(true,true)]public Color ColorEdge = Color.white;
        [ColorUsage(true,true)]public Color ColorOutline = Color.white;
        [Range(0,0.2f)]public float Width = 0.1f;//線框寬度
        [Range(0.1f, 10)] public float Spacing = 1;//線框間距
        [Range(0, 10)] public float Speed = 1; //滾動速度
        [Range(0, 5)] public float SampleDistance = 1; //邊緣採樣半徑
        [Range(0, 5)] public float NormalSensitivity = 1;
        [Range(0, 5)] public float DepthSensitivity = 1;
        public ax AXIS; //方向
    }
    public setting mysetting = new setting();
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material Scanmat = null;
        RenderTargetIdentifier source;
        public Color ColorX = Color.white;
        public Color ColorY = Color.white;
        public Color ColorZ = Color.white;
        public Color ColorEdge = Color.white;
        public Color ColorOutline = Color.white;
        public float Width = 0.05f;
        public float Spacing = 2;
        public float Speed = 0.7f;
        public float SampleDistance = 1;
        public float NormalSensitivity = 1;
        public float DepthSensitivity = 1;
        public ax AXIS;
        public void set(RenderTargetIdentifier source)
        {
            this.source = source;            
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Scanmat == null)
            {
                Debug.LogError("Material not created.");
                return;
            }    
            Scanmat.SetColor("_ColorX", ColorX);
            Scanmat.SetColor("_ColorY", ColorY);
            Scanmat.SetColor("_ColorZ", ColorZ);
            Scanmat.SetColor("_ColorEdge", ColorEdge);
            Scanmat.SetColor("_OutlineColor", ColorOutline);
            Scanmat.SetFloat("_width", Width);
            Scanmat.SetFloat("_Spacing", Spacing);
            Scanmat.SetFloat("_Speed", Speed);
            Scanmat.SetFloat("_SampleDistance", SampleDistance);
            Scanmat.SetFloat("_NormalSensitivity", NormalSensitivity);
            Scanmat.SetFloat("_DepthSensitivity", DepthSensitivity);
            
            if (AXIS == ax.X)
                {
                    Scanmat.DisableKeyword("_AXIS_Y");
                    Scanmat.DisableKeyword("_AXIS_Z");
                    Scanmat.EnableKeyword("_AXIS_X");
                }
            else if (AXIS == ax.Y)
                {
                    Scanmat.DisableKeyword("_AXIS_Z");
                    Scanmat.DisableKeyword("_AXIS_X");
                    Scanmat.EnableKeyword("_AXIS_Y");
                }
            else
                {
                    Scanmat.DisableKeyword("_AXIS_X");
                    Scanmat.DisableKeyword("_AXIS_Y");
                    Scanmat.EnableKeyword("_AXIS_Z");
                }
                
            int temp = Shader.PropertyToID("temp");
            CommandBuffer cmd = CommandBufferPool.Get("ScanEffect");

            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            Camera cam = renderingData.cameraData.camera;

            float height = cam.nearClipPlane * Mathf.Tan(Mathf.Deg2Rad * cam.fieldOfView * 0.5f);
            Vector3 up = cam.transform.up * height;
            Vector3 right = cam.transform.right * cam.aspect * height;//aspect:寬高比 = 寬/高 => 寬 = aspect * height(高)
            Vector3 forward = cam.transform.forward * cam.nearClipPlane;
            Vector3 ButtomLeft = forward - right - up;
            
            float scale = ButtomLeft.magnitude / cam.nearClipPlane;
            
            ButtomLeft.Normalize();
            ButtomLeft *= scale;
            
            Vector3 ButtomRight = forward + right-up;
            ButtomRight.Normalize();
            ButtomRight *= scale;
            
            Vector3 TopRight = forward + right + up;
            TopRight.Normalize();
            TopRight *= scale;
            
            Vector3 TopLeft = forward - right + up;
            TopLeft.Normalize();
            TopLeft *= scale;
            
            Matrix4x4 MATRIX = new Matrix4x4();
            MATRIX.SetRow(0, ButtomLeft);
            MATRIX.SetRow(1, ButtomRight);
            MATRIX.SetRow(2, TopRight);
            MATRIX.SetRow(3, TopLeft);    
                    
            Scanmat.SetMatrix("Matrix", MATRIX);
            
            cmd.GetTemporaryRT(temp, desc);
            cmd.Blit(source, temp, Scanmat);
            cmd.Blit(temp, source);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.ReleaseTemporaryRT(temp);
            CommandBufferPool.Release(cmd);
        }
    }
    CustomRenderPass m_ScriptablePass;
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();
        m_ScriptablePass.Scanmat = mysetting.Scanmat;
        m_ScriptablePass.renderPassEvent = mysetting.Event;
        m_ScriptablePass.ColorX = mysetting.ColorX;
        m_ScriptablePass.ColorY = mysetting.ColorY;
        m_ScriptablePass.ColorZ = mysetting.ColorZ;
        m_ScriptablePass.ColorEdge = mysetting.ColorEdge;
        m_ScriptablePass.ColorOutline = mysetting.ColorOutline;
        m_ScriptablePass.Width = mysetting.Width;
        m_ScriptablePass.Spacing = mysetting.Spacing;
        m_ScriptablePass.Speed = mysetting.Speed;
        m_ScriptablePass.SampleDistance = mysetting.SampleDistance;
        m_ScriptablePass.NormalSensitivity = mysetting.NormalSensitivity;
        m_ScriptablePass.DepthSensitivity = mysetting.DepthSensitivity;
        m_ScriptablePass.AXIS = mysetting.AXIS;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.set(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}