public interface IEnemy
{
    float GetCurrentHP();
    float maxHP { get; }
    // Nuova propriet� per identificare il colore del nemico ("Red", "Blue", "Green", �)
    string EnemyColor { get; }
}
