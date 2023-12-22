#if UNITY_URP
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace AEG.DLSS
{
    public class DLSSRenderPass : ScriptableRenderPass
    {
        private CommandBuffer cmd;

        private DLSS_URP m_dlssURP;

        public DLSSRenderPass(DLSS_URP _fsrURP) {
            renderPassEvent = RenderPassEvent.AfterRendering + 5;
            //renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            m_dlssURP = _fsrURP;
        }

        public void OnSetReference(DLSS_URP _fsrURP) {
            m_dlssURP = _fsrURP;
        }

        // The actual execution of the pass. This is where custom rendering occurs.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
#if AEG_DLSS && UNITY_STANDALONE_WIN && UNITY_64
            ref CameraData cameraData = ref renderingData.cameraData;
#if UNITY_EDITOR
            if(cameraData.isPreviewCamera || cameraData.isSceneViewCamera) {
                return;
            }
            if(cameraData.camera.GetComponent<DLSS_URP>() == null) {
                return;
            }
#endif
            if(!cameraData.resolveFinalTarget) {
                return;
            }

            if(m_dlssURP == null) {
                return;
            }

            //Debug.Log(m_dlssURP.m_colorBuffer);
            if(m_dlssURP.m_depthBuffer != null && m_dlssURP.m_motionVectorBuffer != null) {
                m_dlssURP.CameraGraphicsOutput = cameraData.cameraTargetDescriptor.graphicsFormat;

                cmd = CommandBufferPool.Get();

                m_dlssURP.state.CreateContext(m_dlssURP.dlssData, cmd, true);
                m_dlssURP.state.UpdateDispatch(m_dlssURP.m_colorBuffer, m_dlssURP.m_depthBuffer, m_dlssURP.m_motionVectorBuffer, null, m_dlssURP.m_dlssOutput, cmd);

#if UNITY_2022_1_OR_NEWER
                Blitter.BlitCameraTexture(cmd, m_dlssURP.m_dlssOutput, cameraData.renderer.cameraColorTargetHandle, new Vector4(1, -1, 0, 1), 0, false);
#else
                Blit(cmd, m_dlssURP.m_dlssOutput, cameraData.renderer.cameraColorTarget);
#endif
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
#endif
        }
    }

    public class DLSSBufferPass : ScriptableRenderPass
    {
        private DLSS_URP m_dlssURP;

#if !UNITY_2022_1_OR_NEWER
        private CommandBuffer cmd;
#endif

        private int depthTexturePropertyID = Shader.PropertyToID("_CameraDepthTexture");
        private int motionTexturePropertyID = Shader.PropertyToID("_MotionVectorTexture");

        public DLSSBufferPass(DLSS_URP _dlssURP) {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            ConfigureInput(ScriptableRenderPassInput.Depth);
            m_dlssURP = _dlssURP;
        }

        //2022 and up
        public void Setup() {
#if AEG_DLSS && UNITY_STANDALONE_WIN && UNITY_64
            if(!Application.isPlaying) {
                return;
            }
            if(m_dlssURP == null) {
                return;
            }

            m_dlssURP.m_depthBuffer = Shader.GetGlobalTexture(depthTexturePropertyID);
            m_dlssURP.m_motionVectorBuffer = Shader.GetGlobalTexture(motionTexturePropertyID);
#endif
        }

        public void OnSetReference(DLSS_URP _dlssURP) {
            m_dlssURP = _dlssURP;
        }

        // The actual execution of the pass. This is where custom rendering occurs.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
#if AEG_DLSS && UNITY_STANDALONE_WIN && UNITY_64
            ref CameraData cameraData = ref renderingData.cameraData;
#if UNITY_EDITOR
            if(cameraData.isPreviewCamera || cameraData.isSceneViewCamera) {
                return;
            }
            if(cameraData.camera.GetComponent<DLSS_URP>() == null) {
                return;
            }
#endif

            if(m_dlssURP == null) {
                return;
            }
            if(!cameraData.resolveFinalTarget) {
                return;
            }

#if UNITY_2022_1_OR_NEWER
            m_dlssURP.m_colorBuffer = cameraData.renderer.cameraColorTargetHandle;
#else
            cmd = CommandBufferPool.Get();

            Blit(cmd, cameraData.renderer.cameraColorTarget, m_dlssURP.m_colorBuffer);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            m_dlssURP.m_depthBuffer = Shader.GetGlobalTexture(depthTexturePropertyID);
            m_dlssURP.m_motionVectorBuffer = Shader.GetGlobalTexture(motionTexturePropertyID);
#endif

#endif
        }
    }
}
#endif