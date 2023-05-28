using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    
    public float maxDistance = 3f;
    private bool isPlayerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            PlaySound();
        }
    }

    private void PlaySound()
    {
       SoundManager.instance.PlaySE("Fire");
    }

}
