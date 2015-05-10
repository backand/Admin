using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados.Workflow
{
    public class XmlDocument : Document
    {
        public override string Create(IDocument controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, IDbCommand command)
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

            string transformationFile = GetTransformationFile(fieldName, controller, parameters, view, values, prevRow, pk, connectionString, command);

            if (!System.IO.File.Exists(transformationFile))
            {
                throw new WorkflowEngineException("The xsl file " + transformationFile + " was not found. Please check the Upload configuration, the document template table and the rule parameters.");
            }

            //controller.CreateDocument(filename, template, blocksValues);
            XmlTemplate xmlTemplate = new XmlTemplate() { Schema = template, Xslt = transformationFile };
            CreateDocument(controller, filename, xmlTemplate, transformationFile, data, view);

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(xmlTemplate.Ouput);

            controller.UpdateDocumentRow(view, pk, fieldName, fileInfo.Name, command);

            return filename;
        }

        protected virtual void CreateDocument(IDocument controller, string filename, XmlTemplate template, string transformationFile, object data, View view)
        {
            try
            {

                controller.CreateDocument(filename, template, data, DocumentType.Xml, view);
            }
            catch (DuradosException exception)
            {
                throw new WorkflowEngineException(exception.Message);
            }

        }

        // xsl
        protected virtual string GetTransformationFile(string documentFieldName, IDocument controller, Dictionary<string, Parameter> parameters, View view, Dictionary<string, object> values, DataRow prevRow, string pk, string connectionString, IDbCommand command)
        {
            string transformationFile = controller.GetTransformationFile(view, values, prevRow, pk);

            if (string.IsNullOrEmpty(transformationFile))
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
                string templateViewFileNameFieldName = GetDocumentTemplateTransformationFileNameFieldName(controller, parameters);
                if (!view.Database.Views[templateViewName].Fields.ContainsKey(templateViewFileNameFieldName))
                {
                    throw new WorkflowEngineException("The template view " + templateViewName + " does not contain the file name field " + templateViewFileNameFieldName + ". Please make sure the rule has a TemplateViewFileNameFieldName parameter point to the correct file name field.");
                }
                transformationFile = controller.GetTemplate(view, templateViewName, templatePK, documentFieldName, templateViewFileNameFieldName);

            }

            if (string.IsNullOrEmpty(transformationFile))
            {
                throw new WorkflowEngineException("Could not find the template field for " + view.DisplayName + " make sure that the rule has a TemplateFieldName parameter that points to a template field.");
            }

            return transformationFile;
        }

        protected virtual string GetDocumentTemplateTransformationFileNameFieldName(IDocument controller, Dictionary<string, Parameter> parameters)
        {
            string templateViewFileNameFieldName = null;
            if (parameters.ContainsKey(DocumentParameters.TemplateViewTransformationFileNameFieldName.ToString()))
                templateViewFileNameFieldName = parameters[DocumentParameters.TemplateViewTransformationFileNameFieldName.ToString()].Value;
            else
                templateViewFileNameFieldName = controller.GetDocumentTemplateTransformationFileNameFieldName();

            return templateViewFileNameFieldName;
        }


        protected override object GetData(IDocument controller, string pk, View view)
        {
            return controller.GetDataRow(view, pk);
        }

        protected override string GetFileName(string template, IDocument controller, Dictionary<string, Parameter> parameters, View view, string documentFieldName, Dictionary<string, object> values, System.Data.DataRow prevRow, string pk, string connectionString, System.Data.IDbCommand command, object data)
        {
            data = base.GetData(controller, pk, view);
            return base.GetFileName(template, controller, parameters, view, documentFieldName, values, prevRow, pk, connectionString, command, data);
        }
    }
}
