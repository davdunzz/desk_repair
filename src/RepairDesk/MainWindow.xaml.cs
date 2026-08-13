using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RepairDesk;

public partial class MainWindow : Window
{
    private RepairRecord? _editingRepair;

    public MainWindow()
    {
        InitializeComponent();
        ReloadCatalog();
        LoadSettings();
        LoadStorageSettings();
        RefreshArchive();
        AppointmentsCalendar.SelectedDate = DateTime.Today;
        RefreshCalendar();
    }

    private void ReloadCatalog()
    {
        var brands = Database.GetBrands();
        var current = BrandBox.Text;
        BrandBox.ItemsSource = brands;
        CatalogBrandBox.ItemsSource = brands;
        BrandBox.Text = current;
    }

    private void BrandBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (BrandBox.SelectedItem is string brand) LoadModels(brand);
    }

    private void BrandBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) => LoadModels(BrandBox.Text);

    private void LoadModels(string brand)
    {
        var current = ModelBox.Text;
        ModelBox.ItemsSource = Database.GetModels(brand.Trim());
        ModelBox.Text = current;
    }

    private void SaveAndGenerate_Click(object sender, RoutedEventArgs e)
    {
        SaveForm(true);
    }

    private void SaveOnly_Click(object sender, RoutedEventArgs e) => SaveForm(false);

    private void SaveForm(bool generatePdf)
    {
        if (!ValidateForm() || !TryGetAppointment(out var appointment)) return;
        try
        {
            Database.AddModel(BrandBox.Text, ModelBox.Text);
            var repair = BuildRecord(appointment);
            var wasEditing = _editingRepair is not null;
            if (wasEditing) Database.UpdateRepair(repair); else repair.Id = Database.SaveRepair(repair);
            string? path = generatePdf ? PdfService.Generate(repair, Database.LoadSettings()) : null;
            RefreshArchive();
            RefreshCalendar();
            ReloadCatalog();
            var message = wasEditing ? "Riparazione modificata e salvata." : "Riparazione salvata nell'archivio.";
            if (path is not null) message += $"\n\nPDF creato in:\n{path}";
            MessageBox.Show(message, "Operazione completata", MessageBoxButton.OK, MessageBoxImage.Information);
            if (path is not null) Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            ClearForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Non è stato possibile salvare la scheda.\n\n{ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateForm()
    {
        if (!string.IsNullOrWhiteSpace(FirstNameBox.Text) && !string.IsNullOrWhiteSpace(LastNameBox.Text) &&
            !string.IsNullOrWhiteSpace(PhoneBox.Text) && !string.IsNullOrWhiteSpace(BrandBox.Text) &&
            !string.IsNullOrWhiteSpace(ModelBox.Text) && !string.IsNullOrWhiteSpace(RepairDescriptionBox.Text)) return true;
        MessageBox.Show("Compila tutti i campi contrassegnati con *.", "Dati mancanti", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private bool TryGetAppointment(out DateTime? appointment)
    {
        appointment = null;
        if (AppointmentDatePicker.SelectedDate is null)
        {
            if (!string.IsNullOrWhiteSpace(AppointmentTimeBox.Text)) { MessageBox.Show("Se inserisci l'ora devi scegliere anche la data."); return false; }
            return true;
        }
        if (!TimeSpan.TryParse(AppointmentTimeBox.Text.Trim(), out var time) || time < TimeSpan.Zero || time >= TimeSpan.FromDays(1))
        { MessageBox.Show("Inserisci un orario valido, per esempio 14:30.", "Orario non valido", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
        appointment = AppointmentDatePicker.SelectedDate.Value.Date.Add(time);
        return true;
    }

    private RepairRecord BuildRecord(DateTime? appointment) => new()
    {
        Id = _editingRepair?.Id ?? 0, PracticeNumber = _editingRepair?.PracticeNumber ?? Database.NextPracticeNumber(), CreatedAt = _editingRepair?.CreatedAt ?? DateTime.Now,
        AppointmentAt = appointment, FirstName = FirstNameBox.Text.Trim(), LastName = LastNameBox.Text.Trim(), Phone = PhoneBox.Text.Trim(), Email = EmailBox.Text.Trim(),
        Brand = BrandBox.Text.Trim(), Model = ModelBox.Text.Trim(), Color = ColorBox.Text.Trim(), Imei = ImeiBox.Text.Trim(), RepairDescription = RepairDescriptionBox.Text.Trim(),
        RepairTypes = Checked(RepairTypesPanel), Accessories = Checked(AccessoriesPanel), DeviceConditions = Checked(ConditionsPanel), ConditionNotes = ConditionNotesBox.Text.Trim()
    };

    private static List<string> Checked(Panel panel) => panel.Children.OfType<CheckBox>().Where(x => x.IsChecked == true).Select(x => x.Content?.ToString() ?? "").Where(x => x.Length > 0).ToList();

    private void ClearForm_Click(object sender, RoutedEventArgs e) => ClearForm();

    private void ClearForm()
    {
        foreach (var box in new[] { FirstNameBox, LastNameBox, PhoneBox, EmailBox, ColorBox, ImeiBox, RepairDescriptionBox, ConditionNotesBox }) box.Clear();
        BrandBox.Text = ""; ModelBox.Text = "";
        AppointmentDatePicker.SelectedDate = null; AppointmentTimeBox.Clear();
        foreach (var panel in new[] { RepairTypesPanel, AccessoriesPanel, ConditionsPanel }) foreach (var check in panel.Children.OfType<CheckBox>()) check.IsChecked = false;
        _editingRepair = null; SaveOnlyButton.Content = "SALVA NELL'ARCHIVIO";
        FirstNameBox.Focus();
    }

    private void ClearAppointment_Click(object sender, RoutedEventArgs e) { AppointmentDatePicker.SelectedDate = null; AppointmentTimeBox.Clear(); }

    private void RefreshArchive() => ArchiveGrid.ItemsSource = Database.SearchRepairs(SearchBox?.Text ?? "");
    private void RefreshArchive_Click(object sender, RoutedEventArgs e) => RefreshArchive();
    private void Search_Click(object sender, RoutedEventArgs e) => RefreshArchive();
    private void SearchBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) RefreshArchive(); }

    private void RegeneratePdf_Click(object sender, RoutedEventArgs e)
    {
        if (ArchiveGrid.SelectedItem is not RepairRecord repair) { MessageBox.Show("Seleziona prima una riparazione dall'archivio."); return; }
        try
        {
            var path = PdfService.Generate(repair, Database.LoadSettings());
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Errore PDF", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private void GenerateAppointmentList_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var appointments = Database.GetAllAppointments();
            if (appointments.Count == 0) { MessageBox.Show("Nell'archivio non ci sono appuntamenti con data e ora."); return; }
            var path = PdfService.GenerateAppointmentList(appointments, Database.LoadSettings());
            MessageBox.Show($"Lista di {appointments.Count} appuntamenti creata in:\n{path}", "PDF creato", MessageBoxButton.OK, MessageBoxImage.Information);
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Errore PDF", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private void EditRepair_Click(object sender, RoutedEventArgs e)
    {
        if (ArchiveGrid.SelectedItem is not RepairRecord repair) { MessageBox.Show("Seleziona prima una riparazione."); return; }
        LoadRepairIntoForm(repair);
    }

    private void LoadRepairIntoForm(RepairRecord repair)
    {
        _editingRepair = repair;
        FirstNameBox.Text = repair.FirstName; LastNameBox.Text = repair.LastName; PhoneBox.Text = repair.Phone; EmailBox.Text = repair.Email;
        BrandBox.Text = repair.Brand; LoadModels(repair.Brand); ModelBox.Text = repair.Model; ColorBox.Text = repair.Color; ImeiBox.Text = repair.Imei;
        RepairDescriptionBox.Text = repair.RepairDescription; ConditionNotesBox.Text = repair.ConditionNotes;
        SetChecks(RepairTypesPanel, repair.RepairTypes); SetChecks(AccessoriesPanel, repair.Accessories); SetChecks(ConditionsPanel, repair.DeviceConditions);
        AppointmentDatePicker.SelectedDate = repair.AppointmentAt?.Date; AppointmentTimeBox.Text = repair.AppointmentAt?.ToString("HH:mm") ?? "";
        SaveOnlyButton.Content = "SALVA MODIFICHE"; MainTabs.SelectedIndex = 0; FirstNameBox.Focus();
    }

    private static void SetChecks(Panel panel, List<string> values)
    { foreach (var check in panel.Children.OfType<CheckBox>()) check.IsChecked = values.Contains(check.Content?.ToString() ?? ""); }

    private void DeleteRepair_Click(object sender, RoutedEventArgs e)
    {
        if (ArchiveGrid.SelectedItem is not RepairRecord repair) { MessageBox.Show("Seleziona prima una riparazione."); return; }
        if (MessageBox.Show($"Eliminare definitivamente la pratica {repair.PracticeNumber} di {repair.DisplayName}?", "Conferma eliminazione", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        Database.DeleteRepair(repair.Id); RefreshArchive(); RefreshCalendar();
    }

    private void AppointmentsCalendar_SelectedDatesChanged(object? sender, SelectionChangedEventArgs e) => RefreshCalendar();

    private void RefreshCalendar()
    {
        if (AppointmentsCalendar is null || AppointmentsList is null) return;
        var day = AppointmentsCalendar.SelectedDate ?? DateTime.Today;
        SelectedDayTitle.Text = day.ToString("dddd d MMMM yyyy", new System.Globalization.CultureInfo("it-IT"));
        AppointmentsList.ItemsSource = Database.GetAppointments(day);
    }

    private void AppointmentsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (AppointmentsList.SelectedItem is not RepairRecord repair) return;
        var dialog = new AppointmentDialog(repair.AppointmentAt ?? DateTime.Now) { Owner = this };
        if (dialog.ShowDialog() != true) return;
        Database.UpdateAppointment(repair.Id, dialog.Appointment); RefreshArchive(); RefreshCalendar();
    }

    private void AddBrand_Click(object sender, RoutedEventArgs e)
    {
        Database.AddBrand(NewBrandBox.Text); NewBrandBox.Clear(); ReloadCatalog(); MessageBox.Show("Marca aggiunta.");
    }

    private void AddModel_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CatalogBrandBox.Text) || string.IsNullOrWhiteSpace(NewModelBox.Text)) { MessageBox.Show("Indica marca e modello."); return; }
        Database.AddModel(CatalogBrandBox.Text, NewModelBox.Text); NewModelBox.Clear(); ReloadCatalog(); MessageBox.Show("Modello aggiunto.");
    }

    private void LoadSettings()
    {
        var s = Database.LoadSettings();
        ShopNameBox.Text = s.ShopName; ShopAddressBox.Text = s.Address; ShopPhoneBox.Text = s.Phone; ShopEmailBox.Text = s.Email; ShopVatBox.Text = s.VatNumber;
    }

    private void LoadStorageSettings()
    {
        var options = StorageConfig.Load();
        StorageModeBox.SelectedIndex = options.Mode.Equals("Portable", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        PdfFolderBox.Text = options.CustomPdfFolder;
        UpdateStoragePreview();
    }

    private StorageOptions ReadStorageOptions() => new()
    {
        Mode = StorageModeBox.SelectedItem is ComboBoxItem item && item.Tag?.ToString() == "Portable" ? "Portable" : "PC",
        CustomPdfFolder = PdfFolderBox.Text.Trim()
    };

    private void UpdateStoragePreview()
    {
        if (StorageModeBox is null || StoragePreviewText is null) return;
        var options = ReadStorageOptions();
        StoragePreviewText.Text = $"Archivio: {Path.Combine(StorageConfig.GetDataFolder(options), "repairdesk.db")}\nPDF: {StorageConfig.GetPdfFolder(options)}";
    }

    private void StorageModeBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateStoragePreview();

    private void ChoosePdfFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog { Title = "Scegli dove salvare i PDF di RepairDesk", Multiselect = false };
        if (dialog.ShowDialog(this) != true) return;
        PdfFolderBox.Text = dialog.FolderName; UpdateStoragePreview();
    }

    private void DefaultPdfFolder_Click(object sender, RoutedEventArgs e) { PdfFolderBox.Clear(); UpdateStoragePreview(); }

    private void SaveSettings_Click(object sender, RoutedEventArgs e)
    {
        Database.SaveSettings(new ShopSettings { ShopName = ShopNameBox.Text.Trim(), Address = ShopAddressBox.Text.Trim(), Phone = ShopPhoneBox.Text.Trim(), Email = ShopEmailBox.Text.Trim(), VatNumber = ShopVatBox.Text.Trim() });
        try
        {
            Database.SwitchStorage(ReadStorageOptions());
            RefreshArchive(); RefreshCalendar(); UpdateStoragePreview();
            MessageBox.Show("Impostazioni salvate. L'archivio è stato copiato nella posizione scelta.", "Operazione completata", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex) { MessageBox.Show($"Non è stato possibile cambiare la posizione dei dati.\n\n{ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
}
