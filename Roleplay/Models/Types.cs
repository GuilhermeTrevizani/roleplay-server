using System.ComponentModel.DataAnnotations;

namespace Roleplay.Models
{
    public enum MessageType
    {
        None,
        Error,
        Success,
        Title,
    }

    public enum MessageCategory
    {
        Me,
        Do,
        ChatICNormal,
        ChatICGrito,
        ChatOOC,
        ChatICBaixo,
        Megafone,
        Celular,
        Ame,
        Radio,
        Ado,
        DadosMoeda,
        Microphone,
    }

    public enum FactionType : byte
    {
        [Display(Name = "Policial")]
        Police = 1,

        [Display(Name = "Bombeiros")]
        Firefighter = 2,

        Criminal = 3,

        [Display(Name = "Imprensa")]
        Press = 4,

        [Display(Name = "Legista")]
        Coroner = 5,
    }

    public enum PunishmentType : byte
    {
        Kick = 1,
        Ban = 2,
    }

    public enum LogType : byte
    {
        Todos = 0,

        Staff = 1,

        [Display(Name = "Facção")]
        Faction = 2,

        [Display(Name = "Chat da Facção")]
        FactionChat = 3,

        Dinheiro = 4,

        Venda = 5,

        Entrada = 6,

        [Display(Name = "Saída")]
        Saida = 7,

        Morte = 8,

        Arma = 9,

        [Display(Name = "Troca de Nome")]
        NameChange = 10,

        [Display(Name = "Exclusão de Personagem")]
        ExclusaoPersonagem = 11,

        [Display(Name = "Troca de Placa")]
        PlateChange = 12,

        [Display(Name = "Anúncio")]
        Anuncio = 13,

        [Display(Name = "Anúncio Governamental")]
        AnuncioGov = 14,

        [Display(Name = "Falha Login")]
        LoginFalha = 15,

        [Display(Name = "Falha Esqueci Minha Senha")]
        EsqueciMinhaSenhaFalha = 16,

        [Display(Name = "Entregar Item")]
        EntregarItem = 17,

        [Display(Name = "Dropar Item")]
        DroparItem = 18,

        [Display(Name = "Pegar Item Chão")]
        PegarItemChao = 19,

        [Display(Name = "Roubar Item")]
        RoubarItem = 20,

        [Display(Name = "Descartar Item")]
        DescartarItem = 21,

        [Display(Name = "Dar Item")]
        DarItem = 22,

        [Display(Name = "Armazenar Item Propriedade")]
        ArmazenarItemPropriedade = 23,

        [Display(Name = "Pegar Item Propriedade")]
        PegarItemPropriedade = 24,

        [Display(Name = "Armazenar Item Veículo")]
        ArmazenarItemVeiculo = 25,

        [Display(Name = "Pegar Item Veículo")]
        PegarItemVeiculo = 26,

        [Display(Name = "Reparar Veículo Facção")]
        RepararVeiculoFaccao = 27,

        [Display(Name = "Reparar Veículo Jogador")]
        RepararVeiculoJogador = 28,

        [Display(Name = "Spawnar Veículo")]
        SpawnarVeiculo = 29,

        [Display(Name = "Estacionar Veículo")]
        EstacionarVeiculo = 30,

        [Display(Name = "Me Curar")]
        MeCurar = 31,

        [Display(Name = "Mensagens Privadas")]
        MensagensPrivadas = 32,

        [Display(Name = "Máscara")]
        Mascara = 33,

        [Display(Name = "Destruição de Veículo")]
        DestruicaoVeiculo = 34,

        Hack = 35,

        Droga = 36,

        [Display(Name = "Quebrar Trava")]
        QuebrarTrava = 37,

        [Display(Name = "Ligação Direta")]
        LigacaoDireta = 38,

        Desmanche = 39,

        Arrombar = 40,

        [Display(Name = "Roubar Propriedade")]
        RoubarPropriedade = 41,

        [Display(Name = "Roubar Armazenamento")]
        RoubarArmazenamento = 42,

        [Display(Name = "Visualizar Vendas Boca de Fumo")]
        VisualizarVendasBocaFumo = 43,
        
        [Display(Name = "Editar Mobília Propriedade")]
        EditPropertyFurniture = 44,

        [Display(Name = "Chat OOC Global")]
        GlobalOOCChat = 45,

        [Display(Name = "Chat Staff")]
        StaffChat = 46,

        [Display(Name = "Colocar no Veículo")]
        ColocarVeiculo = 47,

        [Display(Name = "Retirar do Veículo")]
        RetirarVeiculo = 48,

        Empresa = 49,

        [Display(Name = "Anúncio Empresa")]
        AnuncioEmpresa = 50,

