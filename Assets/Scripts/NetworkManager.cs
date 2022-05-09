using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status")]
    [SerializeField] private Text _connectionStatusText;

    [Header("Login UI Panel")]
    [SerializeField] private GameObject _loginUIPanel;
    [SerializeField] private InputField _playerNameInput;

    [Header("Game Options UI Panel")]
    [SerializeField] private GameObject _gameOptionsUIPanel;

    [Header("Create Room UI Panel")]
    [SerializeField] private GameObject _createRoomUIPanel;
    [SerializeField] private InputField _roomNameInputField;
    [SerializeField] private InputField _maxPlayersInputField;

    [Header("Inside Room UI Panel")]
    [SerializeField] private GameObject _insideRoomUIPanel;
    [SerializeField] private GameObject _playerListPrefab;
    [SerializeField] private GameObject _playerListParent;
    [SerializeField] private GameObject _startGameButton;
    [SerializeField] private Text _roomInfoText;

    [Header("Room List UI Panel")]
    [SerializeField] private GameObject _roomListUIPanel;
    [SerializeField] private GameObject _roomListEntryPrefab;
    [SerializeField] private GameObject _roomListParentGameObject;

    [Header("Join Random Room UI Panel")]
    [SerializeField] private GameObject _joinRandomRoomUIPanel;

    private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();
    private Dictionary<string, GameObject> _roomListGameObjects = new Dictionary<string, GameObject>();
    private Dictionary<int, GameObject> _playerListGameObjects;

    #region Unity Methods

    private void Start()
    {
        ActivatePanel(_loginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        SetConnectionStatusText();
    }

    private void SetConnectionStatusText()
    {
        _connectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }

    public void ActivatePanel(string panelToBeActivated)
    {
        _loginUIPanel.SetActive(panelToBeActivated.Equals(_loginUIPanel.name));
        _gameOptionsUIPanel.SetActive(panelToBeActivated.Equals(_gameOptionsUIPanel.name));
        _createRoomUIPanel.SetActive(panelToBeActivated.Equals(_createRoomUIPanel.name));
        _insideRoomUIPanel.SetActive(panelToBeActivated.Equals(_insideRoomUIPanel.name));
        _roomListUIPanel.SetActive(panelToBeActivated.Equals(_roomListUIPanel.name));
        _joinRandomRoomUIPanel.SetActive(panelToBeActivated.Equals(_joinRandomRoomUIPanel.name));
    }

    private void OnJoinRoomButtonClicked(string roomName)
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    private void ClearRoomListView()
    {
        foreach (GameObject roomListGameObject in _roomListGameObjects.Values)
        {
            Destroy(roomListGameObject);
        }

        _roomListGameObjects.Clear();
    }

    private void InstantiatePlayer(Player player)
    {
        GameObject playerListGameObject = Instantiate(_playerListPrefab);
        playerListGameObject.transform.SetParent(_playerListParent.transform);
        playerListGameObject.transform.localScale = Vector3.one;

        playerListGameObject.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;

        _playerListGameObjects.Add(player.ActorNumber, playerListGameObject);

        if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
            return;
        }

        playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
    }

    private void RoomInfoText()
    {
        _roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " Players/Max players: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    private void SetActivityOfStartGameButton()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _startGameButton.SetActive(true);
            return;
        }

        _startGameButton.SetActive(false);
    }

    #endregion

    #region UI Callbacks

    public void OnLoginButtonClicked()
    {
        string playerName = _playerNameInput.text;

        if(string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Player name is invalid!");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = _roomNameInputField.text;

        if(string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        if(string.IsNullOrEmpty(_maxPlayersInputField.text)) // tutaj mozna wpisac wszystko
        {
            _maxPlayersInputField.text = "3";
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(_maxPlayersInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(_gameOptionsUIPanel.name);
    }

    public void OnShowRoomListButtonClicked()
    {
        if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(_roomListUIPanel.name);
    }

    public void OnBackButtonClicked()
    {
        if(PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(_gameOptionsUIPanel.name);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        ActivatePanel(_joinRandomRoomUIPanel.name);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnStartButtonClicked()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnected()
    {
        Debug.Log("Connected to Internet.");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon.");
        ActivatePanel(_gameOptionsUIPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(_insideRoomUIPanel.name);

        SetActivityOfStartGameButton();

        RoomInfoText();

        if (_playerListGameObjects == null)
        {
            _playerListGameObjects = new Dictionary<int, GameObject>();
        }

        //Instantiating player list gameobjects
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            InstantiatePlayer(player);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomInfoText();
        InstantiatePlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomInfoText();
        Destroy(_playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        _playerListGameObjects.Remove(otherPlayer.ActorNumber);

        SetActivityOfStartGameButton();
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(_gameOptionsUIPanel.name);

        foreach(GameObject playerListGameObject in _playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }

        _playerListGameObjects.Clear();
        _playerListGameObjects = null;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room.Name);

            if(!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if(_cachedRoomList.ContainsKey(room.Name))
                {
                    _cachedRoomList.Remove(room.Name);
                }

                return;
            }

            //Update cachedRoom list
            if(_cachedRoomList.ContainsKey(room.Name))
            {
                _cachedRoomList[room.Name] = room;
                return;
            }

            //Add the new room to the cachedRoom list
            _cachedRoomList.Add(room.Name, room);
        }

        foreach(RoomInfo room in _cachedRoomList.Values)
        {
            GameObject roomListEntryGameObject = Instantiate(_roomListEntryPrefab);
            roomListEntryGameObject.transform.SetParent(_roomListParentGameObject.transform);
            roomListEntryGameObject.transform.localScale = Vector3.one;

            roomListEntryGameObject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomListEntryGameObject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount + " / " + room.MaxPlayers;
            roomListEntryGameObject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(room.Name));

            _roomListGameObjects.Add(room.Name, roomListEntryGameObject);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomListView();
        _cachedRoomList.Clear();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion
}