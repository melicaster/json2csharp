using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace json2csharp.Pages.api
{
    [IgnoreAntiforgeryToken]
    public class json2csharpModel : PageModel
    {
        public JsonResult OnGet()
        {
            List<string> testdata = new List<string>();
            testdata.Add("test");
            return new JsonResult(testdata);
        }

        public JsonResult OnPost()
        {
            APIRequest apiRequest = new APIRequest();
            if (Request.Form.ContainsKey("UsePascalCase"))
            {
                var resultWriter = new StringWriter();
                var csharpCodeWriter = new CSharpCodeWriter();
                try
                {
                    Json2CSharpPostRequestDto vm = new Json2CSharpPostRequestDto();
                    vm.UsePascalCase = bool.Parse(Request.Form["UsePascalCase"]);
                    vm.UseProperties = bool.Parse(Request.Form["UseProperties"]);
                    vm.Example = Request.Form["Example"];

                    var gen = new JsonClassGenerator();
                    gen.UsePascalCase = vm.UsePascalCase;
                    gen.UseProperties = vm.UseProperties;
                    gen.Example = vm.Example;
                    gen.MainClass = "MyJsonObject";

                    gen.GenerateClasses();
                    
                    
                    csharpCodeWriter.WriteFileStart(gen, resultWriter);
                    foreach (var type in gen.Types)
                    {
                        csharpCodeWriter.WriteClass(gen, resultWriter, type);
                    }
                    csharpCodeWriter.WriteFileEnd(gen, resultWriter);

                    apiRequest.status = 0;
                    apiRequest.data = resultWriter.ToString();
                }
                catch(Exception ex)
                {
                    apiRequest.status = -1;
                    apiRequest.debugmsg = ex.Message;
                }
            }
            else
            {
                //invalid request
                apiRequest.status = -1;
                apiRequest.message = "Invalid Request";
            }

            return new JsonResult(apiRequest);
        }

        public class APIRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public string stacktrace { get; set; }
            public string debugmsg { get; set; }
            public string data { get; set; }
        }
    }
}