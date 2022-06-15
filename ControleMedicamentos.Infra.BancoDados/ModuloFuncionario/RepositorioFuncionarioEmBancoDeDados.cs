using ControleMedicamentos.Dominio.ModuloFuncionario;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.ModuloFuncionario
{
    public class RepositorioFuncionarioEmBancoDeDados
    {
        private const string enderecobanco =
          @"Data Source=(LocalDB)\MSSqlLocalDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private const string sqlInserir =
           @" USE MEDICAMENTOSDB;
            INSERT INTO [TBFUNCIONARIO]
            (

                [NOME],
                [LOGIN],
                [SENHA]

            )
            VALUES
        
            (
                @NOME,
                @LOGIN,
                @SENHA



            ); SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @" UPDATE [TBFUNCIONARIO]
               SET 
                    [NOME] = @NOME,
                    [LOGIN] = @LOGIN,
                    [SENHA] = @SENHA

                WHERE [ID] = @ID";

        private const string sqlExcluir =
            @"DELETE FROM [TBFUNCIONARIO]
                WHERE [ID] = @ID";

        private const string sqlSelecionarTodos =
             @"SELECT 
                    [ID],
		            [NOME],
                    [LOGIN],
                    [SENHA]

 
	            FROM 
                    [TBFUNCIONARIO] ";

        private const string sqlSelecionarPorID =
             @"SELECT 
					    [ID],
					    [NOME],
					    [LOGIN],
					    [SENHA]
	            FROM 
                    [TBFUNCIONARIO]

		        WHERE
                    [ID] = @ID";

        public ValidationResult Inserir(Funcionario novoFuncionario)
        {
            var validador = new ValidadorFuncionario();

            var resultadoValidacao = validador.Validate(novoFuncionario);

            if (!resultadoValidacao.IsValid)
                return resultadoValidacao;
            SqlConnection conexaoBanco = new(enderecobanco);
            SqlCommand comandoInsersao = new(sqlInserir, conexaoBanco);

            ConfigurarParametrosFuncionario(novoFuncionario, comandoInsersao);

            conexaoBanco.Open();
            var id = comandoInsersao.ExecuteScalar();
            novoFuncionario.Numero = Convert.ToInt32(id);

            conexaoBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Excluir(Funcionario funcionarioExcluir)
        {
            SqlConnection conexaoComBanco = new(enderecobanco);

            SqlCommand comandoExclusao = new(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("NUMERO", funcionarioExcluir.Numero);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            var resultadoValidacao = new ValidationResult();

            if (numeroRegistrosExcluidos == 0)
                resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Editar(Funcionario funcionarioEditar)
        {
            var validador = new ValidadorFuncionario();

            var resultadoValidacao = validador.Validate(funcionarioEditar);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new(enderecobanco);

            SqlCommand comandoEdicao = new(sqlEditar, conexaoComBanco);

            ConfigurarParametrosFuncionario(funcionarioEditar, comandoEdicao);

            conexaoComBanco.Open();
            comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public List<Funcionario> SelecionarTodos()
        {
            SqlConnection conexaoComBanco = new(enderecobanco);

            SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();

            List<Funcionario> funcionarios = new();

            conexaoComBanco.Close();

            return funcionarios;
        }

        public Funcionario SelecionarPorID(int ID)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecobanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorID, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", ID);

            conexaoComBanco.Open();
            SqlDataReader leitorRegistro = comandoSelecao.ExecuteReader();

            Funcionario registro = null;
            if (leitorRegistro.Read())
                registro = ConverterParaRegistro(leitorRegistro);

            conexaoComBanco.Close();

            return registro;
        }
        private Funcionario ConverterParaRegistro(SqlDataReader leitorRegistro)
        {



            int id = Convert.ToInt32(leitorRegistro["ID"]);
            string nomeFuncionario = Convert.ToString(leitorRegistro["NOME"]);
            string login = Convert.ToString(leitorRegistro["LOGIN"]);
            string senha = Convert.ToString(leitorRegistro["SENHA"]);





            var registro = new Funcionario(nomeFuncionario, login, senha);
            registro.Numero = id;



            return registro;
        }
        private static void ConfigurarParametrosFuncionario(Funcionario novoFuncionario, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("@ID", novoFuncionario.Numero);
            comando.Parameters.AddWithValue("@NOME", novoFuncionario.Nome);
            comando.Parameters.AddWithValue("@LOGIN", novoFuncionario.Login);
            comando.Parameters.AddWithValue("@SENHA", novoFuncionario.Senha);

        }
    }
}
