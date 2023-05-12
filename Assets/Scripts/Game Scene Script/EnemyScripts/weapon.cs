using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//둘다 Weapon으로 class가 겹쳐서 임시로 소문자 w으로 바꿈 알아서 보고 병합하든가 바꾸세요
public class weapon : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Type { Hammer, Range, Arrow };
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider hammerArea;
    public TrailRenderer trailEffect;

    public void Use()
    {
        if (type == Type.Hammer)
        {
            StartCoroutine("Swing");
        }
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        hammerArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        hammerArea.enabled = false;


        yield return new WaitForSeconds(0.3f);
        hammerArea.enabled = false;
    }
}
