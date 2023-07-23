// ROGUES
// SERVER
using System;
using EvoS.Framework.Network.Unity;

[Serializable]
// added in rogues
public class ServerGameMetricsNotification : AllianceMessageBase
{
    internal ServerGameMetrics GameMetrics;

    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        SerializeObject(GameMetrics, writer);
    }
    
    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        DeserializeObject(out GameMetrics, reader);
    }
}