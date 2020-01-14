using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClientHw2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Socket socket;
        private int port = 8005; // порт сервера
        private string address = "127.0.0.1";

        public MainWindow()
        {
            InitializeComponent();
            //socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //var endPoint = new IPEndPoint(IPAddress.Parse(address), port);

            //socket.Connect(endPoint);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            //var mes = new Message
            //{
            //    User = userTB.Text,
            //    Text = textTB.Text
            //};

            //var jsonMes = JsonConvert.SerializeObject(mes);

            //var buffer = Encoding.UTF8.GetBytes(jsonMes);
            //ArraySegment<byte> data = new ArraySegment<byte>(buffer);

            //await socket.SendAsync(data, SocketFlags.None);

            //await GetMessages();

            //textTB.Text = string.Empty;

            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                var message = new Message
                {
                    Text = textTB.Text,
                    User = userTB.Text
                };
                var json = JsonConvert.SerializeObject(message);
                byte[] data = Encoding.Unicode.GetBytes(json);
                socket.Send(data);

                // получаем ответ
                data = new byte[1024]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт

                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                var messagesJson = builder.ToString();
                var messagesList = JsonConvert.DeserializeObject<List<Message>>(messagesJson);

                foreach (var mes in messagesList)
                {
                    messageHistory.Content += mes.ToString();
                }

                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                textTB.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task GetMessages()
        {
            messageHistory.Content = string.Empty;
            var stringBuilder = new StringBuilder();
            while (socket.Available > 0)
            {
                var buffer = new byte[1024];
                ArraySegment<byte> data = new ArraySegment<byte>(buffer);
                await socket.ReceiveAsync(data, SocketFlags.None);
                stringBuilder.Append(Encoding.UTF8.GetString(buffer));
            }

            if (stringBuilder.Length < 1)
            {
                return;
            }
            var messages = JsonConvert.DeserializeObject<List<Message>>(stringBuilder.ToString());

            foreach (var mes in messages)
            {
                messageHistory.Content += mes.ToString();
            }

        }
    }
}
