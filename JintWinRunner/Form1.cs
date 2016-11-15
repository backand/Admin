using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.IO;
using Backand;


namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        JsEngine engine = new JsEngine();
        
        private void button1_Click(object sender, EventArgs e)
        {
            outputTextBox.Text = string.Empty;

            bool overwrite = overwritePrefixCheckBox.Checked;

            Application.UseWaitCursor = true;

            string code = codeTextBox.Text;

            try
            {
                outputTextBox.Text = engine.Execute(code, overwrite);
            }
            catch (Exception exception)
            {
                outputTextBox.Text = exception.Message + "\n\r" + exception.StackTrace;
                MessageBox.Show(exception.Message);
            }
            finally
            {
                Application.UseWaitCursor = false;

            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                codeTextBox.Text = File.ReadAllText(openFileDialog1.FileName);
                Text = "Jint: " + openFileDialog1.FileName;
            }
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog1.FileName, codeTextBox.Text);
                Text = "Jint: " + saveFileDialog1.FileName;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(saveFileDialog1.FileName))
            {
                saveAsButton_Click(sender, e);
            }
            else
            {
                File.WriteAllText(saveFileDialog1.FileName, codeTextBox.Text);
            }
        }

        private void reloadXhrButton_Click(object sender, EventArgs e)
        {
            engine.ReloadXhrWrapper();
        }

        
        
    }

    
}
