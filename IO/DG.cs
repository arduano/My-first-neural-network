using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace IO
{
    class DrawGraphics : IDisposable
    {
        #region buffer support

        private Control m_ctl = null;
        private Graphics m_g = null;
        private bool m_useBuffer = false;
        public Bitmap m_buffer = null;

        private int m_width = 0;
        private int m_height = 0;
        private int m_top = 0;
        private int m_left = 0;

        private Pen pen;

        public DrawGraphics(Control ctl, bool useBuffer)
        {
            m_useBuffer = useBuffer;
            m_ctl = ctl;
            Init();

        }

        private void Init()
        {
            m_top = m_ctl.ClientRectangle.Top;
            m_left = m_ctl.ClientRectangle.Left;
            m_width = m_ctl.ClientRectangle.Width;
            m_height = m_ctl.ClientRectangle.Height;

            if (m_useBuffer)
            {
                if (m_buffer != null)
                {
                    m_buffer.Dispose();
                }
                m_buffer = new Bitmap(m_width, m_height);
            }
        }

        public void Start()
        {
            if (m_width != m_ctl.ClientRectangle.Width || m_height != m_ctl.ClientRectangle.Height)
            {
                Init();
            }

            if (m_useBuffer)
            {
                m_g = Graphics.FromImage(m_buffer);
            }
            else
            {
                m_g = m_ctl.CreateGraphics();
            }
        }

        public void Finish()
        {
            if (m_useBuffer)
            {
                Graphics g = m_ctl.CreateGraphics();
                g.DrawImage(m_buffer, 0, 0);
                g.Dispose();
            }
            else
            {
                //do nothing
            }
            m_g.Dispose();
        }

        public void Dispose()
        {
            Finish();
        }

        #endregion

        public float Width
        {
            get { return m_width; }
        }

        public float Height
        {
            get { return m_height; }
        }

        public float Top
        {
            get { return m_top; }
        }

        public float Left
        {
            get { return m_left; }
        }

        public void Clear(Color color)
        {
            m_g.Clear(color);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color color, float width)
        {
            Pen pen = new Pen(color, width);
            pen.EndCap = LineCap.Round;
            pen.StartCap = LineCap.Round;
            m_g.DrawLine(pen, x1, y1, x2, y2);
            pen.Dispose();
        }

        public void DrawShape(Point[] points, Color color)
        {
            Brush brush = new SolidBrush(color);
            m_g.FillPolygon(brush, points);
            brush.Dispose();
        }

        public void DrawEllipse(float x, float y, float width, float height, Color fillColor, Color borderColor, float borderWidth)
        {
            if (fillColor != Color.Empty)
            {
                Brush brush = new SolidBrush(fillColor);
                m_g.FillEllipse(brush, x, y, width, height);
                brush.Dispose();
            }
            if (borderColor != Color.Empty)
            {
                Pen pen = new Pen(borderColor, borderWidth);
                m_g.DrawEllipse(pen, x, y, width, height);
                pen.Dispose();
            }
        }

        public void DrawCircle(float x, float y, float radius, Color fillColor, Color borderColor, float borderWidth)
        {
            DrawEllipse(x - radius, y - radius, radius * 2, radius * 2, fillColor, borderColor, borderWidth);
        }

        public void DrawRectangle(float x, float y, float width, float height, Color fillColor, Color borderColor, float borderWidth)
        {
            if (fillColor != Color.Empty)
            {
                Brush brush = new SolidBrush(fillColor);
                m_g.FillRectangle(brush, x, y, width, height);
                brush.Dispose();
            }
            if (borderColor != Color.Empty)
            {
                Pen pen = new Pen(borderColor, borderWidth);
                m_g.DrawRectangle(pen, x, y, width, height);
                pen.Dispose();
            }
        }


        public void DrawPoint(float x, float y, Color color)
        {
            DrawRectangle(x, y, 1, 1, color, Color.Empty, 0);
        }

        public void DrawText(float x, float y, Color color, string fontName, float fontSize, string text, StringFormat sf)
        {
            Brush brush = new SolidBrush(color);
            Font font = new Font(fontName, fontSize);
            m_g.DrawString(text, font, brush, x, y, sf);
            font.Dispose();
            brush.Dispose();
        }
    }
}
