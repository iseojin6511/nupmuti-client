// Assets/Scripts/Data/PlayerInfo.cs
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerInfo
{
    public string nickname;
    public Sprite profileImage;
    public int rank;
    public List<int> cardValues = new List<int>();
    public int cardsLeft => cardValues.Count;

}