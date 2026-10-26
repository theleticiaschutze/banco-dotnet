using System;

namespace MiniBanco.ConsoleApp.Models
{
    public class ContaPoupanca : Conta
    {
        public double TaxaRendimentoMensal { get; }   // em %
        public int DiaAniversario { get; }

        public ContaPoupanca(string titular, int numero, decimal saldoInicial, double taxaRendimentoMensal, int diaAniversario)
            : base(titular, numero, saldoInicial)
        {
            if (taxaRendimentoMensal < 0)
            {
                throw new ArgumentException("A taxa de rendimento não pode ser negativa.");
            }

            if (diaAniversario < 1 || diaAniversario > 31)
            {
                throw new ArgumentException("O dia de aniversário deve estar entre 1 e 31.");
            }

            this.TaxaRendimentoMensal = taxaRendimentoMensal;
            this.DiaAniversario = diaAniversario;
        }

        public override string TipoConta => "Conta Poupança";

        public override string DescreverAtributosEspecificos()
        {
            return $"Rendimento: {this.TaxaRendimentoMensal}% ao mês | Aniversário: dia {this.DiaAniversario}";
        }

        public override void AplicarFechamentoMensal()
        {
            decimal rendimento = Math.Round(this.Saldo * (decimal)this.TaxaRendimentoMensal / 100m, 2);

            // Transacao exige valor > 0: sem saldo ou sem taxa, não há rendimento a registrar.
            if (rendimento > 0)
            {
                Depositar(rendimento, "Rendimento mensal");
            }
        }
    }
}
