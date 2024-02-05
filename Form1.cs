using System;
using System.Drawing.Imaging;

namespace Paint_App_C_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Width = 1070;
            this.Height = 900;

            // Метод для рисования(растровое изображение)
            bm = new Bitmap(pic.Width, pic.Height);
            g = Graphics.FromImage(bm);
            g.Clear(Color.White);
            pic.Image = bm;
        }





        Bitmap bm;
        Graphics g;

        // Начальное значение параметра рисование
        bool paint = false;

        // Перемещение X и Y
        Point px, py;

        // Создание карандаша(шиирина 2 пиксель)
        Pen p = new Pen(Color.Black, 2);


        // Создание ластика(ширина 10 пикселей)
        Pen erase = new Pen(Color.White, 10);
        int index;
        int x, y, sX, sY, cX, cY;

        // Диалогоовое окно для выбора цвета
        ColorDialog cd = new ColorDialog();
        Color new_color;

        // Метод нажатия ЛКМ
        // На холсте начинается рисование
        private void pic_MouseDown(object sender, MouseEventArgs e)
        {

            paint = true;
            py = e.Location;
            
            // Сохраняем координаты по клику ЛКМ
            cX = e.X;
            cY = e.Y;

            // Плавное движение карандаша и ластика
            p.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            p.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            erase.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            erase.EndCap = System.Drawing.Drawing2D.LineCap.Round;


        }

        // Метод двидения курсора при пажатой ЛКМ
        // Карандаш(index = 1) или ластик(index = 2) для рисования линии произвольной формы по нажатию ЛКМ

        private void pic_MouseMove(object sender, MouseEventArgs e)
        {
            if (paint)
            {
                if (index == 1) // Карандаш(index = 1)
                {
                    px = e.Location;
                    g.DrawLine(p, px, py);
                    py = px;
                }
                if (index == 2) // Ластик(index = 2)
                {
                    px = e.Location;
                    g.DrawLine(erase, px, py);
                    py = px;
                }
            }
            pic.Refresh();

            // Изменение координат
            x = e.X;
            y = e.Y;
            // Получение ширины и высоты
            sX = e.X - cX;
            sY = e.Y - cY;



        }
        // Метод не нажата ЛКМ
        // Остановка рисования
        // Смена индекс приводит к изменению выбора фигуры для дальнейшего рисования
        // 3 - круг, 4 - прямоугольник, 5 - линия
        private void pic_MouseUp(object sender, MouseEventArgs e)
        {
            paint = false;
            sX = x - cX;
            sY = y - cY;

            if (index == 3) // Круг
            {
                g.DrawEllipse(p, cX, cY, sX, sY);
            }
            if (index == 4) // Прямоугольник
            {
                g.DrawRectangle(p, cX, cY, sX, sY);
            }
            if (index == 5) // Линия
            {
                g.DrawLine(p, cX, cY, x, y);
            }

        }
        //Метод карандаша(index = 1)
        private void btn_pencil_Click(object sender, EventArgs e)
        {
            index = 1;
        }
        //Метод ластика(index = 2)
        private void btn_eraser_Click(object sender, EventArgs e)
        {
            index = 2;
        }
        //Метод выбора круга
        private void btn_ellips_Click(object sender, EventArgs e)
        {
            index = 3;
        }
        // Метод выбора прямоугольника
        private void btn_rect_Click(object sender, EventArgs e)
        {
            index = 4;
        }
        // Метод выбора линии
        private void btn_line_Click(object sender, EventArgs e)
        {
            index = 5;
        }

        // Метод для рисования выбранного идекса
        private void pic_Paint(object sender, PaintEventArgs e)
        {
            // Отображение текуей позиции курсора в зависимости от индекса (инструмента)
            Graphics g = e.Graphics; ;
            if (paint)
            {
                if (index == 3)
                {
                    g.DrawEllipse(p, cX, cY, sX, sY);// 3 - круг
                }
                if (index == 4)
                {
                    g.DrawRectangle(p, cX, cY, sX, sY);// 4 - прямоугольник
                }
                if (index == 5)
                {
                    g.DrawLine(p, cX, cY, x, y);// 5 - линия
                }
            }
        }

        // Метод очистки холста(индекс = 0)
        private void btn_clear_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            pic.Image = bm;
            index = 0;

        }

        // Метод для выбора политры
        private void btn_color_Click(object sender, EventArgs e)
        {
            // В диалоговом окне выбирается и устанавливается новый цвет
            cd.ShowDialog();
            new_color = cd.Color;
            pic_color.BackColor = new_color;
            p.Color = cd.Color;

        }

        // Метод для установки и возврата  координыты (для цветовой политры)
        static Point set_point(PictureBox pb, Point pt)
        {
            float pX = 1f * pb.Image.Width / pb.Width;
            float pY = 1f * pb.Image.Height / pb.Height;
            return new Point((int)(pt.X * pX), (int)(pt.Y * pY));
        }

        // Метод для выбора цвета из заготовленной цветовой палитры, используя метод сохранения координыты
        // Присвоение нового цвета
        private void color_picker_MouseClick(object sender, MouseEventArgs e)
        {
            Point point = set_point(color_picker, e.Location);
            pic_color.BackColor = ((Bitmap)color_picker.Image).GetPixel(point.X, point.Y);
            new_color = pic_color.BackColor;
            p.Color = pic_color.BackColor;
        }

        // Метод для проверки пикселя старого цвета, перед заполнением новым цветом
        private void validate(Bitmap bm, Stack<Point> sp, int x, int y, Color old_color, Color new_color)
        {
            Color cx = bm.GetPixel(x, y);
            if (cx == old_color)
            {
                sp.Push(new Point(x, y));
                bm.SetPixel(x, y, new_color);
            }
        }

        //Метод для заливки цветом с проверкой
        // Получение старого цвет пикселя и заливка пространства новым цветом,
        // от координаиы кликаа до тех пор, пока счетщикстекаа не станет больше 0,
        // иначе старый цвет равняется новому цвету, и заливка не произойдет
        public void Fill(Bitmap bm, int x, int y, Color new_clr)
        {
            Color old_color = bm.GetPixel(x, y);
            Stack<Point> pixel = new Stack<Point>();
            pixel.Push(new Point(x, y));
            bm.SetPixel(x, y, new_clr);
            if (old_color == new_clr) return;

            while (pixel.Count > 0)
            {
                Point pt = (Point)pixel.Pop();
                if (pt.X > 0 && pt.Y > 0 && pt.X < bm.Width - 1 && pt.Y < bm.Height - 1)
                {
                    validate(bm, pixel, pt.X - 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y - 1, old_color, new_clr);
                    validate(bm, pixel, pt.X + 1, pt.Y, old_color, new_clr);
                    validate(bm, pixel, pt.X, pt.Y + 1, old_color, new_clr);
                }
            }
        }

        // Метод выбора заливки 
        private void pic_MouseClick(object sender, MouseEventArgs e)
        {
            if (index == 6)
            {
                Point point = set_point(pic, e.Location);
                Fill(bm, point.X, point.Y, new_color);
            }
        }


        // Метод заливки (index = 6)
        private void btn_fill_Click(object sender, EventArgs e)
        {
            index = 6;
        }

        // Метотод изменение размера пятна
        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            p.Width = trackBar.Value;
            erase.Width = trackBar.Value;
        }

        // Метод сохранения изображения
        private void btn_save_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Image(*.jpg)|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                
                //Bitmap btm = bm.Clone(new Rectangle(0, 0, pic.Width, pic.Width), bm.PixelFormat);
                bm.Save(sfd.FileName, ImageFormat.Jpeg);
                MessageBox.Show("Image Saved Sucesfully.");
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pic_color_Click(object sender, EventArgs e)
        {

        }


    }
}
