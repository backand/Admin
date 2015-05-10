using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Workflow;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security;

namespace Durados.Web.Mvc.Specifics.Fibi.P8.BusinessLogic
{
    public class P8CMXmlDocument : Durados.Workflow.XmlDocument
    {
        protected override string GetFileName(string template, Durados.Workflow.IDocument controller, Dictionary<string, Parameter> parameters, Durados.View view, string documentFieldName, Dictionary<string, object> values, System.Data.DataRow prevRow, string pk, string connectionString, System.Data.IDbCommand command, object data)
        {

            Dictionary<string, object> blocksValues = (Dictionary<string, object>)controller.GetBlocksValues(pk,  view);
            string filename = null;
            string documentFileNameKey = null;

            if (parameters.ContainsKey(DocumentParameters.DocumentFileNameKey.ToString()))
                documentFileNameKey = parameters[DocumentParameters.DocumentFileNameKey.ToString()].Value;
            bool overrideExistingFile = false;

            if (parameters.ContainsKey(DocumentParameters.OverrideExistingFile.ToString()))
                overrideExistingFile = Convert.ToBoolean(parameters[DocumentParameters.OverrideExistingFile.ToString()].Value);

            if (IsActiveConfiguration(view, data))
            {
                string activeConfigFileName = GetActiveConfigFileName(template, pk, documentFileNameKey);
                filename = controller.GetFileName(view, documentFieldName,activeConfigFileName );
                RenamePreviousActiveConfigFileName(template, controller, overrideExistingFile, view, documentFieldName, values, pk, blocksValues, documentFileNameKey, command, filename);
            }
            else
            {
                filename = GetNonActiveConfigFileName(template, controller, overrideExistingFile, view, documentFieldName, values, pk, blocksValues, documentFileNameKey);
            }

            return filename;
        }

        private string GetNonActiveConfigFileName(string template, Durados.Workflow.IDocument controller,bool overrideExistingFile,  Durados.View view,  string documentFieldName, Dictionary<string, object> values, string pk, Dictionary<string, object> blocksValues, string documentFileNameKey)
        {
            string filename;
            string xmlFileNameExtention=".xml";
            if (string.IsNullOrEmpty(documentFileNameKey))
            {
                string xmlFileName = GetFileNameWithoutExtention(documentFileNameKey) + xmlFileNameExtention;
                filename = GetFileName(xmlFileName, pk, null);
            }
            else
            {
                filename = controller.GetFileName(pk, template, blocksValues, documentFileNameKey, view, values);
                if (filename == documentFileNameKey)
                {
                    documentFieldName = GetFileNameWithoutExtention(template);
                    string documentFileName = GetFileNameWithoutExtention(documentFileNameKey);
                    filename = GetFileName(documentFieldName + xmlFileNameExtention, pk, documentFileName);
                }
                else
                {
                    filename = controller.GetFileName(view, documentFieldName, filename);
                }
            }

            
            if (!overrideExistingFile)
            {
                filename = GetNotExistFile(filename);
            }
            return filename;
        }

