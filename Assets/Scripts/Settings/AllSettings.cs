public static class AllSettings {

    public static BattleSettings BattleSettings { get; private set; }

    static AllSettings() {
        LoadSettings();
    }

    private static void LoadSettings() {
        BattleSettings = new BattleSettings();
    }

}