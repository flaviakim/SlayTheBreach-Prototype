using System.Collections.Generic;

public class EnemyManager {
    private Battle _battle;
    private List<Creature> _enemies = new();

    public EnemyManager(Battle battle) {
        _battle = battle;
    }

    public void OnBattleStart() {

    }
}