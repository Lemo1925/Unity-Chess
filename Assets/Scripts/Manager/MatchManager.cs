
public class MatchManager
{
    private static MatchManager _instance;
    public static MatchManager Instance => _instance ??= new MatchManager();

    public static Grid CurrentGrid;

    public static Chess CurrentChess;

    public int Checkmate = -1;
}
