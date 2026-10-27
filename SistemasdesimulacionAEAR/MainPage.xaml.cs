using System.Globalization;

namespace SistemasdesimulacionAEAR;

public partial class MainPage : ContentPage
{
    private static readonly CultureInfo Spanish = CultureInfo.GetCultureInfo("es-EC");

    public MainPage()
    {
        InitializeComponent();
        StudentPicker.ItemsSource = new[]
        {
            "Ana Belén Cárdenas", "Bruno Esteban León", "Camila Fernanda Ruiz",
            "Diego Andrés Paredes", "Valentina Sofía Torres"
        };
        StudentPicker.SelectedIndex = 0;
    }

    private async void OnCalculateClicked(object? sender, EventArgs e)
    {
        if (StudentPicker.SelectedItem is null)
        {
            await DisplayAlertAsync("Falta el estudiante", "Selecciona un estudiante para continuar.", "Entendido");
            return;
        }

        var entries = new[] { Follow1Entry, Exam1Entry, Follow2Entry, Exam2Entry };
        var labels = new[] { "Seguimiento 1", "Examen 1", "Seguimiento 2", "Examen 2" };
        var values = new decimal[entries.Length];

        for (var i = 0; i < entries.Length; i++)
        {
            if (!TryParseGrade(entries[i].Text, out values[i]))
            {
                await DisplayAlertAsync("Nota no válida", $"{labels[i]} debe ser un número entre 0 y 10.", "Corregir");
                entries[i].Focus();
                return;
            }
        }

        var partial1 = values[0] * 0.3m + values[1] * 0.2m;
        var partial2 = values[2] * 0.3m + values[3] * 0.2m;
        var final = partial1 + partial2;
        var status = final >= 7m ? "Aprobado" : final >= 5m ? "Complementario" : "Reprobado";

        Partial1Label.Text = $"{partial1.ToString("0.00", Spanish)} / 5,0";
        Partial2Label.Text = $"{partial2.ToString("0.00", Spanish)} / 5,0";
        FinalLabel.Text = $"{final.ToString("0.00", Spanish)} / 10";

        var message = $"Nombre: {StudentPicker.SelectedItem}\n" +
                      $"Fecha: {EvaluationDatePicker.Date:dd/MM/yyyy}\n" +
                      $"Nota Parcial 1: {partial1.ToString("0.00", Spanish)} / 5,0\n" +
                      $"Nota Parcial 2: {partial2.ToString("0.00", Spanish)} / 5,0\n" +
                      $"Nota Final: {final.ToString("0.00", Spanish)} / 10\n" +
                      $"Estado: {status}";

        await DisplayAlertAsync("Resultado del estudiante", message, "Cerrar");
    }

    private static bool TryParseGrade(string? input, out decimal grade)
    {
        grade = 0m;
        if (string.IsNullOrWhiteSpace(input)) return false;
        var normalized = input.Trim().Replace(',', '.');
        return decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out grade)
               && grade >= 0m && grade <= 10m;
    }
}
