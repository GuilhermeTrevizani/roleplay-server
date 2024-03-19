using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Models;
using TrevizaniRoleplay.Server.Scripts;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyPlayer(ICore server, nint nativePointer, uint id) : AsyncPlayer(server, nativePointer, id)
    {
        public User User { get; set; } = default!;

        public Character Character { get; set; } = default!;

        public Personalization Personalization { get; set; } = new();

        public List<Invite> Invites { get; set; } = [];

        public string ICName => Masked ? $"Mascarado {Character.Mask}" : Character.Name;

        public bool OnDuty { get; set; }

        public List<string> IPLs { get; set; }

        public bool Cuffed { get; set; }

        public bool OnAdminDuty { get; set; }

        public List<Wound> Wounds { get; set; } = [];

        public Position ICPosition
        {
            get
            {
                if (Dimension == 0)
                    return Position;

                var prop = Global.Properties.FirstOrDefault(x => x.Number == Dimension);
                if (prop == null)
                    return Position;

                return new Position(prop.EntrancePosX, prop.EntrancePosY, prop.EntrancePosZ);
            }
        }

        public Position? SPECPosition { get; set; }

        public int SPECDimension { get; set; }

        public int? SPECId { get; set; }

        public List<string> SPECIPLs { get; set; } = [];

        public string StaffColor => User.Staff switch
        {
            UserStaff.Moderator => "#668AAA",
            UserStaff.GameAdministrator => "#40BFFF",
            UserStaff.LeadAdministrator => "#00AA00",
            UserStaff.HeadAdministrator => "#D55D28",
            UserStaff.Manager => "#CC4545",
            _ => string.Empty,
        };

        public bool Wounded => Wounds.Count > 0 || Health != MaxHealth || Character.Wound != CharacterWound.None;

        public List<Property> Properties => Global.Properties.Where(x => x.CharacterId == Character.Id).ToList();

        public bool Masked { get; set; }

        public List<Spot> CollectSpots { get; set; } = [];

        public Spot? CollectingSpot { get; set; }

        public bool VehicleAnimation { get; set; }

        public Faction? Faction => Global.Factions.FirstOrDefault(x => x.Id == Character.FactionId);

        public FactionRank? FactionRank => Global.FactionsRanks.FirstOrDefault(x => x.Id == Character.FactionRankId);

        public string RealIp => Ip.Replace("::ffff:", string.Empty);

        public List<CharacterItem> Items { get; set; } = [];

        public CancellationTokenSource? CancellationTokenSourceSetarFerido { get; set; }

        public CharacterItem? DropItem { get; set; }

        public int DropItemQuantity { get; set; }

        public InventoryShowType InventoryShowType { get; set; }

        public Guid? InventoryTargetId { get; set; }

        public Guid? InventoryRightTargetId { get; set; }

        public int Money
        {
            get
            {
                return Items.FirstOrDefault(x => x.Category == ItemCategory.Money)?.Quantity ?? 0;
            }
        }

        public Guid[] TargetConfirmation { get; set; }

        public CancellationTokenSource? CancellationTokenSourceDamaged { get; set; }

        public CancellationTokenSource? CancellationTokenSourceAceitarHospital { get; set; }

        public CancellationTokenSource? CancellationTokenSourceAcao { get; set; }

        public System.Timers.Timer? Timer { get; set; }

        public int SessionId { get; set; } = -1;

        public WalkieTalkieItem RadioCommunicatorItem { get; set; } = new();

        public uint Cellphone { get; set; }

        public CellphoneItem CellphoneItem { get; set; } = new();

        public CellphoneItemCall CellphoneCall { get; set; } = new();

        public EmergencyCallType? EmergencyCallType { get; set; }

        public System.Timers.Timer? TimerCelular { get; set; }

        public byte TimerCelularElapsedCount { get; set; }

        public CharacterJob AguardandoTipoServico { get; set; }

        public Session? LoginSession { get; set; }

        public Session? FactionDutySession { get; set; }

        public Session? AdminDutySession { get; set; }

        public List<StaffFlag> StaffFlags { get; set; } = [];

        public List<FactionFlag> FactionFlags { get; set; } = [];

        public bool IsFactionLeader => FactionRank != null && Global.FactionsRanks.Where(x => x.FactionId == Character.FactionId).Max(x => x.Position) == FactionRank?.Position;

        public System.Timers.Timer DrugTimer { get; set; }

        public int ExtraPayment { get; set; }

        public Furniture? DropFurniture { get; set; }

        public PropertyFurniture? DropPropertyFurniture { get; set; }

        public Spot? RadarSpot { get; set; }

        public List<Company> Companies => Global.Companies.Where(x => x.CharacterId == Character.Id || x.Characters!.Any(y => y.CharacterId == Character.Id)).ToList();

        public VehicleTuning VehicleTuning { get; set; }

        public CancellationTokenSource? CancellationTokenSourceTextAction { get; set; }

        public int AreaNameType { get; set; }

        public string AreaNameJSON { get; set; }

        public MyObject? GarbageBagObject { get; set; }

        public string ObterNomeContato(uint number)
        {
            if (number == Global.EMERGENCY_NUMBER)
                return "Central de Emergência";

            if (number == Global.TAXI_NUMBER)
                return "Downtown Cab Company";

            if (number == Global.MECHANIC_NUMBER)
                return "Central de Mecânicos";

            var contact = CellphoneItem.Contacts.FirstOrDefault(x => x.Number == number);
            return contact == null ? $"#{number}" : $"{contact.Name} #{number}";
        }

        public async Task<string> GiveItem(CharacterItem item) => await GiveItem([item]);

        public async Task<string> GiveItem(List<CharacterItem> itens)
        {
            if (Items.Sum(x => x.Quantity * x.GetWeight()) +
                itens.Sum(x => x.Quantity * x.GetWeight())
                > Global.PESO_MAXIMO_INVENTARIO)
                return $"Não é possível prosseguir pois os novos itens ultrapassarão o peso máximo do inventário ({Global.PESO_MAXIMO_INVENTARIO}).";

            if (Items.Count(x => x.Slot > 0) + itens.Count(x => !x.GetIsStack() || !Items.Any(y => y.Category == x.Category))
                > Global.QUANTIDADE_SLOTS_INVENTARIO)
                return $"Não é possível prosseguir pois os novos itens ultrapassarão a quantidade de slots do inventário ({Global.QUANTIDADE_SLOTS_INVENTARIO}).";

            foreach (var categoria in itens.GroupBy(x => x.Category).Select(x => x.Key))
            {
                var qtdMaximaItemCategoria = Functions.GetItemMaxQuantity(categoria);
                if (Items.Count(x => x.Category == categoria) +
                    itens.Count(x => x.Category == categoria)
                    >
                    qtdMaximaItemCategoria)
                    return $"Não é possível prosseguir pois os novos itens da categoria {categoria.GetDisplay()} ultrapassarão a quantidade máxima dela no inventário ({qtdMaximaItemCategoria}).";

                if (categoria == ItemCategory.Cloth1
                    || categoria == ItemCategory.Cloth5
                    || categoria == ItemCategory.Cloth7
                    || categoria == ItemCategory.Cloth9
                    || categoria == ItemCategory.Cloth10)
                    itens.RemoveAll(x => x.Category == categoria
                        && x.Type == 0
                        && string.IsNullOrWhiteSpace(Functions.Deserialize<ClotheAccessoryItem>(x.Extra)?.DLC));
                else if (categoria == ItemCategory.Cloth3
                    || categoria == ItemCategory.Cloth8
                    || categoria == ItemCategory.Cloth11)
                    itens.RemoveAll(x => x.Category == categoria
                         && x.Type == 15
                         && string.IsNullOrWhiteSpace(Functions.Deserialize<ClotheAccessoryItem>(x.Extra)?.DLC));
                else if (categoria == ItemCategory.Cloth4)
                    itens.RemoveAll(x => x.Category == categoria
                        && x.Type == 14
                        && string.IsNullOrWhiteSpace(Functions.Deserialize<ClotheAccessoryItem>(x.Extra)?.DLC));
                else if (categoria == ItemCategory.Cloth6)
                    itens.RemoveAll(x => x.Category == categoria
                         && x.Type == (Character.Sex == CharacterSex.Man ? 34 : 35)
                         && string.IsNullOrWhiteSpace(Functions.Deserialize<ClotheAccessoryItem>(x.Extra)?.DLC));
            }

            await using var context = new DatabaseContext();
            foreach (var item in itens)
            {
                if (item.GetIsStack())
                {
                    var it = Items.FirstOrDefault(x => x.Category == item.Category);
                    if (it != null)
                    {
                        it.Quantity += item.Quantity;
                        context.CharactersItems.Update(it);
                        continue;
                    }
                }

                item.CharacterId = Character.Id;
                item.Slot = Convert.ToInt16(Enumerable.Range(1, Global.QUANTIDADE_SLOTS_INVENTARIO)
                    .FirstOrDefault(i => !Items.Any(x => x.Slot == i)));

                await context.CharactersItems.AddAsync(item);

                Items.Add(item);
            }

            await context.SaveChangesAsync();

            ShowInventory(this, update: true);

            return string.Empty;
        }

        public void SetarIPLs()
        {
            IPLs ??= [];

            foreach (var ipl in IPLs)
                Emit("Server:RequestIpl", ipl);
        }

        public void LimparIPLs()
        {
            if (IPLs == null)
            {
                IPLs = [];
            }
            else
            {
                foreach (var ipl in IPLs)
                    Emit("Server:RemoveIpl", ipl);

                IPLs = [];
            }
        }

        public void PlayAnimation(string dic, string name, int flag, int duration = -1, bool freeze = false)
        {
            Emit("animation:Play", dic, name, flag, duration, freeze);
        }

        public void StopAnimation()
        {
            ClearTasks();
            Emit("animation:Clear");
            VehicleAnimation = false;
        }

        public void SetPosition(Position position, int dimension, bool spawn)
        {
            Dimension = dimension;
            if (spawn)
                Spawn(position, 0);
            else
                Position = position;

            foreach (var target in Global.SpawnedPlayers.Where(x => x.SPECId == SessionId))
            {
                target.LimparIPLs();
                target.IPLs = IPLs;
                target.SetarIPLs();
                target.SetPosition(new Position(position.X, position.Y, position.Z + 5), Dimension, true);
                target.Emit("SpectatePlayer", this);
            }
        }

        public async Task Unspectate()
        {
            LimparIPLs();
            IPLs = SPECIPLs;
            SetarIPLs();
            SetPosition(SPECPosition.Value, SPECDimension, true);
            SPECPosition = null;
            SPECDimension = 0;
            SPECId = null;
            SPECIPLs = [];
            Emit("UnspectatePlayer");
            await Task.Delay(1000);
            SetNametag();
            Visible = true;
            Invincible = false;
            Collision = true;
            StopAnimation();
        }

        public void Curar(bool curaSomenteSocos = false)
        {
            CancellationTokenSourceSetarFerido?.Cancel(false);
            CancellationTokenSourceSetarFerido = null;

            CancellationTokenSourceAceitarHospital?.Cancel(false);
            CancellationTokenSourceAceitarHospital = null;

            if (Character.Wound != CharacterWound.None)
            {
                StopAnimation();
                SetPosition(Position, Dimension, true);
            }

            Character.Wound = CharacterWound.None;
            SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_INJURED, 0);

            if (!OnAdminDuty)
                Invincible = false;

            if (curaSomenteSocos)
            {
                Health = 125;
            }
            else
            {
                if (Health < MaxHealth)
                    Health = MaxHealth;

                ClearBloodDamage();
                Wounds = [];
            }
        }

        public void GiveEquippedWeapon(CharacterItem item)
        {
            var extra = Functions.Deserialize<WeaponItem>(item.Extra);

            GiveWeapon(item.Type, extra.Ammo, true);
            SetWeaponTintIndex(item.Type, extra.TintIndex);

            foreach (var x in extra.Components)
                AddWeaponComponent(item.Type, x);
        }

        public async Task Spawnar()
        {
            await using var context = new DatabaseContext();
            LoginSession = new();
            LoginSession.Create(Character.Id, SessionType.Login);
            await context.Sessions.AddAsync(LoginSession);
            await context.SaveChangesAsync();

            SendMessage(MessageType.None, $"Olá {{{Global.MAIN_COLOR}}}{User.Name}{{#FFFFFF}}, que bom te ver por aqui! Seu último login foi em {{{Global.MAIN_COLOR}}}{Character.LastAccessDate}{{#FFFFFF}}.");

            if (User.VIPValidDate.HasValue)
                SendMessage(MessageType.None, $"Seu {{{Global.MAIN_COLOR}}}VIP {User.VIP.GetDisplay()}{{#FFFFFF}} {(User.VIPValidDate.Value < DateTime.Now ? "expirou" : "expira")} em {{{Global.MAIN_COLOR}}}{User.VIPValidDate.Value}{{#FFFFFF}}.");

            SendMessage(MessageType.None, $"Seu ID é {{{Global.MAIN_COLOR}}}{SessionId}{{#FFFFFF}}.");

            if ((User.VIPValidDate ?? DateTime.MinValue) < DateTime.Now)
                User.PMToggle = false;

            ToggleGameControls(true);
            SetNametag();
            Emit("nametags:Config", true);
            ConfigurarChat();
            SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_INJURED, 0);
            Invincible = false;
            Frozen = false;
            SetPosition(new Position(Character.PosX, Character.PosY, Character.PosZ), Character.Dimension, true);
            Character.LastAccessDate = DateTime.Now;
            Emit("Server:setArtificialLightsState", Global.Parameter.Blackout);
            Health = Character.Health;
            Armor = Character.Armor;
            Emit("dl:Config", User.VehicleTagToggle);

            var multas = await context.Fines.CountAsync(x => x.CharacterId == Character.Id && !x.PaymentDate.HasValue);
            if (multas > 0)
            {
                var strMultas = multas > 1 ? "s" : string.Empty;
                SendMessage(MessageType.Error, $"Você possui {multas} multa{strMultas} pendente{strMultas}.");
            }

            if (Character.DriverLicenseValidDate.HasValue &&
                (Character.DriverLicenseValidDate ?? DateTime.MinValue).Date < DateTime.Now.Date)
                SendMessage(MessageType.Error, $"Sua licença de motorista está vencida.");

            foreach (var x in Global.Spotlights)
                Emit("Spotlight:Add", x.Id, x.Position, x.Direction,
                    x.Distance, x.Brightness, x.Hardness, x.Radius, x.Falloff);

            Global.Doors.ForEach(x => x.Setup(this));

            Global.AudioSpots.ForEach(x => x.Setup(this));

            foreach (var ipl in Global.IPLs)
                Emit("Server:RequestIpl", ipl);

            var cellphoneItem = Items.FirstOrDefault(x => x.Category == ItemCategory.Cellphone && x.Slot < 0);
            if (cellphoneItem != null)
            {
                Cellphone = cellphoneItem.Type;
                CellphoneItem = Functions.Deserialize<CellphoneItem>(cellphoneItem.Extra);
                ToggleViewCellphone();
            }

            var radioItem = Items.FirstOrDefault(x => x.Category == ItemCategory.WalkieTalkie && x.Slot < 0);
            if (radioItem != null)
                RadioCommunicatorItem = Functions.Deserialize<WalkieTalkieItem>(radioItem.Extra);

            SetupDrugTimer(Character.DrugEndDate.HasValue);

            Timer = new System.Timers.Timer(60000);
            Timer.Elapsed += async (s, e) =>
            {
                try
                {
                    Alt.Log($"Player Timer {Character.Id}");
                    Character.ConnectedTime++;

                    if (OnAdminDuty)
                        User.StaffDutyTime++;

                    if (Character.ConnectedTime % 60 == 0)
                        await Paycheck(false);

                    if (Character.ConnectedTime % 5 == 0)
                        await Save();
                }
                catch (Exception ex)
                {
                    ex.Source = Character.Id.ToString();
                    Functions.GetException(ex);
                }
            };
            Timer.Start();
        }

        public void SetarFerido()
        {
            if (Character.Wound >= CharacterWound.PK)
            {
                SendMessage(MessageType.Error, Global.MENSAGEM_PK);

                if (Character.Wound == CharacterWound.CanHospitalCK)
                {
                    SendMessage(MessageType.Error, "Digite /aceitarhospital para que você receba os cuidados dos médicos e seja levado ao hospital.");
                    SendMessage(MessageType.Error, "Digite /aceitarck para aplicar CK no seu personagem. (ATENÇÃO. ESSA OPERAÇÃO É IRREVERSÍVEL!)");
                }
            }

            Invincible = true;
            Cuffed = false;

            CancellationTokenSourceAceitarHospital?.Cancel();
            CancellationTokenSourceAceitarHospital = null;

            if (Character.Wound < CharacterWound.CanHospitalCK)
            {
                CancellationTokenSourceAceitarHospital = new CancellationTokenSource();
                Task.Delay(300000, CancellationTokenSourceAceitarHospital.Token).ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        return;

                    Character.Wound = CharacterWound.CanHospitalCK;
                    SendMessage(MessageType.Error, "Digite /aceitarhospital para que você receba os cuidados dos médicos e seja levado ao hospital.");
                    SendMessage(MessageType.Error, "Digite /aceitarck para aplicar CK no seu personagem. (ATENÇÃO. ESSA OPERAÇÃO É IRREVERSÍVEL!)");
                    CancellationTokenSourceAceitarHospital = null;
                });
            }

            CancellationTokenSourceSetarFerido?.Cancel();
            CancellationTokenSourceSetarFerido = null;

            if (Character.Wound <= CharacterWound.SeriouslyInjuredInvincible)
            {
                Character.Wound = CharacterWound.SeriouslyInjuredInvincible;
                StopAnimation();
                SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_INJURED, (int)Character.Wound);
                SendMessage(MessageType.Error, "Você foi gravemente ferido. Você deverá ser socorrido em até 5 minutos ou você sofrerá um PK.");

                CancellationTokenSourceSetarFerido = new CancellationTokenSource();
                Task.Delay(5000, CancellationTokenSourceSetarFerido.Token).ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        return;

                    Character.Wound = CharacterWound.SeriouslyInjured;
                    CancellationTokenSourceSetarFerido = null;
                });
            }
            else
            {
                StopAnimation();
                SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_INJURED, (int)Character.Wound);
            }
        }

        public void SetNametag() => SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_NAMETAG,
            !SPECPosition.HasValue ? $"{(OnAdminDuty ? $"~q~{User?.Name}" : ICName)} [{SessionId}]" : string.Empty);

        public void ToggleGameControls(bool enabled) => Emit("Server:ToggleGameControls", enabled);

        public async Task Save()
        {
            if (Character.PersonalizationStep != CharacterPersonalizationStep.Ready)
                return;

            Character.Model = Model;
            Character.PosX = Position.X;
            Character.PosY = Position.Y;
            Character.PosZ = Position.Z;
            Character.Health = Health;
            Character.Armor = Armor;
            Character.Dimension = Dimension;
            Character.LastAccessDate = User.LastAccessDate;
            Character.IPLsJSON = Functions.Serialize(IPLs);
            Character.PersonalizationJSON = Functions.Serialize(Personalization);
            Character.WoundsJSON = Functions.Serialize(Wounds);
            Character.FactionFlagsJSON = Functions.Serialize(FactionFlags);
            await using var context = new DatabaseContext();
            context.Characters.Update(Character);
            await context.SaveChangesAsync();

            context.CharactersItems.UpdateRange(Items);
            await context.SaveChangesAsync();

            User.LastAccessDate = DateTime.Now;
            await using var context2 = new DatabaseContext();
            context2.Users.Update(User);
            await context2.SaveChangesAsync();
        }

        public void SendMessage(MessageType tipoMensagem, string mensagem, string cor = "#FFFFFF", bool notify = false)
        {
            var type = "info";

            if (tipoMensagem == MessageType.Success)
            {
                cor = Global.SUCCESS_COLOR;
                type = "success";
            }
            else if (tipoMensagem == MessageType.Error)
            {
                cor = Global.ERROR_COLOR;
                type = "danger";
            }
            else if (tipoMensagem == MessageType.Title)
            {
                cor = "#B0B0B0";
            }

            if (notify)
            {
                Emit("chat:notify", mensagem, type);
                return;
            }

            var matches = new Regex("{#.*?}").Matches(mensagem).ToList();

            foreach (Match x in matches)
                mensagem = mensagem.Replace(x.Value, $"{(matches.IndexOf(x) != 0 ? "</span>" : string.Empty)}<span style='color:{x.Value.Replace("{", string.Empty).Replace("}", string.Empty)}'>");

            if (matches.Count > 0)
                mensagem += "</span>";

            Emit("chat:sendMessage", mensagem, cor);

            foreach (var target in Global.SpawnedPlayers.Where(x => x.SPECId == SessionId))
                target.Emit("chat:sendMessage", $"[SPEC] {mensagem}", cor);
        }

        public MyPlayer? ObterPersonagemPoridOrName(string idOrName, bool isPodeProprioPlayer = true)
        {
            if (int.TryParse(idOrName, out int id))
            {
                var p = Global.SpawnedPlayers.FirstOrDefault(x => x.SessionId == id);
                if (p != null)
                {
                    if (!isPodeProprioPlayer && this == p)
                    {
                        SendMessage(MessageType.Error, $"O jogador não pode ser você.");
                        return null;
                    }

                    return p;
                }
            }

            var ps = Global.SpawnedPlayers.Where(x => x.Character.Name.ToLower().Contains(idOrName.ToLower())).ToList();
            if (ps.Count == 1)
            {
                if (!isPodeProprioPlayer && this == ps.FirstOrDefault())
                {
                    SendMessage(MessageType.Error, $"O jogador não pode ser você.");
                    return null;
                }

                return ps.FirstOrDefault();
            }

            if (ps.Count > 0)
            {
                SendMessage(MessageType.Error, $"Mais de um jogador foi encontrado com a pesquisa: {idOrName}");
                foreach (var pl in ps)
                    SendMessage(MessageType.None, $"[ID: {pl.SessionId}] {pl.Character.Name}");
            }
            else
            {
                SendMessage(MessageType.Error, $"Nenhum jogador foi encontrado com a pesquisa: {idOrName}");
            }

            return null;
        }

        public bool SendMessageToNearbyPlayers(string message, MessageCategory type, float range, bool excludePlayer = false)
        {
            if (Character.Wound != CharacterWound.None
                && type != MessageCategory.Do && type != MessageCategory.Ado
                && type != MessageCategory.Me && type != MessageCategory.Ame
                && type != MessageCategory.ChatOOC)
            {
                SendMessage(MessageType.Error, Global.MENSAGEM_GRAVEMENTE_FERIDO);
                return false;
            }

            if (type != MessageCategory.ChatOOC)
                message = Functions.CheckFinalDot(message);

            if (type == MessageCategory.Ame || type == MessageCategory.Ado)
            {
                SendMessage(MessageType.None, type == MessageCategory.Ame ? $"> {ICName} {message}" : $"> {message} (( {ICName} ))", "#C2A2DA");

                CancellationTokenSourceTextAction?.Cancel();
                CancellationTokenSourceTextAction = new CancellationTokenSource();

                SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_TEXT_ACTION, type == MessageCategory.Ame ? $"* {ICName} {message}" : $"* {message} (( {ICName} ))");

                Task.Delay(7000, CancellationTokenSourceTextAction.Token).ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        return;

                    DeleteStreamSyncedMetaData(Constants.PLAYER_META_DATA_TEXT_ACTION);
                    CancellationTokenSourceTextAction = null;
                });
            }

            var players = Global.SpawnedPlayers.Where(x => x.Dimension == Dimension).Select(x => new
            {
                Player = x,
                Distance = Position.Distance(x.Position),
            }).Where(x => x.Distance <= range).ToList();
            if (type == MessageCategory.ChatICGrito)
            {
                if (Dimension == 0)
                {
                    var propriedade = Global.Properties
                        .Where(x => Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                        .MinBy(x => Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));
                    if (propriedade != null)
                    {
                        var pos = new Position(propriedade.ExitPosX, propriedade.ExitPosY, propriedade.ExitPosZ);
                        players.AddRange(Global.SpawnedPlayers.Where(x => x.Dimension == propriedade.Number).Select(x => new
                        {
                            Player = x,
                            Distance = pos.Distance(x.Position),
                        }).Where(x => x.Distance <= range).ToList());
                    }
                }
                else
                {
                    var propriedade = Global.Properties.FirstOrDefault(x => x.Number == Dimension);
                    if (propriedade != null)
                    {
                        var pos = new Position(propriedade.EntrancePosX, propriedade.EntrancePosY, propriedade.EntrancePosZ);
                        players.AddRange(Global.SpawnedPlayers.Select(x => new
                        {
                            Player = x,
                            Distance = pos.Distance(x.Position),
                        }).Where(x => x.Distance <= range / 2).ToList());
                    }
                }
            }

            players = players.Distinct().ToList();

            var distanceGap = range / 5;
            foreach (var target in players)
            {
                if (excludePlayer && this == target.Player)
                    continue;

                var chatMessageColor = "#6E6E6E";

                if (target.Distance < distanceGap)
                    chatMessageColor = "#E6E6E6";
                else if (target.Distance < distanceGap * 2)
                    chatMessageColor = "#C8C8C8";
                else if (target.Distance < distanceGap * 3)
                    chatMessageColor = "#AAAAAA";
                else if (target.Distance < distanceGap * 4)
                    chatMessageColor = "#8C8C8C";

                switch (type)
                {
                    case MessageCategory.ChatICNormal:
                        target.Player.SendMessage(MessageType.None, $"{ICName} diz: {message}", chatMessageColor);
                        break;
                    case MessageCategory.ChatICGrito:
                        target.Player.SendMessage(MessageType.None, $"{ICName} grita: {message}", chatMessageColor);
                        break;
                    case MessageCategory.Me:
                        target.Player.SendMessage(MessageType.None, $"* {ICName} {message}", "#C2A2DA");
                        break;
                    case MessageCategory.Do:
                        target.Player.SendMessage(MessageType.None, $"* {message} (( {ICName} ))", "#C2A2DA");
                        break;
                    case MessageCategory.ChatOOC:
                        var cor = OnAdminDuty && !string.IsNullOrWhiteSpace(StaffColor) ? StaffColor : "#BABABA";
                        var nome = OnAdminDuty ? User.Name : ICName;
                        target.Player.SendMessage(MessageType.None, $"(( {{{cor}}}{nome} [{SessionId}]{{#BABABA}}: {message} ))", "#BABABA");
                        break;
                    case MessageCategory.ChatICBaixo:
                        target.Player.SendMessage(MessageType.None, $"{ICName} diz [baixo]: {message}", chatMessageColor);
                        break;
                    case MessageCategory.Megafone:
                        target.Player.SendMessage(MessageType.None, $"{ICName} diz [megafone]: {message}", "#F2FF43");
                        break;
                    case MessageCategory.Celular:
                        target.Player.SendMessage(MessageType.None, $"{ICName} [celular]: {message}", chatMessageColor);
                        break;
                    case MessageCategory.Radio:
                        target.Player.SendMessage(MessageType.None, $"{ICName} [rádio]: {message}", chatMessageColor);
                        break;
                    case MessageCategory.DadosMoeda:
                        target.Player.SendMessage(MessageType.None, message, "#C2A2DA");
                        break;
                    case MessageCategory.Microphone:
                        target.Player.SendMessage(MessageType.None, $"{ICName} diz [microfone]: {message}", "#F2FF43");
                        break;
                }
            }

            return true;
        }

        public void SendFactionMessage(string mensagem)
        {
            foreach (var x in Global.SpawnedPlayers.Where(x => x.Character.FactionId == Character.FactionId && !x.User.FactionToggle))
                x.SendMessage(MessageType.None, mensagem, $"#{Faction.Color}");
        }

        public bool CheckAnimations(bool stopAnim = false, bool onlyInVehicle = false)
        {
            if (!stopAnim)
            {
                if (onlyInVehicle)
                {
                    if (!IsInVehicle)
                    {
                        SendMessage(MessageType.Error, "Você precisa estar dentro de um veículo.");
                        return false;
                    }
                }
                else
                {
                    if (IsInVehicle)
                    {
                        SendMessage(MessageType.Error, "Você não pode utilizar comandos de animação em um veículo.");
                        return false;
                    }
                }
            }

            if (Cuffed)
            {
                SendMessage(MessageType.Error, "Você não pode utilizar comandos de animação algemado.");
                return false;
            }

            if (Character.Wound != CharacterWound.None)
            {
                SendMessage(MessageType.Error, "Você não pode utilizar comandos de animação ferido.");
                return false;
            }

            if (stopAnim)
                StopAnimation();

            if (onlyInVehicle)
                VehicleAnimation = true;

            return true;
        }

        public void SendRadioMessage(int slot, string message)
        {
            if (Character.Wound != CharacterWound.None)
            {
                SendMessage(MessageType.Error, Global.MENSAGEM_GRAVEMENTE_FERIDO);
                return;
            }

            var canal = RadioCommunicatorItem.Channel1;
            if (slot == 2)
                canal = RadioCommunicatorItem.Channel2;
            else if (slot == 3)
                canal = RadioCommunicatorItem.Channel3;
            else if (slot == 4)
                canal = RadioCommunicatorItem.Channel4;
            else if (slot == 5)
                canal = RadioCommunicatorItem.Channel5;

            if (canal == 0)
            {
                SendMessage(MessageType.Error, $"Seu slot {slot} do rádio não possui um canal configurado.");
                return;
            }

            if (((canal >= 911 && canal <= 950) || canal == 999) && !OnDuty)
            {
                SendMessage(MessageType.Error, $"Você só pode falar no canal {canal} quando estiver em serviço.");
                return;
            }

            message = Functions.CheckFinalDot(message);
            Functions.SendRadioMessage(canal, $"{Character.Name}: {message}");

            SendMessageToNearbyPlayers(message, MessageCategory.Radio, Dimension > 0 ? 7.5f : 10.0f, true);
        }

        public async Task GravarLog(LogType type, string description, MyPlayer? target)
        {
            await using var context = new DatabaseContext();
            var log = new Log();
            log.Create(type, description,
                Character?.Id, RealIp, HardwareIdHash, HardwareIdHash,
                target?.Character?.Id, target?.RealIp, target?.HardwareIdExHash ?? 0, target?.HardwareIdExHash ?? 0);
            await context.Logs.AddAsync(log);
            await context.SaveChangesAsync();
        }

        public void ConfigurarChat() => Emit("chat:configure", User.TimeStampToggle, User.ChatFontType, User.ChatFontSize, User.ChatLines);

        public void LimparChat() => Emit("chat:clearMessages");

        public async Task Disconnect(string reason, bool real)
        {
            try
            {
                if (Character.PersonalizationStep != CharacterPersonalizationStep.Ready)
                    return;

                Timer?.Stop();
                Timer = null;

                await using var context = new DatabaseContext();
                LoginSession!.End();
                context.Sessions.Update(LoginSession);
                await context.SaveChangesAsync();

                if (FactionDutySession != null)
                {
                    FactionDutySession.End();
                    context.Sessions.Update(FactionDutySession);
                    await context.SaveChangesAsync();
                }

                if (AdminDutySession != null)
                {
                    AdminDutySession.End();
                    context.Sessions.Update(AdminDutySession);
                    await context.SaveChangesAsync();
                }

                LoginSession = FactionDutySession = AdminDutySession = null;

                RadarSpot?.RemoveIdentifier();

                CancellationTokenSourceAcao?.Cancel();
                CancellationTokenSourceAcao = null;

                CancellationTokenSourceSetarFerido?.Cancel();
                CancellationTokenSourceSetarFerido = null;

                CancellationTokenSourceDamaged?.Cancel();
                CancellationTokenSourceDamaged = null;

                CancellationTokenSourceAceitarHospital?.Cancel();
                CancellationTokenSourceAceitarHospital = null;

                CancellationTokenSourceTextAction?.Cancel();
                CancellationTokenSourceTextAction = null;

                foreach (var collectSpot in CollectSpots)
                    collectSpot.RemoveIdentifier();

                if (CellphoneCall.Number > 0)
                    await EndCellphoneCall();

                foreach (var x in Global.SpawnedPlayers.Where(x => x.Dimension == Dimension && Position.Distance(x.Position) <= 20))
                    x.SendMessage(MessageType.Error, $"(( {ICName} [{SessionId}] saiu do servidor. ))");

                foreach (var x in Global.SpawnedPlayers.Where(x => x.InventoryTargetId == Character.Id))
                    x.CloseInventory();

                if (SPECPosition.HasValue)
                    await Unspectate();

                foreach (var x in Global.SpawnedPlayers.Where(x => x.SPECId == SessionId))
                    await x.Unspectate();

                await GravarLog(LogType.Exit, reason, null);
                await Save();

                if (!real)
                {
                    DeleteStreamSyncedMetaData(Constants.PLAYER_META_DATA_NAMETAG);
                    DeleteStreamSyncedMetaData(Constants.PLAYER_META_DATA_GAME_UNFOCUSED);
                    LimparChat();
                    ClearDrugEffect();
                    StopAnimation();
                    Global.AudioSpots.ForEach(x => x.Remove(this));

                    Character = new();
                    Personalization = new();
                    Invites = [];
                    CellphoneItem = new();
                    OnDuty = Masked = Cuffed = OnAdminDuty = VehicleAnimation = false;
                    AguardandoTipoServico = CharacterJob.None;
                    Cellphone = 0;
                    IPLs = [];
                    Wounds = [];
                    CollectSpots = [];
                    CollectingSpot = new();
                    Items = [];
                    DropFurniture = null;
                    DropPropertyFurniture = null;
                    RadarSpot = null;
                    RadioCommunicatorItem = new();

                    ToggleViewCellphone();
                }
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        public async Task ListarPersonagens(string reason, string alerta)
        {
            await Disconnect(reason, false);

            Despawn();

            Emit("Server:DisableHUD");
            await LoginScript.ListarPersonagens(this, alerta);
        }

        public async Task<string> ObterHTMLStats()
        {
            var html = string.Empty;

            if (HasStreamSyncedMetaData(Constants.PLAYER_META_DATA_GAME_UNFOCUSED))
            {
                GetStreamSyncedMetaData(Constants.PLAYER_META_DATA_GAME_UNFOCUSED, out string dataStr);
                if (DateTime.TryParse(dataStr, out DateTime data))
                {
                    var ts = DateTime.Now - data;
                    html += $"<strong style='color:red;'>Tempo AFK: {ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}</strong><br/><br/>";
                }
            }

            html += $@"OOC: <strong>{User.Name} [{User.Id}]</strong> | Registro: <strong>{Character.RegisterDate}</strong> | VIP: <strong>{User.VIP.GetDisplay()} {(User.VIPValidDate.HasValue ? $"- {(User.VIPValidDate < DateTime.Now ? "Expirado" : "Expira")} em {User.VIPValidDate}" : string.Empty)}</strong><br/>
            Tempo Conectado (minutos): <strong>{Character.ConnectedTime}</strong> | Emprego: <strong>{Character.Job.GetDisplay()}</strong> | Trocas de Nome: <strong>{User.NameChanges} {(Character.NameChangeStatus == CharacterNameChangeStatus.Blocked ? "(BLOQUEADO)" : string.Empty)}</strong> | Trocas de Nome Fórum: <strong>{User.ForumNameChanges}</strong> | Trocas de Placa: <strong>{User.PlateChanges}</strong><br/>
            Banco: <strong>${Character.Bank:N0}</strong> | Poupança: <strong>${Character.Savings:N0}</strong><br/>
            Skin: <strong>{(PedModel)Model}</strong> | Vida: <strong>{(Health > 100 ? Health - 100 : Health)}</strong> | Colete: <strong>{Armor}</strong><br/>
            Usando Droga: <strong>{(Character.DrugItemCategory.HasValue ? Character.DrugItemCategory.GetDisplay() : "N/A")}</strong> | Fim Efeitos Droga: <strong>{(Character.DrugEndDate.HasValue ? Character.DrugEndDate.ToString() : "N/A")}</strong> | Limiar da Morte: <strong>{Character.ThresoldDeath}/100</strong> | Reset Limiar da Morte: <strong>{(Character.ThresoldDeathEndDate.HasValue ? Character.ThresoldDeathEndDate.ToString() : "N/A")}</strong><br/>";

            if (User.Staff > 0)
                html += $"Staff: <strong>{User.Staff.GetDisplay()} [{(int)User.Staff}]</strong> | Tempo Serviço Administrativo (minutos): <strong>{User.StaffDutyTime}</strong> | SOSs Atendidos: <strong>{User.HelpRequestsAnswersQuantity}</strong><br/>";

            if (Faction != null)
                html += $"Facção: <strong>{Faction.Name} [{Character.FactionId}]</strong> | Rank: <strong>{FactionRank.Name} [{Character.FactionRankId}]</strong>";

            html += $"<h4>História (aceito por {Character.EvaluatorStaffUserId})</h4> {Character.History}";

            html += await Paycheck(true);

            html += $"<h4>Inventário</h4>";
            if (Items.Count != 0)
            {
                foreach (var item in Items)
                    html += $"Código: <strong>{item.Id}</strong> | Nome: <strong>{item.GetName()}</strong> | Quantidade: <strong>{item.Quantity:N0}</strong>{(!string.IsNullOrWhiteSpace(item.Extra) ? $" | Extra: <strong>{item.GetExtra().Replace("<br/>", ", ")}</strong>" : string.Empty)}<br/>";
            }
            else
            {
                html += "Nenhum item.<br/>";
            }

            html += $"<h4>Propriedades</h4>";
            if (Properties.Count != 0)
            {
                foreach (var prop in Properties)
                    html += $"Código: <strong>{prop.Id}</strong> | Endereço: <strong>{prop.Address}</strong> | Valor: <strong>${prop.Value:N0}</strong> | Nível de Proteção: <strong>{prop.ProtectionLevel}</strong><br/>";
            }
            else
            {
                html += "Nenhuma propriedade.<br/>";
            }

            await using var context = new DatabaseContext();
            var veiculos = await context.Vehicles.Where(x => x.CharacterId == Character.Id && !x.Sold).ToListAsync();
            html += $"<h4>Veículos</h4>";
            if (veiculos.Count != 0)
            {
                foreach (var veh in veiculos)
                    html += $"Código: <strong>{veh.Id}</strong> | Modelo: <strong>{veh.Model.ToUpper()}</strong> | Placa: <strong>{veh.Plate}</strong> | Nível de Proteção: <strong>{veh.ProtectionLevel}</strong> | XMR: <strong>{(veh.XMR ? "SIM" : "NÃO")}</strong><br/>";
            }
            else
            {
                html += "Nenhum veículo.<br/>";
            }

            html += $"<h4>Empresas</h4>";
            if (Companies.Count != 0)
            {
                foreach (var company in Companies)
                    html += $"Código: <strong>{company.Id}</strong> | Nome: <strong>{company.Name}</strong> | Dono: <strong>{(company.CharacterId == Character.Id ? "SIM" : "NÃO")}</strong><br/>";
            }
            else
            {
                html += "Nenhuma empresa.<br/>";
            }

            html += $"<h4>Convites</h4>";
            if (Invites.Count != 0)
            {
                foreach (var convite in Invites)
                {
                    var ts = DateTime.Now - convite.Date;
                    html += $"Tipo: <strong>{convite.Type.GetDisplay()} ({(int)convite.Type})</strong> | Tempo Aguardando: <strong>{ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}</strong><br/>";
                }
            }
            else
            {
                html += "Nenhum convite recebido.<br/>";
            }

            return html;
        }

        public void SetCanDoDriveBy(byte seat, bool? status = null)
        {
            seat--;

            if (seat >= 4) status = true;

            Emit("SetPlayerCanDoDriveBy", status.HasValue ?
                status
                :
                !((MyVehicle)Vehicle).TemJanelas || Vehicle.IsWindowOpened(seat));
        }

        public async Task<string> Paycheck(bool previa)
        {
            var porcentagemImpostoPropriedade = 0.0015M;
            var porcentagemImpostoVeiculo = 0.001M;
            var porcentagemPoupanca = 0.001M;

            if ((User.VIPValidDate ?? DateTime.MinValue) >= DateTime.Now)
            {
                switch (User.VIP)
                {
                    case UserVIP.Bronze:
                        porcentagemImpostoPropriedade = 0.0013M;
                        porcentagemImpostoVeiculo = 0.0007M;
                        porcentagemPoupanca = 0.00125M;
                        break;
                    case UserVIP.Silver:
                        porcentagemImpostoPropriedade = 0.001M;
                        porcentagemImpostoVeiculo = 0.0005M;
                        porcentagemPoupanca = 0.00150M;
                        break;
                    case UserVIP.Gold:
                        porcentagemImpostoPropriedade = 0.0008M;
                        porcentagemImpostoVeiculo = 0.0003M;
                        porcentagemPoupanca = 0.00175M;
                        break;
                }
            }

            var valorImpostoPropriedade = 0;
            var valorImpostoVeiculo = 0;

            using var context = new DatabaseContext();
            var veiculos = await context.Vehicles.Where(x => x.CharacterId == Character.Id && !x.Sold).ToListAsync();
            if (Properties.Count > 0 || veiculos.Count > 0)
            {
                foreach (var x in Properties)
                    valorImpostoPropriedade += Convert.ToInt32(Convert.ToDecimal(x.Value) * porcentagemImpostoPropriedade);

                foreach (var x in veiculos)
                    valorImpostoVeiculo += Convert.ToInt32(Convert.ToDecimal(Global.Prices.FirstOrDefault(y => y.IsVehicle && y.Name.Equals(x.Model, StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0) * porcentagemImpostoVeiculo);
            }

            var salarioEmprego = 0;
            var salarioFaccao = 0;

            if (FactionRank?.Salary > 0)
                salarioFaccao = FactionRank.Salary;
            else if (Character.Job > 0)
                salarioEmprego = Convert.ToInt32(Math.Abs(Global.Prices.FirstOrDefault(x => x.Type == PriceType.Jobs && x.Name.Equals(Character.Job.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0));

            var poupanca = 0;
            if (Character.Savings > 0)
                poupanca = Convert.ToInt32(Convert.ToDecimal(Character.Savings) * porcentagemPoupanca);

            var salario = 0;
            salario += salarioEmprego;
            salario += salarioFaccao;
            salario += Character.ExtraPayment;
            salario *= Global.Parameter.Paycheck;
            salario -= valorImpostoPropriedade;
            salario -= valorImpostoVeiculo;

            var descricaoPrevia = string.Empty;
            if (previa)
            {
                descricaoPrevia = $"<h4>Prévia do Pagamento {(Global.Parameter.Paycheck > 1 ? $"(PAYCHECK {Global.Parameter.Paycheck}x)" : string.Empty)}</h4>";

                if (Character.FactionId.HasValue && FactionRank.Salary > 0)
                    descricaoPrevia += $"Salário {Faction.Name}: <strong>+ ${FactionRank.Salary:N0}</strong><br/>";

                if (salarioEmprego > 0)
                    descricaoPrevia += $"Salário Emprego: <strong>+ ${salarioEmprego:N0}</strong><br/>";

                if (Character.ExtraPayment > 0)
                    descricaoPrevia += $"Extra Emprego: <strong>+ ${Character.ExtraPayment:N0}</strong><br/>";

                if (valorImpostoPropriedade > 0)
                    descricaoPrevia += $"Imposto Propriedades: <strong>- ${valorImpostoPropriedade:N0}</strong><br/>";

                if (valorImpostoVeiculo > 0)
                    descricaoPrevia += $"Imposto Veículos: <strong>- ${valorImpostoVeiculo:N0}</strong><br/>";

                descricaoPrevia += $"Total: <strong>{(salario >= 0 ? "+" : $"-")} ${Math.Abs(salario):N0}</strong><br/>";

                if (poupanca > 0)
                    descricaoPrevia += $"Poupança: <strong>+ ${poupanca:N0} (${Character.Savings + poupanca:N0})</strong><br/>";
            }
            else
            {
                if (poupanca > 0)
                {
                    Character.Savings += poupanca;

                    var financialTransaction = new FinancialTransaction();
                    financialTransaction.Create(FinancialTransactionType.Deposit, Character.Id, poupanca, "Rendimento da Poupança");

                    await context.FinancialTransactions.AddAsync(financialTransaction);

                    if (Character.Savings > 1000000)
                    {
                        var retiradaPoupanca = Character.Savings - 50000;
                        Character.Bank += retiradaPoupanca;
                        Character.Savings = 50000;

                        var financialTransactionWithdraw = new FinancialTransaction();
                        financialTransactionWithdraw.Create(FinancialTransactionType.Withdraw, Character.Id, retiradaPoupanca, "Retirada Automática da Poupança");

                        await context.FinancialTransactions.AddAsync(financialTransactionWithdraw);

                        var financialTransactionDeposit = new FinancialTransaction();
                        financialTransactionDeposit.Create(FinancialTransactionType.Deposit, Character.Id, retiradaPoupanca, "Depósito Automático pela Poupança");

                        await context.FinancialTransactions.AddAsync(financialTransactionDeposit);
                    }
                }

                Character.Bank += salario;

                if (salario != 0)
                {
                    SendMessage(MessageType.Title, $"Pagamento de {Character.Name} {(Global.Parameter.Paycheck > 1 ? $"(PAYCHECK {Global.Parameter.Paycheck}x)" : string.Empty)}");

                    if (Character.FactionId.HasValue && FactionRank.Salary > 0)
                        SendMessage(MessageType.None, $"Salário {Faction.Name}: {{{Global.SUCCESS_COLOR}}}+ ${FactionRank.Salary:N0}");

                    if (salarioEmprego > 0)
                        SendMessage(MessageType.None, $"Salário Emprego: {{{Global.SUCCESS_COLOR}}}+ ${salarioEmprego:N0}");

                    if (Character.ExtraPayment > 0)
                        SendMessage(MessageType.None, $"Extra Emprego: {{{Global.SUCCESS_COLOR}}}+ ${Character.ExtraPayment:N0}");

                    if (valorImpostoPropriedade > 0)
                        SendMessage(MessageType.None, $"Imposto Propriedades: {{{Global.ERROR_COLOR}}}- ${valorImpostoPropriedade:N0}");

                    if (valorImpostoVeiculo > 0)
                        SendMessage(MessageType.None, $"Imposto Veículos: {{{Global.ERROR_COLOR}}}- ${valorImpostoVeiculo:N0}");

                    SendMessage(MessageType.None, $"Total: {(salario > 0 ? $"{{{Global.SUCCESS_COLOR}}} +" : $"{{{Global.ERROR_COLOR}}} -")} ${Math.Abs(salario):N0}");

                    var financialTransaction = new FinancialTransaction();
                    financialTransaction.Create(salario > 0 ? FinancialTransactionType.Deposit : FinancialTransactionType.Withdraw,
                        Character.Id, salario, "Pagamento");

                    await context.FinancialTransactions.AddAsync(financialTransaction);

                    if (poupanca > 0)
                        SendMessage(MessageType.None, $"Poupança: {{{Global.SUCCESS_COLOR}}}+ ${poupanca:N0} (${Character.Savings:N0})");

                    await context.SaveChangesAsync();
                }

                Character.ExtraPayment = 0;
            }

            return descricaoPrevia;
        }

        public void SetarPersonalizacao(Personalization personalizacaoDados)
        {
            SetHeadBlendData(personalizacaoDados.FaceFather, personalizacaoDados.FaceMother, 0,
                personalizacaoDados.SkinFather, personalizacaoDados.SkinMother, 0,
                personalizacaoDados.FaceMix, personalizacaoDados.SkinMix, 0);

            for (byte i = 0; i < personalizacaoDados.Structure.Count; i++)
                SetFaceFeature(i, personalizacaoDados.Structure[i]);

            foreach (var x in personalizacaoDados.OpacityOverlays)
                SetHeadOverlay(x.Id, x.Value, x.Opacity);

            if (!string.IsNullOrWhiteSpace(personalizacaoDados.HairDLC))
                SetDlcClothes(2, personalizacaoDados.Hair, 0, 0, Alt.Hash(personalizacaoDados.HairDLC));
            else
                SetClothes(2, personalizacaoDados.Hair, 0, 0);
            HairColor = personalizacaoDados.HairColor1;
            HairHighlightColor = personalizacaoDados.HairColor2;

            SetHeadOverlay(1, personalizacaoDados.FacialHair, personalizacaoDados.FacialHairOpacity);
            SetHeadOverlayColor(1, 1, personalizacaoDados.FacialHairColor1, personalizacaoDados.FacialHairColor1);

            SetHeadOverlay(2, personalizacaoDados.Eyebrows, 1);
            SetHeadOverlayColor(2, 1, personalizacaoDados.EyebrowsColor1, personalizacaoDados.EyebrowsColor1);

            foreach (var x in personalizacaoDados.ColorOverlays)
            {
                SetHeadOverlay(x.Id, x.Value, x.Opacity);
                SetHeadOverlayColor(x.Id, 1, x.Color1, x.Color2);
            }

            SetEyeColor(personalizacaoDados.Eyes);

            ClearDecorations();

            if (!string.IsNullOrWhiteSpace(personalizacaoDados.HairOverlay)
                && !string.IsNullOrWhiteSpace(personalizacaoDados.HairCollection))
                AddDecoration(Alt.Hash(personalizacaoDados.HairCollection), Alt.Hash(personalizacaoDados.HairOverlay));

            foreach (var tattoo in personalizacaoDados.Tattoos)
                AddDecoration(Alt.Hash(tattoo.Collection), Alt.Hash(tattoo.Overlay));
        }

        private void SetCloth(ItemCategory tipoCategoriaItem, byte component, ushort defaultDrawable)
        {
            var roupa = Items.FirstOrDefault(x => x.Category == tipoCategoriaItem && x.Slot < 0);
            if (roupa != null)
            {
                var extra = Functions.Deserialize<ClotheAccessoryItem>(roupa.Extra);
                if (!string.IsNullOrWhiteSpace(extra.DLC))
                    SetDlcClothes(component, (ushort)roupa.Type, extra.Texture, 0, Alt.Hash(extra.DLC));
                else
                    SetClothes(component, (ushort)roupa.Type, extra.Texture, 0);
            }
            else
            {
                SetClothes(component, defaultDrawable, 0, 0);
            }
        }

        private void SetAccessory(ItemCategory tipoCategoriaItem, byte component)
        {
            var roupa = Items.FirstOrDefault(x => x.Category == tipoCategoriaItem && x.Slot < 0);
            if (roupa != null)
            {
                var extra = Functions.Deserialize<ClotheAccessoryItem>(roupa.Extra);
                if (!string.IsNullOrWhiteSpace(extra.DLC))
                    SetDlcProps(component, (ushort)roupa.Type, extra.Texture, Alt.Hash(extra.DLC));
                else
                    SetProps(component, (ushort)roupa.Type, extra.Texture);
            }
            else
            {
                ClearProps(component);
            }
        }

        public async Task SetarRoupas()
        {
            SetCloth(ItemCategory.Cloth1, 1, 0);
            SetCloth(ItemCategory.Cloth3, 3, 15);
            SetCloth(ItemCategory.Cloth4, 4, 14);
            SetCloth(ItemCategory.Cloth5, 5, 0);
            SetCloth(ItemCategory.Cloth6, 6, (ushort)(Character.Sex == CharacterSex.Man ? 34 : 35));
            SetCloth(ItemCategory.Cloth7, 7, 0);
            SetCloth(ItemCategory.Cloth8, 8, 15);
            SetCloth(ItemCategory.Cloth9, 9, 0);
            SetCloth(ItemCategory.Cloth10, 10, 0);
            SetCloth(ItemCategory.Cloth11, 11, 15);

            SetAccessory(ItemCategory.Accessory0, 0);
            SetAccessory(ItemCategory.Accessory1, 1);
            SetAccessory(ItemCategory.Accessory2, 2);
            SetAccessory(ItemCategory.Accessory6, 6);
            SetAccessory(ItemCategory.Accessory7, 7);

            Masked = Items.Any(x => x.Category == ItemCategory.Cloth1 && x.Slot < 0
                && x.Type != 73 && x.Type != 120 && x.Type != 121
                && Functions.Deserialize<ClotheAccessoryItem>(x.Extra)?.DLC != "mp_m_eup"
                && Functions.Deserialize<ClotheAccessoryItem>(x.Extra)?.DLC != "mp_f_eup");
            if (Masked && Character.Mask == 0)
            {
                await using var context = new DatabaseContext();
                Character.Mask = (await context.Characters.MaxAsync(x => x.Mask)) + 1;
                context.Characters.Update(Character);
                await context.SaveChangesAsync();

                await GravarLog(LogType.Mask, Character.Mask.ToString(), null);
            }

            SetNametag();
        }

        public void ShowInventory(MyPlayer target, InventoryShowType inventoryShowType = InventoryShowType.Default,
            string rightTitle = "Chão", string rightItems = "[]",
                bool update = false, Guid? rightTargetId = null)
        {
            if (update)
            {
                foreach (var x in Global.SpawnedPlayers.Where(x =>
                    (x.InventoryTargetId == Character.Id && x.InventoryShowType < InventoryShowType.Property)
                        || (x.InventoryRightTargetId == rightTargetId && x.InventoryShowType == inventoryShowType
                            && x.InventoryShowType >= InventoryShowType.Property)))
                {
                    target = x;
                    inventoryShowType = target.InventoryShowType;
                    ConfirmShowInventory();
                }
            }
            else
            {
                if (target.Character.Wound != CharacterWound.None)
                {
                    SendMessage(MessageType.Error, Global.MENSAGEM_GRAVEMENTE_FERIDO);
                    return;
                }

                target.InventoryTargetId = Character.Id;
                target.InventoryShowType = inventoryShowType;
                target.InventoryRightTargetId = rightTargetId;

                ConfirmShowInventory();
            }

            void ConfirmShowInventory()
            {
                var leftTarget = this;
                if (inventoryShowType < InventoryShowType.Property)
                {
                    var items = Global.Items.Where(x =>
                        target.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE
                            && x.Dimension == target.Dimension).ToList();

                    rightItems = Functions.Serialize(items.Select(x => new
                    {
                        ID = x.Id,
                        Image = x.GetImageName(),
                        Name = x.GetName(),
                        x.Quantity,
                        Slot = 1000 + items.IndexOf(x) + 1,
                        Extra = x.GetExtra(),
                        Weight = (x.Quantity * x.GetWeight()).ToString("N2"),
                    }));
                }
                else
                {
                    leftTarget = target;
                }

                target.Emit("Inventory:Show",
                    update,
                    $"{leftTarget.Character.Name} [{leftTarget.SessionId}] ({leftTarget.Items.Sum(x => x.Quantity * x.GetWeight()):N2}/{Global.PESO_MAXIMO_INVENTARIO:N2} kgs)",
                    Functions.Serialize(
                        leftTarget.Items.Select(x => new
                        {
                            ID = x.Id,
                            Image = x.GetImageName(),
                            Name = x.GetName(),
                            x.Quantity,
                            x.Slot,
                            Extra = x.GetExtra(),
                            Weight = (x.Quantity * x.GetWeight()).ToString("N2"),
                        })),
                    rightTitle,
                    rightItems,
                   (int)inventoryShowType);
            }
        }

        public void CloseInventory()
        {
            Emit("Server:CloseView");
            Emit("ActivateCurrentHUD");
        }

        public async Task RemoveStackedItem(ItemCategory itemCategory, int quantity)
        {
            await using var context = new DatabaseContext();
            var item = Items.FirstOrDefault(x => x.Category == itemCategory)!;
            item.SetQuantity(item.Quantity - quantity);
            if (item.Quantity > 0)
            {
                context.CharactersItems.Update(item);
            }
            else
            {
                context.CharactersItems.Remove(item);
                Items.Remove(item);
            }
            await context.SaveChangesAsync();
            ShowInventory(this, update: true);
        }

        public async Task RemoveItem(CharacterItem item) => await RemoveItem([item]);

        public async Task RemoveItem(IEnumerable<CharacterItem> items)
        {
            await using var context = new DatabaseContext();
            foreach (var item in items.ToList())
            {
                if (item.Quantity <= 0)
                    continue;

                context.CharactersItems.Remove(item);
                Items.Remove(item);

                if (item.GetIsCloth())
                {
                    await SetarRoupas();
                }
                else if (item.Category == ItemCategory.Weapon)
                {
                    if (item.Slot < 0)
                    {
                        var isCurrentWeapon = CurrentWeapon == item.Type;
                        RemoveWeapon(item.Type);
                        Emit("RemoveWeapon", item.Type);
                        if (isCurrentWeapon)
                            CurrentWeapon = (uint)WeaponModel.Fist;
                    }
                }
                else if (item.Category == ItemCategory.WalkieTalkie)
                {
                    if (item.Slot < 0)
                        RadioCommunicatorItem = new();
                }
                else if (item.Category == ItemCategory.Cellphone)
                {
                    if (item.Slot < 0)
                    {
                        Cellphone = 0;
                        CellphoneItem = new CellphoneItem();
                        ToggleViewCellphone();
                    }
                }
            }

            await context.SaveChangesAsync();

            ShowInventory(this, update: true);
        }

        public async Task RemoveFromFaction()
        {
            if (Faction?.Government ?? false)
            {
                Character.Badge = 0;
                Armor = 0;
                await RemoveItem(Items.Where(x => !Functions.CanDropItem(Character.Sex, Faction, x)));
            }

            FactionFlags = [];
            Character.FactionId = Character.FactionRankId = null;
            OnDuty = false;
            // Se tiver FactioNDutySession tem que dar end e salvar
        }

        public void SetNametagDamaged()
        {
            CancellationTokenSourceDamaged?.Cancel();
            CancellationTokenSourceDamaged = new CancellationTokenSource();

            SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_DAMAGED, true);

            Task.Delay(250, CancellationTokenSourceDamaged.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_DAMAGED, false);
                CancellationTokenSourceDamaged = null;
            });
        }

        public void ToggleViewCellphone() => Emit("ToggleViewCellphone", Cellphone, CellphoneItem.FlightMode);

        public async Task UpdateCellphoneDatabase()
        {
            var cellphoneItem = Items.FirstOrDefault(x => x.Category == ItemCategory.Cellphone && x.Slot < 0);
            if (cellphoneItem != null)
            {
                cellphoneItem.SetExtra(Functions.Serialize(CellphoneItem));
                await using var context = new DatabaseContext();
                context.CharactersItems.Update(cellphoneItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task EndCellphoneCall()
        {
            TimerCelular?.Stop();
            TimerCelular = null;

            if (CellphoneCall.Number == 0)
                return;

            CellphoneCall.EndDate = DateTime.Now;
            CellphoneItem.Calls.Add(CellphoneCall);

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.CellphoneCall.Number == Cellphone);
            if (target != null)
            {
                target.SendMessage(MessageType.None, $"[CELULAR] Sua ligação com {target.ObterNomeContato(Cellphone)} terminou.", Global.CELLPHONE_SECONDARY_COLOR);

                target.CellphoneCall.EndDate = CellphoneCall.EndDate;
                target.CellphoneItem.Calls.Add(target.CellphoneCall);
                target.CellphoneCall = new();
                await target.UpdateCellphoneDatabase();
            }

            CellphoneCall = new();
            await UpdateCellphoneDatabase();
        }

        public async void TimerCelular_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (sender is not System.Timers.Timer timer)
                return;

            TimerCelularElapsedCount++;

            if (TimerCelularElapsedCount == 5)
            {
                SendMessage(MessageType.None, $"[CELULAR] Sua ligação para {ObterNomeContato(CellphoneCall.Number)} caiu após tocar 5 vezes.", Global.CELLPHONE_SECONDARY_COLOR);
                await EndCellphoneCall();
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.CellphoneCall.Number == Cellphone);
            if (target == null)
            {
                SendMessage(MessageType.None, $"[CELULAR] Sua ligação com {ObterNomeContato(CellphoneCall.Number)} terminou.", Global.CELLPHONE_SECONDARY_COLOR);
                await EndCellphoneCall();
                return;
            }

            target.SendMessage(MessageType.None, $"[CELULAR] O seu celular está tocando! Ligação de {target.ObterNomeContato(Cellphone)}. (/atender ou /des)", Global.CELLPHONE_SECONDARY_COLOR);
            target.SendMessageToNearbyPlayers($"O celular de {target.ICName} está tocando.", MessageCategory.Do, 5, true);
        }

        public void EmitShowMessage(string message, string component = null) => Emit("Server:MostrarErro", message, component);

        public void EmitStaffShowMessage(string message, bool close = false) => Emit("Staff:MostrarMensagem", message, close);

        public void SetupDrugTimer(bool drugEffect)
        {
            DrugTimer?.Stop();

            if (!drugEffect && !Character.ThresoldDeathEndDate.HasValue)
                return;

            var interval = drugEffect
                ?
                (Character.DrugEndDate - DateTime.Now).Value.TotalMilliseconds
                :
                (Character.ThresoldDeathEndDate - DateTime.Now).Value.TotalMilliseconds;

            if (interval < 0)
            {
                DrugTimer_Elapsed(null, null);
                return;
            }

            DrugTimer = new System.Timers.Timer(interval);
            DrugTimer.Elapsed += DrugTimer_Elapsed;
            DrugTimer.Start();

            if (drugEffect)
                Emit("SetDrugEffect", (int)Character.DrugItemCategory);
        }

        public void ClearDrugEffect() => Emit("ClearDrugEffect");

        public void DrugTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Character.DrugItemCategory.HasValue)
            {
                Character.DrugItemCategory = null;
                Character.DrugEndDate = null;
                Character.ThresoldDeathEndDate = DateTime.Now.AddHours(1);
                ClearDrugEffect();
                SetupDrugTimer(false);
                if (Health > MaxHealth)
                    Health = MaxHealth;
            }
            else
            {
                DrugTimer?.Stop();
                Character.ThresoldDeathEndDate = null;
                Character.ThresoldDeath = 0;
            }
        }

        public void ShowConfirm(string title, string message, string clientEvent)
        {
            Emit("chat:ExibirConfirmacao", title, message, clientEvent);
        }
    }
}