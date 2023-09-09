using System;
using System.Windows.Forms;

namespace Sosil_FinalExam
{
    public partial class Result : Form
    {
        public Result(bool isWin)
        {
            InitializeComponent();

            if(isWin == true)
            {
                lblResult.Text = "You Win! Good Play!";
            }
            else
            {
                lblResult.Text = "You Lose... Don't be disappointed";
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
