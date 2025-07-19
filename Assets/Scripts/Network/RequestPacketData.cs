using System;
public abstract record RequestPacketData
{
    public sealed record EnterRoom(int roomId) : RequestPacketData;
    public sealed record LeaveRoom() : RequestPacketData;
    public sealed record GetRoomList() : RequestPacketData;
    public sealed record CreateRoom(string roomName, int maxPlayerCount) : RequestPacketData;
    public sealed record Login(string id, string password) : RequestPacketData;
}
