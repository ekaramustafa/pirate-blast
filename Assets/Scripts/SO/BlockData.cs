using UnityEngine;

[CreateAssetMenu(fileName = "NewBlock", menuName = "BlockSO")]
public class BlockData : UnitData
{
    public BlockColor blockColor;
    public Sprite tntStateSprite;
}


public enum BlockColor
{
    Red,
    Blue,
    Yellow,
    Green,

}

public static class BlockColorExtensions
{
    private static readonly BlockColor[] Colors =
    {
        BlockColor.Red,
        BlockColor.Green,
        BlockColor.Blue,
        BlockColor.Yellow
    };

    public static BlockColor GetRandomBlockColor()
    {
        int index = Random.Range(0,Colors.Length);
        return Colors[index];
    }
}
