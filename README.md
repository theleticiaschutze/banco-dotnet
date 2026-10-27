# MiniBanco.ConsoleApp — Motor de Gestão de Contas Bancárias

> **Identificação:** Letícia Schütze — Desenvolvedor Back-End.NET SCTEC
> **Vídeo de apresentação:** ...sendo feito...

## Objetivo

Aplicação de console em C# que gerencia contas bancárias de três tipos (corrente, poupança e salário), registra
movimentações, aplica o fechamento mensal de cada tipo de conta, calcula o **índice de saúde financeira**,
classifica a conta e emite um relatório com pendências e recomendações para a gerência.

O projeto consolida: tipos primitivos, `List<T>`, estruturas de controle, POO (classes, construtores, encapsulamento,
herança, `virtual`/`override`, polimorfismo), tratamento de exceções e versionamento com Git/GitHub.

## Como executar

```bash
git clone <https://github.com/theleticiaschutze/banco-dotnet.git>
cd minibanco-dotnet/src/MiniBanco.ConsoleApp
dotnet run
```

> Dica (Windows): para exibir corretamente os emojis do relatório, use o **Windows Terminal** ou o terminal integrado do VS Code.
> Ao digitar valores decimais, use **vírgula** (ex.: `1500,50`).

## Funcionalidades (menu principal)

| Opção | Descrição |
|-------|-----------|
| 1 | Abrir nova conta (Corrente, Poupança ou Salário) — coleta dados e atributos específicos |
| 2 | Registrar movimentação (Depósito ou Saque) em uma conta existente |
| 3 | Aplicar fechamento mensal em todas as contas (polimorfismo) |
| 4 | Exibir relatório detalhado de todas as contas |
| 0 | Sair |

## Estrutura do projeto

```
minibanco-dotnet/
├── src/
│   └── MiniBanco.ConsoleApp/
│       ├── Program.cs                  # Menu principal e entrada/saída de dados
│       ├── Models/
│       │   ├── Transacao.cs            # Deposito | Saque | Tarifa (valida tipo e valor)
│       │   ├── Conta.cs                # Classe base abstrata
│       │   ├── ContaCorrente.cs
│       │   ├── ContaPoupanca.cs
│       │   └── ContaSalario.cs
│       ├── Exceptions/
│       │   └── SaldoInsuficienteException.cs
│       ├── Services/
│       │   └── MotorAnalise.cs         # Pontuação, índice de saúde, classificação e relatório
│       └── MiniBanco.ConsoleApp.csproj
└── README.md
```

## Regras de negócio

**Contas**

- **Conta Corrente:** permite sacar até `saldo + limite do cheque especial`. No fechamento mensal cobra tarifa de manutenção de **R$ 10,00**.
- **Conta Poupança:** no fechamento mensal registra um depósito de rendimento = `saldo × taxa / 100`.
- **Conta Salário:** possui limite de saques mensais. O saque que ultrapassa o limite gera também uma tarifa de **R$ 5,00**. O fechamento mensal zera a contagem de saques (sem tarifa de manutenção).
- **Saldo** só pode ser alterado pela própria conta e suas subclasses (`protected set`).

**Pontuação por transação**

| Tipo | Pontos |
|------|--------|
| Deposito | 10 |
| Saque | 5 |
| Tarifa | 0 |

**Índice de saúde** = `PontuaçãoObtida / (TotalDeTransações × 10) × 100`. Conta sem transações retorna 0% (sem divisão por zero).

**Classificação**

| Índice | Classificação | Ação corporativa |
|--------|---------------|------------------|
| 90% a 100% | Conta Saudável | Oferecer investimentos e avaliar aumento de limite |
| 60% a 89% | Conta em Atenção | Contato da gerência para orientação financeira e revisão de tarifas |
| 0% a 59% | Conta em Risco | Bloquear aumento de limite e abrir análise de crédito |

**Relatório:**  
itens críticos = tarifas; 
itens de atenção = saques; seguido de recomendações em texto para a gerência.

**Tratamento de exceções:** `SaldoInsuficienteException`, `ArgumentException` e `FormatException` são tratadas no menu;
o programa exibe uma mensagem clara e volta ao menu, sem encerrar.

**Restrição técnica:** nenhuma busca/filtragem/cálculo usa LINQ — apenas `for`, `foreach` e `if`.

## Exemplo rápido para testar

1. Opção `1` → Conta Corrente: `Ana Souza`, nº `1001`, saldo `1000`, limite `500`
2. Opção `2` → conta `1001`: depósito `2000` ("Salário"), depósito `300`, saque `800`, saque `2800`
3. Opção `3` (fechamento mensal) e depois `4` (relatório) → índice de **60,0%** — _Conta em Atenção_

## Limitações conhecidas

- Os dados ficam apenas em memória (não há persistência em arquivo/banco); ao fechar o programa, as contas são perdidas.
- O fechamento mensal não verifica o `DiaAniversario` da poupança: é aplicado quando o usuário escolhe a opção 3.
- Todos os saques são listados como itens de atenção, sem distinguir se usaram o cheque especial.
- Sem testes automatizados.
