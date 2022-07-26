namespace Roleplay.Models
{
    public class ClotheAccessory
    {
        public string DLC { get; set; }

        public byte Component { get; set; }

        public int Drawable { get; set; }

        public byte Texture { get; set; }

        /// <summary>
        /// -2 compartilhado entre tipo facção 1 e 2
        /// -1 exibe sempre
        /// 0 exibe somente na customização e loja de roupas
        /// > 0 exibe somente para a facção de tipo correspondente
        /// </summary>
        public int TipoFaccao { get; set; }

        public byte MaxTexture { get; set; }
    }
}