using UnityEngine;
using NativeWebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PACKET_TYPE;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private WebSocket socket;
    private bool _isReady = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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
        { PACKET_TYPE.PING, typeof(ResponsePacketData.Pong) },
        { PACKET_TYPE.ENTER_ROOM, typeof(ResponsePacketData.EnterRoom) },
        { PACKET_TYPE.LEAVE_ROOM, typeof(ResponsePacketData.LeaveRoom) },
        { PACKET_TYPE.PLAYER_COUNT_CHANGED, typeof(ResponsePacketData.PlayerCountChanged) },
        { PACKET_TYPE.YOU_ARE_HOST, typeof(ResponsePacketData.YouAreHost) },
        { PACKET_TYPE.READY_GAME, typeof(ResponsePacketData.ReadyGame) },
        { PACKET_TYPE.START_GAME, typeof(ResponsePacketData.StartGame) },
        { PACKET_TYPE.START_ROUND, typeof(ResponsePacketData.StartRound) },
        { PACKET_TYPE.FIRST_ROUND_RULES, typeof(ResponsePacketData.FirstRoundRules) },
        { PACKET_TYPE.SHUFFLE_CARDS, typeof(ResponsePacketData.YourCard) },
        { PACKET_TYPE.YOUR_RANK, typeof(ResponsePacketData.YourRank) },
        { PACKET_TYPE.YOUR_ORDER, typeof(ResponsePacketData.YourOrder) },
        { PACKET_TYPE.ROUND_STARTED, typeof(ResponsePacketData.RoundStarted) },
        { PACKET_TYPE.DEAL_CARDS, typeof(ResponsePacketData.DealCards) },
        { PACKET_TYPE.EXCHANGE_PHASE, typeof(ResponsePacketData.ExchangePhase) },
        { PACKET_TYPE.EXCHANGE_INFO, typeof(ResponsePacketData.ExchangeInfo) },
        { PACKET_TYPE.EXCHANGE_INFO_2, typeof(ResponsePacketData.ExchangeInfo2) },
        { PACKET_TYPE.EXCHANGE_DONE, typeof(ResponsePacketData.ExchangeDone) },
        { PACKET_TYPE.YOUR_TURN, typeof(ResponsePacketData.YourTurn) },
        { PACKET_TYPE.ALL_PASSED, typeof(ResponsePacketData.AllPassed) },
        { PACKET_TYPE.END_TURN, typeof(ResponsePacketData.EndTurn) },
        { PACKET_TYPE.DONE_ROUND, typeof(ResponsePacketData.DoneRound) },
        { PACKET_TYPE.INVALID_CARD, typeof(ResponsePacketData.InvalidCard) }
        // ... 필요시 추가
    };

    private readonly Dictionary<Type, int> _requestSignals = new()
    {
        { typeof(RequestPacketData.Ping), PACKET_TYPE.PING },
        { typeof(RequestPacketData.EnterRoom), PACKET_TYPE.ENTER_ROOM },
        { typeof(RequestPacketData.LeaveRoom), PACKET_TYPE.LEAVE_ROOM },
        { typeof(RequestPacketData.GetRoomInfo), PACKET_TYPE.GET_ROOM_INFO },
        { typeof(RequestPacketData.StartGame), PACKET_TYPE.START_GAME },
        { typeof(RequestPacketData.ThrowSubmit), PACKET_TYPE.THROW_SUBMIT },
        { typeof(RequestPacketData.PlayCard), PACKET_TYPE.PLAY_CARD },
        { typeof(RequestPacketData.Pass), PACKET_TYPE.PASS },
        { typeof(RequestPacketData.DoneRound), PACKET_TYPE.DONE_ROUND }
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
