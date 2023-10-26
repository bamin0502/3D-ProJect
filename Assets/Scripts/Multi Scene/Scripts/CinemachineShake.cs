using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }
    [SerializeField]
    private CinemachineFreeLook cinemachineFreeLook;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;
    private void Awake()
    {
        Instance = this;
        cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
    }                                                   
    public void ShakeCamera(float intensity, float time)
    {
       // CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
       //     cinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
       //
       // cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
       // shakeTimer = time;
       if (cinemachineFreeLook == null)
       {
           Debug.LogError("CinemachineFreeLook is not assigned.");
           return;
       }

       var rig = cinemachineFreeLook.GetRig(0);
       if (rig == null)
       {
           Debug.LogError("Rig is not found.");
           return;
       }

       CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = rig.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
       if (cinemachineBasicMultiChannelPerlin == null)
       {
           Debug.LogError("CinemachineBasicMultiChannelPerlin is not found.");
           return;
       }

       cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
       shakeTimer = time;
    }
    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
                 CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
                     cinemachineFreeLook.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                 
                 cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 
                    Mathf.Lerp(startingIntensity, 0f, shakeTimer / shakeTimerTotal);
        }
         
    }
}
