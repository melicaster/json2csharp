#pragma checksum "C:\Users\melicaster7540\source\repos\melicaster\json2csharp\Pages\index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6b28388f1895d9f8cf9ad20691e3db711c870930"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Pages_index), @"mvc.1.0.razor-page", @"/Pages/index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6b28388f1895d9f8cf9ad20691e3db711c870930", @"/Pages/index.cshtml")]
    public class Pages_index : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "C:\Users\melicaster7540\source\repos\melicaster\json2csharp\Pages\index.cshtml"
  
    Layout = null;

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<!doctype html>\r\n<html lang=\"en\">\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "6b28388f1895d9f8cf9ad20691e3db711c8709302882", async() => {
                WriteLiteral(@"
    <!-- Required meta tags -->
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=no"">
    <meta name=""keywords"" content=""c#,json,converter,netcore,webapi"">
    <meta name=""description"" content=""a .netcore based webinterface for the JSON Class Generator"">
    <!-- Bootstrap CSS -->
    <link rel=""stylesheet"" href=""https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css"" integrity=""sha384-Vkoo8x4CGsO3+Hhxv8T/Q5PaXtkKtu6ug5TOeNV6gBiFeWPGFN9MuhOf23Q9Ifjh"" crossorigin=""anonymous"">
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/10.0.1/styles/default.min.css"">
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/github-fork-ribbon-css/0.2.0/gh-fork-ribbon.min.css"" />
    <title>json2csharp</title>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "6b28388f1895d9f8cf9ad20691e3db711c8709304725", async() => {
                WriteLiteral(@"
    <a class=""github-fork-ribbon"" href=""https://github.com/melicaster/json2csharp/"" title=""Fork me on GitHub"">Fork me on GitHub</a>
    <div class=""jumbotron"">
        <h1 class=""display-3"">json to CSharp converter</h1>
        <p class=""lead"">by melicaster, based on .netcore 3.1, jquery 3.5.0 and bootstrap 4.4.1. </p>
    </div>

    <div class=""container"">
        <form id=""inputform"" action=""/api/json2csharp"" method=""post"">
            <div class=""form-group"">
                <textarea class=""form-control""
                          id=""InputJson""
                          style=""min-width: 100%;min-height:400px;""
                          placeholder='{ ""number"" : 123 }\r\n\r\nUse'></textarea>
            </div>
            <div class=""row"">
                <div class=""col-lg-3"">
                    <input type=""submit"" id=""submitButton"" class=""btn btn-primary"">
                </div>
                <div class=""col-lg-3 form-check"">
                    <label class=""form-check-label"">");
                WriteLiteral("\r\n                        <input id=\"InputUsePascalCase\"");
                BeginWriteAttribute("value", " value=\"", 2022, "\"", 2030, 0);
                EndWriteAttribute();
                WriteLiteral(@" class=""form-check-input"" type=""checkbox"">
                        Use PascalCase
                    </label>
                </div>
                <div class=""col-lg-3 form-check"">
                    <label class=""form-check-label"">
                        <input id=""InputUseProperties""");
                BeginWriteAttribute("value", " value=\"", 2328, "\"", 2336, 0);
                EndWriteAttribute();
                WriteLiteral(@" class=""form-check-input"" type=""checkbox"">
                        Use properties
                    </label>
                </div>
                <div class=""col-lg-3 form-check"">
                    <label class=""form-check-label"">
                        <input id=""InputEscapedDoubleQuote""");
                BeginWriteAttribute("value", " value=\"", 2639, "\"", 2647, 0);
                EndWriteAttribute();
                WriteLiteral(@" class=""form-check-input"" type=""checkbox"">
                        Unescape Double Quote
                    </label>
                </div>
            </div>
        </form>
        <div id=""error"" style=""display:none"">
            <p>An error occured converting your json. Please check if you are using valid json.</p>
        </div>
        <div id=""outputContainer"" style=""padding: 5px 0px;display:none"">
            <button class=""btn btn-primary"" id=""copyToClipboardButton"" onclick=""copyToClipboard('#output')"">Copy to clipboard</button>
            <pre>            
            <code class=""csharp"" id=""output""></code>
             </pre>
        </div>
    </div>

    <!-- Optional JavaScript -->
    <!-- jQuery first, then Popper.js, then Bootstrap JS -->
    <script src=""js/jquery-3.5.0.js""></script>
    <script src=""https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js"" integrity=""sha384-Q6E9RHvbIyZFJoft+2mJbHaEWldlvI9IOYy5n3zV9zzTtmI3UksdQRVvoxMfooAo"" crossorigin=""anon");
                WriteLiteral(@"ymous""></script>
    <script src=""https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/js/bootstrap.min.js"" integrity=""sha384-wfSDF2E50Y2D1uUdj0O3uMBJnjuUD4Ih7YwaYd1iqfktj0Uod8GCExl3Og8ifwB6"" crossorigin=""anonymous""></script>
    <script src=""https://cdnjs.cloudflare.com/ajax/libs/highlight.js/10.0.1/highlight.min.js""></script>
    <script>
        $(""#inputform"").submit(function (event) {
            event.preventDefault();
            var $form = $(this),
                url = $form.attr('action');
            var posting = $.post(url, {
                Example: $('#InputJson').val(),
                UsePascalCase: $('#InputUsePascalCase').is("":checked""),
                UseProperties: $('#InputUseProperties').is("":checked""),
                EscapedDoubleQuote: $('#InputEscapedDoubleQuote').is("":checked"")
            });
            posting.done(function (data) {
                console.log(data);
                if (data.status == 0) {
                    $(""#outputContainer"").show();
     ");
                WriteLiteral(@"               $(""#output"").html(data.data);
                    $(""#error"").hide();
                    $('pre code').each(function (i, block) {
                        hljs.highlightBlock(block);
                    });
                }
                else {
                    $(""#outputContainer"").hide();
                    $(""#error"").show();
                }
            });

            posting.fail(function (data) {
                $(""#outputContainer"").hide();
                $(""#error"").show();
            });
        });


        // see https://stackoverflow.com/questions/22581345/click-button-copy-to-clipboard-using-jquery
        function copyToClipboard(element) {
            var $temp = $(""<input>"");
            $(""body"").append($temp);
            $temp.val($(element).text()).select();
            document.execCommand(""copy"");
            $temp.remove();
        };
    </script>
");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Pages_index> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<Pages_index> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<Pages_index>)PageContext?.ViewData;
        public Pages_index Model => ViewData.Model;
    }
}
#pragma warning restore 1591
