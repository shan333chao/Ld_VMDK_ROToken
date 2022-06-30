﻿namespace RO_TOKEN
{
    partial class Frm_TokenExport
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
            this.btn_box_folder = new System.Windows.Forms.Button();
            this.btn_export_token = new System.Windows.Forms.Button();
            this.btn_import_token = new System.Windows.Forms.Button();
            this.btn_select_export = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cb_start = new System.Windows.Forms.ComboBox();
            this.cb_end = new System.Windows.Forms.ComboBox();
            this.txt_vms_folder = new System.Windows.Forms.TextBox();
            this.txt_export_path = new System.Windows.Forms.TextBox();
            this.txt_log = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_box_folder
            // 
            this.btn_box_folder.Location = new System.Drawing.Point(369, 59);
            this.btn_box_folder.Name = "btn_box_folder";
            this.btn_box_folder.Size = new System.Drawing.Size(123, 23);
            this.btn_box_folder.TabIndex = 0;
            this.btn_box_folder.Text = "选择模拟器安装目录";
            this.btn_box_folder.UseVisualStyleBackColor = true;
            this.btn_box_folder.Click += new System.EventHandler(this.btn_box_folder_Click);
            // 
            // btn_export_token
            // 
            this.btn_export_token.Location = new System.Drawing.Point(129, 237);
            this.btn_export_token.Name = "btn_export_token";
            this.btn_export_token.Size = new System.Drawing.Size(75, 23);
            this.btn_export_token.TabIndex = 1;
            this.btn_export_token.Text = "导出";
            this.btn_export_token.UseVisualStyleBackColor = true;
            this.btn_export_token.Click += new System.EventHandler(this.btn_export_token_Click);
            // 
            // btn_import_token
            // 
            this.btn_import_token.Location = new System.Drawing.Point(210, 237);
            this.btn_import_token.Name = "btn_import_token";
            this.btn_import_token.Size = new System.Drawing.Size(75, 23);
            this.btn_import_token.TabIndex = 2;
            this.btn_import_token.Text = "导入";
            this.btn_import_token.UseVisualStyleBackColor = true;
            this.btn_import_token.Click += new System.EventHandler(this.btn_import_token_Click);
            // 
            // btn_select_export
            // 
            this.btn_select_export.Location = new System.Drawing.Point(369, 157);
            this.btn_select_export.Name = "btn_select_export";
            this.btn_select_export.Size = new System.Drawing.Size(123, 23);
            this.btn_select_export.TabIndex = 3;
            this.btn_select_export.Text = "选择保存目录";
            this.btn_select_export.UseVisualStyleBackColor = true;
            this.btn_select_export.Click += new System.EventHandler(this.btn_select_export_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "模拟器目录";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(55, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "导出目录";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(55, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "结束:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(55, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "开始:";
            // 
            // cb_start
            // 
            this.cb_start.FormattingEnabled = true;
            this.cb_start.Location = new System.Drawing.Point(129, 88);
            this.cb_start.Name = "cb_start";
            this.cb_start.Size = new System.Drawing.Size(97, 25);
            this.cb_start.TabIndex = 9;
            // 
            // cb_end
            // 
            this.cb_end.FormattingEnabled = true;
            this.cb_end.Location = new System.Drawing.Point(129, 121);
            this.cb_end.Name = "cb_end";
            this.cb_end.Size = new System.Drawing.Size(97, 25);
            this.cb_end.TabIndex = 10;
            // 
            // txt_vms_folder
            // 
            this.txt_vms_folder.Location = new System.Drawing.Point(129, 59);
            this.txt_vms_folder.Name = "txt_vms_folder";
            this.txt_vms_folder.Size = new System.Drawing.Size(234, 23);
            this.txt_vms_folder.TabIndex = 11;
            this.txt_vms_folder.Text = "E:\\BaiZhi\\LSPlayer64";
            // 
            // txt_export_path
            // 
            this.txt_export_path.Location = new System.Drawing.Point(129, 157);
            this.txt_export_path.Name = "txt_export_path";
            this.txt_export_path.Size = new System.Drawing.Size(234, 23);
            this.txt_export_path.TabIndex = 12;
            this.txt_export_path.Text = "D:\\模拟器导出数据";
            // 
            // txt_log
            // 
            this.txt_log.AcceptsReturn = true;
            this.txt_log.AcceptsTab = true;
            this.txt_log.Location = new System.Drawing.Point(12, 300);
            this.txt_log.Multiline = true;
            this.txt_log.Name = "txt_log";
            this.txt_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_log.Size = new System.Drawing.Size(767, 138);
            this.txt_log.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txt_log);
            this.Controls.Add(this.txt_export_path);
            this.Controls.Add(this.txt_vms_folder);
            this.Controls.Add(this.cb_end);
            this.Controls.Add(this.cb_start);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_select_export);
            this.Controls.Add(this.btn_import_token);
            this.Controls.Add(this.btn_export_token);
            this.Controls.Add(this.btn_box_folder);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_box_folder;
        private System.Windows.Forms.Button btn_export_token;
        private System.Windows.Forms.Button btn_import_token;
        private System.Windows.Forms.Button btn_select_export;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cb_start;
        private System.Windows.Forms.ComboBox cb_end;
        private System.Windows.Forms.TextBox txt_vms_folder;
        private System.Windows.Forms.TextBox txt_export_path;
        private System.Windows.Forms.TextBox txt_log;
    }
}