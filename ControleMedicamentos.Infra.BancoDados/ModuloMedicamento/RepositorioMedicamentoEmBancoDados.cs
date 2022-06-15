

using ControleMedicamentos.Dominio.ModuloFornecedor;
using ControleMedicamentos.Dominio.ModuloMedicamento;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ControleMedicamento.Infra.BancoDados.ModuloMedicamento
{
    public class RepositorioMedicamentoEmBancoDados
    {
        private const string enderecobanco =
            @"Data Source=(LocalDB)\MSSqlLocalDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        private const string sqlInserir =
            @" USE MEDICAMENTOSDB;
            INSERT INTO [TBMEDICAMENTO]
            (

                [NOME],
                [DESCRICAO],
                [LOTE],
                [VALIDADE],
                [QUANTIDADEDISPONIVEL],
                [FORNECEDOR_ID]
            )
            VALUES
        
            (
                @NOME,
                @DESCRICAO,
                @LOTE,
                @VALIDADE,
                @QUANTIDADEDISPONIVEL,
                @FORNECEDOR_ID

            ); SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @" UPDATE [TBMEDICAMENTO]
               SET 
                    [NOME] = @NOME,
                    [DESCRICAO] = @DESCRICAO,
                    [LOTE] = @LOTE,
                    [VALIDADE] = @VALIDADE,
                    [QUANTIDADEDISPONIVEL] = @QUANTIDADEDISPONIVEL,
                    [FORNECEDOR_ID] = @FORNECEDOR_ID

                WHERE [ID] = @ID";

        private const string sqlExcluir =
            @"DELETE FROM [TBMEDICAMENTO]
                WHERE [ID] = @ID";

        private const string sqlSelecionarTodos =
             @"SELECT 
                    M.[ID] AS ID,
		            M.[NOME] AS NOME,
                    M.[DESCRICAO] AS DESCRICAO,
                    M.[LOTE] AS LOTE,
                    M.[VALIDADE] AS VALIDADE,
                    M.[QUANTIDADEDISPONIVEL] AS QUANTIDADE,
                        F.[ID] AS FORNECEDOR_ID,
                        F.[NOME] AS FORNECEDOR_NOME,
					    F.[EMAIL] AS FORNECEDOR_EMAIL,
					    F.[TELEFONE] AS FORNECEDOR_TELEFONE			    				
                        F.[CIDADE] AS FORNECEDOR_CIDADE,
                        F.[ESTADO] AS FORNECEDOR_ESTADO,
	            FROM 
		            [TBMEDICAMENTO] AS M LEFT JOIN
                    [TBFORNECEDOR] AS F
                ON
                    M.[FORNECEDOR_ID] = F.ID";

        private const string sqlSelecionarPorID  =
             @"SELECT 
                    M.[NUMERO] AS NUMERO,
    		        M.[NOME] AS NOME,
                    M.[DESCRICAO] AS DESCRICAO,
                    M.[LOTE] AS LOTE,
                    M.[VALIDADE] AS VALIDADE,
                    M.[QUANTIDADEDISPONIVEL] AS QUANTIDADE,
					    F.[EMAIL] AS FORNECEDOR_EMAIL,
					    F.[ESTADO] AS FORNECEDOR_ESTADO,
					    F.[ID] AS FORNECEDOR_ID,
					    F.[NOME] AS FORNECEDOR_NOME,
                        F.[CIDADE] AS FORNECEDOR_CIDADE,
					    F.[TELEFONE] AS FORNECEDOR_TELEFONE
	            FROM 
		            [TBMEDICAMENTO] AS M LEFT JOIN
                    [TBFORNECEDOR] AS F
                ON
                    M.[FORNECEDOR_ID] = F.ID
		        WHERE
                    M.[ID] = @ID";


        public ValidationResult Inserir(Medicamento novoMedicamento)
        {
            var validador = new ValidadorMedicamento();

            var resultadoValidacao = validador.Validate(novoMedicamento);

            if (!resultadoValidacao.IsValid)
                return resultadoValidacao;
            SqlConnection conexaoBanco = new(enderecobanco);
            SqlCommand comandoInsersao = new(sqlInserir, conexaoBanco);

            ConfigurarParametrosMedicamentos(novoMedicamento, comandoInsersao);

            conexaoBanco.Open();
            var id = comandoInsersao.ExecuteScalar();
            novoMedicamento.Numero = Convert.ToInt32(id);

            conexaoBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Excluir(Medicamento medicamentoExcluir)
        {
            SqlConnection conexaoComBanco = new (enderecobanco);

            SqlCommand comandoExclusao = new (sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("NUMERO", medicamentoExcluir.Numero);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            var resultadoValidacao = new ValidationResult();

            if (numeroRegistrosExcluidos == 0)
                resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Editar(Medicamento medicamentoEditar)
        {
            var validador = new ValidadorMedicamento();

            var resultadoValidacao = validador.Validate(medicamentoEditar);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new (enderecobanco);

            SqlCommand comandoEdicao = new (sqlEditar, conexaoComBanco);

            ConfigurarParametrosMedicamentos(medicamentoEditar, comandoEdicao);

            conexaoComBanco.Open();
            comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public List<Medicamento> SelecionarTodos()
        {
            SqlConnection conexaoComBanco = new (enderecobanco);

            SqlCommand comandoSelecao = new (sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();           

            List<Medicamento> medicamentos = new ();        

            conexaoComBanco.Close();

            return medicamentos;
        }
        public Medicamento SelecionarPorID(int ID)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecobanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorID, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("ID", ID);

            conexaoComBanco.Open();
            SqlDataReader leitorRegistro = comandoSelecao.ExecuteReader();

            Medicamento registro = null;
            if (leitorRegistro.Read())
                registro = ConverterParaRegistro(leitorRegistro);

            conexaoComBanco.Close();

            return registro;
        }

        private Medicamento ConverterParaRegistro(SqlDataReader leitorRegistro)
        {

            int Id = Convert.ToInt32(leitorRegistro["ID"]);
            string nome = Convert.ToString(leitorRegistro["NOME"]);
            string descricao = Convert.ToString(leitorRegistro["DESCRICAO"]);
            string lote = Convert.ToString(leitorRegistro["LOTE"]);
            DateTime validade = Convert.ToDateTime(leitorRegistro["VALIDADE"]).Date;
            int quantidade = Convert.ToInt32(leitorRegistro["QUANTIDADE"]);


            int idFornecedor = Convert.ToInt32(leitorRegistro["FORNECEDOR_ID"]);
            string nomeFornecedor = Convert.ToString(leitorRegistro["FORNECEDOR_NOME"]);
            string email = Convert.ToString(leitorRegistro["FORNECEDOR_EMAIL"]);
            string estado = Convert.ToString(leitorRegistro["FORNECEDOR_ESTADO"]);
            string cidade = Convert.ToString(leitorRegistro["FORNECEDOR_CIDADE"]);
            string telefone = Convert.ToString(leitorRegistro["FORNECEDOR_TELEFONE"]);




            var fornecedor = new Fornecedor(nomeFornecedor, telefone, email, cidade, estado);
            fornecedor.Numero = idFornecedor;

            var registro = new Medicamento(nome, descricao, lote, validade);
            registro.Numero = Id;
            registro.Fornecedor = fornecedor;
            registro.QuantidadeDisponivel = quantidade;

            return registro;
        }

        private static void ConfigurarParametrosMedicamentos(Medicamento novoMedicamendo, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("@ID", novoMedicamendo.Numero);
            comando.Parameters.AddWithValue("@NOME", novoMedicamendo.Nome);
            comando.Parameters.AddWithValue("@DESCRICAO", novoMedicamendo.Descricao);
            comando.Parameters.AddWithValue("@LOTE", novoMedicamendo.Lote);
            comando.Parameters.AddWithValue("@VALIDADE", novoMedicamendo.Validade);
            comando.Parameters.AddWithValue("@QUANTIDADEDISPONIVEL", novoMedicamendo.QuantidadeDisponivel);
            comando.Parameters.AddWithValue("@FORNECEDOR_ID", novoMedicamendo.Fornecedor.Numero);
        }
    }
}
