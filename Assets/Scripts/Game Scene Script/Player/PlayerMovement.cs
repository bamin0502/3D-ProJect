using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    //임성훈
    
    private NavMeshAgent _navAgent;

    private Camera _camera;

    private void Start()
    {
        Managers mag = Managers.GetInstance();//방민호
        _navAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                // 이동할 위치로 플레이어를 이동
                _navAgent.SetDestination(hit.point);
            }
        }
    }
}
