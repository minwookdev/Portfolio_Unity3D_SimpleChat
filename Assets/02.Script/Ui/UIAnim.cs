using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnim : MonoBehaviour
{
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        Animator anim = GetComponent<Animator>();

        anim.SetBool("isOpen", true);
    }

    public void OnJoinBtnClick()
    {
        //this.gameObject.SetActive(false);

        Animator anim = GetComponent<Animator>();

        anim.SetBool("isOpen", true);

        //if()
    }

    public void OnDisabledSelf()
    {
        this.gameObject.SetActive(false);
    }
}
