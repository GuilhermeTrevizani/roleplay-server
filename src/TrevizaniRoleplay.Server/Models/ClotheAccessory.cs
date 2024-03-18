namespace TrevizaniRoleplay.Server.Models
{
    public class ClotheAccessory
    {
        public string DLC { get; set; }
        public byte Component { get; set; }
        public int Drawable { get; set; }
        public byte Texture { get; set; }

        /// <summary>
        /// -2 = show for faction types 1 and 2
        /// -1 = show always
        /// 0 = show only on character creator and clothes store
        /// > 0 = show only for the faction type
        /// </summary>
        public int TipoFaccao { get; set; }
        public byte MaxTexture { get; set; }
    }
}