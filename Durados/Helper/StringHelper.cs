using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using System.Linq;
using Durados;
using System.IO;
using System.Security.Cryptography;


namespace System
{
    public static class StringHelper
    {

        private static readonly char[] whitespace = new char[] { ' ', '\n', '\t', '\r', '\f', '\v' };
        private static readonly string inclose = "'";
        public static string StripInvisibles(this string source)
        {
            return String.Join(" ", source.Split(whitespace, StringSplitOptions.RemoveEmptyEntries));
        }

        public static int CountOccurrences(this string sentence, string word)
        {
            return new Regex(word).Matches(sentence).Count;
        }

        public static string ReplaceNonAlphaNumeric(this string name, char replacement = '_')
        {
            return name.Replace(' ', replacement).Replace('-', replacement).Replace('+', replacement).Replace('/', replacement).Replace('*', replacement).Replace('.', replacement).Replace('=', replacement).Replace('!', replacement).Replace('@', replacement).Replace('#', replacement).Replace('$', replacement).Replace('%', replacement).Replace('^', replacement).Replace('^', replacement).Replace('&', replacement);
        }

        public static string ReplaceNonAlphaNumeric2(this string name, string replacement = "_")
        {
            return name.Replace(" ", replacement).Replace("-", replacement).Replace("+", replacement).Replace("/", replacement).Replace("*", replacement).Replace(".", replacement).Replace("=", replacement).Replace("!", replacement).Replace("@", replacement).Replace("#", replacement).Replace("$", replacement).Replace("%", replacement).Replace("^", replacement).Replace("&", replacement);
        }

