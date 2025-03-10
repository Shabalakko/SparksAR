public interface IEnemy
{
    float GetCurrentHP();
    float maxHP { get; }
    // Nuova proprietà per identificare il colore del nemico ("Red", "Blue", "Green", …)
    string EnemyColor { get; }
}
