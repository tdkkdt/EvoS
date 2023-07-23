// ROGUES
// SERVER

using System;
using EvoS.Framework.Network.Static;
using EvoS.Framework.Network.Unity;

// server-only
[Serializable]
public class LaunchGameResponse : AllianceResponseBase
{
	internal LobbyGameInfo GameInfo;
	internal string GameServerAddress;

	public override void Serialize(NetworkWriter writer)
	{
		base.Serialize(writer);
		SerializeObject(GameInfo, writer);
		writer.Write(GameServerAddress);
	}

	public override void Deserialize(NetworkReader reader)
	{
		base.Deserialize(reader);
		DeserializeObject(out GameInfo, reader);
		GameServerAddress = reader.ReadString();
	}
}
