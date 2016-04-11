namespace TumblTwo

{
    using System.Windows.Forms;

    public class ListViewEx : ListView
    {
        public ListViewEx()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}

