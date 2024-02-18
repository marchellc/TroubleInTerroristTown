using helpers.Configuration;

namespace TttRewritten.Configs
{
    public static class TttRoundConfigs
    {
        [Config(Name = "Restart Delay", Description = "How many seconds to wait before restarting the round.")]
        public static float RestartDelay { get; set; } = 7f;

        [Config(Name = "Minimum Players", Description = "The minimum required amount of players to start a game.")]
        public static int MinPlayers { get; set; } = 6;
    }
}