using System;
using System.Collections.Generic;
using MiniBanco.ConsoleApp.Models;

namespace MiniBanco.ConsoleApp.Services
{
    public static class MotorAnalise
    {
        private const int PontosDeposito = 10;
        private const int PontosSaque = 5;
        private const int PontosTarifa = 0;
        private const int PontosMaximosPorTransacao = 10;
        private const int LarguraLinha = 68;

        public static int ConverterTipoEmPontos(string tipo)
        {
            if (tipo == Transacao.Deposito)
            {
                return PontosDeposito;
            }
            else if (tipo == Transacao.Saque)
            {
                return PontosSaque;
            }
            else if (tipo == Transacao.Tarifa)
            {
                return PontosTarifa;
            }

            return 0;
        }

        public static int CalcularPontuacaoObtida(Conta conta)
        {
            int total = 0;

            foreach (Transacao t in conta.Extrato)
            {
                total += ConverterTipoEmPontos(t.Tipo);
            }

            return total;
        }

        public static int CalcularPontuacaoMaxima(Conta conta)
        {
            return conta.Extrato.Count * PontosMaximosPorTransacao;
        }

        public static double CalcularPercentual(int pontuacaoObtida, int pontuacaoMaxima)
        {
            // Conta sem transações: evita divisão por zero.
            if (pontuacaoMaxima == 0)
            {
                return 0.0;
            }

            // Cast para double: sem ele, a divisão inteira seria truncada.
            return ((double)pontuacaoObtida / pontuacaoMaxima) * 100.0;
        }

        public static string Classificar(double percentual)
        {
            if (percentual >= 90.0)
            {
                return "Conta Saudável";
            }
            else if (percentual >= 60.0)
            {
                return "Conta em Atenção";
            }
            else
            {
                return "Conta em Risco";
            }
        }

        public static string ObterAcaoCorporativa(double percentual)
        {
            if (percentual >= 90.0)
            {
                return "Oferecer investimentos e avaliar aumento de limite.";
            }
            else if (percentual >= 60.0)
            {
                return "Contato da gerência para orientação financeira e revisão de tarifas.";
            }
            else
            {
                return "Bloquear aumento de limite e abrir análise de crédito.";
            }
        }

        public static void ExibirCabecalho()
        {
            Console.WriteLine(new string('=', LarguraLinha));
            Console.WriteLine("        MINIBANCO.NET - MOTOR DE GESTÃO DE CONTAS");
            Console.WriteLine(new string('=', LarguraLinha));
        }

        public static void ExibirRodape()
        {
            Console.WriteLine(new string('=', LarguraLinha));
            Console.WriteLine("        FIM DO PROCESSAMENTO DE CONTAS");
            Console.WriteLine(new string('=', LarguraLinha));
        }

        public static void ExibirRelatorioConta(Conta conta, int posicao, int totalContas)
        {
            int pontosObtidos = CalcularPontuacaoObtida(conta);
            int pontosMaximos = CalcularPontuacaoMaxima(conta);
            double percentual = CalcularPercentual(pontosObtidos, pontosMaximos);
            string classificacao = Classificar(percentual);

            Console.WriteLine();
            Console.WriteLine($"[{posicao}/{totalContas}] PROCESSANDO CONTA");
            Console.WriteLine(new string('-', LarguraLinha));

            Console.WriteLine("> DADOS DA CONTA:");
            Console.WriteLine($"  - Tipo: {conta.TipoConta}");
            Console.WriteLine($"  - Titular: {conta.Titular} | Conta: {conta.Numero}");
            Console.WriteLine($"  - Atributo Específico: {conta.DescreverAtributosEspecificos()}");
            Console.WriteLine($"  - Saldo Atual: R$ {conta.Saldo:F2}");
            Console.WriteLine();

            ExibirMovimentacoes(conta);

            Console.WriteLine("> RESUMO DA PONTUAÇÃO:");
            Console.WriteLine($"  - Pontuação Atingida: {pontosObtidos} de {pontosMaximos} pontos possíveis");
            Console.WriteLine($"  - Índice de Saúde: {percentual:F1}%");
            Console.WriteLine($"  - Classificação Final: [ {classificacao.ToUpper()} ]");
            Console.WriteLine();

            ExibirPendenciasERecomendacoes(conta, percentual, classificacao);
            Console.WriteLine(new string('-', LarguraLinha));
        }

