using UnityEngine;

public class Fire : MonoBehaviour
{
    
    public float maxDistance = 3f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlaySound();
        }
    }

    private void PlaySound()
    {
       SoundManager.instance.PlaySE("Fire");
    }

}
