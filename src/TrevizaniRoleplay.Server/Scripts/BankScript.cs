using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class BankScript : IScript
    {
        [Command("banco")]
        public static async Task CMD_banco(MyPlayer player)
        {
            if (!Global.Spots.Any(x => x.Type == SpotType.Bank
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
            {
                player.SendMessage(MessageType.Error, "Você não está em um banco.");
                return;
            }

            await ShowBank(player, false, true, false);
        }

        [Command("atm")]
        public static void CMD_atm(MyPlayer player) => player.Emit("ATMCheck");

        private static async Task ShowBank(MyPlayer player, bool atm, bool success, bool update)
        {
            if (atm && !success)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma ATM.");
                return;
            }

            if (player.Character.Wound != CharacterWound.None || player.Cuffed)
            {
                player.SendMessage(MessageType.Error, "Você não pode usar uma ATM ferido ou algemado.");
                return;
            }

            await using var context = new DatabaseContext();
            var multas = (await context.Fines.Where(x => !x.PaymentDate.HasValue && x.CharacterId == player.Character.Id)
                .ToListAsync())
                .OrderBy(x => x.Date)
                .Select(x => new
                {
                    x.Id,
                    x.Reason,
                    Date = x.Date.ToString(),
                    Value = $"${x.Value:N0}",
                });

            var transactions = (await context.FinancialTransactions.Where(x => x.CharacterId == player.Character.Id)
                .ToListAsync())
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Description,
                    x.Type,
                    Value = $"${x.Value:N0}",
                    Date = x.Date.ToString(),
                });

            player.Emit("BankShow",
                update,
                atm,
                player.Character.Id, player.Character.Name,
                player.Character.Bank, player.Character.Savings,
                Functions.Serialize(multas),
                Functions.Serialize(transactions));
        }

        [AsyncClientEvent(nameof(ATMUse))]
        public async Task ATMUse(MyPlayer player, bool success) => await ShowBank(player, true, success, false);

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

            await player.RemoveStackedItem(ItemCategory.Money, value);

            player.Character.AddBank(value);

            player.SendMessage(MessageType.Success, $"Você depositou ${value:N0}.", notify: true);
            await player.GravarLog(LogType.Money, $"Depositar {value}", null);

            await using var context = new DatabaseContext();

            var financialTransaction = new FinancialTransaction();
            financialTransaction.Create(FinancialTransactionType.Deposit, player.Character.Id, value, "Depósito");
            await context.FinancialTransactions.AddAsync(financialTransaction);

            await context.SaveChangesAsync();
            await ShowBank(player, true, true, true);
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

            var characterItem = new CharacterItem();
            characterItem.Create(ItemCategory.Money, 0, value, null);
            var res = await player.GiveItem(characterItem);

            if (!string.IsNullOrWhiteSpace(res))
            {
                player.SendMessage(MessageType.Error, res);
                return;
            }

            player.Character.RemoveBank(value);

            player.SendMessage(MessageType.Success, $"Você sacou ${value:N0}.", notify: true);
            await player.GravarLog(LogType.Money, $"Sacar {value}", null);

            await using var context = new DatabaseContext();
            var financialTransaction = new FinancialTransaction();
            financialTransaction.Create(FinancialTransactionType.Withdraw, player.Character.Id, value, "Saque");
            await context.FinancialTransactions.AddAsync(financialTransaction);
            await context.SaveChangesAsync();
            await ShowBank(player, true, true, true);
        }

        [AsyncClientEvent(nameof(BankTransfer))]
        public async Task BankTransfer(MyPlayer player, string targetIdString, int value, string description, bool confirm)
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

            var targetId = new Guid(targetIdString);
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

            var playerTarget = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == target.Id);
            if (playerTarget != null)
            {
                playerTarget.Character.AddBank(value);
                playerTarget.SendMessage(MessageType.Success, $"{player.Character.Name} transferiu ${value:N0} para sua conta bancária.");
            }
            else
            {
                target.AddBank(value);
                context.Characters.Update(target);
            }

            player.Character.RemoveBank(value);

            player.SendMessage(MessageType.Success, $"Você transferiu ${value:N0} para {target.Name}.", notify: true);
            await player.GravarLog(LogType.Money, $"Transferir {value} {target.Id}", playerTarget);

            if (!string.IsNullOrWhiteSpace(description))
                description = $" ({description})";

            var financialTransactionWithdraw = new FinancialTransaction();
            financialTransactionWithdraw.Create(FinancialTransactionType.Withdraw, player.Character.Id, value, $"Transferência para {target.Name}{description}");
            await context.FinancialTransactions.AddAsync(financialTransactionWithdraw);

            var financialTransactionDeposit = new FinancialTransaction();
            financialTransactionDeposit.Create(FinancialTransactionType.Deposit, target.Id, value, $"Transferência de {player.Character.Name}{description}");
            await context.FinancialTransactions.AddAsync(financialTransactionDeposit);

            await context.SaveChangesAsync();
            await ShowBank(player, true, true, true);
        }

        [AsyncClientEvent(nameof(BankSavingsDeposit))]
        public async Task BankSavingsDeposit(MyPlayer player)
        {
            var poupanca = Global.DEFAULT_SAVINGS;
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

            player.Character.RemoveBank(poupanca);
            player.Character.SetSavings(poupanca);

            await using var context = new DatabaseContext();

            var financialTransactionWithdraw = new FinancialTransaction();
            financialTransactionWithdraw.Create(FinancialTransactionType.Withdraw, player.Character.Id, poupanca, "Retirada para Poupança");
            await context.FinancialTransactions.AddAsync(financialTransactionWithdraw);

            var financialTransactionDeposit = new FinancialTransaction();
            financialTransactionDeposit.Create(FinancialTransactionType.Deposit, player.Character.Id, poupanca, "Depósito na Poupança");
            await context.FinancialTransactions.AddAsync(financialTransactionDeposit);

            await context.SaveChangesAsync();

            player.SendMessage(MessageType.Success, $"Você depositou ${poupanca:N0} na poupança.", notify: true);
            await player.GravarLog(LogType.Money, $"Depositar Poupança {poupanca}", null);
            await ShowBank(player, true, true, true);
        }

        [AsyncClientEvent(nameof(BankSavingsWithdraw))]
        public async Task BankSavingsWithdraw(MyPlayer player)
        {
            if (player.Character.Savings == 0)
            {
                player.SendMessage(MessageType.Error, "Você não possui dinheiro na poupança.");
                return;
            }

            player.Character.AddBank(player.Character.Savings);

            await using var context = new DatabaseContext();

            var financialTransactionWithdraw = new FinancialTransaction();
            financialTransactionWithdraw.Create(FinancialTransactionType.Withdraw, player.Character.Id, player.Character.Savings, "Retirada da Poupança");
            await context.FinancialTransactions.AddAsync(financialTransactionWithdraw);

            var financialTransactionDeposit = new FinancialTransaction();
            financialTransactionDeposit.Create(FinancialTransactionType.Deposit, player.Character.Id, player.Character.Savings, "Depósito pela Poupança");
            await context.FinancialTransactions.AddAsync(financialTransactionDeposit);

            await context.SaveChangesAsync();

            player.SendMessage(MessageType.Success, $"Você sacou ${player.Character.Savings:N0} da poupança.");
            await player.GravarLog(LogType.Money, $"Sacar Poupança {player.Character.Savings}", null);
            player.Character.SetSavings(0);
            await ShowBank(player, true, true, true);
        }

        [AsyncClientEvent(nameof(BankPoliceTicketPayment))]
        public async Task BankPoliceTicketPayment(MyPlayer player, string idString)
        {
            var id = new Guid(idString);
            await using var context = new DatabaseContext();
            var fine = await context.Fines.FirstOrDefaultAsync(x => x.Id == id);
            if (fine == null)
            {
                player.SendMessage(MessageType.Error, Global.RECORD_NOT_FOUND, notify: true);
                return;
            }

            if (player.Character.Bank < fine.Value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.MENSAGEM_ERRO_SALDO_INSUFICIENTE, fine.Value), notify: true);
                return;
            }

            player.Character.RemoveBank(fine.Value);

            fine.Pay();
            context.Fines.Update(fine);

            var financialTransaction = new FinancialTransaction();
            financialTransaction.Create(FinancialTransactionType.Withdraw, player.Character.Id, fine.Value, "Pagamento de Multa");
            await context.FinancialTransactions.AddAsync(financialTransaction);

            await context.SaveChangesAsync();

            player.SendMessage(MessageType.Success, $"Você pagou a multa {id}.", notify: true);
            await ShowBank(player, true, true, true);
        }
    }
}