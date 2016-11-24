﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace boost
{
    public partial class main : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        private Bitmap currentImage;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        Panel imagePanel;

        Panel toolbar;

        public main()
        {
            InitializeComponent();

            this.Width = Screen.FromControl(this).Bounds.Width;
            this.Height = Screen.FromControl(this).Bounds.Height;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(41, 41, 45);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);

            Panel sidebar = new Panel();
            sidebar.Width = 100;
            sidebar.BackColor = Color.FromArgb(44, 44, 49);
            sidebar.Height = this.Height - 60;
            sidebar.Location = new Point(0, 20);

            toolbar = new Panel();
            toolbar.Width = this.Width;
            toolbar.BackColor = Color.FromArgb(54, 184, 183);
            toolbar.Height = 40;
            toolbar.Location = new Point(0, this.Height - 40);

            Panel topBar = new Panel();
            topBar.BackColor = Color.FromArgb(48, 48, 55);
            topBar.Width = this.Width;
            topBar.Height = 20;
            topBar.MouseDown += scrncnt;

            imagePanel = new Panel();
            imagePanel.Location = new Point(100, 20);
            imagePanel.Width = this.Width - 100;
            imagePanel.Height = this.Height - 60;

            this.Controls.Add(imagePanel);
            this.Controls.Add(sidebar);
            this.Controls.Add(toolbar);
            this.Controls.Add(topBar);

            PictureBox close = new PictureBox();
            close.Width = 20;
            close.Height = 20;
            close.SizeMode = PictureBoxSizeMode.Zoom;
            close.Image = Properties.Resources.closeicn;
            close.Location = new Point(topBar.Width - close.Width * 2, topBar.Height / 2 - (close.Height / 2));
            close.Click += Close_Click;
            close.Cursor = Cursors.Hand;

            PictureBox minimize = new PictureBox();
            minimize.Width = 20;
            minimize.Height = 20;
            minimize.SizeMode = PictureBoxSizeMode.Zoom;
            minimize.Image = Properties.Resources.minimize;
            minimize.Location = new Point(close.Location.X - minimize.Width * 2, topBar.Height / 2 - (minimize.Height / 2));
            minimize.Click += Minimize_Click;
            minimize.Cursor = Cursors.Hand;

            topBar.Controls.Add(close);
            topBar.Controls.Add(minimize);

            PictureBox bst = new PictureBox();
            bst.Height = 100;
            bst.Width = 100;
            bst.SizeMode = PictureBoxSizeMode.Zoom;
            bst.Image = Properties.Resources.boost_square;
            bst.BackColor = Color.FromArgb(54, 184, 183);

            PictureBox import = new PictureBox();
            import.Width = 35;
            import.Height = 35;
            import.Location = new Point(sidebar.Width / 2 - (import.Width / 2), bst.Location.Y + bst.Height + 100);
            import.Image = Properties.Resources.import;
            import.SizeMode = PictureBoxSizeMode.Zoom;
            import.Cursor = Cursors.Hand;
            import.Click += Import_Click;

            PictureBox assist = new PictureBox();
            assist.Width = 27;
            assist.Height = 27;
            assist.Location = new Point(sidebar.Width / 2 - (assist.Width / 2), import.Location.Y + 150);
            assist.Image = Properties.Resources.assist;
            assist.SizeMode = PictureBoxSizeMode.Zoom;
            assist.Cursor = Cursors.Hand;
            assist.Click += Assist_Click;

            sidebar.Controls.Add(import);
            sidebar.Controls.Add(bst);
            sidebar.Controls.Add(assist);

            Label importLabel = new Label();
            importLabel.Text = "Import";
            importLabel.Font = new Font("Segoe UI Light", 10, FontStyle.Regular);
            importLabel.Width = importLabel.PreferredWidth;
            importLabel.Height = importLabel.PreferredHeight;
            importLabel.Location = new Point(sidebar.Width / 2 - importLabel.Width / 2, import.Location.Y + import.Height + 20);
            importLabel.ForeColor = Color.White;

            Label assistLabel = new Label();
            assistLabel.Text = "Project";
            assistLabel.TextAlign = ContentAlignment.MiddleCenter;
            assistLabel.Font = new Font("Segoe UI Light", 10, FontStyle.Regular);
            assistLabel.Width = importLabel.PreferredWidth;
            assistLabel.Height = importLabel.PreferredHeight;
            assistLabel.Location = new Point(sidebar.Width / 2 - assistLabel.Width / 2, assist.Location.Y + assist.Height + 20);
            assistLabel.ForeColor = Color.White;

            sidebar.Controls.Add(importLabel);
            sidebar.Controls.Add(assistLabel);

            imageFrameRedraw();
        }

        private void Assist_Click(object sender, EventArgs e)
        {
        }

        private void Import_Click(object sender, EventArgs e)
        {
            OpenFileDialog imageSelectDialog = new OpenFileDialog();
            imageSelectDialog.Title = "Open Image";
            imageSelectDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            if (imageSelectDialog.ShowDialog() == DialogResult.OK)
            {
                currentImage = new Bitmap(Image.FromFile(imageSelectDialog.FileName));

                imageFrameRedraw();
            }
        }

        PictureBox image = new PictureBox();
        Label noImageLabel = new Label();
        private void imageFrameRedraw()
        {
            if (currentImage == null)
            {
                noImageLabel.Text = "NO IMAGE";
                noImageLabel.ForeColor = Color.FromArgb(88, 88, 95);
                noImageLabel.Font = new Font("Segoe UI Light", 15, FontStyle.Regular);
                noImageLabel.Size = noImageLabel.PreferredSize;
                noImageLabel.Location = new Point(imagePanel.Width / 2 - noImageLabel.Width / 2, imagePanel.Height / 2 - noImageLabel.Height / 2);

                imagePanel.Controls.Add(noImageLabel);
            }
            else
            {
                imagePanel.Controls.Remove(noImageLabel);

                PictureBox editButton = new PictureBox();
                editButton.Image = Properties.Resources.edit;
                editButton.Width = 20;
                editButton.Height = 20;
                editButton.SizeMode = PictureBoxSizeMode.Zoom;
                editButton.Location = new Point(toolbar.Width / 2 - editButton.Width / 2, toolbar.Height / 2 - editButton.Height / 2);
                editButton.Cursor = Cursors.Hand;
                editButton.Click += EditButton_Click;

                image.Image = currentImage;
                image.Size = image.PreferredSize;
                image.Location = new Point(imagePanel.Width / 2 - image.Width / 2, imagePanel.Height / 2 - image.Height / 2);

                imagePanel.Controls.Add(image);

                toolbar.Controls.Add(editButton);
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            // TODO
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void scrncnt(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
