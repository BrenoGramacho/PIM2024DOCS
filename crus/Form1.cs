using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MySql.Data;
using MySql.Data.MySqlClient;
using Mysqlx;
using Mysqlx.Cursor;

namespace CRUDFazendaUrbana
{
    public partial class Form1 : Form
    {
        // Variáveis para conexão com o banco de dados
        private MySqlConnection Conexao;
        private string data_source = "datasource=localhost;username=root;password=;database=db_agenda";

        // Variável para armazenar o ID do contato selecionado
        private int ?id_contato_selecionado = null;

        // Construtor do formulário (inicializa ao abrir a aplicação)
        public Form1()
        {
            InitializeComponent();

            // Configurações do ListView para exibição de dados e interação do usuário
            list_contatos.View = View.Details;
            list_contatos.LabelEdit = true;
            list_contatos.AllowColumnReorder = true;
            list_contatos.FullRowSelect = true;
            list_contatos.GridLines = true;

            // Adiciona colunas para "ID", "NOME", "SETOR" e "EMAIL"
            list_contatos.Columns.Add("ID", 30, HorizontalAlignment.Left);
            list_contatos.Columns.Add("NOME", 150, HorizontalAlignment.Left);
            list_contatos.Columns.Add("SETOR", 150, HorizontalAlignment.Left);
            list_contatos.Columns.Add("EMAIL", 150, HorizontalAlignment.Left);

            // Inicializa as caixas de texto com valores vazios
            txtNome.Text = String.Empty;
            txtEmail.Text = "";
            txtSetor.Text = "";

            // Carrega os contatos do banco de dados para o ListView ao iniciar o formulário
            carregar_contatos();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Estabelece conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                // Cria um objeto MySqlCommand para executar comandos SQL
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;

                // Verifica se há um contato selecionado (para atualização)
                if (id_contato_selecionado == null) 
                {
                    // **Inserir novo contato*
                    // Define o comando SQL para inserir um novo contato
                    cmd.CommandText = "INSERT INTO dadosAgenda (nomeColaborador, setorColaborador, email) " +
                                  "VALUES " +
                                  "(@nomeColaborador, @setorColaborador, @email)";

                    // Cria um objeto SqlParameter para cada parâmetro do comando SQL
                    var parameters = cmd.Parameters;
                    parameters.AddWithValue("@nomeColaborador", txtNome.Text);
                    parameters.AddWithValue("@setorColaborador", txtSetor.Text);
                    parameters.AddWithValue("@email", txtEmail.Text);

                    // Executa o comando SQL para inserir o novo contato
                    cmd.ExecuteNonQuery();

                    // Exibe mensagem de sucesso na caixa de diálogo "Contato Inserido"
                    MessageBox.Show("Contato inserido",
                         "OK",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Information);
                }
                else
                {
                    // Define o comando SQL para atualizar um novo contato
                    cmd.CommandText = "UPDATE dadosAgenda SET " +
                              "nomeColaborador = @nomeColaborador, setorColaborador = @setorColaborador, email = @email " +
                              "WHERE idColaborador = @idColaborador ";

                    // Define os parâmetros do comando SQL para atualização
                    var parameters = cmd.Parameters;
                    parameters.AddWithValue("@nomeColaborador", txtNome.Text);
                    parameters.AddWithValue("@setorColaborador", txtSetor.Text);
                    parameters.AddWithValue("@email", txtEmail.Text);
                    parameters.AddWithValue("@idColaborador", id_contato_selecionado);

                    // Executa o comando SQL para atualizar o contato
                    cmd.ExecuteNonQuery();

                    // Exibe mensagem de sucesso na caixa de diálogo
                    MessageBox.Show("Contato atualizado",
                                "OK",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                }

                // Limpa o formulário
                zerar_formulario();

                // Recarrega a lista de contatos com os dados atualizados
                carregar_contatos();



            }
            catch (MySqlException ex)
            {
                // Exibe mensagem de erro específica para exceções MySql
                MessageBox.Show("Erro " + ex.Number + " ocorreu " + ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

            catch (Exception ex)
            {
                // Exibe mensagem de erro genérica para outras exceções
                MessageBox.Show("Erro  ocorreu " + ex.Message,
                     "Erro",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Error);
            }

            finally
            {
                // Fecha a conexão com o banco de dados(sempre executar)
                Conexao.Close();
            }
        }

      

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Estabelece conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                // Cria um objeto MySqlCommand para executar o comando SQL de busca
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;

                // Define o comando SQL para buscar contatos por nome ou email (com LIKE para pesquisa parcial)
                cmd.CommandText = "SELECT * FROM  dadosAgenda WHERE nomeColaborador LIKE @q OR email LIKE @q ";
                var parameters = cmd.Parameters;
                parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");

                // Executa o comando SQL e obtém os resultados
                MySqlDataReader reader = cmd.ExecuteReader();

                // Limpa a lista de contatos antes de preencher com os resultados da busca
                list_contatos.Items.Clear();

                // Percorre os resultados da busca e adiciona cada contato encontrado na lista
                while (reader.Read()) {

                    // Cria uma array de strings para armazenar os dados de cada contato
                    string[] row =
                    {
                        reader.GetInt16(0).ToString(),  //Id
                        reader.GetString(1),            // Nome
                        reader.GetString(2),            // Setor
                        reader.GetString(3),            // E-mail

                    };

                    // Adiciona uma nova linha (ListViewItem) na lista de contatos usando a array de dados
                    list_contatos.Items.Add(new ListViewItem(row));
                }



            }
            catch (MySqlException ex)
            {
                // Exibe mensagem de erro específica para exceções MySql
                MessageBox.Show("Erro " + ex.Number + " ocorreu " + ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

            catch (Exception ex)
            {
                // Exibe mensagem de erro genérica para outras exceções
                MessageBox.Show("Erro  ocorreu " + ex.Message,
                                "Erro",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
            }

            finally
            {
                // Fecha a conexão com o banco de dados
                Conexao.Close();
            }
        }

        // Método para carregar a lista de contatos do banco de dados
        private void carregar_contatos()
        {
            try
            {
                // Estabelece conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                // Cria um objeto MySqlCommand para executar o comando SQL de consulta
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;

                // Define o comando SQL para selecionar todos os contatos ordenados por ID decrescente
                cmd.CommandText = "SELECT * FROM  dadosAgenda ORDER BY idColaborador DESC";
                var parameters = cmd.Parameters; // Parâmetros não utilizados nesta consulta (opcional)

                // Executa o comando e obtém os resultados em um MySqlDataReader
                MySqlDataReader reader = cmd.ExecuteReader();

                // Limpa a lista de contatos antes de preenchê-la com os resultados da busca
                list_contatos.Items.Clear();

                // Percorre os resultados da consulta linha por linha
                while (reader.Read())
                {
                    // Cria uma array de strings para armazenar os dados de cada contato
                    string[] row =
                    {
                        reader.GetInt16(0).ToString(),  //Id
                        reader.GetString(1),            // Nome
                        reader.GetString(2),            // Setor
                        reader.GetString(3),            // E-mail

                    };

                    // Adiciona uma nova linha (ListViewItem) na lista de contatos usando a array de dados
                    list_contatos.Items.Add(new ListViewItem(row));
                }



            }
            catch (MySqlException ex) // Captura erros específicos do MySql
            {
                // Exibe mensagem de erro informando o número do erro e a mensagem do MySql
                MessageBox.Show("Erro " + ex.Number + " ocorreu " + ex.Message,
                                "Erro",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

            catch (Exception ex) // Captura erros genéricos
            {
                // Exibe mensagem de erro informando a mensagem da exceção genérica
                MessageBox.Show("Erro  ocorreu " + ex.Message,
                     "Erro",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Error);
            }

            finally
            {
                // Fecha a conexão com o banco de dados(importante fechar mesmo em caso de erros)
                Conexao.Close();
            }


        }


        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }

       
        private void list_contatos_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // Obtém a coleção de itens selecionados na lista
            ListView.SelectedListViewItemCollection itens_selecionados = list_contatos.SelectedItems;

            // Percorre cada item selecionado na lista
            foreach (ListViewItem item in itens_selecionados)
            {
                // Converte o texto da primeira subcoluna (assumindo que o ID está na primeira coluna) para inteiro e armazena na variável
                id_contato_selecionado = Convert.ToInt32(item.SubItems[0].Text);

                // Preenche as caixas de texto com as informações do contato selecionado (nome, setor, email) obtidas das subcolunas do item
                txtNome.Text = item.SubItems[1].Text;
                txtSetor.Text = item.SubItems[2].Text;
                txtEmail.Text = item.SubItems[3].Text;

                // Torna o botão "Excluir" visível, pois há um contato selecionado para exclusão
                button4.Visible = true;

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Chama o método para limpar o formulário e desfazer seleções
            zerar_formulario();

        }

        private void list_contatos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Chama o método para excluir o contato selecionado
            excluir_contato();
                   
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Chama o método para excluir o contato selecionado
            excluir_contato();

        }

        private void excluir_contato()
        {
            try
            {
                // Exibe uma caixa de diálogo de confirmação para exclusão
                DialogResult conf = MessageBox.Show("Confirmar exclusão ?",
                "Confirmacao",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

                if (conf == DialogResult.Yes) // Se o usuário confirma a exclusão
                {

                    // Estabelece conexão com o banco de dados
                    Conexao = new MySqlConnection(data_source);
                    Conexao.Open();

                    // Cria um objeto MySqlCommand para executar o comando de DELETE
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = Conexao;

                    // Define o comando SQL para excluir o contato pelo ID selecionado
                    cmd.CommandText = "DELETE FROM dadosAgenda WHERE idColaborador = @idColaborador";
                    var parameters = cmd.Parameters;
                    parameters.AddWithValue("@idColaborador", id_contato_selecionado);

                    // Executa o comando para excluir o contato
                    cmd.ExecuteNonQuery();

                    // Exibe mensagem de sucesso informando que o contato foi excluído
                    MessageBox.Show("Contato excluido",
                    "OK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                    // Atualiza a lista de contatos para refletir a exclusão
                    carregar_contatos();

                    // Limpa o formulário
                    zerar_formulario();

                }


            }
            catch (MySqlException ex) // Captura erros específicos do MySql
            {
                // Exibe mensagem de erro informando o número do erro e a mensagem do MySql
                MessageBox.Show("Erro  ocorreu " + ex.Message,
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            }
            catch (Exception ex) // Captura erros genéricos
            {
                // Exibe mensagem de erro informando a mensagem da exceção genérica
                MessageBox.Show("Erro  ocorreu " + ex.Message,
                "Erro",
                MessageBoxButtons.OK,
                 MessageBoxIcon.Error);

            }
            finally // Bloco sempre executado, independente de erros
            {
                // Fecha a conexão com o banco de dados (importante fechar mesmo em caso de erros)

            }

        }

        // Método para limpar o formulário e desfazer seleções
        private void zerar_formulario()
        {
            // Define o ID do contato selecionado como nulo (nenhum contato selecionado)
            id_contato_selecionado = null;

            // Limpa o texto de todas as caixas de texto (nome, email, setor)
            txtNome.Text = String.Empty;
            txtEmail.Text = "";
            txtSetor.Text = "";

            // Coloca o foco na caixa de texto "Nome" para facilitar nova entrada de dados
            txtNome.Focus();

            // Oculta o botão "Excluir" pois nenhum contato está selecionado
            button4.Visible = false;

        }
    }
}
