using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB;

namespace BIMAPI_HW2
{
	public partial class Form1 : System.Windows.Forms.Form
	{
		public Document doc;
		public Selection selElement;
		public string rfa_path;
		public string Object;
		public string Object_type_path;
		public string filename;
		public Form1(Document _doc, Selection _selElement)
		{
			this.doc = _doc;
			this.selElement = _selElement;
			InitializeComponent();
		}
		public string path1;
		public string path2;
		
		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			Object = "Column";
			checkBox2.Checked = false;
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			Object = "Beam";
			checkBox1.Checked = false;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog file = new OpenFileDialog();
				file.ShowDialog();
				string name = file.SafeFileName;
				string directoryPath = System.IO.Path.GetDirectoryName(file.FileName);
				string path = directoryPath + '\\' + name;
				maskedTextBox1.Text = path;
				path1 = path;
				
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: Could not read file from disk. Original error: " + ex.Message);

			}


		}

		private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			// MessageBox.Show(rfa_path);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog file = new OpenFileDialog();
				file.ShowDialog();
				string name = file.SafeFileName;
				string directoryPath = System.IO.Path.GetDirectoryName(file.FileName);
				string path = directoryPath + '\\' + name;
				maskedTextBox2.Text = path;
				path2 = path;
				filename = name;
				string[] lines = System.IO.File.ReadAllLines(path);
				dataGridView1.Rows.Clear();
				int i = 0;
				foreach (string line in lines)
				{
					dataGridView1.Rows.Add();
					dataGridView1.Rows[i].Cells[0].Value = line;
					i++;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: Could not read file from disk. Original error: " + ex.Message);

			}
			
		}

		private void maskedTextBox2_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			// MessageBox.Show(Object_type_path);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Success!");
			Close();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
