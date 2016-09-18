using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.Search
{
    public class SnippetConfigFieldSearcher : ConfigFieldSearcher
    {
        protected override object GetSnippets(System.Data.DataRowView row, string q, int snippetLength, string highlightTag, int tabChars)
        {
            string snippet = base.GetSnippets(row, q, snippetLength, highlightTag, tabChars).ToString();
            
            Line[] lines = GetLines(snippet);

            List<object> snippets = new List<object>();
            string firstWord = q.Split('%')[0];
            int start = 0;
            int i = 0;
            while (start < snippet.Length)
            {
                i = snippet.IndexOf(firstWord, start, StringComparison.OrdinalIgnoreCase);

                if (i < 0)
                    break;

                Line line = GetLine(lines, i);
                snippets.Add(new { snippet = GetSnippet(snippetLength, snippet, firstWord, highlightTag, i), line = line.Number, column = GetColumn(i, line, tabChars) });

                start = i + firstWord.Length;
            }

            return snippets.ToArray();
        }

        private static int GetColumn(int i, Line line, int tabChars)
        {
            int tabs = line.Text.CountOccurrences("\t");
            return i - line.Start - tabs + tabs * tabChars;
        }

        private string GetSnippet(int snippetLength, string snippet, string firstWord, string highlightTag, int i)
        {
            if (snippet.Length < snippetLength)
                return snippet;

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

            return firstHalf + highlightTag + firstWord + GetClosedTag(highlightTag) + secondHalf;
        }

        private string GetClosedTag(string highlightTag)
        {
            if (string.IsNullOrEmpty(highlightTag))
                return string.Empty;
            return "</" + highlightTag.TrimStart('<');
        }

        private Line[] GetLines(string text)
        {
            List<Line> lines = new List<Line>();

            int prevLine = 0;

            int number = 0;

            foreach (string line in text.Split('\n'))
            {
                lines.Add(new Line() { Start = prevLine, Number = ++number, Text = line });
                prevLine += (line.Length + 1);
            }

            return lines.ToArray();
        }

        private Line GetLine(Line[] lines, int pos)
        {
            Line prev = null;
            foreach (Line line in lines)
            {
                if (line.Start > pos)
                    break;
                prev = line;
            }

            return prev;
            //return ((Line)lines.Aggregate((current, next) => ((Line)current).Start > pos ? current : next));
        }

        public class Line
        {
            public int Start { get; set; }
            public int Number { get; set; }

            public string Text { get; set; }
        }
    }
}
