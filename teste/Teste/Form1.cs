using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Teste.DAO;
using Teste.Models;

namespace Teste
{
    public partial class Form1 : Form
    {
        #region Variáveis

        string strConn = ConfigurationManager.AppSettings["StringConnexao"].ToString();
        string folder = ConfigurationManager.AppSettings["CaminhoArquivo"].ToString();
        string captura = "";

        Voo vooCadastrado = new Voo();
        Voo novoCadastro = new Voo();

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
            string createTable = "CREATE TABLE TB_VOO (ID_VOO INTEGER         PRIMARY KEY AUTOINCREMENT, DATA_VOO DATETIME, CUSTO     NUMERIC(10, 2), DISTANCIA INT, CAPTURA   CHAR(1),  NIVEL_DOR INT);";

            try
            {
                string arquivo = Path.Combine(folder, "acme.sqlite");

                if (!File.Exists(arquivo))
                {
                    using (StreamWriter sw = File.CreateText(arquivo))
                    {
                        sw.WriteLine(createTable);
                    }

                    bancodados.CriarTabela(createTable);
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

                lstvVoos.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lstvVoos.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

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
            string query = $"SELECT * FROM TB_VOO where ID_VOO = {id};";

            try
            {
                conn = new SQLiteConnection(strConn);
                SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
                da.AcceptChangesDuringFill = false;
                dt.Clear();
                da.Fill(dt);

                DataRow dr = dt.Rows[0];

                string dataRecebida = Convert.ToDateTime(dr.ItemArray[1].ToString()).ToString("dd/MM/yyyy");

                if (dataRecebida != null)
                    dtpData.Value = DateTime.ParseExact(dataRecebida, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                txtCusto.Text = dr.ItemArray[4].ToString();
                txtDistancia.Text = dr.ItemArray[5].ToString();

                string valorCaptura = dr.ItemArray[2].ToString();

                if (valorCaptura == "S")
                    rbSim.Checked = true;
                else
                    rbNao.Checked = true;

                txtNivelDor.Text = dr.ItemArray[3].ToString();

                vooCadastrado.ID = dr.ItemArray[0].ToString(); ;
                vooCadastrado.Data = dtpData.Value.ToString();
                vooCadastrado.Custo = txtCusto.Text;
                vooCadastrado.Distancia = Convert.ToInt32(txtDistancia.Text);
                vooCadastrado.Captura = valorCaptura;
                vooCadastrado.NivelDor = Convert.ToInt32(txtNivelDor.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro:" + ex.Message);
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
            dtpData.Value = DateTime.Today.AddDays(0);
            txtCusto.Text = "";
            txtDistancia.Text = "";
            txtNivelDor.Text = "";
            rbSim.Checked = false;
            rbNao.Checked = false;
            btnExcluir.Enabled = false;
            btnCancelar.Enabled = false;
            btnSalvar.Enabled = false;
        }

        private void HabilitarSalvarCancelar()
        {
            btnSalvar.Enabled = true;
            btnCancelar.Enabled = true;
        }

        #endregion

        #region Eventos

        public void lstvVoos_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            vooCadastrado.ID = null;

            if (this.lstvVoos.SelectedItems.Count == 0)
                return;

            string id = this.lstvVoos.SelectedItems[0].Text;
            vooCadastrado.ID = this.lstvVoos.SelectedItems[0].Text;

            CarregarParametrosLinhaSelecionada(id);

            AtivarComponentes();

            CarregarDadosListBoxView();

            btnIncluir.Enabled = true;
            btnExcluir.Enabled = true;
            btnCancelar.Enabled = false;
            btnSalvar.Enabled = false;
        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            vooCadastrado.ID = null;

            btnCancelar.Enabled = true;
            btnSalvar.Enabled = true;

            AtivarComponentes();
            LimparCampos();

            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            bancodados.ExcluirRegistro(vooCadastrado.ID);

            CarregarDadosListBoxView();

            LimparCampos();

            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;

            vooCadastrado.ID = null;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;

            if (rbSim.Checked)
                captura = "S";
            if (rbNao.Checked)
                captura = "N";

            string verifica = "^[0-9]";

            if (!Regex.IsMatch(txtCusto.Text, verifica) || !Regex.IsMatch(txtDistancia.Text, verifica) || !Regex.IsMatch(txtDistancia.Text, verifica))
            {
                MessageBox.Show("Por favor, preencher todos os campos corretamente!", "ATENÇÃO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (txtCusto.Text == "" || txtDistancia.Text == "" || captura == "" || txtNivelDor.Text == "")
            {
                MessageBox.Show("Por favor, preencher todos os campos!", "ATENÇÃO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dtpData.Text == "" || txtCusto.Text == "" || txtDistancia.Text == "" || captura == "" || txtNivelDor.Text == "")
            {
                MessageBox.Show("Por favor, preencher todos os campos!", "ATENÇÃO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            novoCadastro.Data = Convert.ToDateTime(dtpData.Text).ToString("yyyy-MM-dd 00:00:00");

            string custo = txtCusto.Text;
            string custoReplace = custo.Replace(",", ".");

            novoCadastro.Custo = custoReplace;

            novoCadastro.Distancia = Convert.ToInt32(txtDistancia.Text);

            novoCadastro.Captura = captura;
            novoCadastro.NivelDor = Convert.ToInt32(txtNivelDor.Text);

            if (vooCadastrado.ID != null)
            {
                if (vooCadastrado.Data != novoCadastro.Data || vooCadastrado.Custo != novoCadastro.Custo || vooCadastrado.Distancia != novoCadastro.Distancia || vooCadastrado.Captura != novoCadastro.Captura || vooCadastrado.NivelDor != novoCadastro.NivelDor)
                    bancodados.AtualizarRegistro(vooCadastrado.ID, novoCadastro.Data, novoCadastro.Custo, novoCadastro.Distancia, novoCadastro.Captura, novoCadastro.NivelDor);
            }
            else
                bancodados.IncluirRegistro(novoCadastro.Data, novoCadastro.Custo, novoCadastro.Distancia, novoCadastro.Captura, novoCadastro.NivelDor);

            CarregarDadosListBoxView();

            LimparCampos();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            btnSalvar.Enabled = false;
            btnCancelar.Enabled = false;

            CarregarDadosListBoxView();
            DesativarComponentes();
            LimparCampos();

            vooCadastrado.ID = null;
        }

        private void txtNivelDor_Leave(object sender, EventArgs e)
        {
            if (txtNivelDor.Text != "")
                novoCadastro.NivelDor = Convert.ToInt16(txtNivelDor.Text);
        }

        private void txtNivelDor_TextChanged(object sender, EventArgs e)
        {
            if (txtNivelDor.Enabled)
                HabilitarSalvarCancelar();
        }

        private void txtCusto_TextChanged(object sender, EventArgs e)
        {
            if (txtCusto.Enabled)
                HabilitarSalvarCancelar();
        }

        private void txtCusto_Leave(object sender, EventArgs e)
        {
            novoCadastro.Custo = txtCusto.Text;
        }

        private void txtDistancia_TextChanged(object sender, EventArgs e)
        {
            if (txtDistancia.Enabled)
                HabilitarSalvarCancelar();
        }

        private void txtDistancia_Leave(object sender, EventArgs e)
        {
            if (txtDistancia.Text != "")
                novoCadastro.Distancia = Convert.ToInt16(txtDistancia.Text);
        }

        private void dtpData_ValueChanged(object sender, EventArgs e)
        {
            if (dtpData.Enabled)
                HabilitarSalvarCancelar();
        }

        private void dtpData_Leave(object sender, EventArgs e)
        {
            novoCadastro.Data = Convert.ToDateTime(dtpData.Text).ToString("yyyy-MM-dd 00:00:00"); ;
        }

        private void rbNao_Click(object sender, EventArgs e)
        {
            if (rbNao.Enabled)
            {
                novoCadastro.Captura = "N";
                HabilitarSalvarCancelar();
            }
        }

        private void rbSim_Click(object sender, EventArgs e)
        {
            if (rbSim.Enabled)
            {
                novoCadastro.Captura = "S";
                HabilitarSalvarCancelar();
            }
        }

        private void txtCusto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',') && (e.KeyChar != '.'))
                e.Handled = true;

            if ((e.KeyChar == ',') && (e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                e.Handled = true;
        }

        private void txtNivelDor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtDistancia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        #endregion
    }
}
