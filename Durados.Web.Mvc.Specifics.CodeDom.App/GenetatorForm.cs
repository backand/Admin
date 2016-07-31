using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Durados.Web.Mvc.Specifics.CodeDom.App
{
    public partial class GenetatorForm : Form
    {
        public GenetatorForm()
        {
            InitializeComponent();
        }

        private void GenetatorForm_Load(object sender, EventArgs e)
        {
            LoadDatabases();
        }

        private void LoadDatabases()
        {
            Type[] types = GetAllProjectTypes();

            foreach (Type type in types)
            {
                databaseComboBox.Items.Add(new ProjectType() { Type = type });
            }

        }

        public class ProjectType
        {
            public Type Type { get; set; }

            public override string ToString()
            {
                return Type.Name.Replace("Project", string.Empty);
            }
        }

        private Type[] GetAllProjectTypes()
        {
            Projects.Project p = new SanDisk.Allegro.AllegroProject();

            List<Type> types = new List<Type>();
            
            Type projectType = typeof(Durados.Web.Mvc.Specifics.Projects.Project);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.BaseType != null && type.BaseType.Equals(projectType))
                        if (!types.Contains(type))
                            types.Add(type);
                }
            }

            return types.ToArray();
        }

        private void destinationButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                destinationTextBox.Text = saveFileDialog.FileName;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            if (databaseComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a database");
                return;
            }

            if (string.IsNullOrEmpty(destinationTextBox.Text))
            {
                MessageBox.Show("Please enter a destination");
                return;
            }
            Type type = ((ProjectType)databaseComboBox.SelectedItem).Type;
            string destination = destinationTextBox.Text;

            try
            {
                Generate(type, destination);
                MessageBox.Show("Successful generation");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Generate(Type type, string destination)
        {
            Durados.CodeDom.EnumGenerator enumGenerator =
                new Durados.CodeDom.EnumGenerator();

            Durados.Web.Mvc.Specifics.Projects.Project project = 
                (Durados.Web.Mvc.Specifics.Projects.Project)Activator.CreateInstance(type);

            enumGenerator.Generate(destination, project.GetDataSet(), type.Namespace);
        }

        
    }
}
