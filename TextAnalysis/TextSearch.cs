using System.Text.RegularExpressions;

namespace TextAnalysis;

public enum TextSearchToken
{
    Word,
    Regex,
    Text
}

public class TextSearch
{
    public List<TextSearchResult> Search(string text, string input, TextSearchToken token = TextSearchToken.Word)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(text))
            return new List<TextSearchResult>();
        
        switch (token)
        {
            case TextSearchToken.Word:
                return SearchForWord(text, input);
            case TextSearchToken.Regex:
                return SearchForRegex(text, input);
            default:
                return SearchForText(text, input);
        }
    }

    private List<TextSearchResult> SearchForWord(string text, string word)
    {
        List<TextSearchResult> results = new List<TextSearchResult>();
        
        var spanText = text.AsSpan();

        int start, offset = 0;
        while ((start = spanText.IndexOf(word, StringComparison.Ordinal)) != -1)
        {
            int nextSpanIndex = start + word.Length; 
            TextSearchResult result = new TextSearchResult
            {
                StartIndex = start + offset,
                EndIndex =  nextSpanIndex + offset,
                Text = word
            };

            results.Add(result);
            offset += result.EndIndex;
            spanText = spanText.Slice(nextSpanIndex);
        }
        
        return results;
    }

    private List<TextSearchResult> SearchForRegex(string text, string pattern)
    {
        List<TextSearchResult> results = new List<TextSearchResult>();
        Regex regex = new Regex(pattern);

        if (regex.IsMatch(text))
        {
            var matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                TextSearchResult result = new TextSearchResult
                {
                    StartIndex = match.Index,
                    EndIndex = match.Index + match.Length,
                    Text = match.Value
                };
                
                results.Add(result);
            }
        }
        
        return results;
    }

    private List<TextSearchResult> SearchForText(string text, string textToSearch)
    {
        return Search(text, textToSearch);
    }
}