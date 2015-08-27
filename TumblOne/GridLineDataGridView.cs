using System.Drawing;
using System.Windows.Forms;

namespace TumblOne
{
    public class GridLineDataGridView : DataGridView
    {
        public GridLineDataGridView()
        {
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int rowHeight = this.RowTemplate.Height;

            int h = this.ColumnHeadersHeight + rowHeight * this.RowCount;
            int imgWidth = this.Width - 2;
            Rectangle rFrame = new Rectangle(0, 0, imgWidth, rowHeight);
            Rectangle rFill = new Rectangle(1, 1, imgWidth - 2, rowHeight);
            Rectangle rowHeader = new Rectangle(2, 2, this.RowHeadersWidth - 3, rowHeight);

            Pen pen = new Pen(this.GridColor, 1);

            Bitmap rowImg = new Bitmap(imgWidth, rowHeight);
            Graphics g = Graphics.FromImage(rowImg);
            g.DrawRectangle(pen, rFrame);
            g.FillRectangle(new SolidBrush(this.DefaultCellStyle.BackColor), rFill);
            g.FillRectangle(new SolidBrush
               (this.RowHeadersDefaultCellStyle.BackColor), rowHeader);

            int w = this.RowHeadersWidth - 1;
            for (int j = 0; j < this.ColumnCount; j++)
            {
                g.DrawLine(pen, new Point(w, 0), new Point(w, rowHeight));
                w += this.Columns[j].Width;
            }

            int loop = (this.Height - h) / rowHeight;
            for (int j = 0; j < loop + 1; j++)
            {
                e.Graphics.DrawImage(rowImg, 1, h + j * rowHeight);
            }
        }
    }
}
