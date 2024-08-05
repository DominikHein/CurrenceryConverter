using System.Data;
using System.Text.RegularExpressions;
using System.Windows;

namespace CurrencyConverter_static
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			lblCurrency.Content = "Currency Converter";
			BindCurrency();
		}

		private void BindCurrency()
		{
			DataTable dtCurrency = new DataTable();
			dtCurrency.Columns.Add("Text");
			dtCurrency.Columns.Add("Value");

			dtCurrency.Rows.Add("--SELECT--", "0");
			dtCurrency.Rows.Add("USD", "1.00");
			dtCurrency.Rows.Add("EUR", "0.85");
			dtCurrency.Rows.Add("GBP", "0.75");
			dtCurrency.Rows.Add("JPY", "110.00");
			dtCurrency.Rows.Add("CHF", "0.92");
			dtCurrency.Rows.Add("CAD", "1.25");
			dtCurrency.Rows.Add("AUD", "1.35");
			dtCurrency.Rows.Add("NZD", "1.40");
			dtCurrency.Rows.Add("CNY", "6.45");
			dtCurrency.Rows.Add("SEK", "8.60");
			dtCurrency.Rows.Add("NOK", "8.90");
			dtCurrency.Rows.Add("DKK", "6.30");
			dtCurrency.Rows.Add("INR", "74.00");
			dtCurrency.Rows.Add("BRL", "5.20");
			dtCurrency.Rows.Add("ZAR", "14.50");
			dtCurrency.Rows.Add("RUB", "73.00");
			dtCurrency.Rows.Add("MXN", "20.00");
			dtCurrency.Rows.Add("SGD", "1.35");
			dtCurrency.Rows.Add("HKD", "7.80");
			dtCurrency.Rows.Add("KRW", "1150.00");

			cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
			cmbFromCurrency.DisplayMemberPath = "Text";
			cmbFromCurrency.SelectedValuePath = "Value";
			cmbFromCurrency.SelectedIndex = 0;

			cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
			cmbToCurrency.DisplayMemberPath = "Text";
			cmbToCurrency.SelectedValuePath = "Value";
			cmbToCurrency.SelectedIndex = 0;
		}

		private void txtCurrency_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{

		}

		private void Convert_Click(object sender, RoutedEventArgs e)
		{
			double ConvertedValue;



			if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
			{
				MessageBox.Show("Please enter the currency value to convert");
				txtCurrency.Focus();
				return;
			}

			else if (cmbFromCurrency.SelectedIndex == 0)
			{
				MessageBox.Show("Please select the currency to convert from");
				cmbFromCurrency.Focus();
				return;
			}

			else if (cmbToCurrency.SelectedIndex == 0 || cmbToCurrency.SelectedIndex == cmbFromCurrency.SelectedIndex)
			{
				MessageBox.Show("Please select the currency to convert to");
				cmbToCurrency.Focus();
				return;
			}

			else
			{
				ConvertedValue = double.Parse(txtCurrency.Text) / double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(cmbToCurrency.SelectedValue.ToString());
				lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
			}
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			lblCurrency.Content = "";
			cmbFromCurrency.SelectedIndex = 0;
			cmbToCurrency.SelectedIndex = 0;
			txtCurrency.Text = "";
		}

		private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}
