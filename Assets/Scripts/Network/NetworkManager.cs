using UnityEngine;
using NativeWebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private WebSocket socket;
    private bool _isReady = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // 씬이 바뀌어도 유지됨
    }

    private async void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("🔗 Trying to connect to: wss://yourserver.com/ws");
        await Connect("wss://yourserver.com/ws");
#else
        Debug.Log("🔗 Trying to connect to: ws://localhost:3000");
        await Connect("ws://localhost:3000");
#endif
    }

    public async Task Connect(string uri)
    {
        socket = new WebSocket(uri);

        socket.OnOpen += () =>
        {
            Debug.Log("Connected to server");
            _isReady = true;
        };

        socket.OnMessage += HandleMessage;

        socket.OnClose += (e) => Debug.Log("WebSocket closed");
        socket.OnError += (e) => Debug.LogError($"WebSocket error: {e}");

        await socket.Connect();
    }

    public bool IsReady() => _isReady;

    private async void HandleMessage(byte[] bytes)
    {
        string json = Encoding.UTF8.GetString(bytes);
        Debug.Log("[Recv] " + json);

        var wrapper = JObject.Parse(json);
        int signal = wrapper["signal"]?.Value<int>() ?? -1;
        int code = wrapper["code"]?.Value<int>() ?? 200;

        if (!_responseTypes.TryGetValue(signal, out var type))
        {
            Debug.LogWarning($"Unknown signal: {signal}");
            return;
        }

        if (code != 200)
        {
            FindObjectOfType<NubmutiController>()?.OnError(signal, code);
            return;
        }

        var dataObj = wrapper["data"]?.ToObject(type);
        if (dataObj is ResponsePacketData response)
        {
            _responseHandlers[type]?.Invoke(response);
        }
    }


    public async void Send(RequestPacketData data)
    {
        int signal = GetSignalFromRequest(data.GetType());
        var packet = new { signal, data };
        string json = JsonConvert.SerializeObject(packet);
        Debug.Log("[Send] " + json);

        if (socket != null && socket.State == WebSocketState.Open)
        {
            await socket.SendText(json);
        }
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        socket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        if (socket != null)
        {
            await socket.Close();
        }
    }

    // ==== 핸들러 관리 ====
    private readonly Dictionary<Type, Action<ResponsePacketData>> _responseHandlers = new();
    private readonly Dictionary<int, Type> _responseTypes = new()
    {
        { 1, typeof(ResponsePacketData.Pong) },
        { 1001, typeof(ResponsePacketData.EnterRoom) },
        { 1002, typeof(ResponsePacketData.LeaveRoom) },
        { 1007, typeof(ResponsePacketData.PlayerCountChanged) },
        { 1004, typeof(ResponsePacketData.CreateRoom) },
        { 1005, typeof(ResponsePacketData.YouAreHost) },
        { 1011, typeof(ResponsePacketData.ReadyGame) },
        { 1010, typeof(ResponsePacketData.StartGame) },
        { 1020, typeof(ResponsePacketData.StartRound) },
        { 1022, typeof(ResponsePacketData.FirstRoundRules) },
        { 1023, typeof(ResponsePacketData.ShuffleCards) },
        { 1024, typeof(ResponsePacketData.DealOneCard) },
        { 1025, typeof(ResponsePacketData.UpdateHand) },
        { 1026, typeof(ResponsePacketData.YourCard) },
        { 1103, typeof(ResponsePacketData.YourRank) },
        { 1104, typeof(ResponsePacketData.YourOrder) },
        { 1105, typeof(ResponsePacketData.RoundStarted) },
        { 1106, typeof(ResponsePacketData.DealCards) },
        { 1107, typeof(ResponsePacketData.ExchangePhase) },
        { 1108, typeof(ResponsePacketData.ExchangeInfo) },
        { 1109, typeof(ResponsePacketData.ExchangeInfo2) },
        { 1111, typeof(ResponsePacketData.ExchangeDone) },
        { 1112, typeof(ResponsePacketData.YourTurn) },
        { 1115, typeof(ResponsePacketData.AllPassed) },
        { 1116, typeof(ResponsePacketData.EndTurn) },
        { 1028, typeof(ResponsePacketData.DoneRound) },
        { 1030, typeof(ResponsePacketData.InvalidCard) },
        { 1031, typeof(ResponsePacketData.AllInfo) },
        { 1032, typeof(ResponsePacketData.NextPage) },
        { 1033, typeof(ResponsePacketData.SubmitError) },
        // ... 필요시 추가
    };

    private readonly Dictionary<Type, int> _requestSignals = new()
    {
        { typeof(RequestPacketData.Ping), 1 },
        { typeof(RequestPacketData.EnterRoom), 1001 },
        { typeof(RequestPacketData.LeaveRoom), 1002 },  
        { typeof(RequestPacketData.GetRoomInfo), 1003 },
        { typeof(RequestPacketData.CreateRoom), 1004 },
        { typeof(RequestPacketData.RoundStarted),1105},
        { typeof(RequestPacketData.StartGame), 1010 },
        { typeof(RequestPacketData.ThrowSubmit), 1110 },
        { typeof(RequestPacketData.PlayCard), 1113 },
        { typeof(RequestPacketData.Pass), 1114 },
        { typeof(RequestPacketData.DoneRound), 1028 },
        { typeof(RequestPacketData.AllInfo), 1031 },

        // ... 필요시 추가
    };

    private int GetSignalFromRequest(Type type)
    {
        if (_requestSignals.TryGetValue(type, out int signal))
            return signal;

        throw new Exception("Unknown request type: " + type.Name);
    }

    public void RegisterHandler<T>(Action<T> handler) where T : ResponsePacketData
    {
        _responseHandlers[typeof(T)] = (data) => handler((T)data);
    }
}

public sealed record RoomInfo(int roomId, string roomName, int playerCount, int maxPlayerCount);
