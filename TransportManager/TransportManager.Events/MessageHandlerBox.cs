using System;
using System.Windows.Forms;

namespace TransportManager.Events
{
    public class MessageHandlerBox
    {
        public void OnSendingEventShowNotification(object sender, SendEventArgs e)
        {
            //уведомления через консоль
            Console.WriteLine($@"{e.Operation} - {e.Status} - {e.ObjectId}");

            //уведомления через новое окно
            //MessageBox.Show($"{e.Operation} - {e.Status} - id {e.ObjectId}");
        }
    }
}