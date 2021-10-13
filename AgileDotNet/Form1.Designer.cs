namespace Deobfuscators
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.OptionsBox = new System.Windows.Forms.GroupBox();
            this.ClearClasses = new System.Windows.Forms.CheckBox();
            this.DeVirtualizeCheck = new System.Windows.Forms.CheckBox();
            this.ControlFloxFix = new System.Windows.Forms.CheckBox();
            this.AntiTamperFix = new System.Windows.Forms.CheckBox();
            this.PatchDelegates = new System.Windows.Forms.CheckBox();
            this.StringDecryption = new System.Windows.Forms.CheckBox();
            this.AssemblieToDeobf = new System.Windows.Forms.CheckedListBox();
            this.AssemblyStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshAssemblyListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.DeobfBtn = new System.Windows.Forms.Button();
            this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.MakeEditableCheck = new System.Windows.Forms.CheckBox();
            this.OptionsBox.SuspendLayout();
            this.AssemblyStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // OptionsBox
            // 
            this.OptionsBox.Controls.Add(this.MakeEditableCheck);
            this.OptionsBox.Controls.Add(this.ClearClasses);
            this.OptionsBox.Controls.Add(this.DeVirtualizeCheck);
            this.OptionsBox.Controls.Add(this.ControlFloxFix);
            this.OptionsBox.Controls.Add(this.AntiTamperFix);
            this.OptionsBox.Controls.Add(this.PatchDelegates);
            this.OptionsBox.Controls.Add(this.StringDecryption);
            this.OptionsBox.Location = new System.Drawing.Point(0, 0);
            this.OptionsBox.Name = "OptionsBox";
            this.OptionsBox.Size = new System.Drawing.Size(274, 332);
            this.OptionsBox.TabIndex = 0;
            this.OptionsBox.TabStop = false;
            this.OptionsBox.Text = "Options";
            // 
            // ClearClasses
            // 
            this.ClearClasses.AutoSize = true;
            this.ClearClasses.Checked = true;
            this.ClearClasses.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ClearClasses.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ClearClasses.Location = new System.Drawing.Point(120, 64);
            this.ClearClasses.Name = "ClearClasses";
            this.ClearClasses.Size = new System.Drawing.Size(125, 24);
            this.ClearClasses.TabIndex = 5;
            this.ClearClasses.Text = "Clear Classes";
            this.ClearClasses.UseVisualStyleBackColor = true;
            // 
            // DeVirtualizeCheck
            // 
            this.DeVirtualizeCheck.AutoSize = true;
            this.DeVirtualizeCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DeVirtualizeCheck.Location = new System.Drawing.Point(4, 64);
            this.DeVirtualizeCheck.Name = "DeVirtualizeCheck";
            this.DeVirtualizeCheck.Size = new System.Drawing.Size(118, 24);
            this.DeVirtualizeCheck.TabIndex = 4;
            this.DeVirtualizeCheck.Text = "De Virtualize";
            this.DeVirtualizeCheck.UseVisualStyleBackColor = true;
            this.DeVirtualizeCheck.CheckedChanged += new System.EventHandler(this.DeVirtualizeCheck_CheckedChanged);
            // 
            // ControlFloxFix
            // 
            this.ControlFloxFix.AutoSize = true;
            this.ControlFloxFix.Checked = true;
            this.ControlFloxFix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ControlFloxFix.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ControlFloxFix.Location = new System.Drawing.Point(138, 40);
            this.ControlFloxFix.Name = "ControlFloxFix";
            this.ControlFloxFix.Size = new System.Drawing.Size(116, 24);
            this.ControlFloxFix.TabIndex = 3;
            this.ControlFloxFix.Text = "Control Flow";
            this.ControlFloxFix.UseVisualStyleBackColor = true;
            // 
            // AntiTamperFix
            // 
            this.AntiTamperFix.AutoSize = true;
            this.AntiTamperFix.Checked = true;
            this.AntiTamperFix.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AntiTamperFix.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.AntiTamperFix.Location = new System.Drawing.Point(4, 40);
            this.AntiTamperFix.Name = "AntiTamperFix";
            this.AntiTamperFix.Size = new System.Drawing.Size(133, 24);
            this.AntiTamperFix.TabIndex = 2;
            this.AntiTamperFix.Text = "Anti Tamper fix";
            this.AntiTamperFix.UseVisualStyleBackColor = true;
            // 
            // PatchDelegates
            // 
            this.PatchDelegates.AutoSize = true;
            this.PatchDelegates.Checked = true;
            this.PatchDelegates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PatchDelegates.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PatchDelegates.Location = new System.Drawing.Point(138, 16);
            this.PatchDelegates.Name = "PatchDelegates";
            this.PatchDelegates.Size = new System.Drawing.Size(122, 24);
            this.PatchDelegates.TabIndex = 1;
            this.PatchDelegates.Text = "Fix delegates";
            this.PatchDelegates.UseVisualStyleBackColor = true;
            // 
            // StringDecryption
            // 
            this.StringDecryption.AutoSize = true;
            this.StringDecryption.Checked = true;
            this.StringDecryption.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StringDecryption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.StringDecryption.Location = new System.Drawing.Point(4, 16);
            this.StringDecryption.Name = "StringDecryption";
            this.StringDecryption.Size = new System.Drawing.Size(133, 24);
            this.StringDecryption.TabIndex = 0;
            this.StringDecryption.Text = "DecryptStrings";
            this.StringDecryption.UseVisualStyleBackColor = true;
            // 
            // AssemblieToDeobf
            // 
            this.AssemblieToDeobf.ContextMenuStrip = this.AssemblyStrip;
            this.AssemblieToDeobf.FormattingEnabled = true;
            this.AssemblieToDeobf.Location = new System.Drawing.Point(278, 30);
            this.AssemblieToDeobf.Name = "AssemblieToDeobf";
            this.AssemblieToDeobf.Size = new System.Drawing.Size(268, 244);
            this.AssemblieToDeobf.TabIndex = 1;
            // 
            // AssemblyStrip
            // 
            this.AssemblyStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAssemblyToolStripMenuItem,
            this.refreshAssemblyListToolStripMenuItem,
            this.removeAssemblyToolStripMenuItem});
            this.AssemblyStrip.Name = "AssemblyStrip";
            this.AssemblyStrip.Size = new System.Drawing.Size(184, 70);
            // 
            // addAssemblyToolStripMenuItem
            // 
            this.addAssemblyToolStripMenuItem.Name = "addAssemblyToolStripMenuItem";
            this.addAssemblyToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.addAssemblyToolStripMenuItem.Text = "Add assembly";
            this.addAssemblyToolStripMenuItem.Click += new System.EventHandler(this.addAssemblyToolStripMenuItem_Click);
            // 
            // refreshAssemblyListToolStripMenuItem
            // 
            this.refreshAssemblyListToolStripMenuItem.Name = "refreshAssemblyListToolStripMenuItem";
            this.refreshAssemblyListToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.refreshAssemblyListToolStripMenuItem.Text = "Refresh assembly list";
            this.refreshAssemblyListToolStripMenuItem.Click += new System.EventHandler(this.refreshAssemblyListToolStripMenuItem_Click);
            // 
            // removeAssemblyToolStripMenuItem
            // 
            this.removeAssemblyToolStripMenuItem.Name = "removeAssemblyToolStripMenuItem";
            this.removeAssemblyToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.removeAssemblyToolStripMenuItem.Text = "Remove assembly";
            this.removeAssemblyToolStripMenuItem.Click += new System.EventHandler(this.removeAssemblyToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(278, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(270, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Assemblies to deobfuscate";
            // 
            // DeobfBtn
            // 
            this.DeobfBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DeobfBtn.Location = new System.Drawing.Point(278, 276);
            this.DeobfBtn.Name = "DeobfBtn";
            this.DeobfBtn.Size = new System.Drawing.Size(270, 50);
            this.DeobfBtn.TabIndex = 3;
            this.DeobfBtn.Text = "Deobfuscate";
            this.DeobfBtn.UseVisualStyleBackColor = true;
            this.DeobfBtn.Click += new System.EventHandler(this.DeobfBtn_Click);
            // 
            // MakeEditableCheck
            // 
            this.MakeEditableCheck.AutoSize = true;
            this.MakeEditableCheck.Checked = true;
            this.MakeEditableCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MakeEditableCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MakeEditableCheck.Location = new System.Drawing.Point(4, 86);
            this.MakeEditableCheck.Name = "MakeEditableCheck";
            this.MakeEditableCheck.Size = new System.Drawing.Size(194, 24);
            this.MakeEditableCheck.TabIndex = 6;
            this.MakeEditableCheck.Text = "Make Editable /w dnspy";
            this.MakeEditableCheck.UseVisualStyleBackColor = true;
            this.MakeEditableCheck.CheckedChanged += new System.EventHandler(this.MakeEditableCheck_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 332);
            this.Controls.Add(this.DeobfBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AssemblieToDeobf);
            this.Controls.Add(this.OptionsBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Deobfuscator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.OptionsBox.ResumeLayout(false);
            this.OptionsBox.PerformLayout();
            this.AssemblyStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox OptionsBox;
        private System.Windows.Forms.CheckBox StringDecryption;
        private System.Windows.Forms.CheckedListBox AssemblieToDeobf;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button DeobfBtn;
        private System.Windows.Forms.ContextMenuStrip AssemblyStrip;
        private System.Windows.Forms.ToolStripMenuItem addAssemblyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshAssemblyListToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog OpenDialog;
        private System.Windows.Forms.CheckBox DeVirtualizeCheck;
        private System.Windows.Forms.CheckBox ControlFloxFix;
        private System.Windows.Forms.CheckBox AntiTamperFix;
        private System.Windows.Forms.CheckBox PatchDelegates;
        private System.Windows.Forms.ToolStripMenuItem removeAssemblyToolStripMenuItem;
        private System.Windows.Forms.CheckBox ClearClasses;
        private System.Windows.Forms.CheckBox MakeEditableCheck;
    }
}

