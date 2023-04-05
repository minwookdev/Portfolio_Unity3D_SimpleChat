using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Text msgList;
    public TextMeshProUGUI clientState;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI peopleInfo;
    public InputField sendMsg;
    public ScrollRect chatScrollRect;
    public Transform[] spawnPos;

    private string randomChara = "P_Chara_";

    private void Start()
    {
        CreateCube();
        //Lobby에서 Chatting Scene으로 입장할 때 끊었던 데이터 통신을 다시 연결시킵니다.
        PhotonNetwork.IsMessageQueueRunning = true;

        //방이름과 현재 인원수를 체크합니다.
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        peopleInfo.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + 
                          PhotonNetwork.CurrentRoom.MaxPlayers;

        //기존에 채팅기록이 남아있던 현상이 있어서 Chattting Text 를 지워줍니다
        msgList.text = "";
    }

    private void CreateCube()
    {
        int randomSpawnChara = Random.Range(1, 12);

        if(randomSpawnChara > 9)
        {
            randomChara += randomSpawnChara.ToString();
        }
        else
        {
            randomChara += "0" + randomSpawnChara.ToString(); 
        }

        int index = Random.Range(0, spawnPos.Length);
        PhotonNetwork.Instantiate(randomChara, spawnPos[index].position, Quaternion.identity);
    }

    private void Update()
    {
        //클라이언트의 접속 상태를 갱신
        clientState.text = PhotonNetwork.NetworkClientState.ToString();

        //채팅창 클리어기능 TEST
        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    msgList.text = "";
        //}
    }

    #region Room Function

    //Disconnect Button
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }

    //새로운 유저가 방에 입장시 호출되는 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ReceiveMsg("<color=yellow>" + newPlayer.NickName + "님이 입장하셨습니다 ! </color>");
        //방 인원수 갱신해줍니다
        CurrentRoomPeopleUpdate();
    }

    //유저가 방에서 나갔을 때 호출되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ReceiveMsg("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다 ! </color>");
        //방 인원수 갱신
        CurrentRoomPeopleUpdate();
    }

    //현재 방의 인원수 갱신함수
    public void CurrentRoomPeopleUpdate()
    {
        peopleInfo.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " +
                  PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    #endregion

    #region Chatting Function

    //Message를 RPC의 버퍼에 보내준다.
    public void OnSendChatMsg()
    {
        //SendMsg의 Text가 Null이라면 전송하지 않는다.
        if(sendMsg.text.Trim().Length == 0 || sendMsg.text.Length > 50)
        {
            Debug.Log("공백, 혹은 50자가 넘도록 입력하여 출력되지 않았습니다. :: <color=yellow>GameManager</color>");
            sendMsg.text = "";
            return;
        }

        string msg = string.Format(" [{0}] : {1} ",
                                    PhotonNetwork.NickName,
                                    sendMsg.text);
        photonView.RPC("ReceiveMsg", RpcTarget.Others, msg);

        sendMsg.text = "";
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

    #endregion
}
