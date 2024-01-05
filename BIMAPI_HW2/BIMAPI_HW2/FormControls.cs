using System;
using System.Drawing;
using System.Windows.Forms;

namespace BIMAPI_HW2
{
    public class FormControls
    {
        public Label SetupLabel1()
        {
            Label label1 = new Label();
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft JhengHei", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(136)));
            label1.Location = new Point(30, 256);
            label1.Margin = new Padding(8, 0, 8, 0);
            label1.Name = "label1";
            label1.Size = new Size(150, 42);
            label1.TabIndex = 0;
            label1.Text = "選擇物件";
            return label1;
        }

        public Button SetupButton1()
        {
            Button button1 = new Button();
            button1.Font = new Font("Microsoft JhengHei", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(136)));
            button1.Location = new Point(948, 342);
            button1.Margin = new Padding(8, 6, 8, 6);
            button1.Name = "button1";
            button1.Size = new Size(60, 45);
            button1.TabIndex = 2;
            button1.Text = "...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += new EventHandler(button1_Click);
            return button1;
        }

        // Similar functions for maskedTextBox1, maskedTextBox2, button2, checkBox1, checkBox2, button3, button4, label2, label3, pictureBox1, dataGridView1, and label4

        private void button1_Click(object sender, EventArgs e)
        {
            // Event handler code here
        }
    }
}
