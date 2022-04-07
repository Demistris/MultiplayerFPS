using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status")]
    [SerializeField] private Text _connectionStatusText;

    [Header("Login UI Panel")]
    [SerializeField] private InputField _playerNameInput;
    [SerializeField] private GameObject _loginUIPanel;

    [Header("Game Options UI Panel")]
    [SerializeField] private GameObject _gameOptionsUIPanel;

    [Header("Create Room UI Panel")]
    [SerializeField] private GameObject _createRoomUIPanel;
    [SerializeField] private InputField _roomNameInputField;
    [SerializeField] private InputField _maxPlayersInputField;

    [Header("Inside Room UI Panel")]
    [SerializeField] private GameObject _insideRoomUIPanel;

    [Header("Room List UI Panel")]
    [SerializeField] private GameObject _roomListUIPanel;

    [Header("Join Random Room UI Panel")]
    [SerializeField] private GameObject _joinRandomRoomUIPanel;

    #region Unity Methods

    private void Start()
    {
        ActivatePanel(_loginUIPanel.name);
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

        if(string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(_maxPlayersInputField.text))
        {
            Debug.Log("Room name or max number of players is invalid!");
            return;
        }

        roomName = "Room " + Random.Range(1000, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(_maxPlayersInputField.text);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(_gameOptionsUIPanel.name);
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
    }

    #endregion
}