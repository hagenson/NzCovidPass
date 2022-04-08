
using System;
using System.Collections.Generic;
using System.Linq;
using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Converters;

namespace NzCovidPass.Core.Cbor
{
    /// <summary>
    /// Extension methods for <see cref="CborReader" />.
    /// </summary>
    internal static class CborReaderExtensions
    {
        /// <summary>
        /// Reads a <see cref="CborObject" /> from the reader.
        /// </summary>
        /// <param name="reader">The <see cref="CborReader" /> to read from.</param>
        /// <returns>The next CBOR encoded object.</returns>
        public static CborObject ReadObject(this CborReader reader)
        {
            var state = reader.GetCurrentDataItemType();

            // Currently only supporting what is needed for this library.
            switch (state)
            {
                case CborDataItemType.Map:
                    return reader.ReadMap();
                case CborDataItemType.Array:
                    return reader.ReadArray();
                case CborDataItemType.String:
                    return new CborTextString(reader.ReadString());
                case CborDataItemType.ByteString:
                    return new CborByteString(reader.ReadByteString().ToArray());
                case CborDataItemType.Unsigned:
                case CborDataItemType.Signed:
                    return new CborInteger(reader.ReadInt32());
                default:
                    throw new NotSupportedException($"Unexpected reader state '{state}'.");
            };
        }

        /// <summary>
        /// Reads a <see cref="CborArray" /> from the reader.
        /// </summary>
        /// <param name="reader">The <see cref="CborReader" /> to read from.</param>
        /// <returns>The next CBOR encoded array.</returns>
        public static CborArray ReadArray(this CborReader reader)
        {
            var conv = (ICborConverter<Dahomey.Cbor.ObjectModel.CborArray>) new CborValueConverter(new Dahomey.Cbor.CborOptions { });
            var raw = conv.Read(ref reader);

            return new CborArray(raw.Select(x => x.ToCbor()));
        }


        /// <summary>
        /// Reads a <see cref="CborMap" /> from the reader.
        /// </summary>
        /// <param name="reader">The <see cref="CborReader" /> to read from.</param>
        /// <returns>The next CBOR encoded map.</returns>
        public static CborMap ReadMap(this CborReader reader)
        {
            var conv = (ICborConverter<Dahomey.Cbor.ObjectModel.CborObject>) new CborValueConverter(new Dahomey.Cbor.CborOptions { });
            var raw = conv.Read(ref reader);

            return new CborMap(raw.ToDictionary(p => p.Key.ToCbor(), p => p.Value.ToCbor()));
        }

        /// <summary>
        /// Attempts to read a <see cref="CborArray" /> from the reader.
        /// </summary>
        /// <param name="reader">The <see cref="CborReader" /> to read from.</param>
        /// <param name="array">The array that was read, if any.</param>
        /// <returns><see langword="true" /> if an array was read; <see langword="false" /> otherwise.</returns>
        public static bool TryReadArray(this CborReader reader, out CborArray array)
        {
            try
            {
                array = reader.ReadArray();

                return true;
            }
            catch
            {
                array = null;

                return false;
            }
        }

        /// <summary>
        /// Attempts to read a <see cref="CborMap" /> from the reader.
        /// </summary>
        /// <param name="reader">The <see cref="CborReader" /> to read from.</param>
        /// <param name="map">The map that was read, if any.</param>
        /// <returns><see langword="true" /> if a map was read; <see langword="false" /> otherwise.</returns>
        public static bool TryReadMap(this CborReader reader, out CborMap map)
        {
            try
            {
                map = reader.ReadMap();

                return true;
            }
            catch
            {
                map = null;

                return false;
            }
        }

        public static CborObject ToCbor(this Dahomey.Cbor.ObjectModel.CborValue val)
        {
            switch (val.Type)
            {
                case Dahomey.Cbor.ObjectModel.CborValueType.ByteString:
                    return new CborByteString(val.Value<System.ReadOnlyMemory<byte>>().ToArray());
                case Dahomey.Cbor.ObjectModel.CborValueType.Boolean:
                    return new CborInteger(val.Value<Int32>());
                case Dahomey.Cbor.ObjectModel.CborValueType.String:
                    return new CborTextString(val.Value<string>());
                case Dahomey.Cbor.ObjectModel.CborValueType.Object:
                    var obj = (Dahomey.Cbor.ObjectModel.CborObject) val;
                    return new CborMap(obj.Keys
                        .ToDictionary(k => k.ToCbor(), k => obj[k].ToCbor()));
                case Dahomey.Cbor.ObjectModel.CborValueType.Positive:
                case Dahomey.Cbor.ObjectModel.CborValueType.Negative:
                case Dahomey.Cbor.ObjectModel.CborValueType.Single:
                case Dahomey.Cbor.ObjectModel.CborValueType.Double:
                case Dahomey.Cbor.ObjectModel.CborValueType.Decimal:
                    return new CborInteger(val.Value<Int32>());
                case Dahomey.Cbor.ObjectModel.CborValueType.Array:
                    var ary = (Dahomey.Cbor.ObjectModel.CborArray) val;
                    return new CborArray(ary.Select(v => v.ToCbor()));
                case Dahomey.Cbor.ObjectModel.CborValueType.Null:
                    return null;
                default:
                    throw new InvalidOperationException("Unhandled data type: " + val.Type.ToString());
            }
        }

    }
}
