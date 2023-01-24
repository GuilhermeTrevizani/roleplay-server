using AltV.Net.Data;
using AltV.Net.EntitySync;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Roleplay.Streamer
{
    public enum MarkerTypes
    {
        MarkerTypeUpsideDownCone = 0,
        MarkerTypeVerticalCylinder = 1,
        MarkerTypeThickChevronUp = 2,
        MarkerTypeThinChevronUp = 3,
        MarkerTypeCheckeredFlagRect = 4,
        MarkerTypeCheckeredFlagCircle = 5,
        MarkerTypeVerticleCircle = 6,
        MarkerTypePlaneModel = 7,
        MarkerTypeLostMCDark = 8,
        MarkerTypeLostMCLight = 9,
        MarkerTypeNumber0 = 10,
        MarkerTypeNumber1 = 11,
        MarkerTypeNumber2 = 12,
        MarkerTypeNumber3 = 13,
        MarkerTypeNumber4 = 14,
        MarkerTypeNumber5 = 15,
        MarkerTypeNumber6 = 16,
        MarkerTypeNumber7 = 17,
        MarkerTypeNumber8 = 18,
        MarkerTypeNumber9 = 19,
        MarkerTypeChevronUpx1 = 20,
        MarkerTypeChevronUpx2 = 21,
        MarkerTypeChevronUpx3 = 22,
        MarkerTypeHorizontalCircleFat = 23,
        MarkerTypeReplayIcon = 24,
        MarkerTypeHorizontalCircleSkinny = 25,
        MarkerTypeHorizontalCircleSkinny_Arrow = 26,
        MarkerTypeHorizontalSplitArrowCircle = 27,
        MarkerTypeDebugSphere = 28,
        MarkerTypeDallorSign = 29,
        MarkerTypeHorizontalBars = 30,
        MarkerTypeWolfHead = 31,
        MarkerTypeQuestionMark = 32
    }

    public class Marker : Entity, IEntity
    {
        private ulong EntityType
        {
            get
            {
                if (!TryGetData("entityType", out ulong type))
                    return 999;
                return type;
            }
            set
            {
                if (EntityType == value)
                    return;

                SetData("entityType", value);
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

        public string TextureDict
        {
            get
            {
                if (!TryGetData("textureDict", out string textureDict))
                    return null;

                return textureDict;
            }
            set
            {
                if (value == null)
                {
                    SetData("textureDict", null);
                    return;
                }

                if (TextureDict == value)
                    return;

                SetData("textureDict", value);
            }
        }

        public string TextureName
        {
            get
            {
                if (!TryGetData("textureName", out string textureName))
                    return null;

                return textureName;
            }
            set
            {
                if (value == null)
                {
                    SetData("textureName", null);
                    return;
                }

                if (TextureName == value)
                    return;

                SetData("textureName", value);
            }
        }

        public bool? Rotate
        {
            get
            {
                if (!TryGetData("rotate", out bool rotate))
                    return false;

                return rotate;
            }
            set
            {
                if (value == null)
                {
                    SetData("rotate", false);
                    return;
                }

                if (Rotate == value)
                    return;

                SetData("rotate", value);
            }
        }

        public bool? DrawOnEnter
        {
            get
            {
                if (!TryGetData("drawOnEnter", out bool drawOnEnter))
                    return false;

                return drawOnEnter;
            }
            set
            {
                if (value == null)
                {
                    SetData("drawOnEnter", false);
                    return;
                }

                if (DrawOnEnter == value)
                    return;

                SetData("drawOnEnter", value);
            }
        }

        public bool? FaceCamera
        {
            get
            {
                if (!TryGetData("faceCam", out bool faceCamera))
                    return false;

                return faceCamera;
            }
            set
            {
                if (value == null)
                {
                    SetData("faceCam", false);
                    return;
                }

                if (FaceCamera == value)
                    return;

                SetData("faceCam", value);
            }
        }

        public bool? BobUpDown
        {
            get
            {
                if (!TryGetData("bobUpDown", out bool bobUpDown))
                    return false;

                return bobUpDown;
            }
            set
            {
                if (value == null)
                {
                    SetData("bobUpDown", false);
                    return;
                }

                if (BobUpDown == value)
                    return;

                SetData("bobUpDown", value);
            }
        }

        public Vector3 Scale
        {
            get
            {
                if (!TryGetData("scale", out Dictionary<string, object> data))
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
                if (Scale.X == value.X && Scale.Y == value.Y && Scale.Z == value.Z && value != new Vector3(0, 0, 0))
                    return;

                var dict = new Dictionary<string, object>()
                {
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["z"] = value.Z,
                };
                SetData("scale", dict);
            }
        }

        public Vector3 Direction
        {
            get
            {
                if (!TryGetData("direction", out Dictionary<string, object> data))
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
                if (Direction.X == value.X && Direction.Y == value.Y && Direction.Z == value.Z && value != new Vector3(0, 0, 0))
                    return;

                var dict = new Dictionary<string, object>()
                {
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["z"] = value.Z,
                };
                SetData("direction", dict);
            }
        }

        public MarkerTypes MarkerType
        {
            get
            {
                if (!TryGetData("markerType", out int markerType))
                    return default;

                return (MarkerTypes)markerType;
            }
            set
            {
                if (MarkerType == value)
                    return;

                SetData("markerType", (int)value);
            }
        }

        public Rgba? Color
        {
            get
            {
                if (!TryGetData("color", out Dictionary<string, object> data))
                    return null;

                return new Rgba(
                    Convert.ToByte(data["r"]),
                    Convert.ToByte(data["g"]),
                    Convert.ToByte(data["b"]),
                    Convert.ToByte(data["a"])
                );
            }
            set
            {
                if (value == null)
                {
                    SetData("color", null);
                    return;
                }

                if (Color != null && Color?.R == value?.R && Color?.G == value?.G && Color?.B == value?.B && Color?.A == value?.A)
                    return;

                Dictionary<string, object> dict = new()
                {
                    { "r", Convert.ToInt32(value?.R) },
                    { "g", Convert.ToInt32(value?.G) },
                    { "b", Convert.ToInt32(value?.B) },
                    { "a", Convert.ToInt32(value?.A) }
                };

                SetData("color", dict);
            }
        }

        public int? Player
        {
            get
            {
                if (!TryGetData("player", out int player))
                    return null;

                return player;
            }
            set
            {
                if (value == null)
                {
                    SetData("player", null);
                    return;
                }

                if (Player == value)
                    return;

                SetData("player", value);
            }
        }

        public static List<Marker> MarkerList { get; set; }

        public Marker(Vector3 position, int dimension, uint range, ulong entityType) : base(entityType, position, dimension, range)
        {
            EntityType = entityType;
        }

        public void Destroy()
        {
            MarkerList.Remove(this);
            AltEntitySync.RemoveEntity(this);
        }
    }

    public static class MarkerStreamer
    {
        public static Marker Create(
            MarkerTypes markerType, Vector3 position, Vector3 scale, Rgba? color, Vector3? rotation = null, Vector3? direction = null, int dimension = 0,
            bool? bobUpDown = false, bool? faceCamera = false, bool? rotate = false, string textureDict = null, string textureName = null,
            bool? drawOnEnter = false, uint streamRange = 20, int? player = null
        )
        {
            var marker = new Marker(position, dimension, streamRange, 0)
            {
                Rotation = rotation ?? new Vector3(0),
                MarkerType = markerType,
                Direction = direction ?? new Vector3(0),
                Scale = scale,
                Color = color ?? null,
                BobUpDown = bobUpDown ?? null,
                FaceCamera = faceCamera ?? null,
                Rotate = rotate ?? null,
                TextureDict = textureDict ?? null,
                TextureName = textureName ?? null,
                DrawOnEnter = drawOnEnter ?? null,
                Player = player,
            };
            Marker.MarkerList.Add(marker);
            AltEntitySync.AddEntity(marker);
            return marker;
        }
    }
}