using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class SocketManager : MonoBehaviour
{
    private string IP = "127.0.0.1";
    private string PORT = "6000";
    private string SERVICE_NAME = "/Server";

    private WebSocket m_Socket = null;

    private async void Start()
    {
    #if UNITY_WEBGL && !UNITY_EDITOR
        string uri = "wss://yourserver.com/ws";
    #else
        string uri = "ws://localhost:3000";
    #endif

        Debug.Log($"🟡 Start() - Connecting to: {uri}");

        m_Socket = new WebSocket(uri);

        m_Socket.OnOpen += () =>
        {
            Debug.Log("Connected to server!");
        };

        m_Socket.OnError += (e) =>
        {
            Debug.LogError("WebSocket Error: " + e);
        };

        m_Socket.OnClose += (e) =>
        {
            Debug.Log("🔌 WebSocket closed");
        };

        m_Socket.OnMessage += (bytes) =>
        {
            string msg = Encoding.UTF8.GetString(bytes);
            Debug.Log($"📩 Received: {msg}");
        };

        await Connect(); // 이제 m_Socket이 null이 아니므로 실행됨
    }

    public async Task Connect()
    {
        if (m_Socket == null)
            return;

        if (m_Socket.State == WebSocketState.Open)
        {
            Debug.Log("Already connected.");
            return;
        }

        try
        {
            await m_Socket.Connect();
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket Connect Error: {e}");
        }
    }

    public async void Disconnect()
    {
        if (m_Socket == null)
            return;

        try
        {
            await m_Socket.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket Close Error: {e}");
        }
    }

    public bool IsConnected => m_Socket != null && m_Socket.State == WebSocketState.Open;

    // 외부에서 안전하게 메시지를 전송하기 위한 메서드
    public void SendMessageToServer(string message)
    {
        SendSocketMessage(message); // 내부 async void 메서드 래핑
    }

    public async void SendSocketMessage(string msg)
    {
        if (m_Socket == null || m_Socket.State != WebSocketState.Open)
        {
            Debug.LogWarning("Socket is not open.");
            return;
        }

        try
        {
            await m_Socket.SendText(msg);
            Debug.Log($"[Send] {msg}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Send Error: {e}");
        }
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        m_Socket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await m_Socket.Close();
    }

    
}
