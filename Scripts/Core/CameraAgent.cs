using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAgent : MonoBehaviour
{
    [SerializeField] Camera m_TargetCamera;
    [SerializeField] float m_BoardUnit;

    // Start is called before the first frame update
    void Start()
    {
        //해상도 변경에 대응
        //m_BoardUnit의 외부 입력값을 이용하여 실행시 orthographicSize를 재계산하여 대응)
        m_TargetCamera.orthographicSize = m_BoardUnit / m_TargetCamera.aspect;   
    }
}
