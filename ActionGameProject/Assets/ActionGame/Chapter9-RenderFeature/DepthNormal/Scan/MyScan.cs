using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MyScan : ScriptableRendererFeature
{
    [System.Serializable]
    public class MyScanSetting
    {
        public string passName = "MyScanPass";
        public Material scanMaterial = null;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;
    }
    public MyScanSetting setting = new MyScanSetting();
    class MyScanRenderPass : ScriptableRenderPass
    {
        public string name;
        public Material myScanMaterial;
        public RenderTargetIdentifier passSource{get; set;}
        MyScanVolume myScanVolume;

        public MyScanRenderPass(string tag)
        {
            name = tag;
            var shader = Shader.Find("URPShader/Chapter9/MyScanShader");
            if(shader == null)
            {
                Debug.Log("Shader not find");
                return;
            }
            myScanMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
        public void setup(RenderTargetIdentifier source)
        {
            passSource = source;
        }        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (myScanMaterial == null)
            {
                Debug.LogError("Material not created.");
                return;
            }    
            if (!renderingData.cameraData.postProcessEnabled) return;
            var stack = VolumeManager.instance.stack;
            myScanVolume = stack.GetComponent<MyScanVolume>();
            if(myScanVolume.enableEffect.value)
            {            
                int tempID = Shader.PropertyToID("temp");      
                int scanDistanceID = Shader.PropertyToID("_ScanDistance");
                int scannerPosID = Shader.PropertyToID("_ScannerPos");
                int scanWidthID = Shader.PropertyToID("_ScanWidth");      

                RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
                Camera camera = renderingData.cameraData.camera;
                
                CommandBuffer cmd = CommandBufferPool.Get("MyScanEffect");

                cmd.SetGlobalFloat(scanDistanceID, myScanVolume.scanDistance.value);
                cmd.SetGlobalVector(scannerPosID, myScanVolume.scannerPos.value);
                cmd.SetGlobalFloat(scanWidthID, myScanVolume.scanWidth.value);

                float fov = camera.fieldOfView;
                float near = camera.nearClipPlane;
                //float far = camera.farClipPlane;
                float aspect = camera.aspect;

                float halfHeight = near * Mathf.Tan(Mathf.Deg2Rad * fov * 0.5f);
                Vector3 up = camera.transform.up * halfHeight;
                Vector3 right = camera.transform.right * halfHeight * aspect;//aspect:寬高比 = 寬/高 => 寬 = aspect * height(高)
                Vector3 forward = camera.transform.forward * near;

                Vector3 topLeft = forward + up - right;
                float scale = topLeft.magnitude / camera.nearClipPlane;
                topLeft.Normalize();
                topLeft *= scale;

                Vector3 topRight = forward + up + right;
                topRight.Normalize();
                topRight *= scale;

                Vector3 buttomLeft = forward - up - right;
                buttomLeft.Normalize();
                buttomLeft *= scale;

                Vector3 buttomRight = forward - up + right;
                buttomRight.Normalize();
                buttomRight *= scale;

                Matrix4x4 matrix = new Matrix4x4();
                matrix.SetRow(0, buttomLeft);
                matrix.SetRow(1, buttomRight);
                matrix.SetRow(2, topRight);
                matrix.SetRow(3, topLeft);

                myScanMaterial.SetMatrix("Matrix", matrix);

                cmd.GetTemporaryRT(tempID, desc);
                cmd.Blit(passSource, tempID, myScanMaterial);
                cmd.Blit(tempID, passSource);

                context.ExecuteCommandBuffer(cmd);
                cmd.ReleaseTemporaryRT(tempID);
                CommandBufferPool.Release(cmd);                
            }
        }        
    }

    MyScanRenderPass myScanPass;

    public override void Create()
    {
        myScanPass = new MyScanRenderPass(this.name);
        myScanPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        myScanPass.setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(myScanPass);
    }
}


