using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace calibration_app.lib
{


    public class ExDataGrid : DataGrid
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
            {
                try
                {
                    base.CommitEdit();
                }
                catch (Exception ex)
                {
                    base.CancelEdit();

                    string mess = ex.Message;
                    if (ex.InnerException != null)
                        mess += "nn" + ex.InnerException.Message;
                    MessageBox.Show(mess);
                }
            }

            base.OnPreviewKeyDown(e);
        }
    }


}
