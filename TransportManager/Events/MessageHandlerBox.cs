using System;
using System.Windows.Forms;

namespace Events
{
    public class MessageHandlerBox
    {
        public void OnSendingEventShowNotification(object sender, SendEventArgs e)
        {
            //����������� ����� �������
            Console.WriteLine($@"{e.Operation} - {e.Status} - {e.ObjectId}");

            //����������� ����� ����� ����
            //MessageBox.Show($"{e.Operation} - {e.Status} - id {e.ObjectId}");
        }
    }
}