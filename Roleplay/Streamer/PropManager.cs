using AltV.Net;
using AltV.Net.EntitySync;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Roleplay.Streamer
{
    public enum TextureVariation
    {
        Pacific = 0,
        Azure = 1,
        Nautical = 2,
        Continental = 3,
        Battleship = 4,
        Intrepid = 5,
        Uniform = 6,
        Classico = 7,
        Mediterranean = 8,
        Command = 9,
        Mariner = 10,
        Ruby = 11,
        Vintage = 12,
        Pristine = 13,
        Merchant = 14,
        Voyager = 15
    }

    public class MoveData : IWritable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Speed { get; set; }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("X");
            writer.Value(X);
            writer.Name("Y");
            writer.Value(Y);
            writer.Name("Z");
            writer.Value(Z);
            writer.Name("Speed");
            writer.Value(Speed);
            writer.EndObject();
        }
    }

    public class Rgb : IWritable
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public Rgb(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("Red");
            writer.Value(Red);
            writer.Name("Green");
            writer.Value(Green);
            writer.Name("Blue");
            writer.Value(Blue);
            writer.EndObject();
        }
    }

    public class Prop : Entity, IEntity
    {
        private static List<Prop> propList = new();

        public static List<Prop> PropList
        {
            get
            {
                lock (propList)
                {
                    return propList;
                }
            }
            set
            {
                propList = value;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                if (!TryGetData("rotation", out Dictionary<string, object> data))
                    return default;

                return new Vector3()
                {
                    X = Convert.ToSingle(data["x"]),
                    Y = Convert.ToSingle(data["y"]),
                    Z = Convert.ToSingle(data["z"]),
                };
            }
            set
            {
                if (Rotation.X == value.X && Rotation.Y == value.Y && Rotation.Z == value.Z && value != new Vector3(0, 0, 0))
                    return;

                var dict = new Dictionary<string, object>()
                {
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["z"] = value.Z,
                };
                SetData("rotation", dict);
            }
        }

        public Vector3 Velocity
        {
            get
            {
                if (!TryGetData("velocity", out Dictionary<string, object> data))
                    return default;

                return new Vector3()
                {
                    X = Convert.ToSingle(data["x"]),
                    Y = Convert.ToSingle(data["y"]),
                    Z = Convert.ToSingle(data["z"]),
                };
            }
            set
            {
                if (Velocity.X == value.X && Velocity.Y == value.Y && Velocity.Z == value.Z && value != new Vector3(0, 0, 0))
                    return;

                var dict = new Dictionary<string, object>()
                {
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["z"] = value.Z,
                };
                SetData("velocity", dict);
            }
        }

        public Vector3 SlideToPosition
        {
            get
            {
                if (!TryGetData("SlideToPosition", out Dictionary<string, object> data))
                    return default;

                return new Vector3()
                {
                    X = Convert.ToSingle(data["x"]),
                    Y = Convert.ToSingle(data["y"]),
                    Z = Convert.ToSingle(data["z"]),
                };
            }
            set
            {
                var dict = new Dictionary<string, object>()
                {
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["z"] = value.Z,
                };
                SetData("SlideToPosition", dict);
            }
        }

        public string Model
        {
            get
            {
                if (!TryGetData("model", out string model))
                    return null;

                return model;
            }
            set
            {
                if (Model == value)
                    return;

                SetData("model", value);
            }
        }

        public uint? LodDistance
        {
            get
            {
                if (!TryGetData("lodDistance", out uint lodDist))
                    return null;

                return lodDist;
            }
            set
            {
                if (value == null)
                {
                    SetData("lodDistance", null);
                    return;
                }

                if (LodDistance == value)
                    return;

                SetData("lodDistance", value);
            }
        }

        public TextureVariation? TextureVariation
        {
            get
            {
                if (!TryGetData("textureVariation", out int variation))
                    return null;

                return (TextureVariation)variation;
            }
            set
            {
                if (value == null)
                {
                    SetData("textureVariation", null);
                    return;
                }

                if (TextureVariation == value)
                    return;

                SetData("textureVariation", (int)value);
            }
        }

        public bool? Dynamic
        {
            get
            {
                if (!TryGetData("dynamic", out bool isDynamic))
                    return false;

                return isDynamic;
            }
            set
            {
                if (value == null)
                {
                    SetData("dynamic", null);
                    return;
                }

                if (Dynamic == value)
                    return;

                SetData("dynamic", value);
            }
        }

        public bool? Visible
        {
            get
            {
                if (!TryGetData("visible", out bool visible))
                    return false;

                return visible;
            }
            set
            {
                if (value == null)
                {
                    SetData("visible", null);
                    return;
                }

                if (Visible == value)
                    return;

                SetData("visible", value);
            }
        }

        public bool? OnFire
        {
            get
            {
                if (!TryGetData("onFire", out bool onFire))
                    return false;

                return onFire;
            }
            set
            {
                if (value == null)
                {
                    SetData("onFire", null);
                    return;
                }

                if (OnFire == value)
                    return;

                SetData("onFire", value);
            }
        }

        public bool? Freeze
        {
            get
            {
                if (!TryGetData("freeze", out bool frozen))
                    return false;

                return frozen;
            }
            set
            {
                if (value == null)
                {
                    SetData("freeze", null);
                    return;
                }

                if (Freeze == value)
                    return;

                SetData("freeze", value);
            }
        }

        public Rgb LightColor
        {
            get
            {
                if (!TryGetData("lightColor", out Dictionary<string, object> data))
                    return null;

                return new Rgb(
                    Convert.ToInt32(data["r"]),
                    Convert.ToInt32(data["g"]),
                    Convert.ToInt32(data["b"])
                );
            }
            set
            {
                if (value == null)
                {
                    SetData("lightColor", null);
                    return;
                }

                if (LightColor != null && LightColor.Red == value.Red && LightColor.Green == value.Green && LightColor.Blue == value.Blue)
                    return;

                var dict = new Dictionary<string, object>
                {
                    { "r", value.Red },
                    { "g", value.Green },
                    { "b", value.Blue }
                };
                SetData("lightColor", dict);
            }
        }

        public Vector3 PositionInitial { get; internal set; }

        public bool? Collision
        {
            get
            {
                if (!TryGetData("collision", out bool collision))
                    return false;

                return collision;
            }
            set
            {
                if (value == null)
                {
                    SetData("collision", null);
                    return;
                }

                if (Collision == value)
                    return;

                SetData("collision", value);
            }
        }

        public int? CharacterId { get; set; }

        public int? FactionId { get; set; }

        public Prop(Vector3 position, int dimension, uint range, ulong entityType) : base(entityType, position, dimension, range) { }

        public void Destroy()
        {
            PropList.Remove(this);
            AltEntitySync.RemoveEntity(this);
        }
    }

    public static class PropStreamer
    {
        public static Prop Create(
            string model, Vector3 position, Vector3 rotation, int dimension = 0, bool? isDynamic = null, bool? frozen = null, uint? lodDistance = null,
            Rgb lightColor = null, bool? onFire = null, TextureVariation? textureVariation = null, bool? visible = null, uint streamRange = 150,
            bool collision = true, int? characterId = null, int? factionId = null
        )
        {
            var obj = new Prop(position, dimension, streamRange, 1)
            {
                Rotation = rotation,
                Model = model,
                Dynamic = isDynamic ?? null,
                Freeze = frozen ?? null,
                LodDistance = lodDistance ?? null,
                LightColor = lightColor ?? null,
                OnFire = onFire ?? null,
                TextureVariation = textureVariation ?? null,
                Visible = visible ?? null,
                PositionInitial = position,
                Collision = collision,
                CharacterId = characterId,
                FactionId = factionId,
            };
            Prop.PropList.Add(obj);
            AltEntitySync.AddEntity(obj);
            return obj;
        }
    }
}