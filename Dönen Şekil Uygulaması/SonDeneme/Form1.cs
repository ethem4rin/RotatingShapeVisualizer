using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SonDeneme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Form Özellikleri
            this.Text = "Döndürme İşlemi";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.DoubleBuffered = true;

            // Olay Bağlantıları
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.Resize += (gonderen, e) => this.Invalidate();
        }

        /// donusumNoktasi = Döndürülecek nokta
        /// merkezNokta = Dönme merkezi
        /// aciDerece = Dönme açısı (Saat yönü negatif, tersi pozitif)
        private PointF NoktaDondur(PointF donusumNoktasi, PointF merkezNokta, float aciDerece)
        {
            // 1. Dereceyi radyana çevirme islemi (Math sınıfı radyan kullanır)
            double aciRadyan = aciDerece * (Math.PI / 180);
            double kosAci = Math.Cos(aciRadyan);
            double sinAci = Math.Sin(aciRadyan);

            // 2. Noktayı merkeze göre öteleme islemi (merkez orijin olacak şekilde)
            float x = donusumNoktasi.X - merkezNokta.X;
            float y = donusumNoktasi.Y - merkezNokta.Y;

            // 3. Orijin etrafında döndürme işlemi
            float yeniX = (float)(x * kosAci - y * sinAci);
            float yeniY = (float)(x * sinAci + y * kosAci);

            // 4. Noktayı eski konumuna geri ötele
            yeniX += merkezNokta.X;
            yeniY += merkezNokta.Y;

            return new PointF(yeniX, yeniY);
        }

        /// Form çizim alanı = eksenler, orijinal üçgen, döndürülmüş üçgen ve merkez noktalar.
        private void Form1_Paint(object gonderen, PaintEventArgs e)
        {
            Graphics cizim = e.Graphics;
            cizim.SmoothingMode = SmoothingMode.AntiAlias;

            //  Koordinat Sistemi Ayarları 
            cizim.TranslateTransform(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
            cizim.ScaleTransform(1, -1); // Y eksenini yukarı pozitif yapmak için

            // Görselliği artırmak için ölçek faktörü
            float olcek = 25.0f;

            // 1. Koordinat eksenleri 
            using (Pen eksenKalemi = new Pen(Color.Gray, 1))
            {
                cizim.DrawLine(eksenKalemi, -this.ClientSize.Width, 0, this.ClientSize.Width, 0);   // X ekseni
                cizim.DrawLine(eksenKalemi, 0, -this.ClientSize.Height, 0, this.ClientSize.Height); // Y ekseni
            }

            // 2. Verilen noktalar 
            PointF NoktaA = new PointF(-3, 3);
            PointF NoktaB = new PointF(-5, 3);
            PointF NoktaC = new PointF(-3, 1);
            PointF MerkezP1 = new PointF(7, 1);
            PointF MerkezP2 = new PointF(1, 8);

            //  3. Orijinal (mavi) üçgeni çizme
            PointF[] orijinalUcgen = {
                new PointF(NoktaA.X * olcek, NoktaA.Y * olcek),
                new PointF(NoktaB.X * olcek, NoktaB.Y * olcek),
                new PointF(NoktaC.X * olcek, NoktaC.Y * olcek)
            };

            using (Pen maviKalem = new Pen(Color.Blue, 2))
                cizim.DrawPolygon(maviKalem, orijinalUcgen);

            // 4. Dönüşüm hesaplamaları

            // İlk dönüşüm: P1 etrafında saat yönünde 44 derece
            PointF A1 = NoktaDondur(NoktaA, MerkezP1, -44);
            PointF B1 = NoktaDondur(NoktaB, MerkezP1, -44);
            PointF C1 = NoktaDondur(NoktaC, MerkezP1, -44);

            // İkinci dönüşüm: P2 etrafında saat yönünün tersi 20 derece 
            PointF A_son = NoktaDondur(A1, MerkezP2, 20);
            PointF B_son = NoktaDondur(B1, MerkezP2, 20);
            PointF C_son = NoktaDondur(C1, MerkezP2, 20);

            // 5. Son (kırmızı) üçgeni çizme 
            PointF[] sonUcgen = {
                new PointF(A_son.X * olcek, A_son.Y * olcek),
                new PointF(B_son.X * olcek, B_son.Y * olcek),
                new PointF(C_son.X * olcek, C_son.Y * olcek)
            };

            using (Pen kirmiziKalem = new Pen(Color.Red, 2))
                cizim.DrawPolygon(kirmiziKalem, sonUcgen);

            // 6. Noktaları Görsel Olarak Belirtiyoruz
            Font yaziTipi = new Font("Arial", 8);

            // Metinlerin düzgün görünmesi için Y eksenini geçici olarak düzeltiyoruz
            cizim.ScaleTransform(1, -1);

            // Orijinal üçgenin köşe noktaları (mavi) ve etiketleri (A, B, C)
            cizim.FillEllipse(Brushes.Blue, (NoktaA.X * olcek) - 3, (-NoktaA.Y * olcek) - 3, 6, 6);
            cizim.DrawString("A", yaziTipi, Brushes.Blue, (NoktaA.X * olcek) + 5, (-NoktaA.Y * olcek) - 5); // YENİ EKLENDİ

            cizim.FillEllipse(Brushes.Blue, (NoktaB.X * olcek) - 3, (-NoktaB.Y * olcek) - 3, 6, 6);
            cizim.DrawString("B", yaziTipi, Brushes.Blue, (NoktaB.X * olcek) - 20, (-NoktaB.Y * olcek) - 5); // YENİ EKLENDİ (Konumu ayarlandı)

            cizim.FillEllipse(Brushes.Blue, (NoktaC.X * olcek) - 3, (-NoktaC.Y * olcek) - 3, 6, 6);
            cizim.DrawString("C", yaziTipi, Brushes.Blue, (NoktaC.X * olcek) + 5, (-NoktaC.Y * olcek) - 5); // YENİ EKLENDİ

            // Dönme merkezleri (P1, P2)
            cizim.FillEllipse(Brushes.Black, (MerkezP1.X * olcek) - 4, (-MerkezP1.Y * olcek) - 4, 8, 8);
            cizim.DrawString("P1", yaziTipi, Brushes.Black, (MerkezP1.X * olcek) + 5, (-MerkezP1.Y * olcek) - 5);

            cizim.FillEllipse(Brushes.Black, (MerkezP2.X * olcek) - 4, (-MerkezP2.Y * olcek) - 4, 8, 8);
            cizim.DrawString("P2", yaziTipi, Brushes.Black, (MerkezP2.X * olcek) + 5, (-MerkezP2.Y * olcek) - 5);

            // Son üçgenin köşe noktaları (kırmızı) ve etiketleri (A', B', C')
            cizim.FillEllipse(Brushes.Red, (A_son.X * olcek) - 3, (-A_son.Y * olcek) - 3, 6, 6);
            cizim.DrawString("A'", yaziTipi, Brushes.Red, (A_son.X * olcek) + 5, (-A_son.Y * olcek) - 5);

            cizim.FillEllipse(Brushes.Red, (B_son.X * olcek) - 3, (-B_son.Y * olcek) - 3, 6, 6);
            cizim.DrawString("B'", yaziTipi, Brushes.Red, (B_son.X * olcek) + 5, (-B_son.Y * olcek) - 5);

            cizim.FillEllipse(Brushes.Red, (C_son.X * olcek) - 3, (-C_son.Y * olcek) - 3, 6, 6);
            cizim.DrawString("C'", yaziTipi, Brushes.Red, (C_son.X * olcek) + 5, (-C_son.Y * olcek) - 5);
        }
    }
}