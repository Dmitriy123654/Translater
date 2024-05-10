using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Data.OleDb;

namespace laba1
{
    public class DataStorage
    {
        private const string StandartFile = "Русский-Английский.json";
        public static Dictionary<string, string> Words = new Dictionary<string, string>();
        public static Dictionary<string, string> TransletedWords = new Dictionary<string, string>();
        public static string JsonPath { get; set; }
        public static string FileName { get; set; }
        public DataStorage()
        {

            /*if (Words.Count == 0 || Words == null)
            {
                InitializeAsync();
            }*/
            // CheckingJsonFromFile();
        }
        private async Task InitializeAsync()
        {
            // Заполнение словарей русско-английскими и англо-русскими словами
            /*Words.Add("яблоко", "apple");
            Words.Add("кот", "cat");
            Words.Add("собака", "dog");
            Words.Add("стол", "table");
            Words.Add("книга", "book");
            Words.Add("машина", "car");
            Words.Add("дом", "house");
            Words.Add("ручка", "pen");
            Words.Add("школа", "school");
            Words.Add("окно", "window");
            Words.Add("компьютер", "computer");
            Words.Add("стул", "chair");
            Words.Add("гитара", "guitar");
            Words.Add("телефон", "phone");
            Words.Add("молоко", "milk");
            Words.Add("класс", "class");
            Words.Add("книжка", "booklet");
            Words.Add("солнце", "sun");
            Words.Add("парк", "park");
            Words.Add("мяч", "ball");
            Words.Add("флаг", "flag");
            Words.Add("река", "river");
            Words.Add("капуста", "cabbage");
            Words.Add("банан", "banana");
            Words.Add("дождь", "rain");
            Words.Add("пианино", "piano");
            Words.Add("зонт", "umbrella");
            Words.Add("ноутбук", "laptop");
            Words.Add("море", "sea");
            Words.Add("музей", "museum");
            Words.Add("звезда", "star");
            Words.Add("птица", "bird");
            Words.Add("космос", "space");
            Words.Add("пароль", "password");
            Words.Add("луна", "moon");
            Words.Add("снег", "snow");
            Words.Add("футбол", "football");
            Words.Add("носок", "sock");
            Words.Add("зебра", "zebra");
            Words.Add("печенье", "cookie");
            Words.Add("гора", "mountain");
            Words.Add("планета", "planet");
            Words.Add("цирк", "circus");
            Words.Add("шапка", "hat");
            Words.Add("змея", "snake");
            Words.Add("шоколад", "chocolate");
            Words.Add("поезд", "train");
            Words.Add("песня", "song");
            Words.Add("платье", "dress");
            Words.Add("фильм", "movie");
            Words.Add("ящик", "box");
            Words.Add("замок", "castle");
            Words.Add("кто", "who");
            Words.Add("кора", "bark");

            // Заполнение англо-русского словаря на основе русско-английского
            foreach (var entry in Words)
            {
                TransletedWords.Add(entry.Value, entry.Key);
            }*/

            /*JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string russianJson = JsonSerializer.Serialize(Words, options);
            string englishJson = JsonSerializer.Serialize(TransletedWords, options);

            File.WriteAllText("Words.json", russianJson, Encoding.UTF8);
            File.WriteAllText("TransletedWords.json", englishJson, Encoding.UTF8);*/

        }
        public static bool CheckingJsonFromFile(string fileName = StandartFile)
        {

            string basePath = Application.StartupPath;
            string filePath = Path.Combine(basePath, fileName);
            FileName = fileName;
            JsonPath = filePath;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileName);
           

