using ControleMedicamentos.Dominio.ModuloPaciente;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.ModuloPaciente
{
    public class RepositorioPacienteEmBancoDeDados
    {
        private const string enderecobanco =
         @"Data Source=(LocalDB)\MSSqlLocalDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private const string sqlInserir =
           @" USE MEDICAMENTOSDB;
            INSERT INTO [TBPACIENTE]
            (

                [NOME],
                [CARTAOSUS]


            )
            VALUES
        
            (
                @NOME,
                @CARTAOSUS



            ); SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @" UPDATE [TBPACIENTE]
               SET 
                    [NOME] = @NOME,
                    [CARTAOSUS] = @CARTAOSUS

                WHERE [ID] = @ID";

        private const string sqlExcluir =
            @"DELETE FROM [TBPACIENTE]
                WHERE [ID] = @ID";

        private const string sqlSelecionarTodos =
             @"SELECT 
                    [ID],
		            [NOME],
                    [CARTAOSUS]

 
	            FROM 
                    [TBPACIENTE] ";

        private const string sqlSelecionarPorID =
             @"SELECT 
					    [ID],
					    [NOME],
					    [CARTAOSUS]
	            FROM 
                    [TBPACIENTE]

		        WHERE
                    [ID] = @ID";

        public ValidationResult Inserir(Paciente novoPaciente)
        {
            var validador = new ValidadorPaciente();

            var resultadoValidacao = validador.Validate(novoPaciente);

            if (!resultadoValidacao.IsValid)
                return resultadoValidacao;
            SqlConnection conexaoBanco = new(enderecobanco);
            SqlCommand comandoInsersao = new(sqlInserir, conexaoBanco);

            ConfigurarParametrosPaciente(novoPaciente, comandoInsersao);

            conexaoBanco.Open();
            var id = comandoInsersao.ExecuteScalar();
            novoPaciente.Numero = Convert.ToInt32(id);

            conexaoBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Excluir(Paciente pacienteExcluir)
        {
            SqlConnection conexaoComBanco = new(enderecobanco);

            SqlCommand comandoExclusao = new(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("NUMERO", pacienteExcluir.Numero);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            var resultadoValidacao = new ValidationResult();

            if (numeroRegistrosExcluidos == 0)
                resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Editar(Paciente pacienteEditar)
        {
            var validador = new ValidadorPaciente();

            var resultadoValidacao = validador.Validate(pacienteEditar);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new(enderecobanco);

            SqlCommand comandoEdicao = new(sqlEditar, conexaoComBanco);

            ConfigurarParametrosPaciente(pacienteEditar, comandoEdicao);

            conexaoComBanco.Open();
            comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public List<Paciente> SelecionarTodos()
        {
            SqlConnection conexaoComBanco = new(enderecobanco);

            SqlCommand comandoSelecao = new(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();

            List<Paciente> pacientes = new();

            conexaoComBanco.Close();

            return pacientes;
        }

        public Paciente SelecionarPorID(int ID)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecobanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorID, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", ID);

            conexaoComBanco.Open();
            SqlDataReader leitorRegistro = comandoSelecao.ExecuteReader();

            Paciente registro = null;
            if (leitorRegistro.Read())
                registro = ConverterParaRegistro(leitorRegistro);

            conexaoComBanco.Close();

            return registro;
        }
        private Paciente ConverterParaRegistro(SqlDataReader leitorRegistro)
        {



            int id = Convert.ToInt32(leitorRegistro["ID"]);
            string nomeFuncionario = Convert.ToString(leitorRegistro["NOME"]);
            string cartaoSus = Convert.ToString(leitorRegistro["CARTAOSUS"]);






            var registro = new Paciente(nomeFuncionario, cartaoSus);
            registro.Numero = id;



            return registro;
        }
        private static void ConfigurarParametrosPaciente(Paciente novoPaciente, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("@ID", novoPaciente.Numero);
            comando.Parameters.AddWithValue("@NOME", novoPaciente.Nome);
            comando.Parameters.AddWithValue("@CARTAOSUS", novoPaciente.CartaoSUS);


        }
    }
}
