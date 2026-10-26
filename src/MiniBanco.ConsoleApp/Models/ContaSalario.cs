using System;
using MiniBanco.ConsoleApp.Exceptions;

namespace MiniBanco.ConsoleApp.Models
{
    /// <summary>
    /// RF03 - Conta com limite de saques mensais; excedeu, paga tarifa de R$ 5,00 por saque.
    /// </summary>
    public class ContaSalario : Conta
    {
        private const decimal TarifaSaqueExcedente = 5.00m;

        public string Empregador { get; }
        public int LimiteSaquesMensais { get; }
        public int SaquesNoMes { get; private set; }

        public ContaSalario(string titular, int numero, decimal saldoInicial, string empregador, int limiteSaquesMensais)
            : base(titular, numero, saldoInicial)
        {
            if (string.IsNullOrWhiteSpace(empregador))
            {
                throw new ArgumentException("O empregador não pode ser vazio.");
            }

            if (limiteSaquesMensais < 0)
            {
                throw new ArgumentException("O limite de saques mensais não pode ser negativo.");
            }

            this.Empregador = empregador;
            this.LimiteSaquesMensais = limiteSaquesMensais;
            this.SaquesNoMes = 0;
        }

        public override string TipoConta => "Conta Salário";

        public override string DescreverAtributosEspecificos()
        {
            return $"Empregador: {this.Empregador} | Limite de Saques Mensais: {this.LimiteSaquesMensais} (usados: {this.SaquesNoMes})";
        }

        public override void Sacar(decimal valor)
        {
            ValidarValor(valor);

            // Este saque ultrapassa o limite mensal? Então há tarifa adicional.
            bool excedeLimite = (this.SaquesNoMes + 1) > this.LimiteSaquesMensais;
            decimal tarifa = excedeLimite ? TarifaSaqueExcedente : 0m;

            // A conta salário não tem cheque especial: saque + tarifa precisam caber no saldo.
            if (valor + tarifa > this.Saldo)
            {
                throw new SaldoInsuficienteException(valor + tarifa, this.Saldo);
            }

            this.Saldo -= valor;
            RegistrarTransacao("Saque", Transacao.Saque, valor);
            this.SaquesNoMes++;

            if (excedeLimite)
            {
                CobrarTarifa(TarifaSaqueExcedente, "Tarifa de saque excedente");
            }
        }

        public override void AplicarFechamentoMensal()
        {
            // Sem tarifa de manutenção: apenas zera a contagem de saques do mês.
            this.SaquesNoMes = 0;
        }
    }
}
