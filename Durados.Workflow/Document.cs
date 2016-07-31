using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados.Workflow
{
    public class Document
    {
        public void Create(object controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, IDbCommand command)
        {
            Create((IDocument)controller, parameters, view, values, prevRow, pk, connectionString, command);
        }

        public virtual string Create(IDocument controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, IDbCommand command)
        {
            //Dictionary<string, object> blocksValues = controller.GetBlocksValues(pk, view);

            object data = GetData(controller, pk, view);

            string fieldName = GetDocumentFieldName(controller, parameters, view, values, prevRow, pk, connectionString, command);

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new WorkflowEngineException("The document field name was not supplied. Please make sure that the rule has the DocumentFieldName paremeter points to the correct document field.");
            }
            if (!view.Fields.ContainsKey(fieldName))
            {
                throw new WorkflowEngineException("The document field " + fieldName + " does not exist in the " + view.DisplayName + " view. Please make sure that the rule parameters are correct.");
            }
            Field field = view.Fields[fieldName];
            if (field.FieldType != FieldType.Column)
            {
                throw new WorkflowEngineException("Please configure the " + field.DisplayName + " to be an upload field.");
            }


            string template = GetTemplate(fieldName, controller, parameters, view, values, prevRow, pk, connectionString, command);

            if (!System.IO.File.Exists(template))
            {
                throw new WorkflowEngineException("The template file " + template + " was not found. Please check the Upload configuration, the document template table and the rule parameters.");
            }

            string filename = GetFileName(template, controller, parameters, view, fieldName, values, prevRow, pk, connectionString, command, data);

            
            //controller.CreateDocument(filename, template, blocksValues);
            CreateDocument(controller, filename, template, data, view);

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);

            controller.UpdateDocumentRow(view, pk, fieldName, fileInfo.Name, command);

            return filename;
        }

        protected virtual object GetData(IDocument controller, string pk, View view)
        {
            return controller.GetBlocksValues(pk, view);
        }

        protected virtual void CreateDocument(IDocument controller, string filename, string template, object data, View view)
        {
            controller.CreateDocument(filename, template, data, DocumentType.Word, view);
        }

        protected virtual string GetDocumentFieldName(IDocument controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, IDbCommand command)
        {
            string fieldName = controller.GetDocumentFieldName(view);
            if (string.IsNullOrEmpty(fieldName))
            {
                if (parameters.ContainsKey(DocumentParameters.DocumentFieldName.ToString()))
                    fieldName = parameters[DocumentParameters.DocumentFieldName.ToString()].Value;
            }
            if (string.IsNullOrEmpty(fieldName))
                throw new WorkflowEngineException("The document field name was not supplied. Please make sure that the rule has the DocumentFieldName paremeter points to the correct document field.");

            return fieldName;
        }

        protected virtual string GetTemplateFieldName(IDocument controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, IDbCommand command)
        {
            string fieldName = controller.GetTemplateFieldName(view);
            if (string.IsNullOrEmpty(fieldName))
            {
                if (parameters.ContainsKey(DocumentParameters.TemplateFieldName.ToString()))
                    fieldName = parameters[DocumentParameters.TemplateFieldName.ToString()].Value;
            }
            return fieldName;
        }

        protected virtual string GetTemplateViewName(IDocument controller, Dictionary<string, Parameter> parameters)
        {
            string templateViewName = null;
            if (parameters.ContainsKey(DocumentParameters.TemplateViewName.ToString()))
                templateViewName = parameters[DocumentParameters.TemplateViewName.ToString()].Value;
            else
                templateViewName = controller.GetDocumentTemplateViewName();
            return templateViewName;
        }

        protected virtual string GetTemplateViewFileNameFieldName(IDocument controller, Dictionary<string, Parameter> parameters)
        {
            string templateViewFileNameFieldName = null;
            if (parameters.ContainsKey(DocumentParameters.TemplateViewFileNameFieldName.ToString()))
                templateViewFileNameFieldName = parameters[DocumentParameters.TemplateViewFileNameFieldName.ToString()].Value;
            else
                templateViewFileNameFieldName = controller.GetDocumentTemplateFileNameFieldName();
       
            return templateViewFileNameFieldName;
        }

        protected virtual string GetTemplate(string documentFieldName, IDocument controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, IDbCommand command)
        {
            string template = controller.GetTemplate(view, values, prevRow, pk);

            if (string.IsNullOrEmpty(template))
            {
                string fieldName = GetTemplateFieldName(controller, parameters, view, values, prevRow, pk, connectionString, command);
                if (string.IsNullOrEmpty(fieldName))
                {
                    throw new WorkflowEngineException("Could not find the template field for " + view.DisplayName + " make sure that the rule has a TemplateFieldName parameter that points to a template field.");
                }
                string templatePK = null;
                if (values.ContainsKey(fieldName) && values[fieldName] != null)
                {
                    templatePK = values[fieldName].ToString();
                }
                else
                {
                    templatePK = view.Fields[fieldName].GetValue(prevRow);
                }
                if (string.IsNullOrEmpty(templatePK))
                {
                    throw new WorkflowEngineException("The template field " + view.Fields[fieldName].DisplayName + " has no value.");
                }
                string templateViewName = GetTemplateViewName(controller, parameters);
                if (!view.Database.Views.ContainsKey(templateViewName))
                {
                    throw new WorkflowEngineException("The template view " + templateViewName + " does not exist. Please make sure the rule has a TemplateViewName parameter point to the correct template view.");
                }
                string templateViewFileNameFieldName = GetTemplateViewFileNameFieldName(controller, parameters);
                if (!view.Database.Views[templateViewName].Fields.ContainsKey(templateViewFileNameFieldName))
                {
                    throw new WorkflowEngineException("The template view " + templateViewName + " does not contain the file name field " + templateViewFileNameFieldName + ". Please make sure the rule has a TemplateViewFileNameFieldName parameter point to the correct file name field.");
                }
                template = controller.GetTemplate(view, templateViewName, templatePK, documentFieldName, templateViewFileNameFieldName);
            
            }

            if (string.IsNullOrEmpty(template))
            {
                throw new WorkflowEngineException("Could not find the template field for " + view.DisplayName + " make sure that the rule has a TemplateFieldName parameter that points to a template field.");
            }

            return template;
        }

        protected virtual string GetFileName(string template, IDocument controller, Dictionary<string, Parameter> parameters, View view, string documentFieldName, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, IDbCommand command, object data)
        {
            Dictionary<string, object> blocksValues = (Dictionary<string, object>)data;
            string filename = null;
            string documentFileNameKey = null;
            if (parameters.ContainsKey(DocumentParameters.DocumentFileNameKey.ToString()))
                documentFileNameKey = parameters[DocumentParameters.DocumentFileNameKey.ToString()].Value;
            
            if (string.IsNullOrEmpty(documentFileNameKey))
            {

                filename = GetFileName(template, pk, null);
            }
            else
            {
                filename = controller.GetFileName(pk, template, blocksValues, documentFileNameKey, view, values);
                if (filename == documentFileNameKey)
                {
                    filename = GetFileName(template, pk, documentFileNameKey);
                }
                else
                {
                    filename = controller.GetFileName(view, documentFieldName, filename);
                }
            }

            bool overrideExistingFile = false;
            if (parameters.ContainsKey(DocumentParameters.OverrideExistingFile.ToString()))
                overrideExistingFile = Convert.ToBoolean(parameters[DocumentParameters.OverrideExistingFile.ToString()].Value);

            if (!overrideExistingFile)
            {
                filename = GetNotExistFile(filename);
            }

            return filename;
        }

        protected virtual string GetFileName(string template, string pk, string baseFileName)
        {
            string filename = string.Empty;
            string dot = ".";

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(template);
            string extension = fileInfo.Extension;

            if (string.IsNullOrEmpty(baseFileName))
            {
                filename = template.TrimEnd((extension).ToCharArray());
                filename += "_" + pk + (extension.StartsWith(dot) ? string.Empty : dot) + extension;
            }
            else
            {
                filename += fileInfo.DirectoryName + @"\" + baseFileName + "_" + pk + extension;
            }

            return filename;
        }

        protected virtual string GetNotExistFile(string fileName)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            string name = fileInfo.Name;

            string path = System.IO.Path.GetDirectoryName(fileName);
            string fileWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(name);
            string extension = System.IO.Path.GetExtension(fileName);

            
            int i = 0;
            while (System.IO.File.Exists(fileName))
            {
                i++;

                name = fileWithoutExtension + "_" + i.ToString() + extension;
                fileName = path + System.IO.Path.DirectorySeparatorChar + name;
            }

            return fileName;
        }
   }

    public interface IDocument
    {
        Dictionary<string, object> GetBlocksValues(string id, View view);
        //void CreateDocument(string newFile, string template, Dictionary<string, object> blocksValues);
        DataRow GetDataRow(Durados.View view, string pk);
        void CreateDocument(string newFile, object template, object data, DocumentType documentType, View view);
        void UpdateDocumentRow(View view, string id, string fieldName, string filename, IDbCommand command);
        string GetDocumentFieldName(Durados.View view);
        string GetTemplateFieldName(Durados.View view);
        string GetTemplate(Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk);
        string GetTransformationFile(Durados.View view, Dictionary<string, object> values, DataRow prevRow, string pk);
        string GetTemplate(View view, string templateViewName, string templatePK, string documentFieldName, string templateViewFileNameFieldName);
        string GetDocumentTemplateViewName();
        string GetDocumentTemplateFileNameFieldName();
        string GetDocumentTemplateTransformationFileNameFieldName();
        string GetFileName(string pk, string template, Dictionary<string, object> blocksValues, string documentFileNameKey, View view, Dictionary<string,object> values);
        string GetFileName(View view, string documentFieldName, string fileName);
    }

    public enum DocumentParameters
    {
        DocumentFieldName,
        DocumentFileNameKey,
        TemplateFieldName,
        TemplateViewName,
        TemplateViewFileNameFieldName,
        TemplateViewTransformationFileNameFieldName,
        OverrideExistingFile
    }

    public enum DocumentType
    {
        Word,
        Xml
    }
}
