using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomProperties
{
    public const string Ping = "ping";
    public const string Gamemode = "gm";
    public const string Map = "map";

}

public static class RoomExtension
{
    /// <summary>Store Ping in Room Properties.</summary>
    /// <param name="room"></param>
    /// <param name="value"></param>
    public static void SetPing(this Room room, int value)
    {
        Hashtable roomProperties = new Hashtable()
        {
            {RoomProperties.Ping,value }
        };
        room.SetCustomProperties(roomProperties);
    }
    /// <summary>Get Ping from Room Propertiess</summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static int GetPing(this Room room)
    {
        object data = room.CustomProperties[RoomProperties.Ping];
        int currentPing = System.Convert.ToInt32(data);
        return currentPing;
    }
    /// <summary>Store picked Map in Room Properties.</summary>
    /// <param name="room"></param>
    /// <param name="mapName"></param>
    public static void SetMap(this Room room, string mapName)
    {
        Hashtable roomProperties = new Hashtable()
        {
            {RoomProperties.Map,mapName }
        };
        room.SetCustomProperties(roomProperties);
    }
    /// <summary>Get Map Name from Room Properties.</summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static string GetMap(this Room room)
    {
        object data = room.CustomProperties[RoomProperties.Map];
        Debug.Log(data);
        string currentMap = data.ToString();
        return currentMap;
    }
    /// <summary>Store Gamemode in Room Properties.</summary>
    /// <param name="room"></param>
    /// <param name="index"></param>
    public static void SetGamemode(this Room room, int index)
    {
        Hashtable roomProperties = new Hashtable()
        {
            {RoomProperties.Gamemode,index }
        };
        room.SetCustomProperties(roomProperties);
    }
    /// <summary>Get GameMode Index from Room Properties.</summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public static int GetGamemode(this Room room)
    {
        object data = room.CustomProperties[RoomProperties.Gamemode];
        int currentGamemode = System.Convert.ToInt32(data);
        return currentGamemode;
    }

}
