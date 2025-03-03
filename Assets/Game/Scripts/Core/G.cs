
public static class G
{
    public static Main main;
    public static RunState run;
}

public class RunState
{
    public int currentLevel;

    public RunState(int level)
    {
        currentLevel = level;
    }
}