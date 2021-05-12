using Ariadna.Core;
using Newtonsoft.Json;

namespace Ariadna.Engine.Core
{
    public class EngineGlobalSettings : BaseViewModel
    {
        [JsonProperty] public string BackgroundColor { get; set; } = "#FF000000";

        [JsonProperty] public string OrtLineColor { get; set; } = "#FF0000FF";

        [JsonProperty] public string GridMajorColor { get; set; } = "#FFAAAAAA";

        [JsonProperty] public string GridMinorColor { get; set; } = "#FFAAAAAA";

        #region Static

        public static EngineGlobalSettings Instance { get; } = new EngineGlobalSettings();

        #endregion
    }
}