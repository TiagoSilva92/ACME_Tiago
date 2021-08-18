using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace Teste
{
    public partial class Form1 : Form
    {
        #region Variáveis
        string strConn = @"Data Source=C:\Users\tiago\OneDrive\Área de Trabalho\TESTE\BD\ACME.db";
        string folder = ConfigurationManager.AppSettings["CaminhoArquivo"].ToString();

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
                string sql = "SELECT ID_VOO, CAPTURA, NIVEL_DOR FROM TB_VOO;";

                conn = new SQLiteConnection(strConn);
                SQLiteDataAdapter da = new SQLiteDataAdapter(sql, strConn);
                da.Fill(dt);

                lstvVoos.Clear();
                lstvVoos.Refresh();

                lstvVoos.View = View.Details;

                string coluna = "ID";
                lstvVoos.Columns.Add(coluna);
                //string coluna1 = "Data";
                //lstvVoos.Columns.Add(coluna1);
                string coluna2 = "Captura";
                lstvVoos.Columns.Add(coluna2);
                string coluna3 = "Nivel de Dor";
                lstvVoos.Columns.Add(coluna3);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];
                    ListViewItem listitem = new ListViewItem(dr["ID_VOO"].ToString());
                    //listitem.SubItems.AddD(dr["DATA_VOO"].ToString());
                    listitem.SubItems.Add(dr["CAPTURA"].ToString());
                    listitem.SubItems.Add(dr["NIVEL_DOR"].ToString());
                    lstvVoos.Items.Add(listitem);
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

        private void CarregarParametrosLinhaSelecionada()
        {
            string data = lstvVoos.Items.ToString();
            string captura = lstvVoos.Columns[1].ToString();
            //string nivelDor = lstvVoos.SelectedIndices[2].Text.ToString();

            txtCaptura.Text = data;
        }

        private void AtivarComponentes()
        {
            txtCaptura.Enabled = true;
            txtCusto.Enabled = true;
            txtDistancia.Enabled = true;
            txtNivelDor.Enabled = true;
            dtpData.Enabled = true;
        }

        private int ExcluirRegistro(string id)
        {
            int resultado = -1;

            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = $"DELETE FROM TB_VOO  WHERE ID_VOO = {id};";
                    cmd.Prepare();
                    //cmd.Parameters.AddWithValue("@Id", voo.Id);
                    try
                    {
                        resultado = cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return resultado;
        }

        #endregion

        #region Eventos
        private void lstvVoos_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            CarregarParametrosLinhaSelecionada();
        }

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            AtivarComponentes();

            //ExcluirRegistro(id);
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {

        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {

        }


        #endregion
    }
}
