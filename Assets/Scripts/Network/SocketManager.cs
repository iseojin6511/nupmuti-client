using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using System.Threading;
using System.Text;


public class SocketManager : MonoBehaviour
{
    private string IP = "127.0.0.1";
    private string PORT = "6000";
    //서버 서비스 이름
    private string SERVICE_NAME = "/Server";

    public WebSocketSharp.WebSocket m_Socket = null;

    private void Start()
    {
        try
        {
            m_Socket = new WebSocketSharp.WebSocket("ws://" + IP + ":" + PORT + SERVICE_NAME);
            m_Socket.OnMessage += Recv;
            m_Socket.OnClose += CloseConnect;
        }
        catch
        { }
       
    }

    //서버 연결함수
    public void Connect()
    {
        try
        {
           if(m_Socket == null || !m_Socket.IsAlive)
                m_Socket.Connect();

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void CloseConnect(object sender, CloseEventArgs e)
    {
        DisconncectServer();
    }
    //연결 해제 함수
    public void DisconncectServer()
    {
        try
        {
            if (m_Socket == null)
                return;

            if (m_Socket.IsAlive)
                m_Socket.Close();

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    
    //서버로 데이터 전송할 함수
    public void SendSocketMessage(string msg)
    {
        if (!m_Socket.IsAlive)
            return;
        try
        {
            m_Socket.Send(Encoding.UTF8.GetBytes(msg));
        }
        catch (Exception)
        {

            throw;
        }
        
    }
    //서버로 부터 받은 데이터 처리
    public void Recv(object sender, MessageEventArgs e)
    {
        //string 데이터
        Debug.Log(e.Data);

        //bytes 데이터
        Debug.Log(e.RawData);
    }
   
    private void OnApplicationQuit()
    {
        DisconncectServer();
    }
}
