[System.Serializable]
public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid;

    public LevelData Clone()
    {
        LevelData clone = new LevelData();
        clone.level_number = this.level_number;
        clone.grid_width = this.grid_width;
        clone.grid_height = this.grid_height;
        clone.move_count = this.move_count;
        clone.grid = this.grid;
        return clone;
            
    }
}