        public static string LowerFirstChar(this string name)
        {
            return Char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
        public static string GetLastWordUpToHere(this string source, int here, out int start, char[] seperators)
        {
            char prevChar = char.MinValue;

            string word = string.Empty;
            HashSet<char> seperatorsHash = new HashSet<char>();
            int index = here;

            while (!seperatorsHash.Contains(prevChar))
            {
                index--;
                prevChar = source[index];

                word = prevChar + word;
            }

            start = index;

            return word;
        }

        private static readonly char[] breaks = new char[] { '[', ']' };
        public static string StripBreaks(this string source)
        {
            return String.Join("", source.Split(breaks, StringSplitOptions.RemoveEmptyEntries));
        }

        public static string Replace(this string template, Dictionary<string, string> nameValueDictionary)
        {
            string content = template;
            foreach (string name in nameValueDictionary.Keys)
            {
                string value = nameValueDictionary[name];
                content = content.Replace(name, value);
            }

            return content;
        }

        public static string Replace(this string template, int currentUserId)
        {
            return template.Replace(Durados.Database.UserPlaceHolder, currentUserId.ToString(), false);
        }

        public static string Replace(this string template, Dictionary<string, object> nameValueDictionary)
        {
            return Replace(template, nameValueDictionary, null, null);
        }

        public static string ReplaceToken(this string content, string name, object value)
        {
            return content.Replace(name, value.ToString()).Replace(name = name.TrimStart('[').TrimEnd(']').AsToken(), value.ToString());
        }

        public static string Replace(this string template, Dictionary<string, object> nameValueDictionary, ITableConverter tableConverter, Durados.View view)
        {
            if (nameValueDictionary == null)
                return template;

            AdjustToShortDictionary(nameValueDictionary);

            string content = template;
            foreach (string name in nameValueDictionary.Keys)
            {
                if (!name.IsToken())
                    continue;

                object value = nameValueDictionary[name];
                if (value is string)
                {
                    content = content.ReplaceToken(name, value.ToString());
                }
                else if (value == null)
                {
                    content = content.ReplaceToken(name, string.Empty);
                }
                else if (value is DataView)
                {
                    if (tableConverter == null || view == null)
                    {
                        content = content.ReplaceToken(name, string.Empty);
                    }
                    else
                    {
                        if (content.Contains(name))
                        {
                            Durados.View childrenView = GetChildrenView(view, name);
                            if (childrenView != null)
                            {
                                content = content.ReplaceToken(name, tableConverter.Convert(childrenView, (DataView)value, nameValueDictionary));
                            }
                            else
                            {
                                content = content.ReplaceToken(name, value.ToString());
                            }
                        }
                    }
                }
                else
                {
                    content = content.ReplaceToken(name, value.ToString());
                }
            }

            return content;
        }

        private static bool IsToken(this string token)
        {
            return token.StartsWith("{{") || token.StartsWith("[");
        }
        public static string ReplaceAllTokens(this string template, View view, Dictionary<string, object> values, string pk, string currentUsetId, string currentUsername, string currentUserRole, DataRow prevRow)
        {
            string content = template.Replace("[pk]", pk)
                 .Replace(Durados.Database.UserPlaceHolder, currentUsetId, false).Replace(Durados.Database.SysUserPlaceHolder.AsToken(), currentUsetId, false)
                 .Replace(Durados.Database.UsernamePlaceHolder, currentUsername, false).Replace(Durados.Database.SysUsernamePlaceHolder.AsToken(), currentUsername)
                 .Replace(Durados.Database.RolePlaceHolder, currentUserRole, false).Replace(Durados.Database.SysRolePlaceHolder.AsToken(), currentUserRole);

            content = (content.IndexOf("#") == -1 && content.IndexOf(Database.DictionaryPrefix + Database.SysPrevPlaceHolder) == -1) ? content : content.ReplaceWithSharp(view, null, prevRow);
            content = (content.IndexOf("$") == -1 && content.IndexOf(Database.DictionaryPrefix) == -1) ? content : content.ReplaceWithDollar(view, values, prevRow);

            //content = ReplaceGlobals(content, view);
            content = ReplaceConfig(content, view);

            return content;
        }

        public static string ReplaceConfig(this string template, View view)
        {
            return ReplaceConfig(template, view.Database);
        }

        public static string ReplaceConfig(this string template, Database database)
        {
            string s = template;

            try
            {
                var dic = database.GetConfigDictionary();
                foreach (string key in dic.Keys)
                {
                    string name = key;
                    object value = dic[key];

                    if (!(value is IDictionary<string, object>) && !(value is IEnumerable<object>))
                    {
                        s = s.Replace((Database.ConfigPlaceHolder + name).AsToken(), value.ToString());
                    }
                }
            }
            catch { }
            return s;
        }

        //public static string ReplaceGlobals(this string template, View view)
        //{
        //    return ReplaceGlobals(template, view.Database);
        //}

        //public static string ReplaceGlobals(this string template, Database database)
        //{
        //    string s = template;

        //    try
        //    {
        //        foreach (int key in database.Globals.Keys)
        //        {
        //            string name = database.Globals[key].Name;
        //            string value = database.Globals[key].Value;

        //            s = s.Replace((Database.SysPlaceHolder + name).AsToken(), value);
        //        }
        //    }
        //    catch { }
        //    return s;
        //}

        private static Durados.View GetChildrenView(Durados.View view, string name)
        {
            name = name.TrimStart('[').TrimEnd(']');

            string[] names = name.Split('.');

            if (names.Length <= 1)
                return null;
            Durados.View root = view.Database.GetView(names[0]);
            if (root == null)
                return null;

            Durados.View parentView = root;

            Durados.Field[] fields = null;
            Durados.Field field = null;
            for (int i = 1; i < names.Length - 1; i++)
            {
                fields = parentView.GetFieldsByDisplayName(names[i]);

                if (fields == null || fields.Length != 1)
                    return null;

                field = fields[0];

                if (field.FieldType != Durados.FieldType.Parent)
                    return null;

                parentView = ((Durados.ParentField)field).ParentView;
            }

            fields = parentView.GetFieldsByDisplayName(names[names.Length - 1]);
            if (fields == null || fields.Length != 1)
                return null;
            field = fields[0];

            if (field.FieldType != Durados.FieldType.Children)
                return null;

            return ((Durados.ChildrenField)field).ChildrenView;
        }

        private static void AdjustToShortDictionary(Dictionary<string, object> nameValueDictionary)
        {
            if (nameValueDictionary == null)
                return;

            if (nameValueDictionary.Count == 0)
                return;

            Dictionary<string, object> shortNameValueDictionary = new Dictionary<string, object>();

            foreach (string name in nameValueDictionary.Keys)
            {
                string shortName = GetShortName(name);
                if (!string.IsNullOrEmpty(shortName))
                    if (!shortNameValueDictionary.ContainsKey(shortName))
                        shortNameValueDictionary.Add(shortName, nameValueDictionary[name]);
            }

            foreach (string shortName in shortNameValueDictionary.Keys)
            {
                if (!nameValueDictionary.ContainsKey(shortName))
                    nameValueDictionary.Add(shortName, shortNameValueDictionary[shortName]);
            }

        }

        private static string GetShortName(string name)
        {
            int firstPointIndex = name.IndexOf('.');

            if (!name.StartsWith("[") || firstPointIndex < 1)
                return null;

            string shortName = name.Remove(1, firstPointIndex);

            return shortName;
        }

        public static string ReplaceWithDollar(this string template, Durados.View view, Dictionary<string, object> nameValueDictionary)
        {
            if (nameValueDictionary == null)
                return template;

            string content = template;
            foreach (string name in nameValueDictionary.Keys)
            {
                object value = nameValueDictionary[name];
                if (value is string)
                {
                    string s;
                    if (view.Fields.ContainsKey(name) && view.Fields[name].FieldType == Durados.FieldType.Column && view.Fields[name].GetColumnFieldType() == Durados.ColumnFieldType.Boolean && value.Equals(string.Empty))
                        s = "False";
                    else
                        s = value.ToString();
                    content = content.ReplaceDictionaryTokens("$", name, s);

                }
                else if (value != null)
                {
                    content = content.ReplaceDictionaryTokens("$", name, value.ToString());

                }
            }

            return content;
        }

        public static string ReplaceDictionaryTokens(this string content, string prefix, string name, string s)
        {
            content = content.Replace(prefix + name, s);
            if (prefix == "#")
                content = content.Replace(name.AsToken(false), s);
            else content = content.Replace(name.AsToken(), s);

            return content;
        }

        public static string AsToken(this string content)
        {
            return content.AsToken(true);

        }

        public static string AsToken(this string content, bool current)
        {
            if (content.StartsWith("{{"))
                return content;
            string token = Database.DictionaryPrefix + (current ? string.Empty : Database.SysPrevPlaceHolder) + content + Database.DictionaryPostfix;
            return token;

        }
        private static bool ToPad(DataType dataType)
        {
            //if (dataType == null) return false;
            return (dataType == DataType.ShortText || dataType == DataType.Email || dataType == DataType.DateTime || dataType == DataType.Html || dataType == DataType.Image || dataType == DataType.LongText || dataType == DataType.Url);
        }
        public static string Pad(this string content, DataType dataType)
        {
            return ToPad(dataType) ? content.Pad() : content;
        }

        public static string Pad(this string content, int dataTypeId)
        {
            DataType dataType = (DataType)dataTypeId;
            return ToPad(dataType) ? content.Pad() : content;
        }
        public static string Pad(this string content)
        {
            return inclose + content + inclose;
        }

        public static string Pad(this string content, string c)
        {
            return c + content + c;
        }

        public static string ReplaceWithDollar(this string template, Durados.View view, Dictionary<string, object> nameValueDictionary, DataRow row)
        {
            return ReplaceWithChar(template, "$", view, nameValueDictionary, row);
        }

        public static string ReplaceWithSharp(this string template, Durados.View view, Dictionary<string, object> nameValueDictionary, DataRow row)
        {
            return ReplaceWithChar(template, "#", view, nameValueDictionary, row);
        }

        public static string ReplaceWithoutPrefix(this string template, Durados.View view, Dictionary<string, object> nameValueDictionary, DataRow row)
        {
            return ReplaceWithChar(template, "", view, nameValueDictionary, row);
        }

        private static string ReplaceWithChar(this string template, string prefixChar, Durados.View view, Dictionary<string, object> nameValueDictionary, DataRow row)
        {
            if (nameValueDictionary == null)
                nameValueDictionary = new Dictionary<string, object>();

            string content = template;
            if (content.IndexOfAny(new char[] { '$', '#', Database.DictionaryPrefix.ToCharArray()[0] }) == -1)
                return content;
            foreach (Durados.Field field in view.Fields.Values)
            {
                string name = field.Name;
                object value = string.Empty;
                if (nameValueDictionary.ContainsKey(name))
                {
                    value = nameValueDictionary[name];
                    content = ReplaceNameValue(content, name, value, prefixChar, view, field);
                }

                if (row != null)
                {
                    name = view.Name + "." + field.Name;
                    value = field.GetValue(row);
                    content = ReplaceNameValue(content, name, value, prefixChar, view, field);
                }
            }

            return content;
        }

        private static string ReplaceNameValue(string content, string name, object value, string prefixChar, View view, Field field)
        {
            if (value is string)
            {
                string s;
                if (view.Fields.ContainsKey(name) && view.Fields[name].FieldType == Durados.FieldType.Column && view.Fields[name].GetColumnFieldType() == Durados.ColumnFieldType.Boolean && value.Equals(string.Empty))
                    s = "False";
                else

                    s = value.ToString();
                content = content.ReplaceDictionaryTokens(prefixChar, field.JsonName, s);
                content = content.ReplaceDictionaryTokens(prefixChar, name, s);

            }
            else if (value != null)
            {
                content = content.ReplaceDictionaryTokens(prefixChar, field.JsonName, value.ToString());
                content = content.ReplaceDictionaryTokens(prefixChar, name, value.ToString());

            }

            return content;
        }

        public static string Replace(this string template, DataRow row)
        {
            string content = template;
            foreach (DataColumn column in row.Table.Columns)
            {
                object value = row[column];
                string s;
                if (column.DataType == typeof(bool) && value == DBNull.Value)
                    s = "False";
                else
                    s = value.ToString();

                content = content.ReplaceDictionaryTokens("$", column.ColumnName, s);

            }

            return content;
        }

        public static string Delimited(this string[] array)
        {
            return Delimited(array, null, null);
        }

        public static string DelimitedColumns(this string[] array)
        {
            return Delimited(array, "[", "]");
        }

        public static string Delimited(this string[] array, string prefix, string suffix)
        {
            StringBuilder delimited = new StringBuilder();
            foreach (string s in array)
            {
                if (!string.IsNullOrEmpty(prefix))
                    delimited.Append(prefix);
                delimited.Append(s);
                if (!string.IsNullOrEmpty(suffix))
                    delimited.Append(suffix);
                delimited.Append(",");
            }

            return delimited.ToString().TrimEnd(',');
        }

        public static string Delimited(this int[] array)
        {
            StringBuilder delimited = new StringBuilder();
            foreach (int i in array)
            {
                delimited.Append(i.ToString() + ",");
            }

            return delimited.ToString().TrimEnd(','); ;
        }

        public static string Replace(this string sourceString, string oldValue, string newValue, bool caseSensitive)
        {
            if (sourceString == null) throw new ArgumentNullException("sourceString");
            if (oldValue == null) throw new ArgumentNullException("searchString");
            if (String.IsNullOrEmpty(oldValue)) throw new ArgumentException("searchString cannot be an empty string.", "searchString");
            if (newValue == null) throw new ArgumentNullException("replaceString");

            StringBuilder retVal = new StringBuilder(sourceString.Length);

            int ptr = 0, lastPtr = 0;

            while (ptr >= 0)
            {
                ptr = sourceString.IndexOf(oldValue, ptr, caseSensitive ? StringComparison.InvariantCulture : StringComparison.OrdinalIgnoreCase);

                int strLength = ptr - lastPtr;
                if (strLength > 0 || ptr == 0)
                {
                    if (ptr > 0)
                        retVal.Append(sourceString.Substring(lastPtr, strLength));

                    retVal.Append(newValue);
                    ptr += oldValue.Length;
                }
                else
                    break;

                lastPtr = ptr;
            }

            if (lastPtr >= 0) //Append the piece of the string left after the last occurrence of searchString, if any.
                retVal.Append(sourceString.Substring(lastPtr));

            return retVal.ToString();
        }

        public static string Parse(this string formula, Durados.Field field, Durados.View view)
        {
            return Parse(formula, field, view, null);
        }

        public static string Parse(this string formula, Durados.Field field, Durados.View view, Dictionary<string, Durados.Field> calcDynasty)
        {

            if (view == null)
            {
                view = field.View;
            }
            string formulaDetail = string.Empty;

            if (view == null || view.Fields == null)
            {
                throw new Durados.DuradosException("Could not parse calculted field, missing view or fields.");
            }

            if (field.IsCalculated && !string.IsNullOrEmpty(field.Formula))
            {
                if (calcDynasty == null)
                {
                    calcDynasty = new Dictionary<string, Durados.Field>();
                }
                else
                {
                    if (calcDynasty.ContainsKey(field.DatabaseNames))
                    {
                        string strace = String.Join(",", calcDynasty.Keys.Select(o => o.ToString()).ToArray());//calcDynasty.Keys.;
                        throw new Durados.DuradosException("Calculted field cannot containe recursive call, check " + strace);//calcDynasty.Keys.
                    }
                }

                calcDynasty.Add(field.DatabaseNames, field);

                formulaDetail = string.Format("({0})", field.Formula);

                foreach (Durados.Field f in view.Fields.Values.Where(r => (r.IsCalculated == true && r.DatabaseNames != field.DatabaseNames)).OrderBy(r => r.DatabaseNames.Length))
                {
                    string pattern = @"\b" + f.DatabaseNames + @"\b";
                    if ((Regex.Match(formulaDetail, pattern)).Success)
                    {
                        formulaDetail = formulaDetail.Replace(string.Format("[{0}]", f.DatabaseNames), f.DatabaseNames);
                        Dictionary<string, Durados.Field> subTree = new Dictionary<string, Durados.Field>(calcDynasty);
                        formulaDetail = string.Format("({0})", Regex.Replace(formulaDetail, pattern, f.Formula.Parse(f, view, subTree)));
                    }
                }


            }

            return formulaDetail;
        }

        public static string Decrypt(this string password, Database database)
        {
            return Durados.Security.CipherUtility.Decrypt<System.Security.Cryptography.AesManaged>(password, database.DefaultMasterKeyPassword, database.Salt);
        }

        public static bool OnlyOnce(this string full, string part)
        {
            var first = full.IndexOf(part);
            return first != -1 && first == full.LastIndexOf(part);
        }

        public static string EscapeApostrophe(this string text)
        {
            return text.Replace("'", "''");
        }

        public static string FromBase64String(this string base64, string encodingName = null)
        {
            const int MOD = 4;
            const char EQ = '=';

            Encoding encoding = Encoding.UTF8;
            if (encodingName != null)
                encoding = System.Text.Encoding.GetEncoding(encodingName);

            return encoding.GetString(System.Convert.FromBase64String(base64.PadRight(base64.Length + (MOD - base64.Length % MOD) % MOD, EQ)));
        }
    }
}