using System;
using System.Collections.Generic;
using MiniBanco.ConsoleApp.Exceptions;

namespace MiniBanco.ConsoleApp.Models
{
    public abstract class Conta
    {
        public string Titular { get; }
        public int Numero { get; }

        // Encapsulamento para  leitura pública, escrita apenas na Conta e nas subclasses.
        public decimal Saldo { get; protected set; }

        public List<Transacao> Extrato { get; }

        protected Conta(string titular, int numero, decimal saldoInicial)
        {
            if (string.IsNullOrWhiteSpace(titular))
            {
                throw new ArgumentException("O titular não pode ser vazio.");
            }

            if (numero <= 0)
            {
                throw new ArgumentException("O número da conta deve ser maior que zero.");
            }

            if (saldoInicial < 0)
            {
                throw new ArgumentException("O saldo inicial não pode ser negativo.");
            }

            this.Titular = titular;
            this.Numero = numero;
            this.Saldo = saldoInicial;
            this.Extrato = new List<Transacao>();
        }

        // Cada subclasse informa seu nome e seus atributos (polimorfismo na exibição).
        public abstract string TipoConta { get; }
        public abstract string DescreverAtributosEspecificos();

        public void RegistrarTransacao(string descricao, string tipo, decimal valor)
        {
            // O construtor de Transacao valida tipo e valor (lança ArgumentException).
            Transacao transacao = new Transacao(descricao, tipo, valor);
            this.Extrato.Add(transacao);
        }

        public void Depositar(decimal valor, string descricao)
        {
            ValidarValor(valor);
            this.Saldo += valor;
            RegistrarTransacao(descricao, Transacao.Deposito, valor);
        }

        public virtual void Sacar(decimal valor)
        {
            ValidarValor(valor);

            if (valor > this.Saldo)
            {
                throw new SaldoInsuficienteException(valor, this.Saldo);
            }

            this.Saldo -= valor;
            RegistrarTransacao("Saque", Transacao.Saque, valor);
        }

        public virtual void AplicarFechamentoMensal()
        {
            // Comportamento padrão: nenhuma regra. Subclasses sobrescrevem.
        }

        // Débito de tarifa: usado pelas subclasses (tarifa pode levar o saldo ao negativo).
        protected void CobrarTarifa(decimal valor, string descricao)
        {
            this.Saldo -= valor;
            RegistrarTransacao(descricao, Transacao.Tarifa, valor);
        }

        protected static void ValidarValor(decimal valor)
        {
            if (valor <= 0)
            {
                throw new ArgumentException("O valor deve ser maior que zero.");
            }
        }
    }
}
