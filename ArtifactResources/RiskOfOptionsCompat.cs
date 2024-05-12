using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;

namespace GeneticsArtifact
{
    public class RiskOfOptionsCompat
    {
        public static void Init()
        {
            ModSettingsManager.SetModIcon(ArtifactOfGenetics.artifactDef.smallIconSelectedSprite);

            ModSettingsManager.AddOption(
                new IntSliderOption(ConfigManager.governorType,
                    new IntSliderConfig { min = 0, max = 2 }));
            ModSettingsManager.AddOption(
                new IntSliderOption(ConfigManager.timeLimit,
                    new IntSliderConfig { min = 5, max = 300 }));
            ModSettingsManager.AddOption(
                new IntSliderOption(ConfigManager.deathLimit,
                    new IntSliderConfig { min = 10, max = 100 }));
            ModSettingsManager.AddOption(
                new CheckBoxOption(ConfigManager.maintainIfDisabled));

            ModSettingsManager.AddOption(
                new SliderOption(ConfigManager.geneCap,
                    new SliderConfig { min = 1f, max = 50f, formatString = "{0:#0.##}x" }));
            ModSettingsManager.AddOption(
                new SliderOption(ConfigManager.geneFloor,
                    new SliderConfig { min = 0.01f, max = 1f, formatString = "{0:0.##}x" }));
            ModSettingsManager.AddOption(
                new SliderOption(ConfigManager.geneProductLimit,
                    new SliderConfig { min = 1f, max = 10f, formatString = "{0:#0.##}x" }));
            ModSettingsManager.AddOption(
                new SliderOption(ConfigManager.geneVarianceLimit,
                    new SliderConfig { min = 0.01f, max = 1f, formatString = "{0:#0.##%}" }));

            ModSettingsManager.AddOption(
                new CheckBoxOption(ConfigManager.enableGeneLimitOverrides));
            ModSettingsManager.AddOption(
                new StringInputFieldOption(ConfigManager.geneLimitOverrides));
        }
    }
}
