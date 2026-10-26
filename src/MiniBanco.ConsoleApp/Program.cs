using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MiniBanco.ConsoleApp.Exceptions;
using MiniBanco.ConsoleApp.Models;
using MiniBanco.ConsoleApp.Services;

namespace MiniBanco.ConsoleApp
{
    public class Program
    {
        // Lista central de contas do banco (RF11).
        private static readonly List<Conta> contas = new List<Conta>();
        private static CultureInfo cultura = CultureInfo.CurrentCulture;

        public static void Main(string[] args)
        {
            ConfigurarConsole();

            bool executando = true;

            // menu em laço de repetição.
            while (executando)
            {
                ExibirMenu();
                string? entrada = Console.ReadLine();

                // Entrada encerrada (Ctrl+Z / Ctrl+D): sai do programa em vez de ficar em laço infinito.
                string opcao = entrada == null ? "0" : entrada.Trim();

                // o menu trata as exceções e volta ao início, sem encerrar o programa.
                try
                {
                    switch (opcao)
                    {
                        case "1":
                            AbrirNovaConta();
                            break;
                        case "2":
                            RegistrarMovimentacao();
                            break;
                        case "3":
                            AplicarFechamentoMensalEmTodas();
                            break;
                        case "4":
                            ExibirRelatorioDasContas();
                            break;
                        case "0":
                            executando = false;
                            Console.WriteLine("Encerrando o MiniBanco. Até logo!");
                            break;
                        default:
                            Console.WriteLine("Opção inválida. Escolha uma das opções do menu.");
                            break;
                    }
                }
                catch (SaldoInsuficienteException ex)
                {
                    Console.WriteLine($"[ERRO] Operação não permitida: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"[ERRO] Dado inválido: {ex.Message}");
                }
                catch (FormatException)
                {
                    Console.WriteLine("[ERRO] Entrada inválida: digite um número válido (use vírgula nos decimais, ex: 1500,50).");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("[ERRO] O número digitado é grande demais.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERRO] Erro inesperado: {ex.Message}");
                }
            }
        }

        private static void AbrirNovaConta()
        {
            Console.WriteLine();
            Console.WriteLine("--- ABRIR NOVA CONTA ---");
            Console.WriteLine("1 - Conta Corrente");
            Console.WriteLine("2 - Conta Poupança");
            Console.WriteLine("3 - Conta Salário");
            string tipo = LerTexto("Tipo de conta: ");

            if (tipo != "1" && tipo != "2" && tipo != "3")
            {
                Console.WriteLine("Tipo de conta inválido.");
                return;
            }

            string titular = LerTexto("Titular: ");
            int numero = LerInteiro("Número da conta: ");

            if (BuscarConta(numero) != null)
            {
                throw new ArgumentException($"Já existe uma conta com o número {numero}.");
            }

            decimal saldoInicial = LerDecimal("Saldo inicial (R$): ");

            Conta novaConta;

            if (tipo == "1")
            {
                decimal limite = LerDecimal("Limite do cheque especial (R$): ");
                novaConta = new ContaCorrente(titular, numero, saldoInicial, limite);
            }
            else if (tipo == "2")
            {
                double taxa = LerDouble("Taxa de rendimento mensal (%): ");
                int dia = LerInteiro("Dia de aniversário (1-31): ");
                novaConta = new ContaPoupanca(titular, numero, saldoInicial, taxa, dia);
            }
            else
            {
                string empregador = LerTexto("Empregador: ");
                int limiteSaques = LerInteiro("Limite de saques mensais: ");
                novaConta = new ContaSalario(titular, numero, saldoInicial, empregador, limiteSaques);
            }

            contas.Add(novaConta);
            Console.WriteLine($"{novaConta.TipoConta} nº {novaConta.Numero} aberta para {novaConta.Titular}.");
        }

        private static void RegistrarMovimentacao()
        {
            Console.WriteLine();
            Console.WriteLine("--- REGISTRAR MOVIMENTAÇÃO ---");
            int numero = LerInteiro("Número da conta: ");

            Conta? conta = BuscarConta(numero);
            if (conta == null)
            {
                Console.WriteLine($"Nenhuma conta encontrada com o número {numero}.");
                return;
            }

            Console.WriteLine("1 - Depósito");
            Console.WriteLine("2 - Saque");
            string operacao = LerTexto("Operação: ");

            if (operacao == "1")
            {
                decimal valor = LerDecimal("Valor do depósito (R$): ");
                string descricao = LerTexto("Descrição (ENTER para padrão): ", true);
                if (descricao == "")
                {
                    descricao = "Depósito em dinheiro";
                }

                conta.Depositar(valor, descricao);
                Console.WriteLine($"Depósito realizado. Saldo atual: R$ {conta.Saldo:F2}");
            }
            else if (operacao == "2")
            {
                decimal valor = LerDecimal("Valor do saque (R$): ");
                conta.Sacar(valor);
                Console.WriteLine($"Saque realizado. Saldo atual: R$ {conta.Saldo:F2}");
            }
            else
            {
                Console.WriteLine("Operação inválida.");
            }
        }

        private static void AplicarFechamentoMensalEmTodas()
        {
            Console.WriteLine();
            if (contas.Count == 0)
            {
                Console.WriteLine("Nenhuma conta aberta até o momento.");
                return;
            }

            // Polimorfismo: não verificamos o tipo, cada conta aplica a sua própria regra.
            foreach (Conta conta in contas)
            {
                conta.AplicarFechamentoMensal();
            }

            Console.WriteLine($"Fechamento mensal aplicado em {contas.Count} conta(s).");
        }

        private static void ExibirRelatorioDasContas()
        {
            Console.WriteLine();
            if (contas.Count == 0)
            {
                Console.WriteLine("Nenhuma conta aberta até o momento.");
                return;
            }

            MotorAnalise.ExibirCabecalho();

            for (int i = 0; i < contas.Count; i++)
            {
                MotorAnalise.ExibirRelatorioConta(contas[i], i + 1, contas.Count);
            }

            Console.WriteLine();
            MotorAnalise.ExibirRodape();
        }

        private static Conta? BuscarConta(int numero)
        {
            for (int i = 0; i < contas.Count; i++)
            {
                if (contas[i].Numero == numero)
                {
                    return contas[i];
                }
            }

            return null;
        }

        private static void ExibirMenu()
        {
            Console.WriteLine();
            Console.WriteLine("==================== MINIBANCO .NET ====================");
            Console.WriteLine("1 - Abrir Nova Conta");
            Console.WriteLine("2 - Registrar Movimentação");
            Console.WriteLine("3 - Aplicar Fechamento Mensal");
            Console.WriteLine("4 - Exibir Relatório das Contas");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha uma opção: ");
        }

        private static string LerTexto(string mensagem, bool permitirVazio = false)
        {
            Console.Write(mensagem);
            string texto = (Console.ReadLine() ?? "").Trim();

            if (texto == "" && !permitirVazio)
            {
                throw new ArgumentException("O campo não pode ficar vazio.");
            }

            return texto;
        }

        // Parse (e não TryParse) de propósito: um texto inválido lança FormatException, tratada no menu (RF12).
        private static int LerInteiro(string mensagem)
        {
            return int.Parse(LerTexto(mensagem), NumberStyles.AllowLeadingSign, cultura);
        }

        // Só aceita sinal e separador decimal (vírgula): "1000.50" vira FormatException, e não 100050.
        private static decimal LerDecimal(string mensagem)
        {
            return decimal.Parse(LerTexto(mensagem), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, cultura);
        }

        private static double LerDouble(string mensagem)
        {
            return double.Parse(LerTexto(mensagem), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, cultura);
        }

        private static void ConfigurarConsole()
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                cultura = new CultureInfo("pt-BR");
            }
            catch (CultureNotFoundException)
            {
                cultura = CultureInfo.CurrentCulture;
            }

            CultureInfo.CurrentCulture = cultura;
        }
    }
}
