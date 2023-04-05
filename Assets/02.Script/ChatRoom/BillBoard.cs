using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private void LateUpdate()
    {
        //닉네임 보드는 계속 메인카메라를 바라봄
        transform.LookAt(Camera.main.transform.position);
    }
}
