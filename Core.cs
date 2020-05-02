using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace json2csharp
{
    public class Json2CSharpPostRequestDto
    {
        public string Example { get; set; }
        public bool UsePascalCase { get; set; }
        public bool UseProperties { get; set; }
        public bool EscapedDoubleQuote { get; set; }
    }

    public class Json2CSharpPostResponseDto
    {
        public string Result { get; set; }
    }
}
