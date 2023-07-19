using System;

namespace EvoS.Framework.Auth;

public class AuthEntitlement
{
    public long accountEntitlementId;
    public long entitlementId;
    public string entitlementCode;
    public int entitlementAmount;
    public DateTime modifiedDate;
    public DateTime expirationDate;

    public bool Expires => expirationDate < DateTime.MaxValue;
}