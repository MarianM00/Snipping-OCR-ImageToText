using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using MouseEventHandler = System.Windows.Forms.MouseEventHandler;

namespace Snipping_OCR_ImageToText
{
    public class SnippingForm : Form
    {
        private Point startPoint;
        private Rectangle selectionRect;
        public Bitmap CapturedImage { get; private set; }

        public SnippingForm()
        {
            this.Bounds = GetCombinedScreenBounds();
            this.DoubleBuffered = true;
            this.TopMost = true;
            this.BackColor = Color.Gray;
            this.Opacity = 0.5;
            this.FormBorderStyle = FormBorderStyle.None;
            

            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
            this.MouseUp += new MouseEventHandler(OnMouseUp);
        }

        public Rectangle GetCombinedScreenBounds()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            foreach (var screen in Screen.AllScreens)
            {
                bounds = Rectangle.Union(bounds, screen.Bounds);
            }
            return bounds;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                selectionRect = new Rectangle(e.Location, new Size(0, 0)); 
                Invalidate();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectionRect.Width = e.X - startPoint.X;
                selectionRect.Height = e.Y - startPoint.Y;
                Invalidate();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (selectionRect.Width > 0 && selectionRect.Height > 0)
            {
                var absoluteX = selectionRect.Left + this.Left; 
                var absoluteY = selectionRect.Top + this.Top;

                using (var bitmap = new Bitmap(selectionRect.Width, selectionRect.Height))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(absoluteX, absoluteY, 0, 0, selectionRect.Size);
                    }
                    CapturedImage = new Bitmap(bitmap);
                }

                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }

            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, selectionRect);
            }
        }
    }
}


