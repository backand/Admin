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
    public interface ITableConverter
    {
        string Convert(Durados.View view, DataView dataView, Dictionary<string, object> nameValueDictionary);
    }
}
