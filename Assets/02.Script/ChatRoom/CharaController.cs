using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CharaController : MonoBehaviourPunCallbacks
{
    [Header("MoveMent")]
    public float moveSpeed = 3.5f;
    public TextMeshProUGUI nickTag;

    private Rigidbody rBody;
    private bl_Joystick joyStick;
    private float h, v;
    private bool isMove = false;

    private float turnSmoothVel;
    private float turnSmoothTime = 0.3f;
    private Animator anim;

    private readonly int hashMove = Animator.StringToHash("isMove");


    private void Start()
    {
        if(photonView.IsMine)
        {
            joyStick = GameObject.FindWithTag(Tags.joyStickTag).GetComponent<bl_Joystick>();
            rBody = gameObject.GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();

            Camera.main.GetComponent<CameraRotate>().player = transform;
        }

        nickTag.text = photonView.Owner.NickName;
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            h = joyStick.Horizontal;
            v = joyStick.Vertical;

            if (!joyStick.IsFree)
            {
                if(h != 0 || v != 0)
                {
                    //string debugTextA = string.Format("{0:0.#} / {1:0.#}", h,v);
                    //Debug.Log(debugTextA);

                    transform.localEulerAngles = new Vector3(0, Mathf.Atan2(h, v) * Mathf.Rad2Deg +
                                                             Camera.main.transform.eulerAngles.y, 0);

                    isMove = true;
                    anim.SetBool(hashMove, true);
                }
            }
            else
            {
                isMove = false;
                anim.SetBool(hashMove, false);
            }
        }
    }

    private void FixedUpdate()
    {
        if(photonView.IsMine)
        {
            if(isMove)
            {
                rBody.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
            }
        }
    }
}