            try
            {
                string[] fileNames = fileNameWithoutExtension.Split('-');

                if (fileNames.Length != 2)
                {
                    FileName = StandartFile;
                    throw new Exception("Неверный формат названия файла словаря (Пример: Русский-Английский)");
                    return false;
                }
               
                string reverseFileName = $"{fileNames[1]}-{fileNames[0]}.json";
                string reverseFilePath = Path.Combine(basePath, reverseFileName);
                string json = File.ReadAllText(JsonPath, Encoding.UTF8);
                if (string.IsNullOrEmpty(json))
                {
                    ShowErrorMessage("Файл не содержит информации", "Были найдены ошибки в структуре JSON");
                    return false;
                }

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                // Проверка наличия пустых ключей
                if (!CheckEmptyKeys(json))
                {
                    return false;
                }

                Words = JsonSerializer.Deserialize<Dictionary<string, string>>(json, options);

                // Дальнейшие действия с полученным словарем

                CreateReverseTranslation(options, reverseFilePath);
            }
            catch (JsonException ex)
            {
                ShowErrorMessage(ex.Message, "Были найдены ошибки в структуре JSON");
                return false;
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message, "Возникла ошибка при чтении");
                return false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "Возникла непредвиденная ошибка");
                return false;
            }
            return true;
        }

        public static bool UpdateJsonFiles()
        {
            try
            {
                string basePath = Application.StartupPath;
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileName);
                string[] fileNames = fileNameWithoutExtension.Split('-');

                if (fileNames.Length != 2)
                {
                    throw new Exception("Неверный формат названия файла словаря (Пример: Русский-Английский)");
                }
                string reverseFileName = $"{fileNames[1]}-{fileNames[0]}.json";
                string reverseFilePath = Path.Combine(basePath, reverseFileName);
                string json = File.ReadAllText(JsonPath, Encoding.UTF8);
                if (string.IsNullOrEmpty(json))
                {
                    ShowErrorMessage("Файл не содержит информации", "Были найдены ошибки в структуре JSON");
                    return false;
                }

                if (!CheckEmptyKeysInDictionary())
                {
                    return false;
                }

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                UpdateReverseTranslation(options, FileName, reverseFilePath);

                string updatedJson = JsonSerializer.Serialize(Words, options);
                File.WriteAllText(JsonPath, updatedJson, Encoding.UTF8);
            }
            catch (JsonException ex)
            {
                ShowErrorMessage(ex.Message, "Были найдены ошибки в структуре JSON");
                return false;
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message, "Возникла ошибка при чтении");
                return false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message, "Возникла непредвиденная ошибка");
                return false;
            }
            return true;
        }

        private static bool CheckEmptyKeys(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            foreach (var property in jsonObject.Properties())
            {
                if (property.Name == "")
                {
                    throw new JsonException("Найден пустой ключ в JSON");
                }
            }
            return true;
        }

        private static void CreateReverseTranslation(JsonSerializerOptions options, string reverseFilePath)
        {
            TransletedWords = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in Words)
            {
                TransletedWords[pair.Value] = pair.Key;
            }

            string reverseJson = JsonSerializer.Serialize(TransletedWords, options);
            File.WriteAllText(reverseFilePath, reverseJson, Encoding.UTF8);
        }

        private static bool CheckEmptyKeysInDictionary()
        {
            foreach (var pair in Words)
            {
                if (string.IsNullOrEmpty(pair.Key))
                {
                    throw new JsonException("Найден пустой ключ в словаре");
                }
            }
            return true;
        }

        private static void UpdateReverseTranslation(JsonSerializerOptions options, string fileName, string reverseFilePath)
        {
            TransletedWords = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in Words)
            {
                TransletedWords[pair.Value] = pair.Key;
            }

            string reverseJson = JsonSerializer.Serialize(TransletedWords, options);
            File.WriteAllText(reverseFilePath, reverseJson, Encoding.UTF8);
        }

        private static void ShowErrorMessage(string message, string caption)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);
        }
        public static Task ChangeLanguage(bool stateOfLanguage)
        {
            if (!stateOfLanguage)
            {
                Dictionary<string, string> temp = new Dictionary<string, string>();
                temp = TransletedWords;
                TransletedWords = Words;
                Words = temp;
            }
            return Task.CompletedTask;
        }
        public static void AddDataToFileOfTheTranslateHistory(string query,OleDbConnection connection,string Word,string WordTranslation)
        {
            try
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", DateTime.Now.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@Time", DateTime.Now.ToString("HH:mm:ss"));
                    command.Parameters.AddWithValue("@Word", string.Join(" ", Word));
                    command.Parameters.AddWithValue("@Translation", string.Join(", ", WordTranslation));
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Файл с историей поврежден(\n{ex.Message})", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        public static void CheckOrCreateTranslationHistoryFile()
        {
            string basePath = Application.StartupPath;
            string filePath = Path.Combine(basePath, "translation_history.txt");
            if (!File.Exists(filePath))
            {
                // Если файл не существует, создаем его и заполняем заголовками
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.Default))
                {
                    writer.WriteLine("Date,Time,Word,Translation");
                }
                MessageBox.Show("Файл истории переводов создан.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
/*
            MessageBox.Show("Файл с историей поврежден", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;*/
        }

    }
}
