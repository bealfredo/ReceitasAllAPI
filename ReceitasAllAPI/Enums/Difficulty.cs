using System.Runtime.Serialization;

namespace ReceitasAllAPI.Enums
{
    public enum Difficulty
    {
        [EnumMember(Value = "Easy")]
        Easy = 1,

        [EnumMember(Value = "Medium")]
        Medium = 2,

        [EnumMember(Value = "Hard")]
        Hard = 3
    }
}
