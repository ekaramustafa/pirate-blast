using UnityEngine;

[CreateAssetMenu(fileName = "NewTNT", menuName = "TNTData")]
public class TNTData : UnitData
{
    public TNTType tntType;
    public int range;
}

public enum TNTType
{
    NORMAL,
    LARGE
}


