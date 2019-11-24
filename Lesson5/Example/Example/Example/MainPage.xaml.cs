using Example;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

// in out ref

// static void Addition(params int[] integers)
// Addition(1, 2, 3, 4, 5)

// Кортежи
/*
static void Main(string[] args)
{
    (int, int)  tuple = (5, 10);
    Console.WriteLine(tuple.Item1); // 5
    Console.WriteLine(tuple.Item2); // 10
    tuple.Item1 += 26;
    Console.WriteLine(tuple.Item1); // 31
    Console.Read();
}

private static (string name, int age) GetTuple((string n, int a) tuple, int x)
{
    // ...
    return result;
}
*/

/*
 * Перегрузка методов
class Counter
{
    public int Value { get; set; }
         
    public static Counter operator +(Counter c1, Counter c2)
    {
        return new Counter { Value = c1.Value + c2.Value };
    }
    public static bool operator >(Counter c1, Counter c2)
    {
        return c1.Value > c2.Value;
    }
    public static bool operator <(Counter c1, Counter c2)
    {
        return c1.Value < c2.Value;
    }
}
 */

/*
object x = null;
object y = x ?? 100;  // равно 100, так как x равен null
 
object z = 200;
object t = z ?? 44; // равно 200, так как z не равен null
*/

// Переопределение методов и свойств. Чтобы апретить переопределение свойств sealed

/* Делегаты
class Program
{
    delegate void Message(); // 1. Объявляем делегат
 
    static void Main(string[] args)
    {
        Message mes; // 2. Создаем переменную делегата
        if (DateTime.Now.Hour < 12)
        {
            mes = GoodMorning; // 3. Присваиваем этой переменной адрес метода
        }
        else
        {
            mes = GoodEvening;
        }
        mes(); // 4. Вызываем метод
        Console.ReadKey();
    }
    private static void GoodMorning()
    {
        Console.WriteLine("Good Morning");
    }
    private static void GoodEvening()
    {
        Console.WriteLine("Good Evening");
    }
}

class Program
{
    delegate void Message();
 
    static void Main(string[] args)
    {
        Message mes1 = Hello;
        mes1 += HowAreYou;  // теперь mes1 указывает на два метода
        mes1(); // вызываются оба метода - Hello и HowAreYou
        Console.Read();
    }
    private static void Hello()
    {
        Console.WriteLine("Hello");
    }
    private static void HowAreYou()
    {
        Console.WriteLine("How are you?");
    }
}

    Делегаты можно суммировать

 */

/*
public new void Move()
{
    Console.WriteLine("Move in HeroAction");
}

    Сокрытие
 */

/*
class Program
{
    static void Main(string[] args)
    {
        string s = "Привет мир";
        char c = 'и';
        int i = s.CharCount(c);
        Console.WriteLine(i);
 
        Console.Read();
    }
}
 
public static class StringExtension
{
    public static int CharCount(this string str, char c)
    {
        int counter = 0;
        for (int i = 0; i<str.Length; i++)
        {
            if (str[i] == c)
                counter++;
        }
        return counter;
    }
} 

    Метод расширения
 */

/*
Деконструкторы (не путать с деструкторами) позволяют выполнить декомпозицию объекта на отдельные части.

Например, пусть у нас есть следующий класс Person:


class Person
{
public string Name { get; set; }
public int Age { get; set; }

public void Deconstruct(out string name, out int age)
{
    name = this.Name;
    age = this.Age;
}
}
В этом случае мы могли бы выполнить декомпозицию объекта Person так:


Person person = new Person { Name = "Tom", Age = 33 };

(string name, int age) = person;

Console.WriteLine(name);    // Tom
Console.WriteLine(age);     // 33
 */

/*
class Reader
{
    Lazy<Library> library = new Lazy<Library>();
    public void ReadBook()
    {
        library.Value.GetBook();
        Console.WriteLine("Читаем бумажную книгу");
    }
 
    public void ReadEbook()
    {
        Console.WriteLine("Читаем книгу на компьютере");
    }
}
 */

namespace Example
{
    public interface IAlertClass
    {
        void ShowAlert();
    }
    
    public class AlertClass : IAlertClass
    {
        public void ShowAlert()
        {
            Console.WriteLine("Default value");
        }
    }


    public class HeaderView : View
    {
        public static readonly BindableProperty TextProperty =
               BindableProperty.Create("Text", typeof(string), typeof(HeaderView), string.Empty);
        public string Text
        {
            set
            {
                SetValue(TextProperty, value);
            }
            get
            {
                return (string)GetValue(TextProperty);
            }
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create("TextColor", typeof(Color), typeof(HeaderView), Color.Default);
        public Color TextColor
        {
            set
            {
                SetValue(TextColorProperty, value);
            }
            get
            {
                return (Color)GetValue(TextColorProperty);
            }
        }
        
        public event EventHandler LongPressEvent;
        public void HandleLongPress(object sender, EventArgs e)
        {
            LongPressEvent?.Invoke(this, e);
            var kkk = DependencyService.Get<IAlertClass>();
            kkk.ShowAlert();
        }
    }

    [DesignTimeVisible(false)]
    public partial class MainPage : CarouselPage
    {
        public class Data
        {
            public string CustomText { get; set; }
        }
        
        public ObservableCollection<Data> Datas { get; } = new ObservableCollection<Data>();

        public MainPage()
        {
            InitializeComponent();
            var text = CrossSettings.Current.GetValueOrDefault("name", string.Empty, "data");
            if (App.Current.Properties.TryGetValue("name", out var datas))
            {
                Datas = datas as ObservableCollection<Data>;
            }
            else if (text != string.Empty)
            {
                Datas.Add(new Data { CustomText = text });
            }
            listView.ItemsSource = Datas;
        }

        private void entry_Completed(object sender, EventArgs e)
        {
            Datas.Add(new Data { CustomText = (sender as Entry).Text });
            App.Current.Properties["name"] = Datas;
            (sender as Entry).Text = "";
            CrossSettings.Current.AddOrUpdateValue("name", (sender as Entry).Text, "data");
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            Datas.Clear();
        }

        private void HeaderView_LongPressEvent(object sender, EventArgs e)
        {
            var view = sender as HeaderView;
            if (view != null)
            {
                view.TextColor = Color.Red;
            }
        }
    }
}
