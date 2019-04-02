using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAT
{
    class PopupMessage
    {
        public static void Open(int type, String title, String message)
        {
            if(type == 1)
            {
                MessageBox.Show(message, title,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if(type == 2)
            {
                MessageBox.Show(message, title,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (type == 3)
            {
                MessageBox.Show(message, title,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (type == 4)
            {
                MessageBox.Show(message, title,
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}
