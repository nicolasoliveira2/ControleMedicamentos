using ControleMedicamentos.Dominio.ModuloRequisicao;
using ControleMedicamentos.Infra.BancoDados.ModuloRequisicao;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.Tests.ModuloRequisicao
{
    [TestClass]
    public class RepositorioRequisicaoEmBancoDeDadosTest
    {
        [TestMethod]
        public void Deve_inserir_requisicao()
        {

            RepositorioRequisicaoEmBancoDeDados repositorioRequisicao = new();

            Requisicao requisicao = new("Jorge", "278164127567812412");

            RepositorioRequisicaoEmBancoDeDados _repositorioReq = new();

            ValidationResult vr = _repositorioReq.Inserir(requisicao);

            Assert.IsTrue(vr.IsValid);

        }
    }
}