        private static void ExibirMovimentacoes(Conta conta)
        {
            Console.WriteLine($"> MOVIMENTAÇÕES REGISTRADAS ({conta.Extrato.Count} TRANSAÇÕES):");

            if (conta.Extrato.Count == 0)
            {
                Console.WriteLine("  (nenhuma movimentação registrada)");
            }

            foreach (Transacao t in conta.Extrato)
            {
                string marcador;
                if (t.Tipo == Transacao.Deposito)
                {
                    marcador = "[OK]";
                }
                else if (t.Tipo == Transacao.Saque)
                {
                    marcador = "[ ! ]";
                }
                else
                {
                    marcador = "[ X ]";
                }

                // Marcador + descrição preenchidos com '-' até largura fixa, para alinhar as colunas.
                string prefixo = (marcador + " " + t.Descricao + " ").PadRight(36, '-');
                int pontos = ConverterTipoEmPontos(t.Tipo);
                Console.WriteLine($"  {prefixo} {t.Tipo}: R$ {t.Valor:F2} ({pontos} pts)");
            }

            Console.WriteLine();
        }

        private static void ExibirPendenciasERecomendacoes(Conta conta, double percentual, string classificacao)
        {
            Console.WriteLine("> RELATÓRIO DE PENDÊNCIAS E RECOMENDAÇÕES DA GERÊNCIA:");

            // 1ª varredura: conta quantos itens de cada tipo existem.
            int qtdTarifas = 0;
            int qtdSaques = 0;
            int qtdDepositos = 0;
            decimal totalTarifas = 0m;

            foreach (Transacao t in conta.Extrato)
            {
                if (t.Tipo == Transacao.Tarifa)
                {
                    qtdTarifas++;
                    totalTarifas += t.Valor;
                }
                else if (t.Tipo == Transacao.Saque)
                {
                    qtdSaques++;
                }
                else if (t.Tipo == Transacao.Deposito)
                {
                    qtdDepositos++;
                }
            }

            if (qtdTarifas == 0 && qtdSaques == 0)
            {
                if (classificacao == "Conta Saudável")
                {
                    Console.WriteLine("  🟢 Nenhuma pendência identificada. Oferecer produtos de investimento ao cliente.");
                }
                else
                {
                    Console.WriteLine("  🟢 Nenhuma pendência identificada.");
                    Console.WriteLine($"  ➜ {ObterAcaoCorporativa(percentual)}");
                }

                return;
            }

            // 2ª varredura: itens críticos (Tarifa).
            if (qtdTarifas > 0)
            {
                Console.WriteLine("  🔴 ITENS CRÍTICOS (TARIFAS):");
                foreach (Transacao t in conta.Extrato)
                {
                    if (t.Tipo == Transacao.Tarifa)
                    {
                        Console.WriteLine($"    - {t.Descricao} (R$ {t.Valor:F2}): Avaliar isenção ou migração para pacote de serviços.");
                    }
                }

                Console.WriteLine();
            }

            // 3ª varredura: itens de atenção (Saque).
            if (qtdSaques > 0)
            {
                Console.WriteLine("  🟡 ITENS DE ATENÇÃO (SAQUES):");
                foreach (Transacao t in conta.Extrato)
                {
                    if (t.Tipo == Transacao.Saque)
                    {
                        Console.WriteLine($"    - Saque de R$ {t.Valor:F2}: Retirada que exige acompanhamento.");
                    }
                }

                Console.WriteLine();
            }

            List<string> recomendacoes = new List<string>();

            if (qtdTarifas > 0)
            {
                recomendacoes.Add($"Revisar o pacote de tarifas do cliente ({qtdTarifas} cobrança(s), total de R$ {totalTarifas:F2}).");
            }

            if (qtdTarifas > 1)
            {
                recomendacoes.Add("Sugerir outro tipo de conta, mais adequado ao perfil de uso do cliente.");
            }

            if (conta.Saldo < 0)
            {
                recomendacoes.Add("Conta usando limite do cheque especial. Entrar em contato com o cliente.");
            }

            if (qtdSaques > qtdDepositos)
            {
                recomendacoes.Add("Saques superam os depósitos. Contatar o cliente para orientação financeira.");
            }

            recomendacoes.Add(ObterAcaoCorporativa(percentual));

            Console.WriteLine("  📋 RECOMENDAÇÕES (AÇÕES PRIORITÁRIAS):");
            for (int i = 0; i < recomendacoes.Count; i++)
            {
                Console.WriteLine($"    {i + 1}. {recomendacoes[i]}");
            }
        }
    }
}
