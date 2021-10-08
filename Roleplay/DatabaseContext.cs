using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;

namespace Roleplay
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Apreensao> Apreensoes { get; set; }
        public DbSet<Armario> Armarios { get; set; }
        public DbSet<ArmarioComponente> ArmariosComponentes { get; set; }
        public DbSet<ArmarioItem> ArmariosItens { get; set; }
        public DbSet<Banimento> Banimentos { get; set; }
        public DbSet<Blip> Blips { get; set; }
        public DbSet<Confisco> Confiscos { get; set; }
        public DbSet<Faccao> Faccoes { get; set; }
        public DbSet<Ligacao911> Ligacoes911 { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Multa> Multas { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<Pergunta> Perguntas { get; set; }
        public DbSet<Personagem> Personagens { get; set; }
        public DbSet<Ponto> Pontos { get; set; }
        public DbSet<Preco> Precos { get; set; }
        public DbSet<Prisao> Prisoes { get; set; }
        public DbSet<Procurado> Procurados { get; set; }
        public DbSet<Propriedade> Propriedades { get; set; }
        public DbSet<Punicao> Punicoes { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Resposta> Respostas { get; set; }
        public DbSet<SOS> SOSs { get; set; }
        public DbSet<Unidade> Unidades { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }

        // Não representam tabelas exatas no banco de dados
        public DbSet<PunicaoAdministrativa> PunicoesAdministrativas { get; set; }
        public DbSet<ProcuradoInfo> ProcuradosInfos { get; set; }
        public DbSet<LogInfo> LogsInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(Global.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apreensao>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Armario>().HasKey(x => x.Codigo);
            modelBuilder.Entity<ArmarioComponente>().HasKey(x => new { x.Codigo, x.Arma, x.Componente });
            modelBuilder.Entity<ArmarioItem>().HasKey(x => new { x.Codigo, x.Arma });
            modelBuilder.Entity<Banimento>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Blip>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Confisco>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Faccao>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Ligacao911>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Log>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Multa>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Parametro>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Parametro>().Property(e => e.Weather).HasConversion<int>();
            modelBuilder.Entity<Pergunta>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Personagem>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Ponto>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Preco>().HasKey(x => new { x.Tipo, x.Nome });
            modelBuilder.Entity<Prisao>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Procurado>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Propriedade>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Punicao>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Rank>().HasKey(x => new { x.Faccao, x.Codigo });
            modelBuilder.Entity<Resposta>().HasKey(x => x.Codigo);
            modelBuilder.Entity<SOS>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Unidade>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Usuario>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Veiculo>().HasKey(x => x.Codigo);

            // Não representam tabelas exatas no banco de dados
            modelBuilder.Entity<PunicaoAdministrativa>().HasKey(x => x.Codigo);
            modelBuilder.Entity<ProcuradoInfo>().HasKey(x => x.Codigo);
            modelBuilder.Entity<LogInfo>().HasKey(x => x.Codigo);
        }
    }
}