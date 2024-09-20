namespace SCP999;
public class AbilityConfig
{
    // Properties for Range and Effect Duration
    public float Range { get; set; }
    public float EffectDuration { get; set; }

    // Default constructor (required for YAML deserialization)
    public AbilityConfig() { }

    // Constructor with parameters (for manual creation)
    public AbilityConfig(float range, float effectDuration)
    {
        Range = range;
        EffectDuration = effectDuration;
    }
}