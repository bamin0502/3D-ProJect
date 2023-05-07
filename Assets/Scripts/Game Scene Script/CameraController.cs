using UnityEngine;

public class CameraController : MonoBehaviour
{
    //임성훈
    
    public Camera mainCamera; //메인 카메라
    public Camera minimapCamera; //미니맵 카메라

    public float cameraMoveSpeed = 30f; //카메라 속도
    public Transform player; //플레이어 위치

    private bool _isPlayerFollow = true; //플레이어 따라다니게 할지 여부
    
    private int _currentStep = 2; //현재 줌 단계
    private Vector3 _targetOffset; //목표 오프셋
    private Vector3 _currentOffset; //현재 오프셋

    private float _currentZoom; // 현재 확대값
    private float _targetAngle; // 목표 각도
    private float _currentAngle; // 현재 각도

    private void Start()
    {
        UpdateStep(0);
    }

    

    void Update()
    {
        Vector3 newPosition = gameObject.transform.position;
        if (minimapCamera.transform.position != newPosition)
        {
            newPosition.y = 60f;
            minimapCamera.transform.position = newPosition;
        }
        
        if (_isPlayerFollow)
        {
            var position = player.position;
            
            gameObject.transform.position = position; // 플레이어 따라다니게
            mainCamera.transform.position = position + _currentOffset;
        }
        if (Input.GetKeyDown(KeyCode.Space)) _isPlayerFollow = true; // 스페이스바 누르면 플레이어 위치로
        float scroll = Input.GetAxis("Mouse ScrollWheel"); //휠 입력 받음
        CameraMove(); // 화살표로 카메라 이동
        ChangeStep(scroll); // 휠로 카메라 줌
        UpdateStep(scroll); // 줌에 따른 카메라 위치, 각도 조정
    }

    private void CameraMove()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            _isPlayerFollow = false;
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(horizontal, 0, vertical);
            transform.Translate(movement * (cameraMoveSpeed * Time.deltaTime), Space.World);
        }
    }

    private void ChangeStep(float scroll)
    {
        if (scroll > 0)
        {
            _currentStep++;
            if (_currentStep > 3)
            {
                _currentStep = 3;
            }
        }
        else if (scroll < 0)
        {
            _currentStep--;
            if (_currentStep < 1)
            {
                _currentStep = 1;
            }
        }
    }

    private void UpdateStep(float scroll)
    {
        if (_isPlayerFollow)
        {
            switch (_currentStep)
            {
                case 1:
                    _targetAngle = 45.0f;
                    _targetOffset = new Vector3(0, 30, -30);
                    break;
                case 2:
                    _targetAngle = 45.0f;
                    _targetOffset = new Vector3(0, 15, -15);
                    break;
                case 3:
                    _targetAngle = 30.0f;
                    _targetOffset = new Vector3(0, 6, -10);
                    break;
            }

            _currentAngle = Mathf.Lerp(_currentAngle, _targetAngle, Time.deltaTime * 10f);
            _currentOffset = Vector3.Lerp(_currentOffset, _targetOffset, Time.deltaTime * 10f);

            Quaternion targetRotation = Quaternion.Euler(_currentAngle, 0, 0);
            mainCamera.transform.rotation = targetRotation;
        }
        else
        {
            if (scroll != 0)
            {
                Vector3 position = mainCamera.transform.position;

                float targetZoom = position.y - scroll * cameraMoveSpeed;
                targetZoom = Mathf.Clamp(targetZoom, 7f, 29f);
                position = new Vector3(position.x, targetZoom, position.z);
                mainCamera.transform.position = position;
            }
        }
    }
}
