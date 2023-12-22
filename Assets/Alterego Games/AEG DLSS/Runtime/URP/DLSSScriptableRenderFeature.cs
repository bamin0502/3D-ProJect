#if UNITY_URP
using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace AEG.DLSS
{
    public class DLSSScriptableRenderFeature : ScriptableRendererFeature
    {
        [HideInInspector]
        public bool IsEnabled = false;

        private DLSS_URP m_fsrURP;

        private DLSSBufferPass fsrBufferPass;
        private DLSSRenderPass fsrRenderPass;

        private CameraData cameraData;

        public void OnSetReference(DLSS_URP _fsrURP) {
            m_fsrURP = _fsrURP;
            fsrBufferPass.OnSetReference(m_fsrURP);
            fsrRenderPass.OnSetReference(m_fsrURP);
        }

        public override void Create() {
            // Pass the settings as a parameter to the constructor of the pass.
            fsrBufferPass = new DLSSBufferPass(m_fsrURP);
            fsrRenderPass = new DLSSRenderPass(m_fsrURP);

            fsrBufferPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }

        public void OnDispose() {
        }

#if UNITY_2022_1_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
            fsrBufferPass.Setup();
        }
#endif

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if(!IsEnabled) {
                return;
            }
#if UNITY_EDITOR
            name = "DLSSScriptableRenderFeature";
            cameraData = renderingData.cameraData;

            if(cameraData.isPreviewCamera || renderingData.cameraData.isSceneViewCamera) {
                return;
            }
            if(cameraData.camera.GetComponent<DLSS_URP>() == null) {
                return;
            }
#endif

            if(!Application.isPlaying) {
                return;
            }
            if(m_fsrURP == null) {
                return;
            }

            // Here you can queue up multiple passes after each other.
            renderer.EnqueuePass(fsrBufferPass);
            renderer.EnqueuePass(fsrRenderPass);
        }
    }
}
#endif