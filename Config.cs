using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using static SCP999.Scp999Role;

namespace SCP999
{
    /// <inheritdoc cref="IConfig"/>
    public class Config : IConfig
    {
        // The plugin configs

        [Description("Whether or not is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not is the plugin is in debug mode?")]
        public bool Debug { get; set; } = false;

        [Description("Is SCP-999 immortal")]
        public bool Scp999GodMode { get; set; } = false;

        [Description("Configs for the SCP-999 role players turn into.")]
        public Scp999Role Scp999RoleConfig { get; set; } = new();

        [Description("SCP-999 AOE abiliities range.")]
        public Dictionary<string, AbilityConfig> Abilities { get; set; } = new Dictionary<string, AbilityConfig>()
    {
        { "Invigorate", new AbilityConfig(10f, 5f) },  // Ability with Range and EffectDuration
        { "Heal", new AbilityConfig(20f, 50f) },
        { "SpeedBoost", new AbilityConfig(15f, 4f) }
    };
        [Description("Does the SpeedBoost ability slow SCPs?")]
        public bool SpeedSlowsSCPs { get; set; } = true;
    }
}