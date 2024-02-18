using Compendium.Attributes;

using System.Collections.Generic;

namespace TttRewritten
{
    public static class TttNicks
    {
        private static readonly List<string> usedNicks = new List<string>();

        private static readonly string[] randomNames = { "Jakub", "Jan", "Tomáš", "Adam", "Martin", "Petr", "Pavel", "Josef", "Marek", "Jiří", "Lukáš", "Michal", "Filip", "David", "Ondřej", "Daniel", "Václav", "Karel", "Ján", "Zdeněk", "Miroslav", "Radek", "Robert", "Milan", "Richard", "Roman", "Aleš", "Vladimír", "Vojtěch", "Ivo", "Jindřich", "Oldřich", "Jaroslav", "Stanislav", "Štěpán", "František", "Bohumil", "Libor", "Vlastimil", "Hynek", "Dominik", "Dalibor", "Radim", "Antonín", "Vít", "Kamil", "Leoš", "Rostislav", "Bohuslav", "Viktor", "Patrik", "Dušan", "Kryštof", "Žan", "Emil", "Vladislav", "Jaromír", "Miroslava", "Petra", "Martina", "Jana", "Eva", "Hana", "Anna", "Lenka", "Lucie", "Marie", "Kateřina", "Alena", "Tereza", "Zuzana", "Veronika", "Barbora", "Monika", "Ivana", "Simona", "Michaela", "Nikola", "Petra", "Andrea", "Denisa", "Irena", "Jitka", "Dana", "Radka", "Aneta", "Blanka", "Eliška", "Věra", "Natálie", "Adéla", "Karolína", "Ludmila", "Šárka", "Vendula", "Kristýna", "Jaroslava", "Gabriela", "Kamila", "Dominika", "Markéta", "Renata", "Lucie", "Lucia", "Viera", "Lívia", "Gabriela", "Zlatica", "Alžbeta", "Marta", "Nataša", "Milena", "Blanka", "Božena", "Darina", "Diana", "Drahomíra", "Elena", "Elvíra", "Estera", "Iveta", "Klaudia", "Ladislava", "Lea", "Leona", "Liliana", "Margita", "Mariana", "Melánia", "Milada", "Nina", "Olívia", "Pavlína", "Regína", "Sabína", "Silvia", "Svetlana", "Tatiana", "Valéria", "Vanda", "Vieroslava", "Vlasta", "Zdenka", "Zora", "Zuzana" };

        public static string GenerateNext()
        {
            var random = SelectRandom();

            while (usedNicks.Contains(random))
                random = SelectRandom();

            usedNicks.Add(random);
            return random;
        }

        internal static string SelectRandom()
            => randomNames.RandomItem();

        [RoundStateChanged(Compendium.Enums.RoundState.WaitingForPlayers)]
        private static void OnWaiting()
            => usedNicks.Clear();
    }
}