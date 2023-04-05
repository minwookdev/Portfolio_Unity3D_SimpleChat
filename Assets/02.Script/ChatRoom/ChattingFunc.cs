using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ChattingFunc : MonoBehaviourPunCallbacks
{
    public Text msgList;
    public InputField sendMsg;
    public ScrollRect chatScrollRect;

    //Message를 RPC의 버퍼에 보내준다.
    public void OnSendChatMsg()
    {
        string msg = string.Format(" [{0}] : {1} ",
                                    PhotonNetwork.NickName,
                                    sendMsg.text);
        photonView.RPC("ReciveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
    }

    [PunRPC] //RPC는 플레이어가 속해있는 방의 모든 인원에게 전달한다
    void ReceiveMsg(string msg)
    {
        msgList.text += "\n" + msg;
        //StartCoroutine(ChatUpdate());
    }

    IEnumerator ChatUpdate()
    {
        //채팅이 올라오는 속도보다 빠르게 Vertical을 맞추는 현상이 있었기 떄문에
        //어거지로 맞추었따
        yield return new WaitForSeconds(0.1f);

        chatScrollRect.verticalNormalizedPosition = 0.0f;
    }
}
