// ROGUES
// SERVER

using System;
using EvoS.Framework.Network.Static;
using EvoS.Framework.Network.Unity;

// server-only
[Serializable]
public class JoinGameServerResponse : AllianceResponseBase
{
	internal int OrigRequestId;
	internal LobbyServerPlayerInfo PlayerInfo;
	internal string GameServerProcessCode;

	public override void Serialize(NetworkWriter writer)
	{
		base.Serialize(writer);
		writer.Write(OrigRequestId);
		SerializeObject(PlayerInfo, writer);
		writer.Write(GameServerProcessCode);
	}
	
	public override void Deserialize(NetworkReader reader)
	{
		base.Deserialize(reader);
		OrigRequestId = reader.ReadInt32();
		DeserializeObject(out PlayerInfo, reader);
		GameServerProcessCode = reader.ReadString();
	}
}
