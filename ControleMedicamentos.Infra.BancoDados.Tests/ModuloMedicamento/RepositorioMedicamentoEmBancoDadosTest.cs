using ControleMedicamento.Infra.BancoDados.ModuloMedicamento;
using ControleMedicamentos.Dominio.ModuloFornecedor;
using ControleMedicamentos.Dominio.ModuloMedicamento;
using ControleMedicamentos.Infra.BancoDados.ModuloFornecedor;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ControleMedicamento.Infra.BancoDados.Tests.ModuloMedicamento
{
    [TestClass]
    public class RepositorioMedicamentoEmBancoDadosTest
    {
        [TestMethod]
        public void Deve_inserir_medicamento()
        {
            
                RepositorioFornecedorEmBancoDeDados repositorioFornecedor = new();

                Medicamento medicamento = new("nome", "10 caracteres", "Teste", DateTime.MaxValue);

                Fornecedor fornecedor = new Fornecedor("nome", "9999-9999", "jajaja@gmail.com", "lages", "sc");

                repositorioFornecedor.Inserir(fornecedor);

                medicamento.Fornecedor = fornecedor;

                RepositorioMedicamentoEmBancoDados _repositorioMed = new();

                ValidationResult vr = _repositorioMed.Inserir(medicamento);

                Assert.IsTrue(vr.IsValid);
            
        }

        [TestMethod]
        public void Deve_Excluir_Medicamento()
        {
            Medicamento medicamento = new("nome", "10 caracteres", "Teste", DateTime.MaxValue);
          
        }
    }
}
