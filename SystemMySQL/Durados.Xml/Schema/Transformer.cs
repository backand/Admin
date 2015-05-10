using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml;
using System.IO;

namespace Durados.Xml.Schema
{
    public class Transformer
    {
        public void Tranform(string inputFile, string transformationFile, string outputFile, TransformationType transformationType)
        {
            switch (transformationType)
            {
                case TransformationType.Xslt:
                    TranformXslt(inputFile, transformationFile, outputFile);
                    break;

                default:
                    break;
            }
        }

        protected virtual void TranformXslt(string xml, string xsl, string outputFile)
        {
            //XmlTextWriter writer = null;
            string guid = "." + Guid.NewGuid().ToString();
            string tempOutputFile = outputFile + guid;
            //try
            //{
                // XPathDocument myXPathDocument = new XPathDocument(xml);//loading the original xml

                System.Xml.Xsl.XslCompiledTransform myXslTransform = new System.Xml.Xsl.XslCompiledTransform();

                //writer = new XmlTextWriter(outputFile,null);//loading the output xml


                try
                {
                    myXslTransform.Load(xsl);//loading xslt file to transformer
                }
                catch (ArgumentNullException)
                {
                    throw new DuradosException("The xml transformation failed while loading the xsl. Xsl file was not provided.");
                }
                catch (XsltException exception)
                {
                    throw new DuradosException("The xml transformation failed while loading the xsl. " + exception.Message, exception);
                }
                catch (FileNotFoundException)
                {
                    throw new DuradosException("The xml transformation failed while loading the xsl. The xsl file: " + xsl + " was not found");
                }
                catch (UriFormatException exception)
                {
                    throw new DuradosException("The xml transformation failed while loading the xsl. The xsl file: " + xsl + " format has errors", exception);
                }
                catch (XmlException exception)
                {
                    throw new DuradosException("The xml transformation failed while loading the xsl. The xsd file: " + xml + " format has errors", exception);
                }
                catch (Exception exception)
                {
                    throw new DuradosException("The xml transformation failed while loading the xsl.", exception);
                }



                try
                {

                    myXslTransform.Transform(xml, tempOutputFile);

                    // myXslTransform.Transform(myXPathDocument, null, writer);
                }
                catch (XsltException exception)
                {
                    throw new DuradosException("The xml transformation failed while transform to xml. " + exception.Message, exception);
                }
                catch (XmlException exception)
                {
                    throw new DuradosException("The xml transformation failed while transform to xml.", exception);
                }


                try
                {
                    if (xml == outputFile)
                    {
                        System.IO.File.Delete(xml);
                    }
                    System.IO.File.Move(tempOutputFile, outputFile);
                }
               
                catch (ArgumentNullException)
                {
                    throw new DuradosException("The xml transformation failed while writing the output xml. output file was not provided.");
                }

                catch (FileNotFoundException)
                {
                    throw new DuradosException("The xml transformation failed while writing the output xml. The output file: " + outputFile + " was not found");
                }
                catch (DirectoryNotFoundException)
                {
                    throw new DuradosException("The xml transformation failed while writing the output xml. The output directory of: " + outputFile + " was not found");
                }
                catch (UnauthorizedAccessException  exception)
                {
                    throw new DuradosException("The xml transformation failed while writing the output xml. :Unauthorized Access to " + outputFile + " or " + xml,exception);
                }
              



        }

        private bool IsXsltFileExists(string xsltFileName)
        {
            return (string.IsNullOrEmpty(xsltFileName) && !System.IO.File.Exists(xsltFileName));

        }
        
    }

    public enum TransformationType
    {
        Xslt
    }
}
