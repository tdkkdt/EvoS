using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace EvoS.Framework.Misc
{
    public static class DefaultJsonSerializer
    {
        private static JsonSerializer s_serializer;
        private static JsonSerializer s_serializerExtended;

        public static JsonSerializer Get()
        {
            if (s_serializer == null)
            {
                s_serializer = new JsonSerializer();
                s_serializer.NullValueHandling = NullValueHandling.Ignore;
                s_serializer.Converters.Add(new StringEnumConverter());
            }

            return s_serializer;
        }

        public static JsonSerializer GetExtended()
        {
            if (s_serializerExtended == null)
            {
                s_serializerExtended = new JsonSerializer();
                s_serializerExtended.NullValueHandling = NullValueHandling.Ignore;
                s_serializerExtended.Converters.Add(new StringEnumConverter());
                s_serializerExtended.Converters.Add(new VectorJsonConverter());
                s_serializerExtended.Converters.Add(new Vector2JsonConverter());
                s_serializerExtended.Converters.Add(new QuatJsonConverter());
                s_serializerExtended.Converters.Add(new ColorJsonConverter());
            }

            return s_serializerExtended;
        }

        public static string Serialize(object o)
        {
            StringWriter stringWriter = new StringWriter();
            Get().Serialize(stringWriter, o);
            return stringWriter.ToString();
        }

        public static T Deserialize<T>(string json)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            return Get().Deserialize<T>(reader);
        }

        public static T DeserializeExtended<T>(string json)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            return GetExtended().Deserialize<T>(reader);
        }
        
        public class VectorJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                Vector3 v = (Vector3)value;
                new JArray(new float[] { v.X, v.Y, v.Z }).WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                JToken token = JToken.ReadFrom(reader);
                if (token.Type != JTokenType.Array)
                {
                    return new Vector3();
                }

                float[] array = ((JArray)token).Select(t => t.ToObject<float>()).ToArray();
                if (array.Length < 3)
                {
                    return new Vector3();
                }

                return new Vector3(array[0], array[1], array[2]);
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(Vector3) == objectType;
            }
        }

        public class Vector2JsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                Vector2 v = (Vector2)value;
                new JArray(new float[] { v.X, v.Y }).WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                JToken token = JToken.ReadFrom(reader);
                if (token.Type != JTokenType.Array)
                {
                    return new Vector2();
                }

                float[] array = ((JArray)token).Select(t => t.ToObject<float>()).ToArray();
                if (array.Length < 2)
                {
                    return new Vector2();
                }

                return new Vector2(array[0], array[1]);
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(Vector2) == objectType;
            }
        }

        public class QuatJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                Quaternion q = (Quaternion)value;
                new JArray(new float[] { q.X, q.Y, q.Z, q.W }).WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                JToken token = JToken.ReadFrom(reader);
                if (token.Type != JTokenType.Array)
                {
                    return new Quaternion();
                }

                float[] array = ((JArray)token).Select(t => t.ToObject<float>()).ToArray();
                if (array.Length < 4)
                {
                    return new Quaternion();
                }

                return new Quaternion(array[0], array[1], array[2], array[3]);
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(Quaternion) == objectType;
            }
        }

        public class ColorJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                Color q = (Color)value;
                new JArray(new float[] { q.R / 255.0f, q.G / 255.0f, q.B / 255.0f, q.A / 255.0f }).WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                JToken token = JToken.ReadFrom(reader);
                if (token.Type != JTokenType.Array)
                {
                    return new Color();
                }

                float[] array = ((JArray)token).Select(t => t.ToObject<float>()).ToArray();
                if (array.Length < 4)
                {
                    return new Color();
                }

                return Color.FromArgb(
                    (int)(array[3] * 255),
                    (int)(array[0] * 255),
                    (int)(array[1] * 255),
                    (int)(array[2] * 255));
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(Color) == objectType;
            }
        }
    }
}