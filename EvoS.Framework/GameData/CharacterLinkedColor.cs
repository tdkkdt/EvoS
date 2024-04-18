using System;

namespace EvoS.Framework.GameData;

[Serializable]
public class CharacterLinkedColor
{
    public CharacterType Character;
    public int SkinIndex;
    public int PatternIndex;
    public int ColorIndex;
}
