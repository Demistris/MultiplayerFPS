using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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

    #endregion
}