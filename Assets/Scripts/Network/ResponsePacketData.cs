using System;
public abstract record ResponsePacketData
{
    public sealed record EnterRoom(int roomId, string roomName, int maxPlayerCount) : ResponsePacketData;
    public sealed record LeaveRoom() : ResponsePacketData;
    public sealed record GetRoomList(RoomInfo[] rooms) : ResponsePacketData;
    public sealed record CreateRoom(int roomId) : ResponsePacketData;
    public sealed record Login(string id, string nickname) : ResponsePacketData;
}
