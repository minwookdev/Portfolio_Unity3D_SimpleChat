using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HidingChatField : MonoBehaviour
{
    public GameObject chatField;
    public GameObject openChatFieldBtn;

    public void OnHideFieldBtnDown()
    {
        chatField.transform.localScale = new Vector3(0f, 0f, 0f);
        openChatFieldBtn.SetActive(true);
    }

    public void OnOpenFieldBtnDown()
    {
        chatField.transform.localScale = new Vector3(1f, 1f, 1f);
        openChatFieldBtn.SetActive(false);
    }
}
