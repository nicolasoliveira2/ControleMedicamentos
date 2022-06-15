using ControleMedicamentos.Dominio.ModuloPaciente;
using ControleMedicamentos.Infra.BancoDados.ModuloPaciente;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleMedicamentos.Infra.BancoDados.Tests.ModuloPaciente
{
    [TestClass]
    public class RepositorioPacienteEmBancoDadosTest
    {
        [TestMethod]
        public void Deve_inserir_paciente()
        {

            RepositorioPacienteEmBancoDeDados repositorioPaciente = new();

            Paciente paciente = new("Jorge", "278164127567812412");

            RepositorioPacienteEmBancoDeDados _repositorioPaci = new();

            ValidationResult vr = _repositorioPaci.Inserir(paciente);

            Assert.IsTrue(vr.IsValid);

        }
    }
}
