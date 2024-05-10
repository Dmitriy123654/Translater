using System.Linq;
using System.Text.RegularExpressions;

namespace laba1
{
    partial class Form1

    {

        public static string inputText = "Русский";
        public static string outputText = "Английский";
        private bool stateOfLanguage = true;
        //private static DataStorage dataStorage;
        private System.ComponentModel.IContainer components = null;
       

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose(); 
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private (List<string>, bool, List<string>) Translate(string inputString, bool stateOfLanguage)
        {
            List<string> OutPutlist = new List<string>();
            string output = "";
            //обрезаем пробелы и \n 
            inputString = inputString.Trim();
            //DataStorage.ChangeLanguage(stateOfLanguage);

            if (stateOfLanguage && DataStorage.Words.ContainsKey(inputString))
            {
                output = DataStorage.Words[inputString];
                OutPutlist.Add(output);
                List<string> updatedPossibleErrors2 = new List<string>();

                foreach (var item in OutPutlist)
                {
                    string[] words = item.Split(' ');

                    foreach (var word in words)
                    {
                        updatedPossibleErrors2.Add(word);
                    }
                }
                return (updatedPossibleErrors2, true, null);
            }
            else if (!stateOfLanguage && DataStorage.TransletedWords.ContainsKey(inputString))
            {
                output = DataStorage.TransletedWords[inputString];
                OutPutlist.Add(output);
                List<string> updatedPossibleErrors3 = new List<string>();

                foreach (var item in OutPutlist)
                {
                    string[] words = item.Split(' ');

                    foreach (var word in words)
                    {
                        updatedPossibleErrors3.Add(word);
                    }
                }
                return (updatedPossibleErrors3, true, null);
            }
            List<string> possibleErrors = new List<string>();

            if (stateOfLanguage)
                possibleErrors = FindPossibleErrors(inputString, DataStorage.Words);
            else if(!stateOfLanguage)
                possibleErrors = FindPossibleErrors(inputString, DataStorage.TransletedWords);

            if (possibleErrors.Count == 1)
            {
                string correctedWord = possibleErrors[0];
                if (stateOfLanguage)
                {
                    if (DataStorage.Words.ContainsKey(correctedWord))
                    {
                        // Найдено исправленное слово в русско-английском словаре
                        output = DataStorage.Words[correctedWord];

                        OutPutlist.Add(output);
                    }
                }
                else if (!stateOfLanguage && DataStorage.TransletedWords.ContainsKey(correctedWord))
                {
                    //Найдено исправленное слово в англо-русском словаре
                    output = DataStorage.TransletedWords[correctedWord];
                    OutPutlist.Add(output);
                }
            }
            else if (possibleErrors.Count > 1)
            {

                OutPutlist.AddRange(possibleErrors.Distinct());
            }


            List<string> NonTranslated = new List<string>();

            if (stateOfLanguage)
            {
                foreach (var NonTranslatedString in possibleErrors)
                {
                    if (DataStorage.TransletedWords.TryGetValue(NonTranslatedString, out var translation))
                    {
                        NonTranslated.Add(translation);
                    }
                    else
                    {
                        string[] words = NonTranslatedString.Split(' ');

                        foreach (var word in words)
                        {
                            if (DataStorage.TransletedWords.TryGetValue(word, out var subTranslation))
                            {
                                NonTranslated.Add(subTranslation);
                                break;
                            }
                        }

                        // Если ни одно из слов не найдено в словаре, добавляем исходное слово без перевода
                        if (NonTranslated.Count == 0)
                        {
                            NonTranslated.Add(NonTranslatedString);
                        }
                    }
                }
            }
            else if(!stateOfLanguage)
            {
                foreach (var NonTranslatedString in possibleErrors)
                {
                    if (DataStorage.Words.TryGetValue(NonTranslatedString, out var translation))
                    {
                        NonTranslated.Add(translation);
                    }
                    else
                    {
                        string[] words = NonTranslatedString.Split(' ');

                        foreach (var word in words)
                        {
                            if (DataStorage.Words.TryGetValue(word, out var subTranslation))
                            {
                                NonTranslated.Add(subTranslation);
                                break;
                            }
                        }

                        // Если ни одно из слов не найдено в словаре, добавляем исходное слово без перевода
                        if (NonTranslated.Count == 0)
                        {
                            NonTranslated.Add(NonTranslatedString);
                        }
                    }
                }
            }
            List<string> updatedPossibleErrors = new List<string>();

            foreach (var item in possibleErrors)
            {
                string[] words = item.Split(' ');

                foreach (var word in words)
                {
                    updatedPossibleErrors.Add(word);
                }
            }

            return (updatedPossibleErrors, false, NonTranslated);
        }

