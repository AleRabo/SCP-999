namespace SCP999;
public class AbilityConfig
{
    // Properties for Range and Effect Duration
    public byte Intensity { get; set; }
    public float EffectDuration { get; set; }
    public bool ScpFriendly { get; set; }

    // Default constructor (required for YAML deserialization)
    public AbilityConfig() { }

    // Constructor with parameters (for manual creation)
    public AbilityConfig(byte intensity, float effectDuration, bool scpFriendly)
    {
        Intensity = intensity;
        EffectDuration = effectDuration;
        ScpFriendly = scpFriendly;
    }
}
