using System;
using System.Xml;
using Microsoft.Translator.net.bing.api;

namespace Microsoft.Translator
{
    public class Translator
    {
        const string AppId = "9DCE2C3C0AE06BFBCEC2148055E75D9139048099";

        public static string Translate(string sourceLanguageCode, string targetLanguageCode, string text)
        {
            using (BingService service = new BingService())
            {
                try
                {
                    SearchRequest request = BuildRequest(text, sourceLanguageCode, targetLanguageCode);

                    // Send the request; display the response.
                    SearchResponse response = service.Search(request);
                    return Response(response);
                }
                catch (System.Web.Services.Protocols.SoapException)
                {
                    // A SOAP Exception was thrown. Display error details.
                    return null;
                }
                catch (System.Net.WebException)
                {
                    // An exception occurred while accessing the network.
                    return null;
                }
            }
        }

        static SearchRequest BuildRequest(string text, string sourceLanguageCode, string targetLanguageCode)
        {
            SearchRequest request = new SearchRequest();

            // Common request fields (required)
            request.AppId = AppId;
            request.Query = text;
            request.Sources = new SourceType[] { SourceType.Translation };

            // SourceType-specific fields (required)
            request.Translation = new TranslationRequest();
            request.Translation.SourceLanguage = sourceLanguageCode;
            request.Translation.TargetLanguage = targetLanguageCode;

            // Common request fields (optional)
            request.Version = "2.2";

            return request;
        }

        static string Response(SearchResponse response)
        {
            if (response.Translation.Results.Length > 0)
                return response.Translation.Results[0].TranslatedTerm;
            else
                return null;
        }
    }
}
