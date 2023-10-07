using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.Timeline;

public class SubtitleMixerBehaviour : PlayableBehaviour
{
    string subtitleKey;
    TextMeshProUGUI textMeshPro;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        textMeshPro = playerData as TextMeshProUGUI;
        if (textMeshPro == null) return;

        int inputCount = playable.GetInputCount();
        bool isOnClip = false;

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight == 1)
            {
                isOnClip = true;

                ScriptPlayable<SubtitleBehaviour> subtitlePlayable = (ScriptPlayable<SubtitleBehaviour>)playable.GetInput(i);
                SubtitleBehaviour subtitleBehaviour = subtitlePlayable.GetBehaviour();

                // Clip change detected
                if (subtitleKey != subtitleBehaviour.subtitleKey)
                {
                    subtitleKey = subtitleBehaviour.subtitleKey;

                    if (Application.isPlaying)
                    {
                        textMeshPro.text = subtitleKey;
                    }
                    else if (Application.isEditor)
                    {
                        textMeshPro.text = subtitleKey;
                    }
                }
            }
        }

        // Passing empty section
        if (isOnClip == false)
        {
            textMeshPro.text = "";
            subtitleKey = "";
        }
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        if(textMeshPro == null) return;
        textMeshPro.text = "";
    }    
}
