using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    public enum ActivePanel
    {
        LOGIN = 0,
        ROOMS = 1,
        SELECTROOM = 2
    }

    public ActivePanel activePanel = ActivePanel.LOGIN;

    private const string gameVersion = "1.3";
    public string userId = "COMPANY";
    public byte maxPlayer = 4;

    public TMP_InputField userIdText;
    public TMP_InputField roomNameText;
    public TextMeshProUGUI errorText = null;
    public TextMeshProUGUI versionText = null;
    public TextMeshProUGUI nickNameText = null;
    public TextMeshProUGUI clientStateText = null;
    public TextMeshProUGUI lobbyInfoText = null;

    [Header("Room Information")]
    public GameObject roomInfoObject;
    public Transform roomInfoGrid;

    public GameObject[] lobbyPanels;

    private void Awake()
    {
        //같은 방을 동기화 한다고 한다.
        PhotonNetwork.AutomaticallySyncScene = true;

        //현재 게임을 실행한 장치알아오기
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            Debug.Log("Current Device : Desktop :: <color=yellow>PhotonInit</color>");
            Screen.SetResolution(1280, 720, false);
        }
        else if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            Debug.Log("Current Device : Mobile :: <color=yellow>PhotonInit</color>");
            Screen.fullScreen = true;
        }
    }

    private void Start()
    {
        versionText.text = gameVersion + "v";
    }

    private void Update()
    {
        //현재 클라이언트의 접속 상태를 계속 갱신합니다
        clientStateText.text = PhotonNetwork.NetworkClientState.ToString();
        //로비인원수, 총 접속 인원수를 계속 갱신합니다
        lobbyInfoText.text = "Lobby " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + " / " +
                             "Total " + (PhotonNetwork.CountOfPlayers);

    }

    #region SELF_CALLBACK_FUNC

    public void OnLogin()
    {
        //닉네임이 Null이거나 여섯글자가 남어갈 경우 에러메세지 출력하고 return
        if(userIdText.text == "")
        {
            StartCoroutine(FadeTextToFull(3.0f, errorText, "Please Entering NickName !"));
            return;
        }

        if(userIdText.text.Length > 6)
        {
            StartCoroutine(FadeTextToFull(3.0f, errorText, "The NickName can be a Maximum of Six Characters"));
            return;
        }

        StartCoroutine(FadeTextToFull(3.0f, errorText, "Now Entering Server..."));

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.NickName = userIdText.text;

        PhotonNetwork.ConnectUsingSettings();

        PlayerPrefs.SetString("USER_ID", PhotonNetwork.NickName);
    }

    public void OnCreateRoomBtnDown()
    {
        PhotonNetwork.CreateRoom("ROOM_" + Random.Range(0, 99),
                                 new RoomOptions { MaxPlayers = this.maxPlayer });

        StartCoroutine(FadeTextToFull(3.0f, errorText, "Now Creating New Room..."));
    }

    public void OnJoinRoomBtnDown()
    {
        PhotonNetwork.JoinRandomRoom();

        StartCoroutine(FadeTextToFull(3.0f, errorText, "Now Searching Room..."));
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("Connect To Server !");
        nickNameText.text = PlayerPrefs.GetString("USER_ID");
        StartCoroutine(FadeTextToFull(3.0f, errorText, "Success Join to Server"));
    }

    public override void OnJoinedLobby()
    {
        ChangePanel(ActivePanel.ROOMS);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        StartCoroutine(FadeTextToFull(3.0f, errorText, "Room Not Found !, Creating New Room"));
        PhotonNetwork.CreateRoom("ROOM_" + Random.Range(0, 99), new RoomOptions { MaxPlayers = this.maxPlayer });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("On Joined Room !");
        StartCoroutine(FadeTextToFull(3.0f, errorText, "Joined Success !"));

        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadScene(1);
    }

    public override void OnRoomListUpdate(List<Photon.Realtime.RoomInfo> roomList)
    {
        //base.OnRoomListUpdate(roomList);

        //방을 선택하는 Panel에서만 방 목록을 체크합니다
        //if (activePanel != ActivePanel.SELECTROOM)
        //    return;

        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("RoomInfo"))
        {
            Destroy(obj);
        }
        foreach(RoomInfo roomInfo in roomList)
        {
            GameObject _room = Instantiate(roomInfoObject, roomInfoGrid);
            RoomData roomData = _room.GetComponent<RoomData>();
            EventTrigger trigger = roomData.gameObject.GetComponentInChildren<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            roomData.roomName = roomInfo.Name;
            roomData.maxPlayer = roomInfo.MaxPlayers;
            roomData.playerCount = roomInfo.PlayerCount;
            roomData.UpdateRoomInfo();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => { OnClickRoom((PointerEventData)data, roomData.roomName); });
            trigger.triggers.Add(entry);

            //기존 Button을 사용한 Event처리
            //roomData.gameObject.GetComponentInChildren<Button>().onClick.AddListener
            //    (
            //        delegate
            //        {
            //            OnClickRoom(roomData.roomName);
            //        }
            //    );
            //roomData.gameObject.GetComponentInChildren<EventTrigger>();
        }
    }

    private void OnClickRoom(PointerEventData data, string roomName)
    {
        //닉네임을 정해야 로비로 진입 가능하기때문에 필요없다
        //PhotonNetwork.NickName = PlayerPrefs.GetString("USER_ID");
        PhotonNetwork.JoinRoom(roomName, null);
        Debug.Log("On Click Room Btn !");
    }

    #endregion

    #region ButtonFunction

    private void ChangePanel(ActivePanel panel)
    {
        foreach(GameObject _panel in lobbyPanels)
        {
            _panel.SetActive(false);
        }

        lobbyPanels[(int)panel].SetActive(true);
        activePanel = panel;
    }

    public void OnSelectRoomBtnDown()
    {
        ChangePanel(ActivePanel.SELECTROOM);

    }

    public void OnExitSelectPanelBtnDown()
    {
        ChangePanel(ActivePanel.ROOMS);
    }

    #endregion

    IEnumerator FadeTextToFull(float time, TextMeshProUGUI targetText, string text)
    {
        targetText.text = text;
        targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b, 1);

        while(targetText.color.a > 0.0f)
        {
            targetText.color = new Color(targetText.color.r, targetText.color.g, targetText.color.b,
                                         targetText.color.a - (Time.deltaTime / time));

            yield return null;
        }
    }

}
