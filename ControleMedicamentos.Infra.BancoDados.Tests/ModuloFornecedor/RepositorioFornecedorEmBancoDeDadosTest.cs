using ControleMedicamentos.Dominio.ModuloFornecedor;
using ControleMedicamentos.Infra.BancoDados.ModuloFornecedor;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.Tests.ModuloFornecedor
{
    [TestClass]
    public class RepositorioFornecedorEmBancoDeDadosTest
    {
        [TestMethod]
        public void Deve_inserir_fornecedor()
        {

            RepositorioFornecedorEmBancoDeDados repositorioPaciente = new();

            Fornecedor fornecedor = new("pharma", "99999999", "uashfuas@gmail.com", "lages", "sc");

            RepositorioFornecedorEmBancoDeDados _repositorioForn = new();

            ValidationResult vr = _repositorioForn.Inserir(fornecedor);

            Assert.IsTrue(vr.IsValid);

        }
    }
}
