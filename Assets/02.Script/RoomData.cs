using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoomData : MonoBehaviour
{
    public string roomName = "";
    public int playerCount = 0;
    public int maxPlayer = 0;

    //사용하면 정보를 제대로 얻어오지 못하는 현상 발견
    //[System.NonSerialized]
    public TextMeshProUGUI roomDataText;

    private void Awake()
    {
        roomDataText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateRoomInfo()
    {
        roomDataText.text = string.Format(" {0} [ {1} / {2} ] ",
                                          roomName,
                                          playerCount.ToString(),
                                          maxPlayer);
    }
}