        [Display(Name = "Tunar Veículo Mecânico")]
        TunarVeiculoMecanico = 51,

        Emprego = 52,
    }

    public enum InviteType
    {
        [Display(Name = "Facção")]
        Faccao = 1,

        [Display(Name = "Venda de Propriedade")]
        VendaPropriedade = 2,

        Revista = 3,

        [Display(Name = "Venda de Veículo")]
        VendaVeiculo = 4,

        [Display(Name = "Localização de Celular")]
        LocalizacaoCelular = 5,

        [Display(Name = "Empresa")]
        Company = 6,

        [Display(Name = "Mecânico")]
        Mechanic = 7,
    }

    public enum PropertyInterior : byte
    {
        Motel = 1,
        CasaBaixa = 2,
        CasaMedia = 3,
        IntegrityWay28 = 4,
        IntegrityWay30 = 5,
        DellPerroHeights4 = 6,
        DellPerroHeights7 = 7,
        RichardMajestic2 = 8,
        TinselTowers42 = 9,
        EclipseTowers3 = 10,
        WildOatsDrive3655 = 11,
        NorthConkerAvenue2044 = 12,
        NorthConkerAvenue2045 = 13,
        HillcrestAvenue2862 = 14,
        HillcrestAvenue2868 = 15,
        HillcrestAvenue2874 = 16,
        WhispymoundDrive2677 = 17,
        MadWayneThunder2133 = 18,
        Modern1Apartment = 19,
        Modern2Apartment = 20,
        Modern3Apartment = 21,
        Mody1Apartment = 22,
        Mody2Apartment = 23,
        Mody3Apartment = 24,
        Vibrant1Apartment = 25,
        Vibrant2Apartment = 26,
        Vibrant3Apartment = 27,
        Sharp1Apartment = 28,
        Sharp2Apartment = 29,
        Sharp3Apartment = 30,
        Monochrome1Apartment = 31,
        Monochrome2Apartment = 32,
        Monochrome3Apartment = 33,
        Seductive1Apartment = 34,
        Seductive2Apartment = 35,
        Seductive3Apartment = 36,
        Regal1Apartment = 37,
        Regal2Apartment = 38,
        Regal3Apartment = 39,
        Aqua1Apartment = 40,
        Aqua2Apartment = 41,
        Aqua3Apartment = 42,
        ArcadiusExecutiveRich = 43,
        ArcadiusExecutiveCool = 44,
        ArcadiusExecutiveContrast = 45,
        ArcadiusOldSpiceWarm = 46,
        ArcadiusOldSpiceClassical = 47,
        ArcadiusOldSpiceVintage = 48,
        ArcadiusPowerBrokerIce = 49,
        ArcadiusPowerBrokeConservative = 50,
        ArcadiusPowerBrokePolished = 51,
        MazeBankExecutiveRich = 52,
        MazeBankExecutiveCool = 53,
        MazeBankExecutiveContrast = 54,
        MazeBankOldSpiceWarm = 55,
        MazeBankOldSpiceClassical = 56,
        MazeBankOldSpiceVintage = 57,
        MazeBankPowerBrokerIce = 58,
        MazeBankPowerBrokeConservative = 59,
        MazeBankPowerBrokePolished = 60,
        LomBankExecutiveRich = 61,
        LomBankExecutiveCool = 62,
        LomBankExecutiveContrast = 63,
        LomBankOldSpiceWarm = 64,
        LomBankOldSpiceClassical = 65,
        LomBankOldSpiceVintage = 66,
        LomBankPowerBrokerIce = 67,
        LomBankPowerBrokeConservative = 68,
        LomBankPowerBrokePolished = 69,
        MazeBankWestExecutiveRich = 70,
        MazeBankWestExecutiveCool = 71,
        MazeBankWestExecutiveContrast = 72,
        MazeBankWestOldSpiceWarm = 73,
        MazeBankWestOldSpiceClassical = 74,
        MazeBankWestOldSpiceVintage = 75,
        MazeBankWestPowerBrokerIce = 76,
        MazeBankWestPowerBrokeConservative = 77,
        MazeBankWestPowerBrokePolished = 78,
        Clubhouse1 = 79,
        Clubhouse2 = 80,
        MethLab = 81,
        WeedFarm = 82,
        CocaineLockup = 83,
        CounterfeitCashFactory = 84,
        DocumentForgeryOffice = 85,
        WarehouseSmall = 86,
        WarehouseMedium = 87,
        WarehouseLarge = 88,
        Nightclub = 89,
    }

    public enum PriceType : byte
    {
        [Display(Name = "Aluguel de Veículos de Empregos")]
        AluguelEmpregos = 1,

