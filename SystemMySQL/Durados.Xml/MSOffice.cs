using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.IO;

namespace Durados.Xml
{
    public class MSOffice
    {
        public void SaveToPdf(string wordFileName, object outputFileName)
        {
            try
            {
                SaveToPdf2(wordFileName, outputFileName);
            }
            catch
            {
                try
                {
                    SaveToPdf2(wordFileName, outputFileName);
                }
                catch
                {
                    System.Threading.Thread.Sleep(100);
                    SaveToPdf2(wordFileName, outputFileName);
                }
            }
        }

        protected void SaveToPdf2(string wordFileName, object outputFileName)
        {
            // Create a new Microsoft Word application object
            lock (this)
            {
                Microsoft.Office.Interop.Word.Application word = null;
                try
                {
                    word = new Microsoft.Office.Interop.Word.Application();
                }
                catch (Exception exception)
                {
                    throw new DuradosException("Failed to create Word Interop: " + exception.Message);
                }

                // C# doesn't have optional arguments so we'll need a dummy value
                object oMissing = System.Reflection.Missing.Value;


                word.Visible = false;
                word.ScreenUpdating = false;

                FileInfo wordFile = new FileInfo(wordFileName);


                // Cast as Object for word Open method
                Object filename = (Object)wordFile.FullName;

                object readOnly = true;

                Document doc;
                // Use the dummy value as a placeholder for optional arguments
                try
                {
                    doc = word.Documents.Open(ref filename,
                            ref oMissing, ref readOnly, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                }
                catch (Exception exception)
                {
                    throw new DuradosException("Failed to open Word: " + exception.Message);
                }
                if (doc == null)
                {
                    throw new DuradosException("Failed to open Word. The word.Documents.Open command fails ");
                }
                try
                {
                    doc.Activate();
                }
                catch (Exception exception)
                {
                    throw new DuradosException("Failed to Activate Word: " + exception.Message);
                }
                object fileFormat = WdSaveFormat.wdFormatPDF;

                try
                {
                    // Save document into PDF Format
                    doc.SaveAs(ref outputFileName,
                        ref fileFormat, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                }
                catch
                {
                    try
                    {
                        // Save document into PDF Format
                        doc.SaveAs(ref outputFileName,
                            ref fileFormat, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                            ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                    }
                    catch
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(100);
                            // Save document into PDF Format
                            doc.SaveAs(ref outputFileName,
                                ref fileFormat, ref oMissing, ref oMissing,
                                ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                        }
                        catch (Exception exception)
                        {
                            throw exception;
                        }
                    }


                    

                    // word has to be cast to type _Application so that it will find
                    // the correct Quit method.
                }
                finally
                {
                    // Close the Word document, but leave the Word application open.
                    // doc has to be cast to type _Document so that it will find the
                    // correct Close method.                
                    object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
                    ((_Document)doc).Close(ref saveChanges, ref oMissing, ref oMissing);
                    doc = null;

                    ((_Application)word).Quit(ref oMissing, ref oMissing, ref oMissing);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(word);
                    word = null;
                } 
            }
            
        }


        //public void ConvetHTML2Doc(object FileName)
        //{
        //    object missing = Type.Missing;
        //    object readOnly = true;
        //    Microsoft.Office.Interop.Word.Application word = null;
        //    word = new Microsoft.Office.Interop.Word.Application();


        //    // SaveAs requires lots of parameters, but we can leave most of them empty:
        //    word.Documents.Open(ref FileName, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
        //                             ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
        //    //Save as words
        //    string newfilename = FileName.ToString().Replace(".html", ".doc");
        //    object o_newfilename = newfilename;
        //    //object o_encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
        //    object o_encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
        //    //object o_format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;
        //    object o_format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument;
        //    // object o_endings = Microsoft.Office.Interop.Word.WdLineEndingType.wdCRLF;
        //    object o_endings = Microsoft.Office.Interop.Word.WdLineEndingType.wdCRLF;
        //    try
        //    {
        //        word.ActiveDocument.SaveAs(ref o_newfilename, ref o_format, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
        //                                         ref missing, ref missing, ref o_encoding, ref missing, ref missing, ref o_endings, ref missing);
        //    }
        //    catch
        //    {
        //        try
        //        {
        //            word.ActiveDocument.SaveAs(ref o_newfilename, ref o_format, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
        //                     ref missing, ref missing, ref o_encoding, ref missing, ref missing, ref o_endings, ref missing);
        //        }
        //        catch
        //        {
        //            System.Threading.Thread.Sleep(100);
        //            word.ActiveDocument.SaveAs(ref o_newfilename, ref o_format, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
        //                    ref missing, ref missing, ref o_encoding, ref missing, ref missing, ref o_endings, ref missing);
     
        //        }
        //    }
        //    ((_Application)word).Quit(ref missing, ref missing, ref missing);
        //    System.Runtime.InteropServices.Marshal.ReleaseComObject(word);
        //    word = null;
        //}

    }
}
