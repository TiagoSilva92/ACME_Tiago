using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Teste.DAO;

namespace Teste
{
    public partial class Form1 : Form
    {
        #region Variáveis
        string strConn = ConfigurationManager.AppSettings["StringConnexao"].ToString();
        string folder = ConfigurationManager.AppSettings["CaminhoArquivo"].ToString();
        BancoDados bancodados = new BancoDados();

        DataTable dt = new DataTable();
        SQLiteConnection conn = null;
        #endregion

        #region Inicialização

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!VerificarExistenciaTabela())
                Application.Exit();

            CarregarDadosListBoxView();
        }

        #endregion

        #region Métodos

        private bool VerificarExistenciaTabela()
        {
            try
            {
                string arquivo = Path.Combine(folder, "acme.sqlite");

                if (!File.Exists(arquivo))
                {
                    string createTable = "CREATE TABLE TB_VOO (ID_VOO INTEGER         PRIMARY KEY AUTOINCREMENT, DATA_VOO DATETIME, CUSTO     NUMERIC(10, 2), DISTANCIA INT, CAPTURA   CHAR(1),  NIVEL_DOR INT);";

                    using (StreamWriter sw = File.CreateText(arquivo))
                    {
                        sw.WriteLine(createTable);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro:" + ex.Message);
                return false;
            }
        }

        private void CarregarDadosListBoxView()
        {
            try
            {
                lstvVoos.Clear();
                dt.Clear();
                lstvVoos.Refresh();

                string sql = "SELECT ID_VOO, DATA_VOO, CAPTURA, NIVEL_DOR FROM TB_VOO;";

                conn = new SQLiteConnection(strConn);
                SQLiteDataAdapter da = new SQLiteDataAdapter(sql, strConn);
                da.Fill(dt);

                lstvVoos.View = View.Details;

                lstvVoos.Columns.Add("ID");
                lstvVoos.Columns.Add("Data");
                lstvVoos.Columns.Add("Captura");
                lstvVoos.Columns.Add("Nivel de Dor");

                lstvVoos.Columns[0].Width = 0;

                foreach (DataRow linha in dt.Rows)
                {
                    ListViewItem item = new ListViewItem(linha[0].ToString());

                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        item.SubItems.Add(linha[i].ToString());
                    }

                    lstvVoos.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro :" + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void CarregarParametrosLinhaSelecionada(string id)
        {
            string sql = $"SELECT * FROM TB_VOO where ID_VOO = {id};";
            try
            {
                conn = new SQLiteConnection(strConn);
                SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn);
                da.Fill(dt);

                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    txtCusto.Text = dr.ItemArray[4].ToString();
                    txtDistancia.Text = dr.ItemArray[5].ToString();
                    txtNivelDor.Text = dr.ItemArray[3].ToString();

                    string valorCaptura = dr.ItemArray[2].ToString();

                    if (valorCaptura == "S")
                        rbSim.Checked = true;
                    else
                        rbNao.Checked = true;

                    string dataRecebida = Convert.ToDateTime(dr.ItemArray[1].ToString()).ToString("dd/MM/yyyy");

                    if (dataRecebida != null)
                        dtpData.Value = DateTime.ParseExact(dataRecebida, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro :" + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void AtivarComponentes()
        {
            rbSim.Enabled = true;
            rbNao.Enabled = true;
            txtCusto.Enabled = true;
            txtDistancia.Enabled = true;
            txtNivelDor.Enabled = true;
            dtpData.Enabled = true;
        }

        private void DesativarComponentes()
        {
            rbSim.Enabled = false;
            rbNao.Enabled = false;
            txtCusto.Enabled = false;
            txtDistancia.Enabled = false;
            txtNivelDor.Enabled = false;
            dtpData.Enabled = false;
        }

        private void LimparCampos()
        {
            string data = DateTime.Now.ToString("dd-MM-yyyy");
            dtpData.Value = Convert.ToDateTime(data);
            txtCusto.Text = "";
            txtDistancia.Text = "";
            txtNivelDor.Text = "";
            rbSim.Checked = false;
            rbNao.Checked = false;
            btnExcluir.Enabled = false;
            btnCancelar.Enabled = true;
        }
        #endregion

        #region Eventos
        private void lstvVoos_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string id = lstvVoos.SelectedItems[0].Text.ToString();
            btnExcluir.Enabled = true;
            btnCancelar.Enabled = true;
            CarregarParametrosLinhaSelecionada(id);
        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            btnCancelar.Enabled = true;
            btnSalvar.Enabled = true;
            AtivarComponentes();
            LimparCampos();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            string id = lstvVoos.SelectedItems[0].Text.ToString();

            bancodados.ExcluirRegistro(id);

            CarregarDadosListBoxView();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            string captura = "";

            string data = Convert.ToDateTime(dtpData.Text).ToString("yyyy-MM-dd 00:00:00");

            double custo = Convert.ToDouble(txtCusto.Text);
            int distancia = Convert.ToInt32(txtDistancia.Text);

            if (rbSim.Checked)
                captura = "S";

            if (rbNao.Checked)
                captura = "S";

            int nivelDor = Convert.ToInt32(txtNivelDor.Text);

            bancodados.IncluirRegistro(data, custo, distancia, captura, nivelDor);

            CarregarDadosListBoxView();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;

            CarregarDadosListBoxView();
            DesativarComponentes();
        }
        #endregion
    }
}
