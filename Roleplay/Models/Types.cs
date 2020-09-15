using System.ComponentModel.DataAnnotations;

namespace Roleplay.Models
{
    public enum TipoMensagem
    {
        Nenhum,
        Erro,
        Sucesso,
        Titulo,
        Punicao,
    }

    public enum TipoMensagemJogo
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
    }

    public enum TipoFaccao
    {
        Policial = 1,

        [Display(Name = "Médica")]
        Medica = 2,

        Criminosa = 3,
    }

    public enum TipoPunicao
    {
        Kick = 1,
        Ban = 2,
    }

    public enum TipoLog
    {
        Staff = 1,
        FaccaoLider = 2,
        FaccaoGestor = 3,
        Dinheiro = 4,
        Venda = 5,
        Entrada = 6,
        Saida = 7,
        Morte = 8,
        Arma = 9,
    }

    public enum TipoConvite
    {
        Faccao = 1,
        VendaPropriedade = 2,
        Revista = 3,
        VendaVeiculo = 4,
    }

    public enum TipoInterior
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
    }

    public enum TipoPreco
    {
        [Display(Name = "Carros e Motos")]
        CarrosMotos = 1,

        [Display(Name = "Conveniência")]
        Conveniencia = 2,

        Barcos = 3,

        [Display(Name = "Helicópteros")]
        Helicopteros = 4,

        Industrial = 5,

        [Display(Name = "Aviões")]
        Avioes = 6,
    }

    public enum TipoPonto
    {
        Multas = 1,
        Banco = 2,
        ATM = 3,
        LojaConveniencia = 4,
        LojaRoupas = 5,
        SpawnVeiculosFaccao = 6,
        ApreensaoVeiculos = 7,
        LiberacaoVeiculos = 8,
        Barbearia = 9,
        Uniforme = 10,
        MDC = 11,
    }

    public enum TipoEmprego
    {
        Nenhum = 0,
        Taxista = 1,
    }

    public enum TipoStaff
    {
        Nenhum = 0,

        Helper = 1,

        [Display(Name = "Game Administrator")]
        GameModerator = 2,

        [Display(Name = "Game Administrator")]
        GameAdministrator = 3,

        [Display(Name = "Lead Administrator")]
        LeadAdministrator = 100,

        Manager = 1337,
    }

    public enum TipoRespostaSOS
    {
        Aguardando = 0,

        Aceito = 1,

        Rejeitado = 2,

        [Display(Name = "Sem Resposta")]
        SemResposta = 3,
    }

    public enum TipoStatusLigacao
    {
        Nenhum = 0,
        EmLigacao = 1,
        AguardandoInformacao = 2,
    }

    public enum ModeloVeiculo : uint
    {
        PoliceSlick,
        PoliceOld,
        PScout,
        BeachP,
        Polmerit2,
        Police42,
        PolSpeedo,
        PolRiot,
        LSPDB,
        Pulice,
        Pulice2,
        Pulice3,
        Pulice4,
    }
}