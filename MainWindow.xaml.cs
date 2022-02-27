using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TestTask1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<int> id_strings = new List<int>(); // List with identifiers of correct user's requested strings
        public List<string> error_ids = new List<string>(); // List with uncorrects identifiers
        public MainWindow()
        {
            InitializeComponent();
            InsideGRID.VerticalAlignment = VerticalAlignment.Top; /// alignment grid inside WPF-form
        }
        private void Up_Click(object sender, RoutedEventArgs e) // scrolling of scrollview event handler
        {
            scroll.LineUp(); // scroll Scrollview up
        }

        private void Down_Click(object sender, RoutedEventArgs e)// scrolling of scrollview event handler
        {
            scroll.LineDown(); // scroll Scrollview down
        }
        public string jsonconvert(string line, int i) // method of convertating a JSON-structure string and adding a text to GRID
        {
            var obj = JObject.Parse(line); // parsing a default string to jsonobject type value
            TextBlock text = new TextBlock(); // initializing a new text field in grid
            text.TextWrapping = TextWrapping.Wrap; // ability to wrapping a text in grid
            text.Text = obj.SelectToken("text").ToString() + "\n"; // getting text field from json-structure string and set it to new text field in grid
            InsideGRID.Children.Add(text); // adding to GRID
            Grid.SetRow(text, i + 1); // adding to GRID
            Grid.SetColumn(text, 0); // adding to GRID
            return text.Text;
        }
        public void wordcounts(string line, int i) // method of counting words in string
        {  
            string[] words = line.Split(' '); // splitting string by space
            TextBlock text = new TextBlock(); // initializing a new text field in grid
            text.Text = words.Length.ToString(); // set a count of words in new text field in grid
            text.HorizontalAlignment = HorizontalAlignment.Center; // alignment text field inside grid
            text.VerticalAlignment = VerticalAlignment.Center; // alignment text field inside grid
            InsideGRID.Children.Add(text); // adding to GRID
            Grid.SetRow(text, i + 1); // adding to GRID
            Grid.SetColumn(text, 1); // adding to GRID
        }
        public void vowelcounts(string line, int i) // method of counting vowels-letters in string
        {
            char[] ch = { 'а', 'у', 'о', 'ы', 'и', 'э', 'я', 'ю', 'е', 'ё', 'a', 'e', 'o', 'u', 'i', 'y' }; // all cyrillic and latin alphabet vowel letters
            int count = 0; // count of vowels letters
            string check = line.ToLower(); // make a lower register for all of symbols to correct counting
            for (int j = 0; j < check.Length; j++)
            {
                for (int k = 0; k < ch.Length; k++)
                    if (check[j] == ch[k]) count++; // if string contains vowel letter, add 1 to "count" variable
            }
            TextBlock text = new TextBlock(); // initializing a new text field in grid
            text.Text = count.ToString(); // set a count of vowel letters in new text field in grid
            text.HorizontalAlignment = HorizontalAlignment.Center; // alignment text field inside grid
            text.VerticalAlignment = VerticalAlignment.Center; // alignment text field inside grid
            InsideGRID.Children.Add(text); // adding to GRID
            Grid.SetRow(text, i + 1); // adding to GRID
            Grid.SetColumn(text, 2); // adding to GRID
        }
        private void Button_Click(object sender, RoutedEventArgs e) // Clicking of button event handler
        { 
            string[] temp = textbox1.Text.TrimEnd(new char[] {',', ' ', ';'}).Split(new char[] { ',', ';'}); // getting identifiers from text field in WPF-form
            int count_strings = temp.Length; // count of identifiers equals count of elements in identifiers array
            id_strings.Clear(); // clear a List with identifiers 
            error_ids.Clear(); // clear a List with errors 
            foreach (string temps in temp)
            {
                temps.Trim(new char[] { ' ', ',', ';' }); // deleting exsess separating symbols from identifiers in string type to correct converting to int type
                int tp; // now-checkable identifier of chosen strings
                if (int.TryParse(temps, out tp) && temps != "") // if converting to int type passes correct and identifier not empty
                {
                    if (!id_strings.Contains(tp) && tp <= 20) id_strings.Add(tp); // if now-checkable identifier wasn't added to id-Lust early and number of identifier not exceed 20, then add id to List
                    else if (tp > 20) error_ids.Add(tp.ToString()); // id int-id exceed 20, add him to error List
                }
                else error_ids.Add(temps); // if converting to int type doesn't passes correctly, add id to error List
            } 
            if (error_ids.Count > 0) // if there are error id's in List
            {
                string errors = "";
                for (int i = 0; i < error_ids.Count - 1; i++) //
                {                                             //
                    errors += error_ids[i] + ", ";            // making a string with error id's
                }                                             //
                errors += error_ids[error_ids.Count - 1];     //
                MessageBox.Show($"Идентификаторы: {errors} некорректны. Проверьте ввод. Идентификаторы должны быть натуральными и не превышать 20."); // inputing an error id's
                // To be honest, I did not understand the task "highlight values". 
                // If it meant that the identifier would turn red in the original line, I would partly assemble a string of identifiers,
                // and when entering an incorrect identifier, I would change its color to red, and output it to the textbox field text. In the meantime, I decided to leave it like this.
            }
            for (int i = 0; i < id_strings.Count; i++)
            {
                InsideGRID.RowDefinitions.Add(new RowDefinition()); // adding a new row in GRID
            }
            for (int i = 0; i < id_strings.Count; i++)
            {
                try // trying to send a request and get a response from server
                {
                    WebRequest request = WebRequest.Create("https://tmgwebtest.azurewebsites.net/api/textstrings/" + id_strings[i].ToString()); // creating request
                    request.Headers["TMG-Api-Key"] = "0J/RgNC40LLQtdGC0LjQutC4IQ=="; // set header to connecting server
                    WebResponse response = request.GetResponse(); // getting response from server
                    using (Stream stream = response.GetResponseStream())  
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line = "";
                            while ((line = reader.ReadLine()) != null) // reading a string from server if it isn't null
                            {
                                line = jsonconvert(line, i); //convertating a JSON - structure string and adding a text to GRID
                                wordcounts(line, i); // counting words in string
                                vowelcounts(line, i); // counting vowels-letters in string
                            }
                        }
                    }
                    response.Close(); // closing response stream
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + $"\n К тексту с идентификатором {id_strings[i]} подключиться не удалось."); // showing a text of Excepion with connecting to server
                }
            }
        }
    }
}
