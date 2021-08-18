using System;
using System.Configuration;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Teste.DAO
{
    class BancoDados
    {
        string strConn = ConfigurationManager.AppSettings["StringConnexao"].ToString();

        public int ExcluirRegistro(string id)
        {
            int resultado = -1;

            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = $"DELETE FROM TB_VOO  WHERE ID_VOO = {id};";
                    
                    try
                    {
                        resultado = cmd.ExecuteNonQuery();

                        if(resultado == 1)
                            MessageBox.Show("Cadastro excluído com sucesso!", "Cadastro Excluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                        {
                            MessageBox.Show("Erro ao excluir cadastro!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show($"Erro ao excluir cadastro! Erro: {ex}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public int IncluirRegistro(string data, double custo, int distancia, string captura, int nivelDor)
        {
            int resultado = -1;

            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = $"INSERT INTO TB_VOO (DATA_VOO, CUSTO, DISTANCIA, CAPTURA, NIVEL_DOR) VALUES('{data}', '{custo}', '{distancia}', '{captura}', '{nivelDor}');";

                    try
                    {
                        resultado = cmd.ExecuteNonQuery();

                        if (resultado == 1)
                            MessageBox.Show("cadastro realizado com sucesso!", "Cadastro Excluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                        {
                            MessageBox.Show("Erro ao cadastrar!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show($"Erro ao cadastrar cadastro! Erro: {ex}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        public int AtualizarRegistro(string id, string data, double custo, int distancia, string captura, int nivelDor)
        {
            int resultado = -1;

            using (SQLiteConnection conn = new SQLiteConnection(strConn))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = $"UPDATE TB_VOO SET DATA_VOO = {data}, CUSTO = {custo}, DISTANCIA = {distancia}, CAPTURA = {captura}, NIVEL_DOR = {nivelDor} WHERE ID_VOO = {id}";

                    try
                    {
                        resultado = cmd.ExecuteNonQuery();

                        if (resultado == 1)
                            MessageBox.Show("Cadastro excluído com sucesso!", "Cadastro Excluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                        {
                            MessageBox.Show("Erro ao excluir cadastro!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show($"Erro ao excluir cadastro! Erro: {ex}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}
