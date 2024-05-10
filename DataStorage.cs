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
        public static void AddDataToFileOfTheTranslateHistory(string query, OleDbConnection connection, string Word, string WordTranslation)
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
            catch (Exception ex)
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


        }
    }
}
