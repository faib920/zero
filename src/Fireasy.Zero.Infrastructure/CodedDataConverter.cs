using Fireasy.Common.Security;
using Fireasy.Data;

namespace Fireasy.Zero.Infrastructure
{
    class CodedDataConverter : Data.Converter.CodedDataConverter
    {
        const string KEY = "__09d98Iie8*983";

        protected override CodedData DecodeDataFromBytes(byte[] data)
        {
            if (data.Length == 0)
            {
                return null;
            }

            var des = CryptographyFactory.Create(CryptoAlgorithm.DES) as SymmetricCrypto;
            des.SetKey(KEY);

            return new CodedData(des.Decrypt(data));
        }

        protected override byte[] EncodeDataToBytes(CodedData data)
        {
            var des = CryptographyFactory.Create(CryptoAlgorithm.DES) as SymmetricCrypto;
            des.SetKey(KEY);

            return des.Encrypt(data.Data);
        }
    }
}