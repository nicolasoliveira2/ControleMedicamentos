using ControleMedicamentos.Dominio.ModuloFuncionario;
using ControleMedicamentos.Infra.BancoDados.ModuloFuncionario;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.Tests.ModuloFuncionario
{
    [TestClass]
    public class RepositorioFuncionarioEmBancoDeDadosTest
    {
        [TestMethod]
        public void Deve_inserir_funcionario()
        {

            RepositorioFuncionarioEmBancoDeDados repositorioFuncionario = new();

            Funcionario funcionario = new("Jorge", "jorginho123", "123456");

            RepositorioFuncionarioEmBancoDeDados _repositorioFunc = new();

            ValidationResult vr = _repositorioFunc.Inserir(funcionario);

            Assert.IsTrue(vr.IsValid);

        }
    }
}
