/*
 * Copyright (C) 2012-2014 Arctium Emulation <http://arctium.org>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Cryptography
{
    public class RsaData
    {
        public RSAParameters RsaParams;

        public RsaData(int keySize = 4096)
        {
            RSACryptoServiceProvider.UseMachineKeyStore = false;

            var rsaProvider = new RSACryptoServiceProvider(keySize);

            rsaProvider.PersistKeyInCsp = false;

            // Execution time depends on the keySize
            RsaParams = rsaProvider.ExportParameters(true);

            // Let's use little-endian
            RsaParams.D        = RsaParams.D.Reverse().ToArray();
            RsaParams.DP       = RsaParams.DP.Reverse().ToArray();
            RsaParams.DQ       = RsaParams.DQ.Reverse().ToArray();
            RsaParams.Exponent = RsaParams.Exponent.Reverse().ToArray();
            RsaParams.InverseQ = RsaParams.InverseQ.Reverse().ToArray();
            RsaParams.Modulus  = RsaParams.Modulus.Reverse().ToArray();
            RsaParams.P        = RsaParams.P.Reverse().ToArray();
            RsaParams.Q        = RsaParams.Q.Reverse().ToArray();

            // We just need it for rsa data generation
            rsaProvider = null;
        }

        void WritePublicByteArray(ref StringBuilder sb, string name, byte[] data)
        {
            sb.AppendFormat("    public static byte[] {0} = {{ ", name);

            for (int i = 0; i < data.Length; i++)
            {
                if (i == data.Length - 1)
                    sb.Append(string.Format("0x{0:X2} }};", data[i]));
                else
                    sb.Append(string.Format("0x{0:X2}, ", data[i]));
            }

            sb.AppendLine();
        }

        public void WriteRSAParamsToFile(string file)
        {
            using (var sw = new StreamWriter(new FileStream(file, FileMode.Append, FileAccess.Write)))
            {
                var sb = new StringBuilder();

                sb.AppendLine("class RsaStore");
                sb.AppendLine("{");

                // Write all private & public rsa parameters.
                WritePublicByteArray(ref sb, "D", RsaParams.D);
                WritePublicByteArray(ref sb, "DP", RsaParams.DP);
                WritePublicByteArray(ref sb, "DQ", RsaParams.DQ);
                WritePublicByteArray(ref sb, "Exponent", RsaParams.Exponent);
                WritePublicByteArray(ref sb, "InverseQ", RsaParams.InverseQ);
                WritePublicByteArray(ref sb, "Modulus", RsaParams.Modulus);
                WritePublicByteArray(ref sb, "P", RsaParams.P);
                WritePublicByteArray(ref sb, "Q", RsaParams.Q);

                sb.AppendLine("}");

                sw.WriteLine(sb.ToString());
            }

            // Reset all values
            RsaParams = new RSAParameters();
        }
    }
}
