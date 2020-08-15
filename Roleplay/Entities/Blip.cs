using AltV.Net.Data;
using AltV.Net.Elements.Entities;

namespace Roleplay.Entities
{
    public class Blip
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;
        public int Tipo { get; set; } = 0;
        public int Cor { get; set; } = 0;
        public bool Inativo { get; set; } = false;

        public void CriarIdentificador(IPlayer player)
        {
            DeletarIdentificador(player);

            if (!Inativo)
                player.Emit("blip:create", Codigo, Tipo, Nome, Cor, new Position(PosX, PosY, PosZ));
        }

        public void DeletarIdentificador(IPlayer player) => player.Emit("blip:remove", Codigo);
    }
}