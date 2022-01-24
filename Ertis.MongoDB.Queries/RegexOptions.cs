using System;

namespace Ertis.MongoDB.Queries
{
    [Flags]
    public enum RegexOptions
    {
        /// <summary>
        /// 'i' - Case insensitivity to match upper and lower cases.
        /// </summary>
        CaseInsensitivity = 1,
        
        /// <summary>
        /// 'm' - For patterns that include anchors (i.e. ^ for the start, $ for the end), match at the beginning or end of each line for strings with multiline values.
        /// </summary>
        Multiline = 2,
        
        /// <summary>
        /// 'x' - "Extended" capability to ignore all white space characters in the $regex pattern unless escaped or included in a character class.
        /// </summary>
        Extended = 4,
        
        /// <summary>
        /// 's' - Allows the dot character (i.e. .) to match all characters including newline characters.
        /// </summary>
        AllowDot = 8
    }
}