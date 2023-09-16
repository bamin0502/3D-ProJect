using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace MNF.Crypt
{
    /*
    [Perl]
        use Digest::MD5 qw(md5_hex);
        my $hashString = md5_hex($stringToHash);
    [PHP]
        $hashString = md5($stringToHash);
    [Python]
        import hashlib
        def md5Sum(inputString):
            return hashlib.md5(inputString).hexdigest()
    [Ruby]
        require 'digest/md5'
        def md5Sum(inputString)
            Digest::MD5.hexdigest(inputString)
        end
    [Shell]
        Requires that you have the md5sum program installed on the server.
        HASH = `echo "$STRING_TO_HASH" | md5sum | cut -f 1 -d' '`
     */
    public class MD5Ref
    {
        private MD5CryptoServiceProvider md5 = null;

        public MD5Ref()
        {
            md5 = new MD5CryptoServiceProvider();
        }

        public string Md5SumToString(string strToEncrypt)
        {
            var ue = new UTF8Encoding();

            // encrypt bytes
            var hashBytes = Md5Sum(ue.GetBytes(strToEncrypt));

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');

            return hashString.PadLeft(32, '0');
        }

        public byte[] Md5Sum(string strToEncrypt)
        {
            var ue = new UTF8Encoding();
            return Md5Sum(ue.GetBytes(strToEncrypt));
        }

        public byte[] Md5Sum(byte[] byteToEncrypt)
        {
            return Md5Sum(byteToEncrypt, 0, byteToEncrypt.Length);
        }

        public byte[] Md5Sum(byte[] byteToEncrypt, int offset, int count)
        {
            _checkMd5Sum();
            return md5.ComputeHash(byteToEncrypt, offset, count);
        }

        private void _checkMd5Sum()
        {
            if (md5 == null)
                md5 = new MD5CryptoServiceProvider();
        }

        public void convertToInteger(byte[] encryptedByte, out UInt64 checksum1, out UInt64 checksum2)
        {
            checksum1 = BitConverter.ToUInt64(encryptedByte, 0);
            checksum2 = BitConverter.ToUInt64(encryptedByte, 8);

            //for (int fieldIndex = 0;  fieldIndex < 8; ++fieldIndex)
            //{
            //    checksum1 |= encryptedByte[fieldIndex];
            //    checksum1 <<= 8;
            //}

            //for (int fieldIndex = 8; fieldIndex < 16; ++fieldIndex)
            //{
            //    checksum2 |= encryptedByte[fieldIndex];
            //    checksum2 <<= 8;
            //}
        }
    }
}
