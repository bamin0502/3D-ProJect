using UnityEngine;

public class Missile : MonoBehaviour
{
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right * (30 * Time.deltaTime));
    }
}