        [Display(Name = "Conveniência")]
        Conveniencia = 2,

        Barcos = 3,

        [Display(Name = "Helicópteros")]
        Helicopteros = 4,

        Carros = 5,

        [Display(Name = "Aviões")]
        Avioes = 6,

        Armas = 7,

        Empregos = 8,

        [Display(Name = "Motocicletas e Bicicletas")]
        MotocicletasBicicletas = 9,

        Drogas = 10,

        Tuning = 11,
    }

    public enum SpotType : byte
    {
        Banco = 1,

        [Display(Name = "Loja de Conveniência")]
        LojaConveniencia = 2,

        [Display(Name = "Loja de Roupas")]
        LojaRoupas = 3,

        [Display(Name = "Spawn de Veículos da Facção")]
        SpawnVeiculosFaccao = 4,

        [Display(Name = "Apreensão de Veículos")]
        ApreensaoVeiculos = 5,

        [Display(Name = "Liberação de Veículos")]
        LiberacaoVeiculos = 6,

        Barbearia = 7,

        Uniforme = 8,

        MDC = 9,

        DMV = 10,

        Entrada = 11,

        [Display(Name = "Me Curar")]
        MeCurar = 12,

        [Display(Name = "Prisão")]
        Prisao = 13,

        Lixeiro = 14,

        [Display(Name = "Atendimento LSPD")]
        AtendimentoLSPD = 15,

        Confisco = 16,

        [Display(Name = "Estúdio de Tatuagens")]
        TattooShop = 17,

        [Display(Name = "Oficina Mecânica")]
        MechanicWorkshop = 18,
    }

    public enum CharacterJob : byte
    {
        [Display(Name = "Nenhum")]
        None = 0,

        [Display(Name = "Taxista")]
        TaxiDriver = 1,

        [Display(Name = "Mecânico")]
        Mechanic = 2,

        [Display(Name = "Lixeiro")]
        Garbageman = 3,

        [Display(Name = "Caminhoneiro")]
        Trucker = 4,
    }

    public enum UserStaff
    {
        [Display(Name = "Nenhum")]
        None = 0,

        Moderator = 1,

        [Display(Name = "Game Administrator")]
        GameAdministrator = 2,

        [Display(Name = "Lead Administrator")]
        LeadAdministrator = 100,

        [Display(Name = "Head Administrator")]
        HeadAdministrator = 200,

        Manager = 1337,
    }

    public enum VehicleModelMods : uint
    {
        #region LSPD
        UMKSCOUT,
        PDUMKSX,
        NSCOUTLSPD,
        POLTHRUST,
        SWATSTALKER,
        PSCOUT,
        LSPDB,
        LSPDHELI,
        POLICE42,
        VVPI,
        VVPI2,
        BCAT,
        #endregion LSPD
        #region LSFD
        LSFD,
        LSFD2,
        LSFD3,
        LSFD4,
        LSFD5,
        LSFDTRUCK,
        LSFDTRUCK2,
        LSFDTRUCK3,
        LADDER,
        #endregion LSFD
        #region Coroner
        CORONER2,
        #endregion Coroner
    }

    public enum CharacterNameChangeStatus : byte
    {
        Liberado = 0,
        Bloqueado = 1,
        Realizado = 2,
    }

    public enum CharacterPersonalizationStep : byte
    {
        Character = 0,
        Tattoos = 1,
        Clothes = 2,
        Ready = 3,
    }

    public enum AnimationFlags
    {
        Loop = 1 << 0,
        StopOnLastFrame = 1 << 1,
        OnlyAnimateUpperBody = 1 << 4,
        AllowPlayerControl = 1 << 5,
        Cancellable = 1 << 7
    };

    public enum UserVIP : byte
    {
        [Display(Name = "Nenhum")]
        None = 0,

        Bronze = 1,

        [Display(Name = "Prata")]
        Silver = 2,

        [Display(Name = "Ouro")]
        Gold = 3,
    }

    public enum EmergencyCallType : byte
    {
        [Display(Name = "Polícia")]
        Police = 1,

        Medic = 2,

        Both = 3,
    }

    public enum CharacterSex : byte
    {
        [Display(Name = "Mulher")]
        Woman = 0,

        [Display(Name = "Homem")]
        Man = 1,
    }

    public enum CharacterWound : byte
    {
        Nenhum = 0,
        GravementeFeridoInvencivel = 1,
        GravementeFerido = 2,
        PK = 3,
        PodeHospitalCK = 4
    }

    public enum ItemCategory : byte
    {
        [Display(Name = "Arma")]
        Weapon = 1,

