namespace ElectricalOverStressDetection
{
    partial class FrmBindInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel_Binding = new System.Windows.Forms.TableLayoutPanel();
            this.button_Confirm = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.propertyGrid_Binding = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel_Binding.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_Binding
            // 
            this.tableLayoutPanel_Binding.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel_Binding.ColumnCount = 2;
            this.tableLayoutPanel_Binding.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Binding.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Binding.Controls.Add(this.button_Confirm, 0, 1);
            this.tableLayoutPanel_Binding.Controls.Add(this.button_Cancel, 1, 1);
            this.tableLayoutPanel_Binding.Controls.Add(this.propertyGrid_Binding, 0, 0);
            this.tableLayoutPanel_Binding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Binding.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Binding.Name = "tableLayoutPanel_Binding";
            this.tableLayoutPanel_Binding.RowCount = 2;
            this.tableLayoutPanel_Binding.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.tableLayoutPanel_Binding.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel_Binding.Size = new System.Drawing.Size(434, 461);
            this.tableLayoutPanel_Binding.TabIndex = 0;
            // 
            // button_Confirm
            // 
            this.button_Confirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Confirm.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Confirm.Location = new System.Drawing.Point(4, 394);
            this.button_Confirm.Name = "button_Confirm";
            this.button_Confirm.Size = new System.Drawing.Size(209, 63);
            this.button_Confirm.TabIndex = 7;
            this.button_Confirm.Text = "确定";
            this.button_Confirm.UseVisualStyleBackColor = true;
            this.button_Confirm.Click += new System.EventHandler(this.button_Confirm_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Cancel.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Cancel.Location = new System.Drawing.Point(220, 394);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(210, 63);
            this.button_Cancel.TabIndex = 6;
            this.button_Cancel.Text = "取消";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // propertyGrid_Binding
            // 
            this.tableLayoutPanel_Binding.SetColumnSpan(this.propertyGrid_Binding, 2);
            this.propertyGrid_Binding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid_Binding.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGrid_Binding.HelpVisible = false;
            this.propertyGrid_Binding.Location = new System.Drawing.Point(4, 4);
            this.propertyGrid_Binding.Name = "propertyGrid_Binding";
            this.propertyGrid_Binding.Size = new System.Drawing.Size(426, 383);
            this.propertyGrid_Binding.TabIndex = 8;
            this.propertyGrid_Binding.ToolbarVisible = false;
            // 
            // FrmBinding
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 461);
            this.Controls.Add(this.tableLayoutPanel_Binding);
            this.Name = "FrmBinding";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmBinding";
            this.Load += new System.EventHandler(this.FrmBinding_Load);
            this.tableLayoutPanel_Binding.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Binding;
        private System.Windows.Forms.Button button_Confirm;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.PropertyGrid propertyGrid_Binding;
    }
}