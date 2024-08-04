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

namespace Framework.Cryptography
{
    public sealed class SARC4
    {
        byte[] s;
        byte tmp, tmp2;

        public SARC4()
        {
            s = new byte[0x100];
            tmp = 0;
            tmp2 = 0;
        }

        public void PrepareKey(byte[] key)
        {
            for (int i = 0; i < 0x100; i++)
                s[i] = (byte)i;

            var i2 = 0;

            for (int i = 0; i < 0x100; i++)
            {
                i2 = (byte)((i2 + s[i] + key[i % key.Length]) % 0x100);

                var tempS = s[i];

                s[i] = s[i2];
                s[i2] = tempS;
            }
        }

        public void ProcessBuffer(byte[] data, int length)
        {
            for (int i = 0; i < length; i++)
            {
                tmp = (byte)((tmp + 1) % 0x100);
                tmp2 = (byte)((tmp2 + s[tmp]) % 0x100);

                var sTemp = s[tmp];

                s[tmp] = s[tmp2];
                s[tmp2] = sTemp;

                data[i] = (byte)(s[(s[tmp] + s[tmp2]) % 0x100] ^ data[i]);
            }
        }
    }
}