        private void InitializeComponent()
        {
           
            this.title = new System.Windows.Forms.Label();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.inputLanguageLabel = new System.Windows.Forms.Label();
            this.OutputLanguageLabel = new System.Windows.Forms.Label();
            this.OutputListBox = new System.Windows.Forms.ListBox();
            this.translateButton = new System.Windows.Forms.Button();
            this.ChangeLanguageButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.SettingsButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.historyButton = new System.Windows.Forms.Button();
            // Создание новой кнопки "historyButton"
            

            // 
            // historyButton
            // 

            this.historyButton.Text = "История переводов";
            this.historyButton.Location = new Point(10, 10); // Задаем координаты кнопки на форме
            this.historyButton.Size = new Size(125, 30); // Задаем размер кнопки
            this.historyButton.Location = new System.Drawing.Point(0, 400);
            this.historyButton.Click += new EventHandler(historyButton_Click); // Добавляем обработчик события Click

            

            this.SuspendLayout();



            // 
            // title
            // 
            this.title.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.title.AutoSize = true;
            this.title.Location = new System.Drawing.Point(383, 9);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(94, 20);
            this.title.TabIndex = 0;
            this.title.Text = "Переводчик";
            this.title.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(95, 85);
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.PlaceholderText = "Введите текст";
            this.inputTextBox.Size = new System.Drawing.Size(174, 108);
            this.inputTextBox.TabIndex = 1;
            // 
            // inputLanguageLabel
            // 
            this.inputLanguageLabel.AutoSize = true;
            this.inputLanguageLabel.Location = new System.Drawing.Point(95, 60);
            this.inputLanguageLabel.Name = "inputLanguageLabel";
            this.inputLanguageLabel.Size = new System.Drawing.Size(63, 20);
            this.inputLanguageLabel.TabIndex = 2;
            this.inputLanguageLabel.Text = "Русский";
            // 
            // OutputLanguageLabel
            // 
            this.OutputLanguageLabel.AutoSize = true;
            this.OutputLanguageLabel.Location = new System.Drawing.Point(591, 64);
            this.OutputLanguageLabel.Name = "OutputLanguageLabel";
            this.OutputLanguageLabel.Size = new System.Drawing.Size(92, 20);
            this.OutputLanguageLabel.TabIndex = 3;
            this.OutputLanguageLabel.Text = "Английский";
            this.OutputLanguageLabel.Click += new System.EventHandler(this.label3_Click);
            // 
            // OutputListBox
            // 
            this.OutputListBox.FormattingEnabled = true;
            this.OutputListBox.ItemHeight = 20;
            this.OutputListBox.Location = new System.Drawing.Point(591, 89);
            this.OutputListBox.Name = "OutputListBox";
            this.OutputListBox.Size = new System.Drawing.Size(175, 104);
            this.OutputListBox.TabIndex = 4;
            // 
            // translateButton
            // 
            this.translateButton.Location = new System.Drawing.Point(371, 123);
            this.translateButton.Name = "translateButton";
            this.translateButton.Size = new System.Drawing.Size(117, 29);
            this.translateButton.TabIndex = 5;
            this.translateButton.Text = "Перевести";
            this.translateButton.UseVisualStyleBackColor = true;
            this.translateButton.Click += new System.EventHandler(this.translateButton_Click);
            // 
            // ChangeLanguageButton
            // 
            this.ChangeLanguageButton.Location = new System.Drawing.Point(371, 60);
            this.ChangeLanguageButton.Name = "ChangeLanguageButton";
            this.ChangeLanguageButton.Size = new System.Drawing.Size(117, 32);
            this.ChangeLanguageButton.TabIndex = 6;
            this.ChangeLanguageButton.Text = "Сменить язык";
            this.ChangeLanguageButton.UseVisualStyleBackColor = true;
            this.ChangeLanguageButton.Click += new System.EventHandler(this.ChangeLanguageButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(371, 164);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(117, 29);
            this.ClearButton.TabIndex = 7;
            this.ClearButton.Text = "Очистить";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // SettingsButton
            // 
            this.SettingsButton.Location = new System.Drawing.Point(758, 0);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(110, 29);
            this.SettingsButton.TabIndex = 8;
            this.SettingsButton.Text = "Настройки";
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // EditButton
            // 
            this.EditButton.Location = new System.Drawing.Point(-2, 0);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(120, 29);
            this.EditButton.TabIndex = 9;
            this.EditButton.Text = "Редактировать";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 461);
            this.Controls.Add(this.EditButton);
            this.Controls.Add(this.SettingsButton);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.ChangeLanguageButton);
            this.Controls.Add(this.translateButton);
            this.Controls.Add(this.OutputListBox);
            this.Controls.Add(this.OutputLanguageLabel);
            this.Controls.Add(this.inputLanguageLabel);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.title);