        [Display(Name = "Chave de Propriedade")]
        PropertyKey = 2,

        [Display(Name = "Máscara")]
        Cloth1 = 3,

        [Display(Name = "Torso")]
        Cloth3 = 4,

        [Display(Name = "Calça")]
        Cloth4 = 5,

        [Display(Name = "Mochila")]
        Cloth5 = 6,

        [Display(Name = "Sapato")]
        Cloth6 = 7,

        [Display(Name = "Extra")]
        Cloth7 = 8,

        [Display(Name = "Camisa")]
        Cloth8 = 9,

        [Display(Name = "Colete")]
        Cloth9 = 10,

        [Display(Name = "Bordado")]
        Cloth10 = 11,

        [Display(Name = "Jaqueta")]
        Cloth11 = 12,

        [Display(Name = "Chapéu")]
        Accessory0 = 13,

        [Display(Name = "Óculos")]
        Accessory1 = 14,

        [Display(Name = "Orelha")]
        Accessory2 = 15,

        [Display(Name = "Relógio")]
        Accessory6 = 16,

        [Display(Name = "Bracelete")]
        Accessory7 = 17,

        [Display(Name = "Dinheiro")]
        Money = 18,

        // 19

        [Display(Name = "Chave de Veículo")]
        VehicleKey = 20,

        [Display(Name = "Rádio Comunicador")]
        WalkieTalkie = 21,

        [Display(Name = "Celular")]
        Cellphone = 22,

        [Display(Name = "Maconha")]
        Weed = 23,

        [Display(Name = "Cocaína")]
        Cocaine = 24,

        Crack = 25,

        [Display(Name = "Heroína")]
        Heroin = 26,

        MDMA = 27,

        Xanax = 28,

        Oxycontin = 29,

        Metanfetamina = 30,

        Boombox = 31,

        [Display(Name = "Microfone")]
        Microphone = 32,
    }

    public enum InventoryShowType : byte
    {
        Default = 0,
        Inspect = 1,
        Property = 2,
        Vehicle = 3,
    }

    public enum CellphoneMessageType : byte
    {
        Text = 0,
        Location = 1,
    }

    public enum CellphoneCallType : byte
    {
        Perdida = 0,
        Atendida = 1,
    }

    public enum SessionType : byte
    {
        Login = 1,
        FactionDuty = 2,
        StaffDuty = 3,
    }

    public enum FinancialTransactionType : byte
    {
        Deposit = 1,
        Withdraw = 2,
    }

    public enum StaffFlag : byte
    {
        [Display(Name = "Desbanimento")]
        Unban = 1,

        [Display(Name = "Portas")]
        Doors = 2,

        [Display(Name = "Preços")]
        Prices = 3,

        [Display(Name = "Facções")]
        Factions = 4,

        [Display(Name = "Arsenais de Facções")]
        FactionsArmories = 5,

        [Display(Name = "Propriedades")]
        Properties = 6,

        [Display(Name = "Pontos")]
        Spots = 7,

        Blips = 8,

        [Display(Name = "Veículos")]
        Vehicles = 9,

        CK = 10,

        [Display(Name = "Dar Item")]
        GiveItem = 11,

        [Display(Name = "Drug Houses")]
        FactionsDrugsHouses = 12,

        [Display(Name = "Bocas de Fumo")]
        CrackDens = 13,

        [Display(Name = "Localizações de Caminhoneiro")]
        TruckerLocations = 14,

        [Display(Name = "Mobílias")]
        Furnitures = 16,

        [Display(Name = "Animações")]
        Animations = 17,

        [Display(Name = "Empresas")]
        Companies = 18,
    }

    public enum FactionFlag : byte
    {
        [Display(Name = "Adicionar Membro")]
        InviteMember = 1,

        [Display(Name = "Editar Membro")]
        EditMember = 2,

        [Display(Name = "Remover Membro")]
        RemoveMember = 3,

        [Display(Name = "Bloquear Chat")]
        BlockChat = 4,

        [Display(Name = "Anúncio do Governo")]
        GovernmentAnnouncement = 5,

        HQ = 6,

        [Display(Name = "Arsenal")]
        Armory = 7,

        [Display(Name = "Drogas")]
        DrugHouse = 8,

        [Display(Name = "Deletar Todas Barreiras")]
        RemoveAllBarriers = 9,
    }

    public enum CompanyFlag : byte
    {
        [Display(Name = "Adicionar Funcionário")]
        InviteCharacter = 1,

        [Display(Name = "Editar Funcionário")]
        EditCharacter = 2,

        [Display(Name = "Remover Funcionário")]
        RemoveCharacter = 3,

        [Display(Name = "Abrir")]
        Open = 4,
    }
}