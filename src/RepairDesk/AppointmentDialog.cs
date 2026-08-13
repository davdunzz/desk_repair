using System.Windows;
using System.Windows.Controls;

namespace RepairDesk;

public sealed class AppointmentDialog : Window
{
    private readonly DatePicker _date = new();
    private readonly TextBox _time = new();
    public DateTime? Appointment { get; private set; }

    public AppointmentDialog(DateTime current)
    {
        Title = "Sposta appuntamento"; Width = 390; Height = 245; ResizeMode = ResizeMode.NoResize; WindowStartupLocation = WindowStartupLocation.CenterOwner;
        _date.SelectedDate = current.Date; _date.Margin = new Thickness(0, 5, 0, 12); _time.Text = current.ToString("HH:mm"); _time.Margin = new Thickness(0, 5, 0, 16);
        var save = new Button { Content = "SALVA NUOVO APPUNTAMENTO", Margin = new Thickness(4) }; save.Click += Save_Click;
        var remove = new Button { Content = "RIMUOVI APPUNTAMENTO", Background = System.Windows.Media.Brushes.IndianRed, Margin = new Thickness(4) }; remove.Click += (_, _) => { Appointment = null; DialogResult = true; };
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right }; buttons.Children.Add(remove); buttons.Children.Add(save);
        var panel = new StackPanel { Margin = new Thickness(22) };
        panel.Children.Add(new TextBlock { Text = "Nuova data" }); panel.Children.Add(_date); panel.Children.Add(new TextBlock { Text = "Nuova ora (es. 14:30)" }); panel.Children.Add(_time); panel.Children.Add(buttons); Content = panel;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_date.SelectedDate is null || !TimeSpan.TryParse(_time.Text.Trim(), out var time) || time < TimeSpan.Zero || time >= TimeSpan.FromDays(1))
        { MessageBox.Show("Inserisci una data e un orario validi, per esempio 14:30."); return; }
        Appointment = _date.SelectedDate.Value.Date.Add(time); DialogResult = true;
    }
}
