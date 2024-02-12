using System;
using EvoS.Framework.Misc;
using EvoS.Framework.Network;

// Token: 0x020009D7 RID: 2519
[Serializable]
[EvosMessage(770, typeof(ServerMessage))]
public class ServerMessage
{
    public static implicit operator ServerMessage(string value)
    {
        return new ServerMessage
        {
            EN = value,
            FR = value,
            DE = value,
            RU = value,
            ES = value,
            IT = value,
            PL = value,
            PT = value,
            KO = value,
            ZH = value,
            NL = value,
            BR = value
        };
    }

    public ServerMessage FillMissingLocalizations()
    {
	    return new ServerMessage
	    {
		    EN = EN,
		    FR = FR.IsNullOrEmpty() ? EN : FR,
		    DE = DE.IsNullOrEmpty() ? EN : DE,
		    RU = RU.IsNullOrEmpty() ? EN : RU,
		    ES = ES.IsNullOrEmpty() ? EN : ES,
		    IT = IT.IsNullOrEmpty() ? EN : IT,
		    PL = PL.IsNullOrEmpty() ? EN : PL,
		    PT = PT.IsNullOrEmpty() ? EN : PT,
		    KO = KO.IsNullOrEmpty() ? EN : KO,
		    ZH = ZH.IsNullOrEmpty() ? EN : ZH,
		    NL = NL.IsNullOrEmpty() ? EN : NL,
		    BR = BR.IsNullOrEmpty() ? EN : BR
	    };
    }

    public string EN { get; set; }
    public string FR { get; set; }
    public string DE { get; set; }
    public string RU { get; set; }
    public string ES { get; set; }
    public string IT { get; set; }
    public string PL { get; set; }
    public string PT { get; set; }
    public string KO { get; set; }
    public string ZH { get; set; }
    
    // custom ones for the launcher
    [field: NonSerialized] public string NL { get; set; }
    [field: NonSerialized] public string BR { get; set; }

    public bool IsEmpty()
    {
	    return EN.IsNullOrEmpty();
    }

    public string GetValue(ServerMessageLanguage language)
    {
	    return (string)GetType().GetProperty(language.ToString()).GetValue(this, null);
    }

    public string GetValue(string languageCode)
    {
	    ServerMessageLanguage language = (ServerMessageLanguage)Enum.Parse(typeof(ServerMessageLanguage), languageCode, true);
	    return GetValue(language);
    }

    /*
	[JsonIgnore]
	public IEnumerable<string> Languages
	{
		get
		{
            
			foreach (ServerMessageLanguage language in Enum.GetValues(typeof(ServerMessageLanguage)).Cast<ServerMessageLanguage>())
			{
				if (!this.GetValue(language).IsNullOrEmpty())
				{
					yield return language.ToString();
				}
			}
			yield break;
		}
	}
    */
}
