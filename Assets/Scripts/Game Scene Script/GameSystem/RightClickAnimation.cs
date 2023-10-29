using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickAnimation : MonoBehaviour
{
    //임성훈
    
    public GameObject clickAnimationPrefab; // 클릭 애니메이션 프리팹
    public int poolSize = 20; // 풀 크기
    public float animationDuration = 1.5f; // 애니메이션 지속 시간 (초)
    private Camera _camera;
    private readonly Queue<GameObject> _clickAnimationPool = new (); // 오브젝트 풀

    private void Start()
    {
        // 오브젝트 풀 초기화
        for (int i = 0; i < poolSize; i++)
        {
            GameObject clickAnimation = Instantiate(clickAnimationPrefab, gameObject.transform, true);
            clickAnimation.SetActive(false);
            _clickAnimationPool.Enqueue(clickAnimation);
        }
        
        _camera = Camera.main;
    }

    private void Update()
    {
        if (!MultiScene.Instance.isDead && Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider.gameObject.CompareTag("Ground")) // 클릭된 위치가 지면일 경우
                {
                    StartCoroutine(PlayClickAnimation(hit.point)); // 클릭한 위치에 애니메이션 생성
                }
            }
        }
    }

    private IEnumerator PlayClickAnimation(Vector3 position)
    {
        //풀에서 오브젝트 생성 및 해제
        var clickAnimation = _clickAnimationPool.Count > 0 ? _clickAnimationPool.Dequeue() : Instantiate(clickAnimationPrefab);

        position.y += 0.3f;
        clickAnimation.transform.position = position;
        clickAnimation.SetActive(true);


        yield return new WaitForSeconds(animationDuration);

        clickAnimation.SetActive(false);

        if (_clickAnimationPool.Count < poolSize)
        {
            _clickAnimationPool.Enqueue(clickAnimation);
        }
        else
        {
            Destroy(clickAnimation);
        }
    }
}
