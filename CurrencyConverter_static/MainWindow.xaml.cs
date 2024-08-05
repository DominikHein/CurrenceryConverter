using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CurrencyConverter_static
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// SQL-Verbindungsobjekte
		SqlConnection con = new SqlConnection();
		SqlCommand cmd = new SqlCommand();
		SqlDataAdapter da = new SqlDataAdapter();

		// Variablen zur Speicherung von Währungsinformationen
		private int CurrencyID = 0;
		private double FromAmount = 0;
		private double ToAmount = 0;

		public MainWindow()
		{
			InitializeComponent();
			lblCurrency.Content = "Currency Converter";
			BindCurrency(); // Schreibt währungen in Dropdown
			GetData(); // Lädt die Währungsdaten in das DataGrid
		}

		// Methode zur Herstellung der SQL-Verbindung
		public void myconnection()
		{
			string connection = ConfigurationManager.ConnectionStrings["CurrencyConverterDB"].ConnectionString;
			con = new SqlConnection(connection);
			con.Open();
		}

		// Methode zum schreiben von Währungen in Dropdown
		private void BindCurrency()
		{
			myconnection();

			DataTable dt = new DataTable();

			cmd = new SqlCommand("Select Id, CurrencyName from Currency_Master", con);
			cmd.CommandType = CommandType.Text;
			da = new SqlDataAdapter(cmd);

			da.Fill(dt);

			// Fügt eine Standardoption Select Currency hinzu
			DataRow newRow = dt.NewRow();
			newRow["Id"] = 0;
			newRow["CurrencyName"] = "Select Currency";

			dt.Rows.InsertAt(newRow, 0);

			if (dt != null && dt.Rows.Count > 0)
			{
				cmbFromCurrency.ItemsSource = dt.DefaultView;
				cmbFromCurrency.DisplayMemberPath = "CurrencyName";
				cmbFromCurrency.SelectedValuePath = "Id";
				cmbFromCurrency.SelectedIndex = 0;

				cmbToCurrency.ItemsSource = dt.DefaultView;
				cmbToCurrency.DisplayMemberPath = "CurrencyName";
				cmbToCurrency.SelectedValuePath = "Id";
				cmbToCurrency.SelectedIndex = 0;
			}
			con.Close();
		}

		// Event Handler Convert Button
		private void Convert_Click(object sender, RoutedEventArgs e)
		{
			double ConvertedValue;

			// Validierung der Eingaben
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
				// Berechnung des umgerechneten Wertes
				ConvertedValue = double.Parse(txtCurrency.Text) / double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(cmbToCurrency.SelectedValue.ToString());
				lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
			}
		}

		// Input Felder leeren
		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			lblCurrency.Content = "";
			cmbFromCurrency.SelectedIndex = 0;
			cmbToCurrency.SelectedIndex = 0;
			txtCurrency.Text = "";
		}

		// Prüfen ob nur Zahlen eingegeben werden
		private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		// Event Handler Save Button
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Eingabe Prüfen
				if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
				{
					MessageBox.Show("Please enter amount");
					txtAmount.Focus();
					return;
				}
				else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
				{
					MessageBox.Show("Please select the currency name");
					txtCurrencyName.Focus();
					return;
				}
				else
				{
					if (CurrencyID > 0)
					{
						// Währung ändern
						myconnection();
						cmd = new SqlCommand("Update Currency_Master set CurrencyName = @CurrencyName, Amount = @Amount where Id = @CurrencyID", con);
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
						cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
						cmd.Parameters.AddWithValue("@CurrencyID", CurrencyID);
						cmd.ExecuteNonQuery();
						con.Close();
						MessageBox.Show("Currency updated successfully");
					}
					else
					{
						// Neue Währung hinzufügen
						myconnection();
						cmd = new SqlCommand("Insert into Currency_Master (CurrencyName, Amount) values (@CurrencyName, @Amount)", con);
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
						cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
						cmd.ExecuteNonQuery();
						con.Close();
						MessageBox.Show("Currency added successfully");
					}
				}

				clearMaster(); // Eingabefelder leeren
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		// Event Handler Cancel Button
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				clearMaster(); // Eingabefelder leeren
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// Methode zum leeren der Eingabefelder im Master Tab
		private void clearMaster()
		{
			txtCurrencyName.Text = "";
			txtAmount.Text = "";
			btnSave.Content = "Save";
			GetData(); // Lädt die Währungsdaten neu
			CurrencyID = 0;
			BindCurrency(); // Fügt Währungen in Dropdown hinzu
			txtAmount.Focus();
		}

		// Lädt Daten ins Grid View
		private void GetData()
		{
			myconnection();
			DataTable dt = new DataTable();
			cmd = new SqlCommand("Select Id, CurrencyName, Amount from Currency_Master", con);
			cmd.CommandType = CommandType.Text;
			da = new SqlDataAdapter(cmd);
			da.Fill(dt);
			if (dt != null && dt.Rows.Count > 0)
			{
				dgvCurrency.ItemsSource = dt.DefaultView;
			}
			else
			{
				dgvCurrency.ItemsSource = null;
			}
			con.Close();
		}

		// Event Handler für Änderungen der ausgewählten Zellen im DataGrid
		private void dgvCurrency_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
		{
			try
			{
				DataGrid dataGrid = (DataGrid)sender;
				DataRowView row = dataGrid.CurrentItem as DataRowView;

				if (row != null)
				{
					if (dgvCurrency.Items.Count > 0)
					{
						if (dataGrid.SelectedCells.Count > 0)
						{
							DataGridCellInfo cell = dataGrid.SelectedCells[0];
							if (cell.Column != null)
							{
								// Setzt die CurrencyID basierend auf der ausgewählten Zeile
								CurrencyID = Int32.Parse(row["Id"].ToString());
								if (dataGrid.SelectedCells[0].Column.DisplayIndex == 0)
								{
									// Zeigt die Werte in den Textfeldern an
									txtAmount.Text = row["Amount"].ToString();
									txtCurrencyName.Text = row["CurrencyName"].ToString();
									btnSave.Content = "Update";
								}
								if (dataGrid.SelectedCells[0].Column.DisplayIndex == 1)
								{
									// Bestätigt das Löschen der Währung
									if (MessageBox.Show("Are you sure you want to delete?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
									{
										myconnection();
										DataTable dt = new DataTable();
										cmd = new SqlCommand("Delete from Currency_Master where Id = @CurrencyID", con);
										cmd.CommandType = CommandType.Text;
										cmd.Parameters.AddWithValue("@CurrencyID", CurrencyID);
										cmd.ExecuteNonQuery();
										con.Close();

										MessageBox.Show("Currency deleted successfully");
										clearMaster(); // Leert Eingabefelder 
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}
