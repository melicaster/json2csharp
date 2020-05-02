using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace json2csharp.Controllers
{
    [Route("api/json2csharp")]
    [ApiController]
    public class Json2CSharpController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Get(Json2CSharpPostRequestDto vm)
        {
            try
            {
                var gen = new JsonClassGenerator();
                gen.UsePascalCase = vm.UsePascalCase;
                gen.UseProperties = vm.UseProperties;
                gen.Example = vm.Example;
                gen.MainClass = "MyJsonObject";

                gen.GenerateClasses();
                var csharpCodeWriter = new CSharpCodeWriter();
                var resultWriter = new StringWriter();
                csharpCodeWriter.WriteFileStart(gen, resultWriter);
                foreach (var type in gen.Types)
                {
                    csharpCodeWriter.WriteClass(gen, resultWriter, type);
                }
                csharpCodeWriter.WriteFileEnd(gen, resultWriter);

                return Ok(new Json2CSharpPostResponseDto()
                {
                    Result = resultWriter.ToString()
                });
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}