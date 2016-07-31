using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using WebSupergoo.ABCpdf7;

namespace Durados.Xml
{
    public class OpenXml
    {
        const string wordmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        public void CreateDocument(string newFile, string template)
        {
            CreateDocument(newFile, template, null);
        }

        public void CreateDocument(string newFile, string template, Dictionary<string, object> values)
        {
            if (template != newFile)
                File.Copy(template, newFile, true);

            using (WordprocessingDocument theDoc = WordprocessingDocument.Open(newFile, true))
            {
                MainDocumentPart mainPart = theDoc.MainDocumentPart;

                UpdateAllBlocksOld(mainPart, values);
                UpdateAllBlocksNew(mainPart, values);

                mainPart.Document.Save();

                theDoc.Close();
            }

        }

        private void UpdateAllBlocksOld(MainDocumentPart mainPart, Dictionary<string, object> values)
        {
            UpdateAllBlocks(mainPart, values, "w:p", "w:name", true);
        }

        private void UpdateAllBlocksNew(MainDocumentPart mainPart, Dictionary<string, object> values)
        {
            UpdateAllBlocks(mainPart, values, "w:sdt", "w:tag", false);
        }

        private void UpdateAllBlocks(MainDocumentPart mainPart, Dictionary<string, object> values, string parentNodeName, string tagNodeName, bool legacy)
        {

            // Manage namespaces to perform XML XPath queries.
            NameTable nt = new NameTable();
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
            nsManager.AddNamespace("w", wordmlNamespace);

            // Get the document part from the package.
            // Load the XML in the part into an XmlDocument instance.
            XmlDocument xdoc = new XmlDocument(nt);
            xdoc.LoadXml(mainPart.Document.Body.OuterXml);

            XmlNodeList nodes = xdoc.SelectNodes("//" + parentNodeName, nsManager);

            foreach (XmlNode parentNode in nodes)
            {
                XmlNode tagNode = parentNode.SelectSingleNode("descendant::" + tagNodeName, nsManager);
                if (tagNode != null)
                {
                    string key = tagNode.Attributes["w:val"].Value;
                    if (legacy)
                    {
                        key = key.Replace("__", ".");
                    }

                    if (values.ContainsKey(key))
                    {
                        string value = values[key].ToString();

                        if (IsMultiLine(value))
                        {
                            XmlNode sdtContent = parentNode.SelectSingleNode("descendant::w:sdtContent", nsManager);
                            XmlNode rNode = sdtContent.SelectSingleNode("descendant::w:r", nsManager);
                            if (rNode != null)
                            {
                                string[] lines = GetLines(value);

                                string innerXml = string.Empty;
                                foreach (string line in lines)
                                {
                                    string outerXml = rNode.OuterXml;

                                    if (!line.Equals(lines.First()))
                                    {
                                        string br = "<w:r w:rsidR=\"00A3159A\"><w:rPr><w:rFonts w:ascii=\"Tahoma\" w:hAnsi=\"Tahoma\" w:cs=\"Tahoma\" /><w:b /><w:bCs /><w:sz w:val=\"20\" /><w:szCs w:val=\"20\" /><w:lang w:val=\"en-US\" w:bidi=\"he-IL\" /></w:rPr><w:br /></w:r>";
                                        outerXml = br + outerXml.Replace("<w:t>", "<w:t xml:space=\"preserve\">");
                                    }

                                    innerXml += outerXml;
                                }

                                sdtContent.InnerXml = innerXml;

                                XmlNodeList valNodes = sdtContent.SelectNodes("descendant::w:t", nsManager);

                                for (int i = 0; i < lines.Length; i++)
                                {
                                    valNodes[i].InnerXml = lines[i];
                                }

                                //foreach (string line in lines)
                                //{

                                //    XmlNode node = rNode.Clone();

                                //    node.InnerXml = rNode.InnerXml;

                                //    sdtContent.AppendChild(node);

                                //    XmlNode valNode = node.SelectSingleNode("descendant::w:t", nsManager);
                                //    if (valNode != null)
                                //    {
                                //        valNode.InnerXml = line;
                                //        if (!line.Equals(lines.First()))
                                //        {
                                //            XmlAttribute attribute = xdoc.CreateAttribute("xml", "space", null);
                                //            attribute.Value = "preserve";
                                //            valNode.Attributes.Append(attribute);
                                //        }
                                //    }
                                //}
                            }
                        }
                        else
                        {

                            XmlNode valNode = parentNode.SelectSingleNode("descendant::w:t", nsManager);
                            if (valNode != null)
                            {
                                valNode.InnerXml = value;
                            }
                        }
                    }
                }
            }

            mainPart.Document.Body.InnerXml = xdoc.InnerXml;
        }

        private bool IsMultiLine(string s)
        {
            return s.Contains('\n');
        }

        private string[] GetLines(string s)
        {
            return s.Split('\n');
        }

        //private void UpdateAllBlocks(MainDocumentPart mainPart, Dictionary<string, string> values)
        //{

        //    // Manage namespaces to perform XML XPath queries.
        //    NameTable nt = new NameTable();
        //    XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
        //    nsManager.AddNamespace("w", wordmlNamespace);

        //    // Get the document part from the package.
        //    // Load the XML in the part into an XmlDocument instance.
        //    XmlDocument xdoc = new XmlDocument(nt);
        //    xdoc.LoadXml(mainPart.Document.Body.OuterXml);

        //    foreach (string key in values.Keys)
        //    {
        //        XmlNode node = xdoc.SelectSingleNode("//w:t[ancestor::w:sdt[descendant::w:tag[@w:val='" + key + "']]]", nsManager);
        //        if (node != null)
        //        {
        //            node.InnerXml = values[key];
        //        }
        //        //XmlNodeList nodes = xdoc.SelectNodes("//w:sdt", nsManager);
        //        //XmlNode node = xdoc.SelectSingleNode("//w:tag[@w:val='NAME2']", nsManager);
        //    }

        //    mainPart.Document.Body.InnerXml = xdoc.InnerXml;
        //}


        public void DocToPdfOffice2010(string inputDoc, string outputPdf)
        {
            if (string.IsNullOrEmpty(inputDoc))
            {
                throw new DuradosException("The word input file name was not supplied");
            }
            if (string.IsNullOrEmpty(outputPdf))
            {
                throw new DuradosException("The pdf output file name was not supplied");
            }
            try
            {
                MSOffice msOffice = new MSOffice();
                msOffice.SaveToPdf(inputDoc, outputPdf);
            }
            catch (Exception exception)
            {
                throw new DuradosException("The word to pdf conversion was failed. Word input file: " + inputDoc, exception);
            }
        }

        public void DocToPdf(string inputDoc, string outputPdf)
        {

            using (Doc theDoc = new Doc())
            {
                FileStream file = null;
                try
                {
                    theDoc.Read(inputDoc);

                    byte[] theData = theDoc.GetData();
                    file = new FileStream(outputPdf, FileMode.Create);
                    
                    file.Write(theData, 0, theData.Length);
                }
                catch (Exception)
                {
                    File.Copy(inputDoc, outputPdf, true);
                    //throw ex;
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
        }


        
    }
}
