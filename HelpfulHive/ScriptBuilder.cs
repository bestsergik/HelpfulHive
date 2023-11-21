using Microsoft.JSInterop;
using System.Text.RegularExpressions;
using System.Text;
using HtmlAgilityPack;

namespace HelpfulHive
{
    public class ScriptBuilder
    {
        private readonly IJSRuntime _jsRuntime;
        private StringBuilder finalScript = new StringBuilder();
        private string valueBuffer = "";
        private string listUINs = "";
        private string topScript = "";

        public ScriptBuilder(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task BuildScriptAsync(string script)
        {
            script = ExtractTextFromHtml(script);
            if (script.Contains("select") || script.Contains("{NUMERIC}") || script.Contains("{NUMERIC_VARCHAR}") || script.Contains("{SPECIFIC_VARCHAR}") || script.Contains("SELECT") || script == "comma")
            {
                topScript = script;
                valueBuffer = await _jsRuntime.InvokeAsync<string>("getClipboardText");
              
                await GetScriptAsync();
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("copyTextToClipboard", script);
            }
        }

        public static string ExtractTextFromHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var text = htmlDoc.DocumentNode.InnerText;

            // Заменяем HTML-сущность &nbsp; на обычный пробел
            text = text.Replace("&nbsp;", "");

            return text;
        }
        private async Task GetScriptAsync()
        {
            try
            {
                listUINs = GetUINsWithHyphens(valueBuffer);

                if (string.IsNullOrEmpty(listUINs))
                {
                    listUINs = GetUINs(valueBuffer);
                }
                if (string.IsNullOrEmpty(listUINs))
                {
                    listUINs = GetIds(valueBuffer);
                }
                if (string.IsNullOrEmpty(listUINs))
                {
                    listUINs = GetUINsWithHyphensAndAdditionalFormat(valueBuffer);
                }

                // Проверка на необходимость переноса всего результата listUINs на новую строку
                if (!string.IsNullOrEmpty(listUINs))
                {
                    listUINs = Environment.NewLine + listUINs;
                }

                if (topScript == "comma")
                {
                    finalScript.Append(listUINs);
                }
                else if (topScript.Contains("NUMERIC_VARCHAR"))
                {
                    listUINs = GetUINsWithVariableLength(valueBuffer);
                    finalScript.Append(listUINs);
                }
                else if (!string.IsNullOrEmpty(listUINs))
                {
                    finalScript.Append(ReplaceTokens(topScript, listUINs));
                }

                await _jsRuntime.InvokeVoidAsync("copyTextToClipboard", finalScript.ToString());
            }
            catch (Exception ex)
            {
                // Здесь можно добавить логирование исключения, если это необходимо
                // Например: Console.WriteLine(ex.ToString());
            }
            finalScript.Clear();
        }



        private string GetUINs(string input)
        {
            var uinPattern = new Regex(@"(?<first>.)-\d{2}-\d{10}-\d");
            var uins = uinPattern.Matches(input).OfType<Match>().Select(m => "'" + m.Groups["first"].Value + m.Value + "'").Distinct();

            if (!uins.Any())
            {
                uinPattern = new Regex(@"\d{16}");
                uins = uinPattern.Matches(input).OfType<Match>().Select(m => m.Value).Distinct();
            }
            return string.Join(",", uins);
        }

        private string ReplaceTokens(string input, string uins)
        {
            using (TextReader tr = new StringReader(input))
            {
                var output = new StringWriter();
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    line = line.Replace("{NUMERIC}", uins.Contains("'") ? "1" : uins);
                    line = line.Replace("{SPECIFIC_VARCHAR}", uins.Contains("'") ? uins : "''");
                    output.WriteLine(line);
                }
                return output.ToString();
            }
        }

        private string GetUINsWithHyphens(string input)
        {
            List<string> uins = new List<string>();
            int index = 0;

            while (index < input.Length)
            {
                int hyphenIndex = input.IndexOf('-', index);
                if (hyphenIndex == -1 || hyphenIndex < 1) break;

                if (hyphenIndex + 13 < input.Length && input[hyphenIndex + 3] == '-' && input[hyphenIndex + 13] == '-')
                {
                    char firstLetter = input[hyphenIndex - 1];
                    string replacedFirstLetter = DefineFirstLetter(firstLetter);
                    string uin = $"'{replacedFirstLetter}{input.Substring(hyphenIndex, 16)}'";

                    if (!uins.Contains(uin))
                    {
                        uins.Add(uin);
                    }

                    index = hyphenIndex + 16;
                }
                else
                {
                    index = hyphenIndex + 1;
                }
            }

            return string.Join(",", uins);
        }

        private string GetUINsWithVariableLength(string input)
        {
   
            var uinPattern = new Regex(@"\d+");
            var uins = uinPattern.Matches(input).OfType<Match>().Select(m => $"'{m.Value}'").Distinct();

        
            var result = string.Join(",", uins);
            return result;
        }




        private string GetUINsWithHyphensAndAdditionalFormat(string input)
        {
            var uuidPattern = new Regex(@"\b[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\b(?!\.png)");
            var matches = uuidPattern.Matches(input);

            var uins = matches.Cast<Match>()
                              .Select(m => $"'{m.Value}'")
                              .Distinct();

            return string.Join(",", uins);
        }


        private string GetIds(string input)
        {
            var idPattern = new Regex(@"\d{14}");
            var ids = idPattern.Matches(input).OfType<Match>().Select(m => m.Value).Distinct();

            return string.Join(",", ids);
        }


        private string DefineFirstLetter(char firstLetter)
        {
            char[] russianLetters = { 'А', 'В', 'Е', 'К', 'М', 'Н', 'О', 'Р', 'С', 'Т', 'Х' };
            char[] englishLetters = { 'A', 'B', 'E', 'K', 'M', 'H', 'O', 'P', 'C', 'T', 'X' };

            for (int i = 0; i < russianLetters.Length; i++)
            {
                if (char.ToUpper(firstLetter) == russianLetters[i])
                {
                    return englishLetters[i].ToString();
                }
            }
            return char.ToUpper(firstLetter).ToString();
        }
    }
}
