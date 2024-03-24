﻿namespace TrevizaniRoleplay.Server.Models
{
    public class Personalization
    {
        public uint FaceFather { get; set; }
        public uint FaceMother { get; set; }
        public uint SkinFather { get; set; }
        public uint SkinMother { get; set; }
        public float FaceMix { get; set; } = 0.5f;
        public float SkinMix { get; set; } = 0.5f;
        public List<float> Structure { get; set; } = [];
        public ushort Hair { get; set; } = 4;
        public byte HairColor1 { get; set; } = 1;
        public byte HairColor2 { get; set; } = 5;
        public string? HairDLC { get; set; }
        public string? HairCollection { get; set; }
        public string? HairOverlay { get; set; }
        public byte FacialHair { get; set; }
        public byte FacialHairColor1 { get; set; }
        public float FacialHairOpacity { get; set; }
        public byte Eyebrows { get; set; }
        public float EyebrowsOpacity { get; set; }
        public byte EyebrowsColor1 { get; set; }
        public ushort Eyes { get; set; }
        public List<OpacityOverlay> OpacityOverlays { get; set; } = [];
        public List<ColorOverlay> ColorOverlays { get; set; } = [];
        public List<Tattoo> Tattoos { get; set; } = [];

        public class Tattoo
        {
            public string Collection { get; set; } = string.Empty;
            public string Overlay { get; set; } = string.Empty;
        }

        public class OpacityOverlay(byte id)
        {
            public byte Id { get; set; } = id;
            public byte Value { get; set; }
            public float Opacity { get; set; }
        }

        public class ColorOverlay(byte id)
        {
            public byte Id { get; set; } = id;
            public float Opacity { get; set; }
            public byte Color1 { get; set; }
            public byte Color2 { get; set; }
            public byte Value { get; set; }
        }
    }
}