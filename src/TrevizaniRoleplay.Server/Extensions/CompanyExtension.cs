using AltV.Net;
using AltV.Net.Data;
using Discord;
using Discord.WebSocket;
using System.Drawing;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class CompanyExtension
    {
        public static bool GetIsOpen(this Company company)
        {
            var blip = Global.MyBlips.FirstOrDefault(x => x.CompanyId == company.Id);
            return blip != null;
        }

        public static void CreateIdentifier(this Company company)
        {
            RemoveIdentifier(company);

            if (!company.CharacterId.HasValue)
            {
                var pos = new Position(company.PosX, company.PosY, company.PosZ - 0.95f);

                // TODO: Rollback commentary when alt:V implements
                //var marker = (MyMarker)Alt.CreateMarker(MarkerType.MarkerHalo, pos, Global.MainRgba);
                //marker.Scale = new Vector3(1, 1, 1.5f);
                //marker.CompanyId = company.Id;

                var colShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 1, 3);
                colShape.Description = $"[{company.Name}] {{#FFFFFF}}Use /alugarempresa para alugar por ${company.WeekRentValue:N0} semanalmente.";
                colShape.CompanyId = company.Id;
            }
        }

        public static void RemoveIdentifier(this Company company)
        {
            var marker = Global.Markers.FirstOrDefault(x => x.CompanyId == company.Id);
            marker?.Destroy();

            var colShape = Global.ColShapes.FirstOrDefault(x => x.CompanyId == company.Id);
            colShape?.Destroy();

            var blip = Global.MyBlips.FirstOrDefault(x => x.CompanyId == company.Id);
            blip?.Destroy();
        }

        public static async Task RemoveOwner(this Company company)
        {
            company.ResetOwner();
            await using var context = new DatabaseContext();
            context.Companies.Update(company);
            await context.SaveChangesAsync();

            if (company.Characters!.Count != 0)
            {
                context.CompaniesCharacters.RemoveRange(company.Characters);
                await context.SaveChangesAsync();
                company.Characters.Clear();
            }

            CreateIdentifier(company);
        }

        public static void ToggleOpen(this Company company)
        {
            var blip = Global.MyBlips.FirstOrDefault(x => x.CompanyId == company.Id);

            if (blip == null)
            {
                blip = (MyBlip)Alt.CreateBlip(true, 4, new Position(company.PosX, company.PosY, company.PosZ), Array.Empty<MyPlayer>());
                blip.Sprite = company.BlipType;
                blip.Name = company.Name;
                blip.Color = company.BlipColor;
                blip.ShortRange = true;
                blip.ScaleXY = new Vector2(0.8f, 0.8f);
                blip.Display = 2;
                blip.CompanyId = company.Id;
            }
            else
            {
                blip?.Destroy();
            }
        }

        public static async Task Announce(this Company company, MyPlayer player, string message)
        {
            message = Functions.CheckFinalDot(message);
            foreach (var x in Global.SpawnedPlayers)
            {
                x.SendMessage(Models.MessageType.None, $"[{company.Name}] {{#FFFFFF}}{message}", $"#{company.Color}");

                if (x.User.Staff != UserStaff.None)
                    x.SendMessage(Models.MessageType.None, $"{player.Character.Name} [{player.SessionId}] ({player.User.Name}) enviou o anúncio da empresa.", Global.STAFF_COLOR);
            }

            await player.GravarLog(LogType.CompanyAdvertisement, $"{company.Id} | {message}", null);

            if (Global.DiscordClient == null
                || Global.DiscordClient.GetChannel(Global.CompanyAnnouncementDiscordChannel) is not SocketTextChannel channel)
                return;

            var cor = ColorTranslator.FromHtml($"#{company.Color}");
            var embedBuilder = new EmbedBuilder
            {
                Title = company.Name,
                Description = message,
                Color = new Discord.Color(cor.R, cor.G, cor.B),
            };
            embedBuilder.WithFooter($"Enviado em {DateTime.Now}.");

            await channel.SendMessageAsync(embed: embedBuilder.Build());
        }
    }
}