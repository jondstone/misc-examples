using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeleteJPGImages
{
    public partial class WindowsApp : Form
    {
        RunInterface _runProgram = new RunInterface();

        public WindowsApp()
        {
            InitializeComponent();
        }
        private void btnRunProgram_Click(object sender, EventArgs e)
        {
            _runProgram.RunRemovalProgram();
        }
    }
}
