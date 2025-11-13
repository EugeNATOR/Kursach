using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.Generic;

namespace MeteoStation
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer dataTimer;
        private Random random = new Random();
        private bool isConnected = false;

        public ObservableCollection<WeatherData> WeatherHistory { get; set; }
        public ObservableCollection<Device> AvailableDevices { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Инициализация коллекций
            WeatherHistory = new ObservableCollection<WeatherData>();
            AvailableDevices = new ObservableCollection<Device>();

            DataGrid.ItemsSource = WeatherHistory;
            DeviceComboBox.ItemsSource = AvailableDevices;

            InitializeTimer();
            InitializeDevices();
            UpdateConnectionStatus();
            UpdateIndicators(0, 0); // Начальное состояние индикаторов
        }

        private void InitializeTimer()
        {
            dataTimer = new DispatcherTimer();
            dataTimer.Interval = TimeSpan.FromSeconds(2);
            dataTimer.Tick += DataTimer_Tick;
        }

        private void InitializeDevices()
        {
            AvailableDevices.Add(new Device { Id = 1, Name = "Датчик 1 (USB)" });
            AvailableDevices.Add(new Device { Id = 2, Name = "Датчик 2 (Bluetooth)" });
            AvailableDevices.Add(new Device { Id = 3, Name = "Датчик 3 (Wi-Fi)" });

            if (AvailableDevices.Count > 0)
                DeviceComboBox.SelectedIndex = 0;
        }

        private void DataTimer_Tick(object sender, EventArgs e)
        {
            if (isConnected)
            {
                var newData = GenerateRandomData();
                UpdateCurrentReadings(newData);
                UpdateIndicators(newData.Temperature, newData.Humidity);
            }
        }

        private WeatherData GenerateRandomData()
        {
            return new WeatherData
            {
                Time = DateTime.Now,
                Temperature = Math.Round(random.NextDouble() * 50 - 10, 1), // -10 до +40°C
                Humidity = random.Next(20, 95) // 20-95%
            };
        }

        private void UpdateCurrentReadings(WeatherData data)
        {
            TemperatureText.Text = $"{data.Temperature} °C";
            HumidityText.Text = $"{data.Humidity} %";
        }

        private void UpdateIndicators(double temperature, int humidity)
        {
            // Индикатор температуры
            if (temperature >= 18 && temperature <= 25)
                TempIndicator.Fill = new SolidColorBrush(Colors.Green);
            else if ((temperature >= 15 && temperature < 18) || (temperature > 25 && temperature <= 30))
                TempIndicator.Fill = new SolidColorBrush(Colors.Orange);
            else
                TempIndicator.Fill = new SolidColorBrush(Colors.Red);

            // Индикатор влажности
            if (humidity >= 40 && humidity <= 60)
                HumidityIndicator.Fill = new SolidColorBrush(Colors.Green);
            else if ((humidity >= 30 && humidity < 40) || (humidity > 60 && humidity <= 70))
                HumidityIndicator.Fill = new SolidColorBrush(Colors.Orange);
            else
                HumidityIndicator.Fill = new SolidColorBrush(Colors.Red);

        }

        private void UpdateConnectionStatus()
        {
            if (isConnected)
            {
                StatusText.Text = "Подключено";
                StatusText.Foreground = new SolidColorBrush(Colors.Green);
                StatusText.FontStyle = FontStyles.Normal;
            }
            else
            {
                StatusText.Text = "Не подключено";
                StatusText.Foreground = new SolidColorBrush(Colors.Gray);
                StatusText.FontStyle = FontStyles.Italic;
            }
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите устройство для подключения", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            isConnected = true;
            dataTimer.Start();
            UpdateConnectionStatus();

            // Генерируем первое значение сразу после подключения
            var initialData = GenerateRandomData();
            UpdateCurrentReadings(initialData);
            UpdateIndicators(initialData.Temperature, initialData.Humidity);
        }

        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            isConnected = false;
            dataTimer.Stop();
            UpdateConnectionStatus();

            // Сбрасываем индикаторы при отключении
            TempIndicator.Fill = new SolidColorBrush(Colors.Gray);
            HumidityIndicator.Fill = new SolidColorBrush(Colors.Gray);

        }

        private void ReadDataBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                var newRecord = new WeatherData
                {
                    Time = DateTime.Now,
                    Temperature = double.Parse(TemperatureText.Text.Replace(" °C", "")),
                    Humidity = int.Parse(HumidityText.Text.Replace(" %", ""))
                };
                WeatherHistory.Insert(0, newRecord); // Добавляем в начало
            }
            else
            {
                MessageBox.Show("Сначала подключите устройство", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция экспорта в CSV будет реализована в следующей версии",
                          "Экспорт CSV", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WeatherHistory.Count > 0)
            {
                var result = MessageBox.Show("Очистить всю таблицу данных?", "Подтверждение",
                                           MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    WeatherHistory.Clear();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BackgroundGrid.Background = new SolidColorBrush(Color.FromRgb(22, 50, 58));
            ConnectDevice.Background = new SolidColorBrush(Color.FromRgb(22, 50, 58));
            Measure.Background = new SolidColorBrush(Color.FromRgb(22, 50, 58));
            BackgroundTable.Background = new SolidColorBrush(Color.FromRgb(22, 50, 58));
            TableName.Background = new SolidColorBrush(Color.FromRgb(22, 50, 58));
            Indicators.Background = new SolidColorBrush(Color.FromRgb(22, 50, 58));
            ConnectText.Foreground = new SolidColorBrush(Colors.White);
            TemperatureText.Foreground = new SolidColorBrush(Colors.White);
            HumidityText.Foreground = new SolidColorBrush(Colors.White);
            TableName.Foreground = new SolidColorBrush(Colors.White);
            Temp.Foreground = new SolidColorBrush(Colors.White);
            Hum.Foreground = new SolidColorBrush(Colors.White);
            Temperature.Foreground = new SolidColorBrush(Colors.White);
            Humidity.Foreground = new SolidColorBrush(Colors.White);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BackgroundGrid.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            ConnectDevice.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            Measure.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            BackgroundTable.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            TableName.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            Indicators.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            ConnectText.Foreground = new SolidColorBrush(Colors.Black);
            TemperatureText.Foreground = new SolidColorBrush(Colors.Black);
            HumidityText.Foreground = new SolidColorBrush(Colors.Black);
            TableName.Foreground = new SolidColorBrush(Colors.Black);
            Temp.Foreground = new SolidColorBrush(Colors.Black);
            Hum.Foreground = new SolidColorBrush(Colors.Black);
            Temperature.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102));
            Humidity.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102));
        }
    }

    public class WeatherData : INotifyPropertyChanged
    {
        private DateTime time;
        private double temperature;
        private int humidity;

        public DateTime Time
        {
            get => time;
            set { time = value; OnPropertyChanged(nameof(Time)); }
        }

        public double Temperature
        {
            get => temperature;
            set { temperature = value; OnPropertyChanged(nameof(Temperature)); }
        }

        public int Humidity
        {
            get => humidity;
            set { humidity = value; OnPropertyChanged(nameof(Humidity)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}