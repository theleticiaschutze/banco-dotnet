using System;

namespace MiniBanco.ConsoleApp.Exceptions
{
    /// <summary>
    /// Lançada por Sacar() quando o saque não é permitido (saldo/limite insuficiente).
    /// </summary>
    public class SaldoInsuficienteException : Exception
    {
        public decimal ValorSolicitado { get; }
        public decimal ValorDisponivel { get; }

        public SaldoInsuficienteException(decimal valorSolicitado, decimal valorDisponivel)
            : base($"Saldo insuficiente. Valor solicitado: R$ {valorSolicitado:F2} | Disponível para saque: R$ {valorDisponivel:F2}")
        {
            this.ValorSolicitado = valorSolicitado;
            this.ValorDisponivel = valorDisponivel;
        }
    }
}
