using System;
using MiniBanco.ConsoleApp.Exceptions;

namespace MiniBanco.ConsoleApp.Models
{
    public class ContaCorrente : Conta
    {
        private const decimal TarifaManutencao = 10.00m;

        public decimal LimiteChequeEspecial { get; }

        public ContaCorrente(string titular, int numero, decimal saldoInicial, decimal limiteChequeEspecial)
            : base(titular, numero, saldoInicial)
        {
            if (limiteChequeEspecial < 0)
            {
                throw new ArgumentException("O limite do cheque especial não pode ser negativo.");
            }

            this.LimiteChequeEspecial = limiteChequeEspecial;
        }

        public override string TipoConta => "Conta Corrente";

        public override string DescreverAtributosEspecificos()
        {
            return $"Limite Cheque Especial: R$ {this.LimiteChequeEspecial:F2}";
        }

        // Permite sacar até saldo + limite.
        public override void Sacar(decimal valor)
        {
            ValidarValor(valor);

            decimal disponivel = this.Saldo + this.LimiteChequeEspecial;
            if (valor > disponivel)
            {
                throw new SaldoInsuficienteException(valor, disponivel);
            }

            this.Saldo -= valor;
            RegistrarTransacao("Saque", Transacao.Saque, valor);
        }

        public override void AplicarFechamentoMensal()
        {
            CobrarTarifa(TarifaManutencao, "Tarifa de manutenção");
        }
    }
}
