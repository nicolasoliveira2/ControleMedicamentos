using ControleMedicamentos.Dominio.ModuloFornecedor;
using ControleMedicamentos.Dominio.ModuloMedicamento;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ControleMedicamentos.Infra.BancoDados.ModuloFornecedor
{
    public class RepositorioFornecedorEmBancoDeDados
    {
        private const string enderecobanco =
           @"Data Source=(LocalDB)\MSSqlLocalDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private const string sqlInserir =
           @" USE MEDICAMENTOSDB;
            INSERT INTO [TBFORNECEDOR]
            (

                [NOME],
                [TELEFONE],
                [EMAIL],
                [CIDADE],
                [ESTADO]

            )
            VALUES
        
            (
                @NOME,
                @TELEFONE,
                @EMAIL,
                @CIDADE,
                @ESTADO


            ); SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @" UPDATE [TBFORNECEDOR]
               SET 
                    [NOME] = @NOME,
                    [TELEFONE] = @TELEFONE,
                    [EMAIL] = @EMAIL,
                    [CIDADE] = @CIDADE,
                    [ESTADO] = @ESTADO,

                WHERE [ID] = @ID";

        private const string sqlExcluir =
            @"DELETE FROM [TBFORNECEDOR]
                WHERE [ID] = @ID";

        private const string sqlSelecionarTodos =
             @"SELECT 
                    [ID],
		            [NOME],
                    [TELEFONE],
                    [EMAIL],
                    [CIDADE],
                    [ESTADO],
 
	            FROM 
                    [TBFORNECEDOR] ";

        private const string sqlSelecionarPorID =
             @"SELECT 
					    [ID],
					    [NOME],
					    [EMAIL],
					    [ESTADO],
                        [CIDADE] ,
					    [TELEFONE] 
	            FROM 
                    [TBFORNECEDOR]

		        WHERE
                    [ID] = @ID";


        public ValidationResult Inserir(Fornecedor novoFornecedor)
        {
            var validador = new ValidadorFornecedor();

            var resultadoValidacao = validador.Validate(novoFornecedor);

            if (!resultadoValidacao.IsValid)
                return resultadoValidacao;
            SqlConnection conexaoBanco = new(enderecobanco);
            SqlCommand comandoInsersao = new(sqlInserir, conexaoBanco);

            ConfigurarParametrosFornecedor(novoFornecedor, comandoInsersao);

            conexaoBanco.Open();
            var id = comandoInsersao.ExecuteScalar();
            novoFornecedor.Numero = Convert.ToInt32(id);

            conexaoBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Excluir(Fornecedor fornecedorExcluir)
        {
            SqlConnection conexaoComBanco = new(enderecobanco);

            SqlCommand comandoExclusao = new(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("NUMERO", fornecedorExcluir.Numero);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            var resultadoValidacao = new ValidationResult();

            if (numeroRegistrosExcluidos == 0)
                resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Editar(Fornecedor fornecedorEditar)
        {
            var validador = new ValidadorFornecedor();

            var resultadoValidacao = validador.Validate(fornecedorEditar);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new(enderecobanco);

            SqlCommand comandoEdicao = new(sqlEditar, conexaoComBanco);

            ConfigurarParametrosFornecedor(fornecedorEditar, comandoEdicao);

            conexaoComBanco.Open();
            comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public List<Fornecedor> SelecionarTodos()
        {
            SqlConnection conexaoComBanco = new(enderecobanco);

            SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();
            SqlDataReader leitorRegistro = comandoSelecao.ExecuteReader();

            List<Fornecedor> registros = new List<Fornecedor>();

            while (leitorRegistro.Read())
            {
                Fornecedor registro = ConverterParaRegistro(leitorRegistro);

                registros.Add(registro);
            }

            conexaoComBanco.Close();


            return registros;
        }

        public Fornecedor SelecionarPorID(int ID)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecobanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorID, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", ID);

            conexaoComBanco.Open();
            SqlDataReader leitorRegistro = comandoSelecao.ExecuteReader();

            Fornecedor registro = null;
            if (leitorRegistro.Read())
                registro = ConverterParaRegistro(leitorRegistro);

            conexaoComBanco.Close();

            return registro;
        }

        private Fornecedor ConverterParaRegistro(SqlDataReader leitorRegistro)
        {



            int id = Convert.ToInt32(leitorRegistro["ID"]);
            string nomeFornecedor = Convert.ToString(leitorRegistro["NOME"]);
            string email = Convert.ToString(leitorRegistro["EMAIL"]);
            string estado = Convert.ToString(leitorRegistro["ESTADO"]);
            string cidade = Convert.ToString(leitorRegistro["CIDADE"]);
            string telefone = Convert.ToString(leitorRegistro["TELEFONE"]);




            var registro = new Fornecedor(nomeFornecedor, telefone, email, cidade, estado);
            registro.Numero = id;

           

            return registro;
        }

        private static void ConfigurarParametrosFornecedor(Fornecedor novoFornecedor, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("@NUMERO", novoFornecedor.Numero);
            comando.Parameters.AddWithValue("@NOME", novoFornecedor.Nome);
            comando.Parameters.AddWithValue("@TELEFONE", novoFornecedor.Telefone);
            comando.Parameters.AddWithValue("@EMAIL", novoFornecedor.Email);
            comando.Parameters.AddWithValue("@CIDADE", novoFornecedor.Cidade);
            comando.Parameters.AddWithValue("@ESTADO", novoFornecedor.Estado);

        }
    }
}
