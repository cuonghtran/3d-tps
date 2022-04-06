using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    private void Start()
    {
        var placeholder = (TextMeshProUGUI)createInput.placeholder;
        placeholder.text = "Room name...";
        placeholder = (TextMeshProUGUI)joinInput.placeholder;
        placeholder.text = "Room name...";
    }

    public void CreateRoom()
    {
        if (createInput.text.Length > 0)
        {
            //RoomOptions roomOptions = new RoomOptions();
            //roomOptions.IsVisible = false;
            //roomOptions.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(createInput.text);
            Debug.Log("Create and Join room: " + createInput.text);
        }            
    }

    public void JoinRoom()
    {
        if (joinInput.text.Length > 0)
        {
            PhotonNetwork.JoinRoom(joinInput.text);
            Debug.Log("Join room: " + joinInput.text);
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
}