            // Добавляем кнопку на форму
            Controls.Add(historyButton);

            this.Name = "Form1";
            this.Text = "Переводчик";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
            if (!DataStorage.CheckingJsonFromFile())
            {
                using (var dialog = new Form())
                {
                    dialog.Text = "Выберите язык ввода и перевода";
                    dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dialog.StartPosition = FormStartPosition.CenterScreen;

                    Label inputLabel = new Label();
                    inputLabel.Text = "Язык ввода:";
                    inputLabel.Location = new System.Drawing.Point(12, 15);

                    TextBox inputTextBox = new TextBox();
                    inputTextBox.Location = new System.Drawing.Point(120, 12);

                    Label translationLabel = new Label();
                    translationLabel.Text = "Язык перевода:";
                    translationLabel.Location = new System.Drawing.Point(12, 45);

                    TextBox translationTextBox = new TextBox();
                    translationTextBox.Location = new System.Drawing.Point(120, 42);

                    Button okButton = new Button();
                    okButton.Text = "ОК";
                    okButton.DialogResult = DialogResult.OK;
                    okButton.Location = new System.Drawing.Point(50, 80);

                    Button cancelButton = new Button();
                    cancelButton.Text = "Отмена";
                    cancelButton.DialogResult = DialogResult.Cancel;
                    cancelButton.Location = new System.Drawing.Point(130, 80);

                    okButton.Click += (sender, e) =>
                    {
                        string inputLanguage = inputTextBox.Text;
                        string translationLanguage = translationTextBox.Text;

                        string fileName = $"{inputLanguage}-{translationLanguage}.json";
                        // Создание и сохранение JSON файла с указанным именем
                        File.WriteAllText(fileName, "{}");
                        this.inputLanguageLabel.Text = inputLanguage;
                        this.OutputLanguageLabel.Text = translationLanguage;
                        DataStorage.CheckingJsonFromFile(fileName);

                        dialog.DialogResult = DialogResult.OK;
                    };

                    dialog.AcceptButton = okButton;
                    dialog.CancelButton = cancelButton;
                    dialog.Controls.AddRange(new Control[]
                    {
                        inputLabel,
                        inputTextBox,
                        translationLabel,
                        translationTextBox,
                        okButton,
                        cancelButton
                    });

                    if (dialog.ShowDialog() == DialogResult.Cancel)
                    {
                        this.Close();
                    }
                }

               
            }
            DataStorage.CheckOrCreateTranslationHistoryFile();
        }

        static List<string> FindPossibleErrors(string word, Dictionary<string, string> dictionaryWords)
        {
            List<string> possibleErrors = new List<string>();
            word = word.ToLower();


            foreach (string dictWord in dictionaryWords.Keys)
            {
                if (word.Length == dictWord.Length)
                {
                    // Проверка замены одной буквы
                    int mismatchCount = 0;
                    int mismatchIndex = -1;

                    for (int i = 0; i < word.Length; i++)
                    {
                        if (word[i] != dictWord[i])
                        {
                            mismatchCount++;
                            mismatchIndex = i;
                        }
                    }

                    if (mismatchCount == 1 && mismatchIndex + 1 < word.Length)
                    {
                        // Проверяем, что в замененных позициях символы не поменяны местами
                        if (word[mismatchIndex] == dictWord[mismatchIndex + 1] && word[mismatchIndex + 1] == dictWord[mismatchIndex])
                        {
                            possibleErrors.Add(dictionaryWords[dictWord]);
                        }
                    }

                    // Проверка перестановки букв
                    string sortedWord = String.Concat(word.OrderBy(c => c));
                    string sortedDictWord = String.Concat(dictWord.OrderBy(c => c));

                    if (sortedWord.Equals(sortedDictWord, StringComparison.OrdinalIgnoreCase))
                    {
                        var (swapsCount, nearestSymbols) = CountSwappedNeighboringCharacters(word, dictWord);
                        if (swapsCount == 2 && nearestSymbols)
                        {
                            possibleErrors.Add(dictionaryWords[dictWord]);
                        }
                     
                    }
                }
                else if (Math.Abs(word.Length - dictWord.Length) == 1)
                {
                    // Проверка пропущенного символа
                    for (int i = 0; i <= dictWord.Length; i++)
                    {
                        string partialDictWord = dictWord.Insert(i, word[word.Length - 1].ToString());

                        if (word.Equals(partialDictWord, StringComparison.OrdinalIgnoreCase))
                        {
                            possibleErrors.Add(dictionaryWords[dictWord]);
                        }
                    }

                    // Проверка лишнего символа
                    for (int i = 0; i < word.Length; i++)
                    {
                        string partialWord = word.Remove(i, 1);

                        if (partialWord.Equals(dictWord, StringComparison.OrdinalIgnoreCase))
                        {
                            possibleErrors.Add(dictionaryWords[dictWord]);
                        }
                    }
                }
            }
            // Проверка пропущенных букв
            foreach (string dictWord in dictionaryWords.Keys)
            {
                if (IsSimilarWord(word, dictWord))
                {
                    possibleErrors.Add(dictionaryWords[dictWord]);
                }
            }
            return possibleErrors.Distinct().ToList();
        }
        // Проверка пропущенных букв
        static bool IsSimilarWord(string word, string dictWord)
        {
            int wordLength = word.Length;
            int dictWordLength = dictWord.Length;

            if (Math.Abs(wordLength - dictWordLength) > 1)
            {
                return false;
            }

            if (wordLength == dictWordLength)
            {
                int mismatchCount = 0;

                for (int i = 0; i < wordLength; i++)
                {
                    if (word[i] != dictWord[i])
                    {
                        mismatchCount++;
                    }

                    if (mismatchCount > 1)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                // Проверяем, содержит ли более короткое слово пропущенную букву
                string shorterWord = (wordLength < dictWordLength) ? word : dictWord;
                string longerWord = (wordLength < dictWordLength) ? dictWord : word;

                int shorterLength = shorterWord.Length;
                int longerLength = longerWord.Length;

                for (int i = 0; i < shorterLength; i++)
                {
                    if (shorterWord[i] != longerWord[i])
                    {
                        return shorterWord.Substring(i) == longerWord.Substring(i + 1);
                    }
                }

                // Если все символы совпадают, проверяем, является ли оставшаяся часть длинного слова пропущенной буквой
                return shorterLength == longerLength - 1;
            }
        }
        static (int,bool) CountSwappedNeighboringCharacters(string word1, string word2)
        {
            int count = 0;
            bool NearestSymbols = false;

            if (word1.Length != word2.Length)
            {
                return (count,NearestSymbols);
            }

            for (int i = 0; i < word1.Length - 1; i++)
            {
                if (word1[i] != word2[i])
                {
                    count++;

                    if (count < 2 && word1[i] == word2[i + 1] && word1[i + 1] == word2[i])
                    {
                        NearestSymbols = true;
                        if (word1[i] == word2[i + 1] && i + 1 == word1.Length - 1)
                            count++;
                    }
                }
            }

            return (count,NearestSymbols);
        }
        static Tuple<bool, bool> CheckLanguageValidity(string inputString)
        {
            Regex englishRegex = new Regex("^[a-zA-Z]+$");
            Regex russianRegex = new Regex("^[а-яА-ЯёЁ]+$");

            bool language;
            bool isValid = true;

            int englishCharCount = 0;
            int russianCharCount = 0;
            int specialCharCount = 0;

            foreach (char c in inputString)
            {
                if (englishRegex.IsMatch(c.ToString()))
                    englishCharCount++;
                else if (russianRegex.IsMatch(c.ToString()))
                    russianCharCount++;
                else
                    specialCharCount++;
            }

            if(russianCharCount > englishCharCount)
            {
                if((englishCharCount > 1 && russianCharCount > 0 && specialCharCount == 0) 
                    ||(englishCharCount >= 1  && specialCharCount >= 1 && russianCharCount > 0)
                    ||(specialCharCount > 1 && russianCharCount > 0 && englishCharCount == 0))
                {
                    isValid = false;
                    return Tuple.Create(false, false);
                }
            }
            else if(englishCharCount  > russianCharCount)
            {
                if((russianCharCount > 1 && englishCharCount > 0 && specialCharCount == 0)
                    || (russianCharCount >= 1 && specialCharCount >= 1 && englishCharCount > 0)
                    || (specialCharCount > 1 && englishCharCount > 0 && russianCharCount == 0))
                {
                    isValid = false;
                    return Tuple.Create(false, false);
                }
            }

            if (russianCharCount > englishCharCount)
            {
                return Tuple.Create(true, true);
            }
            else
            {
                return Tuple.Create(true, false);
            }
        }

    

        #endregion

        private Label title;
        private TextBox inputTextBox;
        private Label inputLanguageLabel;
        private Label OutputLanguageLabel;
        private ListBox OutputListBox;
        private Button translateButton;
        private Button ChangeLanguageButton;
        private Button ClearButton;
        private Button SettingsButton;
        private Button EditButton;
        private Button historyButton;
    }

}