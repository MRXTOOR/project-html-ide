using FastColoredTextBoxNS;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace IDE_HTML
{
    public partial class Form1 : Form
    {
        public string currentFile = null;
        public Form1()
        {
            InitializeComponent();
            fastColoredTextBox1.AllowDrop = true;
            fastColoredTextBox1.DragEnter += new DragEventHandler(fastColoredTextBox1_DragEnter);
            fastColoredTextBox1.DragDrop += new DragEventHandler(fastColoredTextBox1_DragDrop);
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            fastColoredTextBox1.TextChanged += fastColoredTextBox1_TextChanged;

            copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;

            //--------------------------------------


            AutocompleteMenu autocompleteMenu = new AutocompleteMenu(fastColoredTextBox1);

            // Добавляем список слов, для которых будут показываться подсказки
            autocompleteMenu.Items.SetAutocompleteItems(new[] {
                "<html>", "<head>", "<title>", "<body>",
                "<h1>", "<h2>", "<h3>", "<h4>", "<h5>", "<h6>", "<p>"
                , "<a>", "<img>", "<ul>", "<ol>"
                , "<li>", "<table>", "<tr>", "<td>"
                , "<form>", "<input>", "<select>", "<option>", "<textarea>"
            });

            // Включаем показ подсказок
            autocompleteMenu.Enabled = true;
        }

        public class HtmlAutocompleteItem : AutocompleteItem
        {
            public string Description { get; set; }

            public HtmlAutocompleteItem(string text) : base(text)
            {
            }

            public HtmlAutocompleteItem(string text, string description) : base(text)
            {
                Description = description;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        public DateTime startTime;
        public string GetFilePath()
        {
            return filePath;
        }
        private bool isTextChanged = false;
        private void fastColoredTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                // Проверить, что тип перетаскиваемого объекта поддерживается
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка с файлом");
            }

        }
        private void fastColoredTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            isTextChanged = true;
            webBrowser1.DocumentText = fastColoredTextBox1.Text;
            labelLines.Text = $"Lines: {fastColoredTextBox1.LinesCount}";
            labelChars.Text = $"Chars: {fastColoredTextBox1.TextLength}";
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (isTextChanged)
                {
                    DialogResult result = MessageBox.Show("Сохранить изменения?", "Предупреждение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.Filter = "HTML Files|*.html|All Files|*.*";
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            SaveFile(saveFileDialog1.FileName);
                        }
                        else
                        {
                            e.Cancel = true;
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Ошибка с закрытием программы");
            }

        }

        private void SaveFile(string fileName)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(fastColoredTextBox1.Text);
            }
        }


        private void fastColoredTextBox1_DragDrop(object sender, DragEventArgs e)
        {
            // Получить файл и открыть его в fastColoredTextBox1
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                StreamReader sr = new StreamReader(files[0]);
                fastColoredTextBox1.Text = sr.ReadToEnd();
                sr.Close();
                this.Text = files[0];
            }
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //стерает весь текст из поля
            fastColoredTextBox1.Text = "";
        }
        private void OpenDlg()
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "HTML File|*.html|Any File|*.*";
            if (of.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(of.FileName);
                fastColoredTextBox1.Text = sr.ReadToEnd();
                sr.Close();
                this.Text = of.FileName;
            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenDlg();
        }
        private void SaveDlg()
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text HTML|*.html|Any File|*.*";
            sf.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            sf.FileName = "Новый документ.html";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                filePath = sf.FileName;
                StreamWriter sr = new StreamWriter(filePath);
                sr.WriteLine(fastColoredTextBox1.Text);
                sr.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                SaveDlg();
            }
            else
            {
                try
                {
                    StreamWriter sw = new StreamWriter(filePath);
                    sw.WriteLine(fastColoredTextBox1.Text);
                    sw.Close();
                }
                catch
                {
                    MessageBox.Show("Не удалось сохранить файл.");
                }
            }
        }



        public string filePath = "";


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveDlg();
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Paste();
        }

        private void backgraundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                fastColoredTextBox1.BackColor = cd.Color;
            }
        }

        private void textColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                fastColoredTextBox1.ForeColor = cd.Color;
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog cd = new FontDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                fastColoredTextBox1.Font = cd.Font;
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fastColoredTextBox1.Redo();
        }

        private void backgraundImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Открыть диалог выбора файла изображения
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Загрузить выбранное изображение
                Image backgroundImage = Image.FromFile(openFileDialog.FileName);

                // Установить изображение в качестве фонового изображения и центрировать его
                fastColoredTextBox1.BackgroundImage = backgroundImage;
                fastColoredTextBox1.BackgroundImageLayout = ImageLayout.Center;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                undoToolStripMenuItem_Click(null, null);
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                redoToolStripMenuItem_Click(null, null);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            filePath = this.Text;

        }

        private void usageHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Создание экземпляра второй формы
            Form2 historyForm = new Form2();

            // Отображение второй формы
            historyForm.Show();
        }

        private void terminalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("cmd.exe");
        }

        private void aboutUsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Создание экземпляра второй формы
            Form3 aboutUs = new Form3();

            // Отображение второй формы
            aboutUs.Show();
        }

        private void referenceToolStripMenuItem_Click(object sender, EventArgs e)
        {


            Help.ShowHelp(this, @"C:\Users\Harry\Desktop\Курсовая\IDE_HTML\IDE_HTML\справка.chm", HelpNavigator.TableOfContents);

        }
       
    }
}
