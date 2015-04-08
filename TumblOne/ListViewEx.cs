namespace TumblOne
{
    using System;
    using System.Windows.Forms;

    public class ListViewEx : ListView
    {
        public ListViewEx()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}

