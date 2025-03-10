public abstract class ScoreSystemBase
{
    protected int totalScore = 0;

    public abstract void AddScore(string color);
    public abstract void AddScoreCustom(string color, int basePoints);
    public abstract void Update();

    // Aggiungi eventualmente il metodo OnHit (utile solo per il sistema Combo)
    public virtual void OnHit() { }

    public int GetTotalScore()
    {
        return totalScore;
    }
}
