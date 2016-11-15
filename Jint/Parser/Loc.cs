namespace Jint.Parser
{
    public class Location
    {
        public Position Start;
        public Position End;
        [System.Web.Script.Serialization.ScriptIgnore]
        public string Source;
    }
}
