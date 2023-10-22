using AltV.Net;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;

namespace Roleplay.Scripts
{
    public class BankScript : IScript
    {
        [AsyncClientEvent(nameof(ATMUse))]
        public async Task ATMUse(MyPlayer player, bool success) => await player.ShowBank(true, success, false);

        [AsyncClientEvent(nameof(BankDeposit))]
        public async Task BankDeposit(MyPlayer player, int value)
        {
            if (value <= 0)
            {
                player.SendMessage(MessageType.Error, "Valor inválido.", notify: true);
                return;
            }

            if (player.Money < value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, value), notify: true);
                return;
            }

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = value
            });

            player.Character.Bank += value;

            player.SendMessage(MessageType.Success, $"Você depositou ${value:N0}.", notify: true);
            await player.GravarLog(LogType.Dinheiro, $"Depositar {value}", null);

            await using var context = new DatabaseContext();
            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Deposit,
                CharacterId = player.Character.Id,
                Value = value,
                Description = "Depósito"
            });
            await context.SaveChangesAsync();
            await player.ShowBank(true, true, true);
        }

        [AsyncClientEvent(nameof(BankWithdraw))]
        public async Task BankWithdraw(MyPlayer player, int value)
        {
            if (value <= 0)
            {
                player.SendMessage(MessageType.Error, "Valor inválido.", notify: true);
                return;
            }

            if (player.Character.Bank < value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.MENSAGEM_ERRO_SALDO_INSUFICIENTE, value), notify: true);
                return;
            }

            var res = await player.GiveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = value
            });

            if (!string.IsNullOrWhiteSpace(res))
            {
                player.SendMessage(MessageType.Error, res);
                return;
            }

            player.Character.Bank -= value;

            player.SendMessage(MessageType.Success, $"Você sacou ${value:N0}.", notify: true);
            await player.GravarLog(LogType.Dinheiro, $"Sacar {value}", null);

            await using var context = new DatabaseContext();
            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Withdraw,
                CharacterId = player.Character.Id,
                Value = value,
                Description = "Saque",
            });
            await context.SaveChangesAsync();
            await player.ShowBank(true, true, true);
        }

        [AsyncClientEvent(nameof(BankTransfer))]
        public async Task BankTransfer(MyPlayer player, int targetId, int value, string description, bool confirm)
        {
            if (value <= 0)
            {
                player.SendMessage(MessageType.Error, "Valor inválido.", notify: true);
                return;
            }

            if (player.Character.Bank < value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.MENSAGEM_ERRO_SALDO_INSUFICIENTE, value), notify: true);
                return;
            }

            await using var context = new DatabaseContext();
            var target = await context.Characters.FirstOrDefaultAsync(x => x.Id == targetId);
            if (target == null)
            {
                player.SendMessage(MessageType.Error, $"Nenhum conta bancária foi encontrada com o código {targetId}.", notify: true);
                return;
            }

            if (player.User.Id == target.UserId)
            {
                player.SendMessage(MessageType.Error, "Você não pode fazer uma transferência para outro personagem que seja seu.", notify: true);
                return;
            }

            if (!confirm)
            {
                player.Emit("BankTransferConfirm", targetId, value, description, target.Name);
                return;
            }

            var playerTarget = Global.Players.FirstOrDefault(x => x.Character.Id == target.Id);
            if (playerTarget != null)
            {
                playerTarget.Character.Bank += value;
                playerTarget.SendMessage(MessageType.Success, $"{player.Character.Name} transferiu ${value:N0} para sua conta bancária.");
            }
            else
            {
                target.Bank += value;
                context.Characters.Update(target);
            }

            player.Character.Bank -= value;

            player.SendMessage(MessageType.Success, $"Você transferiu ${value:N0} para {target.Name}.", notify: true);
            await player.GravarLog(LogType.Dinheiro, $"Transferir {value} {target.Id}", playerTarget);

            if (!string.IsNullOrWhiteSpace(description))
                description = $" ({description})";

            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Withdraw,
                CharacterId = player.Character.Id,
                Value = value,
                Description = $"Transferência para {target.Name}{description}",
            });

            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Deposit,
                CharacterId = target.Id,
                Value = value,
                Description = $"Transferência de {player.Character.Name}{description}",
            });

            await context.SaveChangesAsync();
            await player.ShowBank(true, true, true);
        }

        [AsyncClientEvent(nameof(BankSavingsDeposit))]
        public async Task BankSavingsDeposit(MyPlayer player)
        {
            var poupanca = 50000;
            if (player.Character.Bank < poupanca)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.MENSAGEM_ERRO_SALDO_INSUFICIENTE, poupanca), notify: true);
                return;
            }

            if (player.Character.Savings > 0)
            {
                player.SendMessage(MessageType.Error, "Você já possui dinheiro na poupança.", notify: true);
                return;
            }

            player.Character.Bank -= poupanca;
            player.Character.Savings = poupanca;

            await using var context = new DatabaseContext();
            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Withdraw,
                CharacterId = player.Character.Id,
                Value = poupanca,
                Description = "Retirada para Poupança",
            });

            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Deposit,
                CharacterId = player.Character.Id,
                Value = poupanca,
                Description = "Depósito na Poupança",
            });
            await context.SaveChangesAsync();

            player.SendMessage(MessageType.Success, $"Você depositou ${poupanca:N0} na poupança.", notify: true);
            await player.GravarLog(LogType.Dinheiro, $"Depositar Poupança {poupanca}", null);
            await player.ShowBank(true, true, true);
        }

        [AsyncClientEvent(nameof(BankSavingsWithdraw))]
        public async Task BankSavingsWithdraw(MyPlayer player)
        {
            if (player.Character.Savings == 0)
            {
                player.SendMessage(MessageType.Error, "Você não possui dinheiro na poupança.");
                return;
            }

            player.Character.Bank += player.Character.Savings;

            await using var context = new DatabaseContext();
            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Withdraw,
                CharacterId = player.Character.Id,
                Value = player.Character.Savings,
                Description = "Retirada da Poupança",
            });

            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Deposit,
                CharacterId = player.Character.Id,
                Value = player.Character.Savings,
                Description = "Depósito pela Poupança",
            });
            await context.SaveChangesAsync();

            player.SendMessage(MessageType.Success, $"Você sacou ${player.Character.Savings:N0} da poupança.");
            await player.GravarLog(LogType.Dinheiro, $"Sacar Poupança {player.Character.Savings}", null);
            player.Character.Savings = 0;
            await player.ShowBank(true, true, true);
        }

        [AsyncClientEvent(nameof(BankPoliceTicketPayment))]
        public async Task BankPoliceTicketPayment(MyPlayer player, int id)
        {
            await using var context = new DatabaseContext();
            var multa = await context.Fines.FirstOrDefaultAsync(x => x.Id == id);

            if (player.Character.Bank < multa.Value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.MENSAGEM_ERRO_SALDO_INSUFICIENTE, multa.Value), notify: true);
                return;
            }

            player.Character.Bank -= multa.Value;

            multa.PaymentDate = DateTime.Now;
            context.Fines.Update(multa);

            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Withdraw,
                CharacterId = player.Character.Id,
                Value = multa.Value,
                Description = $"Pagamento da Multa #{multa.Id}",
            });

            await context.SaveChangesAsync();

            player.SendMessage(MessageType.Success, $"Você pagou a multa {id}.", notify: true);
            await player.ShowBank(true, true, true);
        }
    }
}