using System;
using EvoS.Framework.Network.Static;
using EvoS.Framework.Network.Unity;

[Serializable]
public class PlayerDisconnectedNotification : AllianceMessageBase
{
    public LobbySessionInfo SessionInfo;
    public LobbyServerPlayerInfo PlayerInfo;

    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        SerializeObject(SessionInfo, writer);
        SerializeObject(PlayerInfo, writer);
    }

    // custom
    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        DeserializeObject(out SessionInfo, reader);
        DeserializeObject(out PlayerInfo, reader);
    }
}