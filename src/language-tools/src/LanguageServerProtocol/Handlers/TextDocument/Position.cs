using Newtonsoft.Json;

namespace LanguageServerProtocol.Handlers.TextDocument
{
    public class Position
    {
        public Position()
        {
        }

        public Position(int line, int character)
        {
            Line = line;
            Character = character;
        }

        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("character")]
        public int Character { get; set; }
    }
}