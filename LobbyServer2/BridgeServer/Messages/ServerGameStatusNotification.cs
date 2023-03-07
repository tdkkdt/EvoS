using System;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.Network.Unity;

[Serializable]
// added in rogues
public class ServerGameStatusNotification : AllianceMessageBase
{
    public GameStatus GameStatus;

    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        writer.Write((byte)GameStatus);
    }

    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        GameStatus = (GameStatus)reader.ReadByte();
    }
}