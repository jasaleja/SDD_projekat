using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDDLibrary.Utils
{
    /// <summary>
    /// Extension metode za pojednostavljen pristup formi iz drugih niti.
    /// </summary>
    public static class FormExt
    {
        public static T SafeInvoke<TForm, T>(this TForm form, Func<TForm, T> call)
            where TForm : Form
        {
            if (form.InvokeRequired)
            {
                object endResult = form.Invoke(call, new object[] { form });
                return (T)endResult;
            }
            else
                return call(form);
        }

        public static void SafeInvoke<TForm>(this TForm form, Action<TForm> call)
            where TForm : Form
        {
            if (form.InvokeRequired)
                form.Invoke(call, new object[] { form });
            else
                call(form);
        }

        public static void SafeInvokeAsync<TForm>(this TForm form, Action<TForm> call)
            where TForm : Form
        {
            if (form.InvokeRequired)
                form.BeginInvoke(call, new object[] { form });
            else
                call(form);
        }
    }
}
