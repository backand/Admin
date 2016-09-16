using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.Search
{
    public class SnippetConfigFieldSearcher : ConfigFieldSearcher
    {
        protected override object GetSnippet(System.Data.DataRowView row, string q, int snippetLength)
        {
            string snippet = base.GetSnippet(row, q, snippetLength).ToString();
            if (snippet.Length < snippetLength)
                return snippet;

            string firstWord = q.Split('%')[0];
            int i = snippet.IndexOf(firstWord, StringComparison.OrdinalIgnoreCase);

            int halfSnippetLength = snippetLength / 2;

            string firstHalf = snippet.Substring(0, i);
            if (i > halfSnippetLength)
            {
                firstHalf = snippet.Substring(i - halfSnippetLength, halfSnippetLength);
            }

            int secondHalfStart = i + firstWord.Length;

            string secondHalf;

            if (snippet.Length < secondHalfStart + halfSnippetLength)
            {
                secondHalf = snippet.Substring(secondHalfStart, snippet.Length - secondHalfStart);
            }
            else
            {
                secondHalf = snippet.Substring(secondHalfStart, halfSnippetLength);
            }

            return firstHalf + firstWord + secondHalf;
        }
    }
}
