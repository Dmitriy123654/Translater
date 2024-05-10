/*using System.Data;
using System.Data.OleDb;*/
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Text.Unicode;

namespace laba1
{
    public partial class Form1 : Form
    {
        OleDbConnection connection;

        public Form1()
        {
            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void historyButton_Click(object sender, EventArgs e)
        {
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Directory.GetCurrentDirectory()};Extended Properties='text;HDR=yes;FMT=Delimited'";
            string query = "SELECT * FROM [translation_history.txt]";

            string basePath = Application.StartupPath;
            string filePath = Path.Combine(basePath, "translation_history.txt");

            // Проверяем, существует ли файл
            if (!File.Exists(filePath))
            {
                // Если файл не существует, создаем его и заполняем заголовками
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.Default))
                {
                    writer.WriteLine("Date,Time,Word,Translation");
                }
                MessageBox.Show("Файл истории переводов создан.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Проверяем, есть ли данные в файле
            string[] lines = File.ReadAllLines(filePath, Encoding.Default);
            if (lines.Length <= 1)
            {
                MessageBox.Show("История переводов пуста.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                try
                {
                    // Читаем содержимое файла в массив строк с указанием кодировки Windows-1251
                    string[] lines2 = File.ReadAllLines("translation_history.txt", Encoding.GetEncoding(1251));

                    // Создаем таблицу данных для хранения истории переводов
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("Date");
                    dataTable.Columns.Add("Time");
                    dataTable.Columns.Add("Word");
                    dataTable.Columns.Add("Translation");

                    int corruptedLinesCount = 0;

                    // Обрабатываем каждую строку файла
                    foreach (string line in lines2.Skip(1))
                    {
                        string[] fields = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        // Проверяем, есть ли все необходимые поля в строке
                        if (fields.Length == 4)
                        {
                            // Удаляем кавычки вокруг значений
                            string date = fields[0].Trim('"');
                            string time = fields[1].Trim('"');
                            string word = fields[2].Trim('"');
                            string translation = fields[3].Trim('"');

                            // Добавляем строку в таблицу данных
                            dataTable.Rows.Add(date, time, word, translation);
                        }
                        else
                        {
                            // Увеличиваем счетчик поврежденных строк
                            corruptedLinesCount++;
                        }
                    }

                    // Проверяем количество поврежденных строк
                    if (corruptedLinesCount > 0)
                    {
                        MessageBox.Show($"В файле истории переводов есть {corruptedLinesCount} поврежденных строк. Они будут исключены из отображения.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    // Проверяем, пуста ли таблица
                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("История переводов пуста.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Создаем новую форму для отображения таблицы с историей переводов
                    using (Form historyForm = new Form())
                    {
                        historyForm.Text = "История переводов";
                        historyForm.StartPosition = FormStartPosition.CenterParent;
                        historyForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                        historyForm.MaximizeBox = false;
                        historyForm.MinimizeBox = false;
                        historyForm.Size = new Size(800, 600);

                        // Создаем DataGridView и привязываем к нему данные из таблицы
                        DataGridView dataGridView = new DataGridView();
                        dataGridView.DataSource = dataTable;
                        dataGridView.ReadOnly = true;
                        dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView.Dock = DockStyle.Fill;
                        dataGridView.Size = new Size(780, 570);

                        // Создайте кнопку для очистки файла
                        Button clearButton = new Button();
                        clearButton.Text = "Очистить историю";
                        clearButton.Click += ClearButton_Click;
                        clearButton.Dock = DockStyle.Bottom;

                        // Добавьте кнопку и DataGridView на форму
                        historyForm.Controls.Add(clearButton);
                        historyForm.Controls.Add(dataGridView);

                        // Обработчик события кнопки для очистки файла
                        void ClearButton_Click(object sender, EventArgs e)
                        {
                            try
                            {
                                // Удалите существующий файл истории переводов
                                string filePath = "translation_history.txt";
                                if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }

                                // Создайте новый файл истории переводов и добавьте заголовки столбцов
                                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.Default))
                                {
                                    writer.WriteLine("Date,Time,Word,Translation");
                                }

                                // Очистите таблицу
                                //dataGridView.Rows.Clear();

                                // Показать сообщение об успешной очистке и создании нового файла
                                MessageBox.Show("Файл истории переводов успешно очищен.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                // Показать сообщение об ошибке при очистке и создании файла
                                //MessageBox.Show("Ошибка при очистке и создании файла истории переводов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                historyForm.Close();
                                return;
                            }
                        }

                        historyForm.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при чтении файла истории переводов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void ChangeLanguageButton_Click(object sender, EventArgs e)
        {
            var temp = this.OutputLanguageLabel.Text;
            this.OutputLanguageLabel.Text = this.inputLanguageLabel.Text;
            this.inputLanguageLabel.Text = temp;
            var inputText = this.inputTextBox.Text;
            string[] words = inputText.Split(new[] { "\r\n", " " }, StringSplitOptions.RemoveEmptyEntries);
            List<string> outputWords = new List<string>();
            foreach (string word in this.OutputListBox.Items)
            {
                outputWords.Add(word);
            }
            this.OutputListBox.Items.Clear();
            foreach (string word in words)
            {
                this.OutputListBox.Items.Add(word);
            }
            string newInputText = "";
            foreach (string word in outputWords)
            {
                newInputText += word;
                newInputText += "\r\n";
            }
            this.inputTextBox.Text = newInputText;
            stateOfLanguage = false;

        }

        private void translateButton_Click(object sender, EventArgs e)
        {
            if (this.inputTextBox.Text.Length == 0)
            {
                string message = "Поле ввода не заполнено, пожалуйста, введите слово, требующее перевод";
                string caption = "Ошибка";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
                return;

            }
            this.OutputListBox.Items.Clear();
            string outputString = "";
            (List<string> outputText, bool isCorrect, List<string> primalWord) translatedText = Translate(inputTextBox.Text, stateOfLanguage);
            if (translatedText.outputText.Count() == 0 || translatedText.outputText == null)
            {
                string message = "Для введённого вами слова не был найден перевод, возможно данное слово отсутствует в словаре или содержит ошибку, перепроверьте введённое слово";
                string caption = "Ошибка";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
                return;
            }
            if (translatedText.isCorrect)
            {
                foreach (string item in translatedText.outputText)
                {
                    this.OutputListBox.Items.Add(item);
                    // Добавляем запись в файл истории переводов
                    string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;;Data Source={Directory.GetCurrentDirectory()};Extended Properties='text;HDR=yes;FMT=Delimited'";
                    string query = "INSERT INTO [translation_history.txt] ([Date], [Time], [Word], [Translation]) VALUES (?, ?, ?, ?)";

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        DataStorage.AddDataToFileOfTheTranslateHistory(query, connection, inputTextBox.Text, translatedText.outputText.First());

                    }
                }
            }
            if (!translatedText.isCorrect)
            {
                var inputVariantsText = "";
                this.inputTextBox.Text = "";
                foreach (var item in translatedText.primalWord)
                {
                    inputVariantsText += item + "\r\n";
                }
                this.inputTextBox.Text = inputVariantsText;
                if (translatedText.outputText.Count() > 1)
                {
                    string message = "Для данного слова не был найден однозначный перевод, поэтому представлены потенциальные варианты перевода и соответствующие исходые слова";
                    string caption = "Не был найден однозначный перевод";



                    string[] words = inputTextBox.Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int i = 0;

                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, caption, buttons);

                    string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;;Data Source={Directory.GetCurrentDirectory()};Extended Properties='text;HDR=yes;FMT=Delimited'";
                    string query = "INSERT INTO [translation_history.txt] ([Date], [Time], [Word], [Translation]) VALUES (?, ?, ?, ?)";
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        foreach (string item in translatedText.outputText)
                        {
                            using (OleDbCommand command = new OleDbCommand(query, connection))
                            {
                                DataStorage.AddDataToFileOfTheTranslateHistory(query, connection, words[i++], item);
                            }
                        }
                    }
                }
                else
                {
                    string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;;Data Source={Directory.GetCurrentDirectory()};Extended Properties='text;HDR=yes;FMT=Delimited'";
                    string query = "INSERT INTO [translation_history.txt] ([Date], [Time], [Word], [Translation]) VALUES (?, ?, ?, ?)";

                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {
                        connection.Open();
                        DataStorage.AddDataToFileOfTheTranslateHistory(query, connection, inputTextBox.Text, translatedText.outputText.First());

                    }
                }
                foreach (string item in translatedText.outputText)
                {
                    this.OutputListBox.Items.Add(item);
                }

                this.OutputListBox.Text = outputString;
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            this.inputTextBox.Text = "";
            this.OutputListBox.Items.Clear();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            Settings SettingsForm = new Settings();
            SettingsForm.Show();
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            using (var editDialog = new Form())
            {
                editDialog.Text = "Редактирование и удаление слова";
                editDialog.StartPosition = FormStartPosition.CenterParent;
                editDialog.Height = 370;
                editDialog.Width = 280;

                var wordList = new ListBox()
                {
                    Location = new Point(10, 10),
                    Size = new Size(200, 200)
                };
                editDialog.Controls.Add(wordList);

                // Заполнение списка словами и их переводами
                foreach (var pair in DataStorage.Words)
                {
                    wordList.Items.Add($"{pair.Key} - {pair.Value}");
                }

                var editButton = new Button()
                {
                    Text = "Редактировать",
                    Location = new Point(10, 220),
                    DialogResult = DialogResult.Yes,
                    Width = 120,
                    Height = 30
                };
                editDialog.Controls.Add(editButton);

                var deleteButton = new Button()
                {
                    Text = "Удалить",
                    Location = new Point(140, 220),
                    DialogResult = DialogResult.No,
                    Height = 30
                };
                editDialog.Controls.Add(deleteButton);

                var addButton = new Button()
                {
                    Text = "Добавить",
                    Location = new Point(10, 255),
                    DialogResult = DialogResult.OK,
                    Width = 120,
                    Height = 30
                };
                editDialog.Controls.Add(addButton);

                editDialog.AcceptButton = editButton;
                editDialog.CancelButton = deleteButton;

                DialogResult result = editDialog.ShowDialog();

                if (result == DialogResult.Yes)
                {
                    // Редактирование выбранного слова
                    string selectedWord = wordList.SelectedItem?.ToString();

                    if (selectedWord == null)
                    {
                        MessageBox.Show("Сначала выберите слово для редактирования", "Ошибка", MessageBoxButtons.OK);
                        return;
                    }

                    string[] parts = selectedWord.Split(new string[] { " - " }, StringSplitOptions.None);
                    string word = parts[0];

                    using (var editWordDialog = new Form())
                    {
                        editWordDialog.Height = 170;
                        editWordDialog.Text = "Редактирование слова";
                        editWordDialog.StartPosition = FormStartPosition.CenterParent;

                        var label = new Label()
                        {
                            Text = "Введите новое определение:",
                            Location = new Point(10, 10),
                            AutoSize = true
                        };
                        editWordDialog.Controls.Add(label);

                        var textBox = new TextBox()
                        {
                            Text = word,
                            Location = new Point(10, 30),
                            Size = new Size(258, 20)
                        };
                        editWordDialog.Controls.Add(textBox);

                        var saveButton = new Button()
                        {
                            Text = "Сохранить",
                            Location = new Point(10, 60),
                            Width = 120,
                            Height = 30,
                            DialogResult = DialogResult.OK
                        };
                        editWordDialog.Controls.Add(saveButton);

                        var cancelButton = new Button()
                        {
                            Text = "Отмена",
                            Location = new Point(150, 65),
                            Width = 120,
                            Height = 30,
                            DialogResult = DialogResult.Cancel
                        };
                        editWordDialog.Controls.Add(cancelButton);

                        editWordDialog.AcceptButton = saveButton;
                        editWordDialog.CancelButton = cancelButton;

                        DialogResult editResult = editWordDialog.ShowDialog();

                        if (editResult == DialogResult.OK)
                        {
                            // Получение отредактированного слова из текстового поля
                            string editedWord = textBox.Text;

                            if (editedWord == "" || editedWord == null)
                            {
                                string message = "Одно из полей ввода не содержит информации, перепроверьте введённые даные";
                                string caption = "Ошибка";
                                MessageBoxButtons buttons = MessageBoxButtons.OK;
                                MessageBox.Show(message, caption, buttons);
                                return;
                            }

                            // Редактирование слова в словаре
                            if (DataStorage.Words.ContainsKey(word))
                            {
                                string translation = DataStorage.Words[word];
                                DataStorage.Words.Remove(word);
                                DataStorage.Words[editedWord] = translation;
                            }

                            // Обновление списка слов
                            UpdateWordList();
                        }
                    }
                }
                else if (result == DialogResult.No)
                {
                    // Удаление выбранного слова
                    string selectedWord = wordList.SelectedItem?.ToString();

                    if (selectedWord == null)
                    {
                        MessageBox.Show("Сначала выберите слово для удаления", "Ошибка", MessageBoxButtons.OK);
                        return;
                    }

                    string[] parts = selectedWord.Split(new string[] { "-" }, StringSplitOptions.None);
                    string word = parts[0];
                    word = word.TrimEnd();

                    // Удаление слова из словаря
                    if (DataStorage.Words.ContainsKey(word))
                    {
                        DataStorage.Words.Remove(word);
                    }

                    // Обновление списка слов
                    UpdateWordList();
                }
                else if (result == DialogResult.OK)
                {
                    // Добавление нового слова
                    using (var addWordDialog = new Form())
                    {
                        addWordDialog.Width = 310;
                        addWordDialog.Height = 200;
                        addWordDialog.Text = "Добавление слова";
                        addWordDialog.StartPosition = FormStartPosition.CenterParent;

                        var label = new Label()
                        {
                            Text = "Введите новое слово и его перевод:",
                            Location = new Point(10, 10),
                            AutoSize = true
                        };
                        addWordDialog.Controls.Add(label);

                        var wordLabel = new Label()
                        {
                            Text = "Слово:",
                            Location = new Point(10, 30),
                            AutoSize = true
                        };
                        addWordDialog.Controls.Add(wordLabel);

                        var wordTextBox = new TextBox()
                        {
                            Location = new Point(110, 30),
                            Size = new Size(160, 20)
                        };
                        addWordDialog.Controls.Add(wordTextBox);

                        var translationLabel = new Label()
                        {
                            Text = "Перевод:",
                            Location = new Point(10, 60),
                            AutoSize = true
                        };
                        addWordDialog.Controls.Add(translationLabel);

                        var translationTextBox = new TextBox()
                        {
                            Location = new Point(110, 60),
                            Size = new Size(160, 20)
                        };
                        addWordDialog.Controls.Add(translationTextBox);

                        var saveButton = new Button()
                        {
                            Text = "Сохранить",
                            Width = 100,
                            Height = 30,
                            AutoSize = true,
                            Location = new Point(10, 90),
                            DialogResult = DialogResult.OK
                        };
                        addWordDialog.Controls.Add(saveButton);

                        var cancelButton = new Button()
                        {
                            Text = "Отмена",
                            Location = new Point(196, 90),
                            Height = 30,
                            DialogResult = DialogResult.Cancel
                        };
                        addWordDialog.Controls.Add(cancelButton);

                        addWordDialog.AcceptButton = saveButton;
                        addWordDialog.CancelButton = cancelButton;

                        DialogResult addResult = addWordDialog.ShowDialog();

                        if (addResult == DialogResult.OK)
                        {
                            // Получение нового слова и его перевода из текстовых полей
                            string newWord = wordTextBox.Text;
                            string newTranslation = translationTextBox.Text;

                            if (newWord == "" || newWord == null || newTranslation == "" || newTranslation == null)
                            {
                                string message = "Одно из полей ввода не содержит информации, перепроверьте введённые даные";
                                string caption = "Ошибка";
                                MessageBoxButtons buttons = MessageBoxButtons.OK;
                                MessageBox.Show(message, caption, buttons);
                                return;
                            }

                            // Добавление нового слова в словарь
                            if (!DataStorage.Words.ContainsKey(newWord))
                            {
                                DataStorage.Words[newWord] = newTranslation;
                            }

                            // Обновление списка слов
                            this.OutputListBox.Items.Clear();
                            UpdateWordList();
                        }
                    }
                }
            }
            OutputListBox.Items.Clear();
            DataStorage.UpdateJsonFiles();
        }

        private void UpdateWordList()
        {
            OutputListBox.Items.Clear();
            DataStorage.UpdateJsonFiles();
        }



        public void UpdateLanguageLabels()
        {
            this.inputLanguageLabel.Text = inputText;
            this.OutputLanguageLabel.Text = outputText;

        }
    }

}