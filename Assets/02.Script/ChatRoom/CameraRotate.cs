using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField]
    DitzeGames.MobileJoystick.TouchField touchField;

    public float cameraAngle;
    public Transform player;
    public Vector3 cameraPos = new Vector3(0, 3,-4);
    [Range(0f, 1.0f)] public float cameraRotateSpeed;

    void LateUpdate()
    {
        cameraAngle += touchField.TouchDist.x;

        transform.position = player.position + Quaternion.AngleAxis(cameraAngle * cameraRotateSpeed, Vector3.up) * cameraPos;
        transform.rotation = Quaternion.LookRotation(player.position + Vector3.up * 1f - transform.position, Vector3.up);
    }
}
