using helpers.Configuration;

using System.Collections.Generic;

using TttRewritten.Enums;

namespace TttRewritten.Configs
{
    public static class TttRoleConfigs
    {
        [Config(Name = "Role Names", Description = "A list of role name translations.")]
        public static Dictionary<TttRoleType, string> RoleNames { get; set; } = new Dictionary<TttRoleType, string>()
        {
            [TttRoleType.None] = "Nikdo",
            [TttRoleType.Dead] = "Mrtvý",
            [TttRoleType.Murderer] = "Vrah",
            [TttRoleType.Innocent] = "Nevinný",
            [TttRoleType.Detective] = "Detektiv"
        };

        [Config(Name = "Role Descriptions", Description = "A list of role descriptions.")]
        public static Dictionary<TttRoleType, string> RoleDescriptions { get; set; } = new Dictionary<TttRoleType, string>()
        {
            [TttRoleType.Murderer] = "<b>Jsi <color=#ff0000>Vrah</color>! Tvým úkolem je zabít co nejvíce lidí bez toho aby tě <color=#0051ff>Detektiv</color> odhalil.</b>",
            [TttRoleType.Detective] = "<b>Jsi <color=#0051ff>Detektiv</color>! Tvým ůkolem je co nejdříve odhalit <color=#ff0000>Vraha</color> a zachranít co nejvíce <color=#05ff00>Nevinných</color>.</b>",
            [TttRoleType.Innocent] = "<b>Jsi <color=#05ff00>Nevinný</color>! Tvým úkolem je přežít co nejdéle a pomáhat <color=#0051ff>Detektivovi</color> v odhalení <color=#ff0000>Vraha</color>.</b>"
        };

        public static void DisplayRoleDescription(TttPlayer player)
        {
            if (RoleDescriptions.TryGetValue(player.RoleType, out var desc))
            {
                player.Show($"\n\n\n\n\n\n\n{desc}", 15f);
            }
        }

        public static string GetRoleName(this TttRoleType type)
        {
            if (RoleNames.TryGetValue(type, out var name))
                return name;

            return type.ToString();
        }

        public static string GetColoredRoleName(this TttRoleType type)
        {
            var name = GetRoleName(type);
            var color = string.Empty;

            switch (type)
            {
                case TttRoleType.None:
                    color = "fcff00";
                    break;

                case TttRoleType.Dead:
                    color = "ff8a00";
                    break;

                case TttRoleType.Detective:
                    color = "0051ff";
                    break;

                case TttRoleType.Innocent:
                    color = "05ff00";
                    break;

                case TttRoleType.Murderer:
                    color = "ff0000";
                    break;
            }

            return $"<color=#{color}>{name}</color>";
        }
    }
}
