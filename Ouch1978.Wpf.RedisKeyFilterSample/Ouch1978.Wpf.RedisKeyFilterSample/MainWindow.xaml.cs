using StackExchange.Redis;

using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ouch1978.Wpf.RedisKeyFilterSample
{
    public partial class MainWindow : Window
    {
        private Stopwatch _stopwatch = new Stopwatch();

        private IServer _server;


        public MainWindow()
        {
            InitializeComponent();

            InitRedis();

        }

        private void InitRedis()
        {
            var endPoint = RedisConnectionFactory.GetConnection.GetEndPoints(true).FirstOrDefault();

            _server = RedisConnectionFactory.GetConnection.GetServer(endPoint);

        }

        private void btnQueryWithLinq_Click(object sender, RoutedEventArgs e)
        {
            _stopwatch.Restart();

            var keys = _server.Keys()
                .Where(x => Regex.IsMatch(x, "Test_By_Ouch_*"))
                .ToList();

            _stopwatch.Stop();

            lstMessages.Items.Add(new ListBoxItem { Content = $"{keys.Count()} key(s) match, {_stopwatch.Elapsed.TotalSeconds} spent.", Background = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString("#FFCC44FF") } });


        }

        private void btnQueryWithFilter_Click(object sender, RoutedEventArgs e)
        {
            int pageSize = int.Parse(((ComboBoxItem)cmbPageSize.SelectedItem).Tag.ToString());

            _stopwatch.Restart();

            var keys = _server.Keys(pattern: "Test_By_Ouch_*", pageSize: pageSize)
                .ToList();

            _stopwatch.Stop();

            lstMessages.Items.Add(new ListBoxItem { Content = $"{keys.Count()} key(s) match, {_stopwatch.Elapsed.TotalSeconds} spent, with pageSize {pageSize}.", Background = new SolidColorBrush { Color = (Color)ColorConverter.ConvertFromString("#FF33CC33") } });

        }

        private async void btnAddTestData_Click(object sender, RoutedEventArgs e)
        {
            int testDataCount = _server.Keys(pattern: "00Test_By_Ouch_*", pageSize: 1000).Count();

            for (int i = 1; i <= 50000; i++)
            {
                await RedisConnectionFactory.RedisDb.StringSetAsync(key: $"Test_By_Ouch_{testDataCount + i}", value: $"Test_By_Ouch_{testDataCount + i}");
            }

            lstMessages.Items.Add(new ListBoxItem { Content = "Add test data done." });

        }

        private async void btnRemoveTestData_Click(object sender, RoutedEventArgs e)
        {
            foreach (var key in _server.Keys(pattern: "Test_By_Ouch_*"))
            {
                await RedisConnectionFactory.RedisDb.KeyDeleteAsync(key);
            }

            lstMessages.Items.Add(new ListBoxItem { Content = "Remove test data done." });

        }
    }
}
