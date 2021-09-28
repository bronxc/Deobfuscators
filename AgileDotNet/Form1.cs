using AgileDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deobfuscators
{
    public partial class Form1 : Form
    {
        Deobfuscator deobf;
        public Form1()
        {
            InitializeComponent();
        }

        private void DeobfBtn_Click(object sender, EventArgs e)
        {
            deobf.UpdateSettings(new DeobfuscatorSettings()
            {
                DecryptStrings = StringDecryption.Checked,
                DelegateFixer = PatchDelegates.Checked,
                AntiTamper = AntiTamperFix.Checked,
                ControlFlow = ControlFloxFix.Checked,
                DeVirtualize = DeVirtualizeCheck.Checked,
                ClearClasses = ClearClasses.Checked
            });

            string[] files = new string[AssemblieToDeobf.CheckedItems.Count];
            int pos = 0;

            for(int i = 0; i < AssemblieToDeobf.CheckedItems.Count; i++)
            {
                if (AssemblieToDeobf.GetItemChecked(i))
                {
                    files[pos] = AssemblieToDeobf.Items[i].ToString();
                    pos++;
                }
            }

            foreach(string file in files)
            {
                Console.WriteLine("File: "+file);
                deobf.Deobfuscate(file);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists("asms"))
                Directory.CreateDirectory("asms");

            if (!Directory.Exists("deobfed"))
                Directory.CreateDirectory("deobfed");

            deobf = new Deobfuscator(new DeobfuscatorSettings()
            {
                DecryptStrings = true,
                DelegateFixer = true,
                AntiTamper = true,
                ControlFlow = true,
                DeVirtualize = true,
                ClearClasses = true
            });

            refreshAssemblyListToolStripMenuItem_Click(sender, e);
        }

        private bool IsAdded(string file)
        {
            string fileName = Path.GetFileName(file);

            foreach(string cFile in Directory.GetFiles("asms"))
            {
                if (Path.GetFileName(cFile) == fileName)
                    return true;
            }

            return false;
        }

        private void addAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDialog.Filter = "Executable files (.exe)|*.exe|Dll files (.dll)|*.dll";
            if(OpenDialog.ShowDialog() == DialogResult.OK)
            {
                if (IsAdded(OpenDialog.FileName))
                {
                    Console.WriteLine("This file is already added!");
                    return;
                }

                File.Copy(OpenDialog.FileName, "asms/" + Path.GetFileName(OpenDialog.FileName));
                refreshAssemblyListToolStripMenuItem_Click(sender, e);
            }
        }

        private void refreshAssemblyListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssemblieToDeobf.Items.Clear();

            foreach(string file in Directory.GetFiles("asms"))
            {
                if (file.EndsWith(".exe") || file.EndsWith(".dll"))
                    AssemblieToDeobf.Items.Add(file);
            }
        }

        private void removeAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AssemblieToDeobf.SelectedItem is null)
                return;

            File.Delete(AssemblieToDeobf.SelectedItem.ToString());

            refreshAssemblyListToolStripMenuItem_Click(sender, e);
        }
    }
}
