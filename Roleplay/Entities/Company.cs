﻿using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Discord;
using Discord.WebSocket;
using Roleplay.Factories;
using Roleplay.Models;
using Roleplay.Streamer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Roleplay.Entities
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public int WeekRentValue { get; set; }

        public DateTime? RentPaymentDate { get; set; }

        public int? CharacterId { get; set; }

        public string Color { get; set; } = "000000";

        public ushort BlipType { get; set; }

        public byte BlipColor { get; set; }

        [NotMapped, JsonIgnore]
        public bool Open { get; set; }

        [JsonIgnore]
        public ICollection<CompanyCharacter> Characters { get; set; }

        [NotMapped, JsonIgnore]
        public Marker Marker { get; set; }

        [NotMapped, JsonIgnore]
        public MyColShape ColShape { get; set; }

        [NotMapped, JsonIgnore]
        public IBlip Blip { get; set; }

        public void CreateIdentifier()
        {
            RemoveIdentifier();

            if (!CharacterId.HasValue)
            {
                var pos = new Position(PosX, PosY, PosZ - 0.95f);

                Marker = MarkerStreamer.Create(MarkerTypes.MarkerTypeHorizontalCircleSkinny,
                        pos,
                        new Vector3(1, 1, 1.5f),
                        Global.MainRgba);
                ColShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 1, 3);
                ColShape.Description = $"[{Name}] {{#FFFFFF}}Use /alugarempresa para alugar por ${WeekRentValue:N0} semanalmente.";
            }
        }

        public void RemoveIdentifier()
        {
            Marker?.Destroy();
            Marker = null;

            ColShape?.Remove();
            ColShape = null;
        }

        public async Task RemoveOwner()
        {
            CharacterId = null;
            RentPaymentDate = null;
            await using var context = new DatabaseContext();
            context.Companies.Update(this);
            await context.SaveChangesAsync();

            if (Characters.Any())
            {
                context.CompaniesCharacters.RemoveRange(Characters);
                await context.SaveChangesAsync();
                Characters.Clear();
            }

            CreateIdentifier();
        }

        public void ToggleOpen()
        {
            Open = !Open;

            if (Open)
            {
                Blip = Alt.CreateBlip(4, new Position(PosX, PosY, PosZ));
                Blip.Sprite = BlipType;
                Blip.Name = Name;
                Blip.Color = BlipColor;
                Blip.ShortRange = true;
                Blip.ScaleXY = new Vector2(0.8f, 0.8f);
                Blip.Display = 2;
            }
            else
            {
                Blip?.Remove();
                Blip = null;
            }
        }

        public async Task Announce(MyPlayer player, string message)
        {
            message = Functions.CheckFinalDot(message);
            foreach (var x in Global.Players.Where(x => x.Character.Id > 0))
            {
                x.SendMessage(Models.MessageType.None, $"[{Name}] {{#FFFFFF}}{message}", $"#{Color}");

                if (x.User.Staff != UserStaff.None)
                    x.SendMessage(Models.MessageType.None, $"{player.Character.Name} [{player.SessionId}] ({player.User.Name}) enviou o anúncio da empresa.", Global.STAFF_COLOR);
            }

            if (!string.IsNullOrWhiteSpace(Global.DiscordBotToken))
            {
                var cor = ColorTranslator.FromHtml($"#{Color}");
                var x = new EmbedBuilder
                {
                    Title = Name,
                    Description = message,
                    Color = new Discord.Color(cor.R, cor.G, cor.B),
                };
                x.WithFooter($"Enviado em {DateTime.Now}.");

                await (Global.DiscordClient.GetChannel(Global.CompanyAnnouncementDiscordChannel) as SocketTextChannel).SendMessageAsync(embed: x.Build());
            }

            await player.GravarLog(LogType.AnuncioEmpresa, $"{Id} | {message}", null);
        }
    }
}