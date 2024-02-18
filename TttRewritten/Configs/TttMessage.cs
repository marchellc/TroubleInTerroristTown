using Compendium.Extensions;

using System;

using UnityEngine;

using YamlDotNet.Serialization;

namespace TttRewritten.Configs
{
    public class TttMessage
    {
        public string Message { get; set; } = string.Empty;

        public float Duration { get; set; } = 5f;

        public bool IsHint { get; set; } = true;

        [YamlIgnore]
        public bool IsValid
        {
            get => !string.IsNullOrWhiteSpace(Message) && Duration > 0f;
        }

        public void SendDistance(Vector3 position, float distance, Func<string, string> textModifier)
            => SendConditionally(textModifier, player => player.ReferenceHub.transform.position.IsWithinDistance(position, distance));

        public void SendConditionally(Func<string, string> textModifier, Func<TttPlayer, bool> condition)
        {
            var message = Message;

            if (textModifier != null)
                message = textModifier(message);

            foreach (var player in TttPlayer.Players)
            {
                try
                {
                    if (!condition(player))
                        continue;
                }
                catch { continue; }

                if (IsHint)
                    player.Show(message, Duration);
                else
                    player.SendBroadcast(message, (ushort)Duration);
            }
        }

        public void Send(Func<string, string> textModifier, params TttPlayer[] targets)
        {
            var targetMessage = Message;

            if (textModifier != null)
                targetMessage = textModifier(targetMessage);

            for (int i = 0; i < targets.Length; i++)
            {
                if (IsHint)
                    targets[i].Show(targetMessage, Duration);
                else
                    targets[i].SendBroadcast(targetMessage, (ushort)Duration);
            }
        }
    }
}