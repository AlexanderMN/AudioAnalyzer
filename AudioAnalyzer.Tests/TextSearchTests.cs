using System.Collections;
using FluentAssertions;
using TextAnalysis;
using Xunit;

namespace AudioAnalyzer.Tests;

public class TextSearchTests
{
    public static IEnumerable<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            "В лесу родилась елочка", "о", TextSearchToken.Word,
            new TextSearchResult[]
            {
                new TextSearchResult
                {
                    EndIndex = 9,
                    StartIndex = 8,
                    Text = "o"
                },
                new TextSearchResult
                {
                    EndIndex = 19,
                    StartIndex = 18,
                    Text = "о"
                }
            }
        };
    }
    
    [Theory]
    [MemberData(nameof(GetEnumerator))]
    public void SearchText(string text, string word, TextSearchToken token, TextSearchResult[] expectedResults)
    {
        TextSearch textSearch = new TextSearch();
        var result = textSearch.Search(text, string.Intern(word), token);
        var array = result.ToArray();
        expectedResults.Should().BeEquivalentTo(array);
        
    }
}