        private void RenamePreviousActiveConfigFileName(string template, Durados.Workflow.IDocument controller, bool overrideExistingFile, Durados.View view, string documentFieldName, Dictionary<string, object> values, string pk, Dictionary<string, object> blocksValues, string documentFileNameKey, System.Data.IDbCommand command, string filename)
        {

            string prevActiveConfigId = GetConfigFileExists(view, documentFieldName, filename, pk);
            int id;
            if (string.IsNullOrEmpty(prevActiveConfigId))
            {
                return;//no other configuration- nothing to rename
            }

            if (!int.TryParse(prevActiveConfigId, out id))
            {
                throw new DuradosException("Failed to rename previous active configuration file. Unable to parse previous config id");
            }
            else
            {
                FileInfo fileInfo = new FileInfo(filename);
                string prevActiveFileName = GetNonActiveConfigFileName(template, controller, overrideExistingFile, view, documentFieldName, values, prevActiveConfigId, blocksValues, documentFileNameKey);

                try
                {
                    fileInfo.CopyTo(prevActiveFileName, true);
                    FileInfo renamedFile = new FileInfo(prevActiveFileName);

                    controller.UpdateDocumentRow(view, prevActiveConfigId, documentFieldName, renamedFile.Name, command);
                }
                catch (ArgumentException)
                {
                    throw new DuradosException("Failed to rename previous active configuration file. The new file: " + prevActiveFileName + " was not found .");
                }
                catch (IOException exception)
                {
                    throw new DuradosException("Failed to rename previous active configuration file. " + exception.Message, exception);
                }
                catch (SecurityException exception)
                {
                    throw new DuradosException("Failed to rename previous active configuration file. Encountered file security violation." + exception.Message, exception);
                }

                catch (UnauthorizedAccessException exception)
                {
                    throw new DuradosException("TFailed to rename previous active configuration file. Could not access new file location." + exception.Message, exception);
                }
                catch (NotSupportedException exception)
                {
                    throw new DuradosException("Failed to rename previous active configuration file. The xsd file: " + exception.Message, exception);
                }
            }


        }
            
                     
        

        private string  GetConfigFileExists(Durados.View view, string documentFieldName, string filename,string pk)
        {
            object scalar = null;
            FileInfo fileinfo = new FileInfo(filename);
            string sql = string.Format("select id from [{0}] where {1} = '{2}' and id <> {3}", view.Name, documentFieldName, fileinfo.Name,pk);

            using (SqlConnection connection = new SqlConnection(view.Database.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    scalar = command.ExecuteScalar();
                }
            }

            if (scalar == null || scalar == DBNull.Value)
                return null;
            else
                return scalar.ToString();
        }

        private string GetFileNameWithoutExtention(string documentFieldName)
        {
            string fileName = string.Empty; ;
            int dotIndex= documentFieldName.LastIndexOf(".");
            if (dotIndex>0)
            {
                 fileName = documentFieldName.Remove(dotIndex);
            }
            return fileName;
        }

        
        private string GetActiveConfigFileName(string template, string pk, string documentFileNameKey)
        {
            return !string.IsNullOrEmpty(documentFileNameKey) ? documentFileNameKey : GetActiveFileName(template, pk, null);
        }

       
        protected bool IsActiveConfiguration(Durados.View view, object data)
        {
            if (view != null && !view.Fields.ContainsKey("Active"))
            {
                 throw new DuradosException("Missing Active Config Field.");
            }
            if(data != null && !(data is  DataRow))
            {
                throw new DuradosException("Missing data row for xml document file name.");
            }

            Durados.Field field = view.Fields["Active"];
            string activeConfig = field.GetValue((DataRow)data);
           
            bool isActiveConfig =false;;
            //string outputXmlFileName = newFile;
            if (!string.IsNullOrEmpty(activeConfig) && field is ColumnField && bool.TryParse(((ColumnField)field).ConvertFromString(activeConfig).ToString(), out isActiveConfig))
            {
               return isActiveConfig;
            }
            else
            {
                return false;
            }
         
        }
        
        protected virtual string GetActiveFileName(string template, string pk, string baseFileName)
        {
            
            string filename = string.Empty;
            string dot = ".";

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(template);
            string orgExtension = fileInfo.Extension ?? string.Empty;
            string extension = ".xml";

            if (string.IsNullOrEmpty(baseFileName))
            {
                filename = template.TrimEnd((orgExtension).ToCharArray());
                filename +=  (extension.StartsWith(dot) ? string.Empty : dot) + extension;
            }
            else
            {
                filename += fileInfo.DirectoryName + @"\" + baseFileName;
                
                if (!baseFileName.EndsWith(".xml"))
                {
                    filename += (extension.StartsWith(dot) ? string.Empty : dot) + extension;
                }
            }

            return filename;
           // return base.GetFileName(template, pk, baseFileName);
        }
    
    }
}
