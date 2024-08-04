﻿/*
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

class RsaStore
{
    public static byte[] D = { 0x71, 0x07, 0x71, 0x14, 0xD0, 0x8C, 0xEB, 0xC9, 0x8B, 0xE2, 0x06, 0x16, 0x55, 0xB4, 0x43, 0x61,
                               0x10, 0x86, 0x87, 0xE9, 0x01, 0x81, 0x4F, 0x1B, 0x4B, 0x0B, 0x5B, 0x69, 0xCF, 0x62, 0xD1, 0x20,
                               0xF1, 0x39, 0x9E, 0xF8, 0x7D, 0xBE, 0x8E, 0x28, 0xE4, 0x48, 0xFF, 0x62, 0x13, 0xBB, 0xF1, 0x4E,
                               0xC2, 0x26, 0x8D, 0x7C, 0xFF, 0x4E, 0x58, 0x91, 0xCD, 0x44, 0x69, 0x58, 0x5F, 0x83, 0xE8, 0x8F,
                               0x07, 0x8C, 0xFE, 0x3F, 0x58, 0x1F, 0xD8, 0x8A, 0x82, 0x9B, 0xE4, 0x61, 0x39, 0x09, 0x98, 0xFA,
                               0x36, 0xB7, 0xDA, 0x5F, 0x3A, 0x77, 0x2E, 0x1E, 0xCE, 0xC1, 0x52, 0xCD, 0x48, 0x88, 0xF7, 0x71,
                               0x9D, 0x6D, 0xDB, 0x6B, 0x46, 0x73, 0xA9, 0x37, 0x42, 0xAF, 0x0D, 0xCA, 0xA8, 0x39, 0xD2, 0xF8,
                               0x05, 0x59, 0xA2, 0xA0, 0x94, 0x73, 0xC9, 0xD5, 0x71, 0xB4, 0xB4, 0x84, 0xFC, 0xD6, 0xC0, 0xB1,
                               0x8A, 0xD4, 0xEC, 0x05, 0xDD, 0x11, 0xC9, 0x4F, 0xB6, 0xAC, 0x5D, 0x0C, 0xC0, 0x36, 0xF5, 0xB2,
                               0xAD, 0x88, 0x1E, 0xE1, 0x11, 0x60, 0x76, 0x4F, 0xE8, 0xA6, 0xF0, 0x74, 0x82, 0xDA, 0xB0, 0x7F,
                               0x80, 0xFC, 0x3D, 0x12, 0xD6, 0xD5, 0x32, 0x6C, 0x1D, 0xA9, 0x80, 0x3F, 0xAC, 0x32, 0xB3, 0xD3,
                               0xF1, 0x20, 0x52, 0x93, 0xAA, 0x24, 0x59, 0xCE, 0x50, 0x5C, 0x07, 0xFE, 0xCA, 0x25, 0x86, 0x2E,
                               0x04, 0xED, 0x1D, 0xFF, 0xBA, 0xDB, 0x9B, 0x26, 0x04, 0x55, 0xCE, 0x54, 0xF0, 0x7F, 0x5E, 0x44,
                               0xF7, 0x22, 0xDB, 0xA4, 0x1A, 0x76, 0xED, 0x0B, 0xC7, 0x04, 0xAF, 0xD7, 0x23, 0x76, 0x68, 0x20,
                               0xE1, 0x44, 0xAF, 0x90, 0x6A, 0xF5, 0x06, 0x0E, 0xC7, 0x6A, 0x1F, 0x1B, 0xC9, 0x8F, 0xD7, 0xDD,
                               0xF0, 0x6B, 0xAB, 0x5B, 0x9C, 0xA6, 0xA0, 0x52, 0x6A, 0x63, 0x47, 0x87, 0x3F, 0x25, 0x64, 0x55,
                               0xB2, 0x8D, 0x24, 0x93, 0x50, 0x87, 0x24, 0x59, 0x83, 0x1F, 0x5E, 0x82, 0xFB, 0x9C, 0x41, 0xE7,
                               0x9C, 0x0D, 0x02, 0xA3, 0x7F, 0x69, 0x8D, 0xAD, 0x7F, 0xF5, 0x51, 0xC0, 0x28, 0x12, 0x08, 0x05,
                               0xE9, 0xFF, 0x2A, 0xD8, 0xD1, 0x6B, 0x46, 0xF1, 0x20, 0xDE, 0x04, 0x03, 0x9F, 0x44, 0x70, 0xD6,
                               0xD8, 0x37, 0xE4, 0xE6, 0x5A, 0x97, 0x5D, 0xE8, 0x90, 0x25, 0x9F, 0xFC, 0x9E, 0x5C, 0x76, 0xFE,
                               0xDC, 0x0C, 0x0F, 0xBF, 0x53, 0xB3, 0xB2, 0x54, 0x87, 0x0F, 0x7A, 0x57, 0x1E, 0xFC, 0x8F, 0x97,
                               0x4E, 0x5C, 0xCE, 0x9B, 0xEB, 0x98, 0xA2, 0xCC, 0x60, 0x4F, 0xC7, 0x28, 0xF3, 0x41, 0xD3, 0xE7,
                               0x9F, 0x7A, 0x5B, 0xAB, 0xA2, 0x97, 0x46, 0x43, 0x7D, 0xE0, 0x13, 0xC7, 0x0B, 0x66, 0x0B, 0x3A,
                               0xDE, 0xDA, 0x15, 0x1B, 0x8C, 0x88, 0x5C, 0x41, 0x8F, 0xD4, 0x0B, 0x86, 0x09, 0x5E, 0x99, 0xCE,
                               0xBE, 0xE5, 0xBE, 0xDF, 0x06, 0xFD, 0x6C, 0xDD, 0x68, 0xE0, 0x90, 0x6D, 0x9A, 0x70, 0x8A, 0x93,
                               0xF0, 0xBE, 0xA1, 0xA9, 0x4F, 0x10, 0x5A, 0x59, 0x50, 0x5C, 0xA6, 0x14, 0xDF, 0x12, 0xA3, 0x2E,
                               0xF2, 0xE1, 0x34, 0xC0, 0xD0, 0xBC, 0x15, 0x06, 0x3B, 0xB5, 0x7C, 0x1F, 0xDE, 0x4C, 0xC2, 0x2F,
                               0x53, 0x43, 0xA2, 0x6B, 0x52, 0x7E, 0x98, 0xB4, 0xD3, 0x46, 0x0C, 0xC3, 0x7C, 0x63, 0x29, 0x39,
                               0xE7, 0xD3, 0xC2, 0x09, 0xE6, 0x62, 0x12, 0xCB, 0xD3, 0x54, 0xE1, 0x4A, 0xD3, 0x1A, 0x75, 0x1D,
                               0xB6, 0xAA, 0x9E, 0xC9, 0x53, 0xBF, 0xFF, 0xDD, 0xCE, 0x4A, 0x5F, 0xCA, 0xFE, 0x35, 0x68, 0x63,
                               0xA0, 0xDC, 0xD1, 0xA3, 0xFD, 0x94, 0xBB, 0xEA, 0x94, 0x85, 0x18, 0x20, 0x23, 0x95, 0x1B, 0x9F,
                               0x04, 0x00, 0x19, 0x0F, 0x8C, 0xBE, 0x6A, 0xD0, 0x25, 0x8F, 0x46, 0xC8, 0x9E, 0xFC, 0x96, 0x11 };

    public static byte[] DP = { 0x3F, 0x8E, 0x87, 0xAC, 0x4A, 0xEB, 0x29, 0x3B, 0xDB, 0x12, 0x06, 0x7C, 0x44, 0xCF, 0x03, 0x1B,
                                0xBE, 0xF2, 0x3B, 0x40, 0x6A, 0xB1, 0xAA, 0x48, 0x61, 0xEC, 0xB3, 0xB3, 0x08, 0x57, 0x12, 0x6D,
                                0xD6, 0xE6, 0xD5, 0x80, 0xC2, 0x03, 0x18, 0x4B, 0x54, 0xD3, 0xDD, 0x9C, 0x57, 0x87, 0x90, 0x07,
                                0xCD, 0x9E, 0xFA, 0xBB, 0x90, 0x3A, 0x5E, 0xF6, 0x5C, 0x13, 0xF3, 0xEA, 0xAC, 0xCA, 0x9C, 0x41,
                                0x66, 0x6D, 0x60, 0xC7, 0xBF, 0x04, 0x32, 0x84, 0x3B, 0x71, 0xB3, 0xA9, 0xDE, 0xBF, 0xBE, 0x87,
                                0xB7, 0xE4, 0xE9, 0x43, 0xB4, 0x2E, 0x0F, 0xEE, 0xBD, 0x9F, 0x2D, 0x73, 0xC3, 0xC0, 0xD0, 0x7A,
                                0x3C, 0xFA, 0x7A, 0x3C, 0x10, 0xF2, 0xD8, 0x12, 0x48, 0xBD, 0xEE, 0x0C, 0xC9, 0xF2, 0x27, 0x7B,
                                0x32, 0x77, 0x5B, 0xDA, 0xD8, 0x56, 0xFD, 0x37, 0x37, 0x68, 0x41, 0xE8, 0x35, 0x48, 0x5D, 0x4D,
                                0x92, 0xBF, 0xDB, 0xE5, 0xAF, 0xFA, 0x65, 0x5C, 0x42, 0x20, 0x16, 0x04, 0x6D, 0x32, 0xB8, 0x95,
                                0xD3, 0x46, 0x07, 0xFC, 0x11, 0x0E, 0xE6, 0x75, 0x7D, 0xEC, 0x2B, 0xCF, 0xBB, 0x56, 0xC6, 0x00,
                                0x67, 0x4D, 0xA5, 0x8F, 0x29, 0x1B, 0xD1, 0x80, 0x03, 0x59, 0xE1, 0x6A, 0xD9, 0x46, 0xC3, 0x63,
                                0x1B, 0x4A, 0x75, 0x01, 0x3D, 0x47, 0x8C, 0xBB, 0xCC, 0xDD, 0x5C, 0xFB, 0x25, 0x8C, 0xD3, 0xDB,
                                0xA0, 0xD3, 0xCA, 0x98, 0x62, 0xE3, 0x7A, 0xFD, 0xCC, 0x78, 0xDC, 0x27, 0xDD, 0xD9, 0xC7, 0x87,
                                0x5C, 0x49, 0x59, 0x14, 0x6E, 0x6F, 0x4A, 0x5B, 0x16, 0x1B, 0xC0, 0xC6, 0x84, 0x92, 0x70, 0x3B,
                                0x17, 0xC9, 0xFF, 0x08, 0xD4, 0x35, 0x26, 0x6A, 0x97, 0x6B, 0x1D, 0xD2, 0x42, 0xF2, 0x3B, 0x17,
                                0x2C, 0x59, 0x33, 0x17, 0x68, 0x1B, 0x8B, 0xD0, 0xC1, 0xAF, 0x6D, 0x39, 0xFF, 0x46, 0x37, 0x1B };

    public static byte[] DQ = { 0x59, 0x3D, 0xA9, 0xA8, 0xE5, 0x02, 0x76, 0x86, 0x2A, 0xE9, 0x8F, 0x37, 0x42, 0xBA, 0xF8, 0xD5,
                                0x28, 0xE9, 0x08, 0xB1, 0x8B, 0xE6, 0xA6, 0x9C, 0xAC, 0x08, 0x96, 0x74, 0xEA, 0x61, 0x83, 0x97,
                                0xE3, 0x43, 0x18, 0xB0, 0xDB, 0xF2, 0xCD, 0x3B, 0x4C, 0x7F, 0x36, 0x09, 0x75, 0x98, 0x5D, 0xB4,
                                0x01, 0x1A, 0xF3, 0x9D, 0xB0, 0x09, 0xE9, 0x46, 0xC7, 0xB6, 0x8E, 0x26, 0x47, 0x39, 0x2D, 0x75,
                                0x16, 0x15, 0x71, 0xAF, 0x2A, 0xB2, 0x8E, 0x43, 0x35, 0x13, 0x0E, 0x86, 0x1A, 0xE5, 0xFF, 0x25,
                                0x50, 0x6E, 0xA7, 0x15, 0xC8, 0xA0, 0x93, 0x19, 0x70, 0x6C, 0x5D, 0x82, 0xBA, 0xAD, 0x99, 0x7B,
                                0x4A, 0x93, 0xC3, 0x43, 0x5B, 0x0F, 0x4F, 0x74, 0x62, 0x31, 0xE4, 0xED, 0x05, 0x46, 0xAD, 0x0D,
                                0x1D, 0x97, 0x8F, 0xE1, 0xCF, 0xDA, 0x69, 0x8D, 0xEA, 0xEF, 0xA2, 0x52, 0x0A, 0x12, 0x51, 0x32,
                                0x1B, 0x72, 0x72, 0xB6, 0xDA, 0xBA, 0xAB, 0x7D, 0xF7, 0x34, 0xE0, 0xD0, 0x1D, 0x0E, 0x4D, 0x26,
                                0xA6, 0xBD, 0xE5, 0x55, 0xA3, 0x55, 0x34, 0x58, 0xC1, 0x80, 0x38, 0xB8, 0x8F, 0x16, 0xB4, 0xE4,
                                0xD6, 0xD9, 0x17, 0x51, 0x46, 0x08, 0x46, 0x79, 0x3D, 0xA0, 0xFA, 0x5A, 0x29, 0x01, 0xAF, 0xE0,
                                0xD9, 0xE8, 0x3F, 0xDF, 0x52, 0x62, 0x99, 0x63, 0x15, 0x01, 0xF3, 0x48, 0x48, 0x95, 0xFE, 0x5D,
                                0x4C, 0x76, 0xE6, 0x75, 0x94, 0x5D, 0xDD, 0x7A, 0x6A, 0xE1, 0x26, 0x9B, 0x43, 0xD0, 0xEA, 0xC6,
                                0x2C, 0xF3, 0x45, 0x8D, 0x75, 0xD8, 0xE0, 0xD0, 0x3F, 0x05, 0xCA, 0x4D, 0x23, 0x44, 0xF9, 0xDE,
                                0x6E, 0x3F, 0x03, 0x3C, 0xBA, 0x31, 0x34, 0x3E, 0xE3, 0x34, 0x34, 0x34, 0x21, 0x80, 0xB3, 0x0A,
                                0x7D, 0x77, 0xF3, 0x8A, 0xB5, 0x1B, 0xD1, 0x8B, 0x9D, 0x74, 0xDF, 0xA2, 0x61, 0x90, 0x16, 0x08 };

    public static byte[] Exponent = { 0x01, 0x00, 0x01 };
    public static byte[] InverseQ = { 0x6B, 0x7D, 0x7E, 0xC5, 0xB0, 0x44, 0x12, 0xB9, 0xB1, 0x9E, 0xAD, 0x07, 0xBE, 0x41, 0xB9, 0x52,
                                      0x2C, 0x25, 0x4D, 0xEE, 0x3C, 0xF6, 0x51, 0xFD, 0x32, 0x19, 0xDB, 0x62, 0x2D, 0x1B, 0xB9, 0xE9,
                                      0x97, 0x4D, 0x86, 0xBE, 0xD8, 0x7B, 0x23, 0x5C, 0xE4, 0xD0, 0x4F, 0x30, 0x98, 0xBC, 0x0C, 0x8F,
                                      0xDE, 0xAC, 0x94, 0x17, 0x69, 0x09, 0xC4, 0x73, 0xCF, 0x78, 0x91, 0xCC, 0xA8, 0xCE, 0x75, 0x6B,
                                      0x26, 0x2C, 0xD0, 0xA7, 0x5D, 0xBD, 0x8A, 0xE2, 0x68, 0xB6, 0xA9, 0xB1, 0x93, 0xF8, 0x7D, 0x9D,
                                      0xF1, 0x80, 0xA9, 0x2A, 0xC1, 0x31, 0x06, 0x0D, 0x0D, 0x6B, 0x32, 0x91, 0xAE, 0x71, 0x54, 0x61,
                                      0x7C, 0x04, 0xC6, 0x8D, 0x70, 0x4B, 0x2F, 0xAD, 0xB5, 0xCB, 0x51, 0x7E, 0x31, 0xA2, 0xD3, 0xD7,
                                      0x1D, 0xE9, 0x85, 0x6F, 0x92, 0xCA, 0xB2, 0x84, 0x0C, 0x05, 0x1C, 0xEB, 0x80, 0x6F, 0xF4, 0xFA,
                                      0xE0, 0xDC, 0xF8, 0xD5, 0x9F, 0x1B, 0x23, 0xCC, 0x34, 0xEF, 0x9A, 0xBA, 0x12, 0x7D, 0x65, 0x5C,
                                      0x57, 0xA7, 0x06, 0xCD, 0x06, 0x74, 0xB2, 0x11, 0xC7, 0x87, 0x6C, 0x65, 0xFD, 0xC3, 0x18, 0xF5,
                                      0x21, 0x92, 0x25, 0xAE, 0xC0, 0x6F, 0x1D, 0x9E, 0xBA, 0x61, 0x9A, 0x89, 0x6B, 0xAD, 0xC8, 0x0E,
                                      0x65, 0x19, 0xAF, 0x07, 0x7A, 0xCD, 0x3E, 0x50, 0xE5, 0x1D, 0x0C, 0x32, 0x44, 0x70, 0xEA, 0xDB,
                                      0x93, 0x62, 0x86, 0x0A, 0x7A, 0x49, 0x9D, 0x32, 0x5A, 0x49, 0xEA, 0xBE, 0xD1, 0xE6, 0x12, 0x93,
                                      0x82, 0x84, 0x4E, 0x30, 0xDA, 0xF7, 0xAC, 0xF2, 0x29, 0x50, 0xA2, 0xF5, 0x72, 0xDD, 0x00, 0xFD,
                                      0x51, 0x73, 0x08, 0x46, 0xB0, 0xEE, 0x41, 0xE1, 0x71, 0xBB, 0x09, 0xE0, 0xD2, 0xF1, 0x35, 0x55,
                                      0x32, 0xBA, 0x63, 0xF7, 0xD3, 0x24, 0x5D, 0xE4, 0x93, 0x83, 0x92, 0xA1, 0xCE, 0x34, 0x94, 0x9F };

    public static byte[] Modulus = { 0xF5, 0xCF, 0xFD, 0x42, 0xF8, 0x6F, 0xF5, 0x52, 0x31, 0x02, 0x95, 0xFB, 0xC2, 0x7D, 0xB1, 0x48,
                                     0xDB, 0xCE, 0x67, 0x98, 0x7E, 0x32, 0x17, 0x01, 0xB0, 0xA1, 0xEE, 0xCB, 0x45, 0x59, 0x3E, 0x16,
                                     0xC1, 0xBD, 0x61, 0xE7, 0x84, 0xFD, 0xEC, 0x83, 0x62, 0x0F, 0x50, 0x9F, 0xC6, 0x07, 0x59, 0x84,
                                     0xDB, 0xA3, 0xB0, 0xD3, 0x59, 0xE8, 0xCD, 0xA3, 0xCB, 0x54, 0xCE, 0x23, 0xEA, 0xCE, 0xAB, 0x83,
                                     0xBC, 0xF5, 0x0D, 0x40, 0x09, 0x7E, 0x20, 0x36, 0x15, 0x89, 0x50, 0xF2, 0x88, 0x82, 0xFD, 0x9B,
                                     0x6D, 0x5E, 0x7E, 0x1F, 0x34, 0xA0, 0xBF, 0xD3, 0xE3, 0x5F, 0x7C, 0x0C, 0x46, 0x97, 0xBE, 0x3F,
                                     0xF1, 0xBA, 0x2C, 0x2E, 0x14, 0xB3, 0x12, 0x52, 0x5A, 0x16, 0xDE, 0xF6, 0xAF, 0xC0, 0x87, 0x63,
                                     0x65, 0xA0, 0xE1, 0x6E, 0xCE, 0x01, 0xA9, 0x35, 0x52, 0x18, 0x60, 0xEB, 0xA4, 0xC0, 0xCA, 0xE5,
                                     0x79, 0x28, 0xD1, 0x90, 0x84, 0xE4, 0xB9, 0x86, 0x87, 0xED, 0xE2, 0x9D, 0x74, 0x20, 0x57, 0x09,
                                     0xF4, 0x8E, 0xD4, 0x2D, 0x36, 0x9A, 0xDA, 0x80, 0x7E, 0x3B, 0xCA, 0x3E, 0xA8, 0x9A, 0xEA, 0x45,
                                     0x89, 0xE8, 0xE2, 0x42, 0x27, 0x42, 0xF8, 0x75, 0x40, 0xA6, 0x7F, 0xA9, 0x6A, 0xFA, 0x48, 0x8C,
                                     0x29, 0x12, 0x9D, 0x29, 0x5C, 0x7B, 0xA9, 0x8B, 0x46, 0xF3, 0xD3, 0x06, 0xE7, 0x50, 0x58, 0xF8,
                                     0x65, 0x3B, 0x65, 0xAB, 0x71, 0x2F, 0xBB, 0x8D, 0x23, 0xAF, 0x1A, 0x67, 0x82, 0x74, 0xA2, 0xEA,
                                     0xE2, 0x2D, 0x2A, 0xC1, 0xB5, 0x57, 0x9B, 0x32, 0x0D, 0xCF, 0xA2, 0x14, 0x22, 0xF2, 0xB3, 0x48,
                                     0x23, 0xBF, 0xC0, 0x8F, 0x61, 0x69, 0xCA, 0xBB, 0x36, 0x50, 0xE3, 0x18, 0x30, 0x94, 0x86, 0x98,
                                     0xA2, 0xCE, 0x02, 0x15, 0xFE, 0x40, 0xFB, 0x44, 0x68, 0xA7, 0x84, 0x15, 0x70, 0xAD, 0xBB, 0x8F,
                                     0x9E, 0xBA, 0xDF, 0x91, 0x50, 0xBF, 0x49, 0xC7, 0x5A, 0x7D, 0x7B, 0x26, 0x1F, 0x3B, 0x43, 0x28,
                                     0xAE, 0xE6, 0x59, 0x76, 0xDB, 0x32, 0xF1, 0x6B, 0x89, 0x4B, 0x4D, 0x94, 0x56, 0xD5, 0xF4, 0xCA,
                                     0xB3, 0x89, 0x6D, 0x67, 0x69, 0x13, 0xC7, 0x75, 0xF1, 0x07, 0x52, 0xDD, 0x55, 0xF4, 0xF4, 0xCA,
                                     0x41, 0xEE, 0x40, 0x15, 0x5E, 0xF0, 0x86, 0x32, 0x08, 0x6A, 0x41, 0xD6, 0x20, 0x90, 0x3E, 0x5F,
                                     0x0C, 0x20, 0x57, 0x57, 0x0B, 0xEA, 0xC4, 0xE1, 0xFF, 0xE1, 0x6F, 0xCC, 0x29, 0x93, 0xE4, 0x7F,
                                     0x02, 0x9D, 0xF5, 0x64, 0xA8, 0x3C, 0x2B, 0x80, 0x55, 0x8C, 0x1D, 0x1A, 0xB6, 0x5C, 0xD1, 0xEA,
                                     0x3D, 0xB2, 0x5B, 0x71, 0xEE, 0x60, 0xC4, 0x0E, 0xD7, 0xCF, 0x88, 0xC9, 0x56, 0x4E, 0x24, 0x3E,
                                     0x92, 0xD9, 0x6F, 0x00, 0x09, 0xB5, 0x64, 0x66, 0x3D, 0xB7, 0x5B, 0x82, 0x04, 0x79, 0x8A, 0x6D,
                                     0xC7, 0x44, 0x42, 0xE0, 0x9A, 0x4A, 0x3D, 0xD1, 0x06, 0xED, 0x4D, 0x89, 0xEA, 0x5E, 0x4F, 0x93,
                                     0xFE, 0x1F, 0x49, 0xFA, 0x71, 0x60, 0x88, 0x8D, 0x00, 0x1D, 0x8E, 0xB1, 0xE3, 0xDB, 0xD4, 0x1C,
                                     0xEB, 0x86, 0xE4, 0x93, 0x74, 0xAC, 0x38, 0xDC, 0x59, 0xF5, 0x7E, 0x6B, 0x6C, 0x6D, 0x16, 0x1C,
                                     0x09, 0xA4, 0xCB, 0x65, 0x90, 0xED, 0xE5, 0x60, 0xB9, 0xEE, 0xA9, 0x59, 0x0E, 0xA3, 0x4F, 0xC7,
                                     0xFC, 0x20, 0xDF, 0x95, 0x81, 0x50, 0x57, 0xBA, 0x9D, 0x8C, 0xD2, 0xCE, 0xAD, 0x71, 0x7E, 0x0C,
                                     0x73, 0x77, 0x37, 0x0A, 0x07, 0x72, 0xBE, 0x6E, 0x6B, 0xF0, 0x67, 0x93, 0xD6, 0xA6, 0x49, 0x62,
                                     0x05, 0x89, 0x0E, 0xDC, 0x8E, 0x0D, 0x2A, 0x10, 0xCF, 0x5E, 0x2E, 0x83, 0x43, 0x7A, 0xEB, 0xC4,
                                     0x1F, 0x32, 0x43, 0x47, 0xF0, 0x44, 0x23, 0x52, 0xE4, 0x58, 0xF9, 0x26, 0xAA, 0x4A, 0xED, 0x8C };

    public static byte[] P = { 0x53, 0xA0, 0xDA, 0xF7, 0xB7, 0x9C, 0x2B, 0xF4, 0x40, 0x18, 0xC1, 0x20, 0xEB, 0x32, 0x88, 0x19,
                               0x91, 0x80, 0xD7, 0x50, 0x3C, 0x24, 0xF5, 0xF0, 0x77, 0x7C, 0xA8, 0x5F, 0x81, 0xD5, 0x7C, 0xBF,
                               0x11, 0x87, 0x84, 0x88, 0xE7, 0xE0, 0x7E, 0x5A, 0x6D, 0x20, 0x57, 0xC9, 0x0B, 0x7C, 0xED, 0x78,
                               0xA1, 0x86, 0xF8, 0x10, 0x4A, 0xDA, 0xEE, 0x1A, 0xAD, 0x55, 0x3A, 0x0C, 0xF9, 0xD1, 0xDF, 0x8B,
                               0xE4, 0x64, 0x66, 0x49, 0xBF, 0x0E, 0xF7, 0xFA, 0x2A, 0x15, 0x66, 0x47, 0x91, 0x43, 0xCC, 0x90,
                               0x6C, 0x54, 0x71, 0x55, 0x6B, 0x37, 0x38, 0x65, 0x1C, 0x7A, 0xF8, 0x06, 0xD8, 0xBB, 0xAB, 0x3F,
                               0xA8, 0x11, 0x28, 0xE5, 0x37, 0xD7, 0x47, 0x9F, 0xC8, 0x25, 0x16, 0x80, 0x87, 0x69, 0x9C, 0xBE,
                               0x26, 0x07, 0x12, 0x3C, 0x78, 0xC3, 0x8B, 0xD4, 0xCE, 0x21, 0x35, 0x34, 0x1A, 0x54, 0x03, 0x71,
                               0x2A, 0x89, 0x02, 0x53, 0x51, 0xD4, 0x14, 0x09, 0x79, 0xEE, 0xB4, 0x9C, 0xE4, 0x77, 0x7A, 0xF1,
                               0xAB, 0x9E, 0x2F, 0x50, 0x50, 0x04, 0x0C, 0xAD, 0x19, 0x5E, 0x19, 0x91, 0x56, 0xE6, 0x9D, 0x45,
                               0xFA, 0x97, 0x7F, 0x8F, 0x12, 0x8F, 0xF6, 0x24, 0x31, 0xCB, 0x1A, 0xAF, 0x88, 0x7C, 0xBB, 0x67,
                               0x5A, 0xC0, 0x92, 0xC9, 0xF7, 0x36, 0xA9, 0x20, 0xC9, 0x3F, 0xD0, 0x46, 0x92, 0x8B, 0x52, 0x73,
                               0x55, 0x92, 0x19, 0x60, 0x21, 0x81, 0x19, 0x71, 0xED, 0x07, 0xA4, 0xE5, 0x3B, 0x04, 0xDC, 0xF5,
                               0xA9, 0x5E, 0xAA, 0xB9, 0x75, 0x01, 0x37, 0xA9, 0x52, 0x99, 0x70, 0x98, 0x60, 0x00, 0x21, 0x21,
                               0x84, 0x7C, 0xA8, 0x1D, 0x34, 0x37, 0x41, 0x90, 0x30, 0xA9, 0x15, 0x4A, 0xE9, 0xBB, 0x83, 0xC8,
                               0xA9, 0x08, 0xBA, 0x42, 0x3E, 0xB5, 0x01, 0x3D, 0x9B, 0xC7, 0xEF, 0x9D, 0x91, 0x69, 0x38, 0xB7 };

    public static byte[] Q = { 0x97, 0xE5, 0x05, 0x6A, 0xAD, 0x43, 0x85, 0x44, 0xB4, 0x82, 0x77, 0x8E, 0x3E, 0x18, 0x6F, 0x40,
                               0xBF, 0xD9, 0x50, 0x12, 0x95, 0x5E, 0x54, 0x23, 0xF8, 0xFD, 0x95, 0x75, 0x8B, 0xA7, 0xC3, 0x3C,
                               0xFA, 0x45, 0xE7, 0x7D, 0x32, 0x64, 0xC3, 0x4A, 0xC2, 0xB0, 0x50, 0x4E, 0x7D, 0xFF, 0x75, 0x7D,
                               0x8E, 0x9D, 0xFF, 0x8E, 0xD1, 0x63, 0x20, 0x5F, 0x32, 0x1B, 0x4E, 0x6A, 0x1E, 0xC8, 0x2F, 0xAE,
                               0x67, 0x92, 0x01, 0xCD, 0xCC, 0x7E, 0x81, 0x09, 0x98, 0x86, 0x8F, 0x8D, 0x96, 0xAF, 0x59, 0xB7,
                               0xF2, 0x07, 0x49, 0x81, 0x48, 0xCF, 0xB6, 0x4E, 0x72, 0x15, 0x9B, 0xE4, 0x8D, 0x37, 0xE3, 0x95,
                               0xCF, 0x22, 0x68, 0x24, 0x68, 0xA3, 0x1D, 0x33, 0x26, 0x25, 0x70, 0xC1, 0x3E, 0x28, 0xA3, 0xE8,
                               0x6F, 0x23, 0x0E, 0x19, 0x16, 0x15, 0x77, 0x41, 0x09, 0xE7, 0xC4, 0x6B, 0x10, 0xEE, 0x85, 0x11,
                               0xCD, 0x8E, 0x2C, 0xB5, 0x17, 0x9E, 0x42, 0xBF, 0x72, 0x5F, 0x9B, 0x49, 0x3E, 0x94, 0x75, 0x98,
                               0xEC, 0xAB, 0xBF, 0x2D, 0x2A, 0xFC, 0x7F, 0x3B, 0x01, 0x93, 0x3C, 0x0F, 0x4A, 0x91, 0x73, 0x2F,
                               0x66, 0xC3, 0x7A, 0x48, 0xF9, 0x27, 0xBA, 0x97, 0x92, 0x65, 0xE3, 0x09, 0x8F, 0x3F, 0x6F, 0x94,
                               0x5C, 0x1F, 0x34, 0x89, 0x7C, 0x29, 0x4E, 0x64, 0xA4, 0xC9, 0x70, 0x6A, 0x17, 0x3A, 0x99, 0xEA,
                               0x6A, 0x1C, 0x82, 0x45, 0x28, 0x2B, 0xCB, 0xE0, 0xE3, 0x00, 0xE2, 0x2D, 0x22, 0xA1, 0xA4, 0x15,
                               0x2D, 0x38, 0xE9, 0x9B, 0xB5, 0x3C, 0xF6, 0xC3, 0x65, 0x2A, 0x73, 0x66, 0xAE, 0x1A, 0xE1, 0x19,
                               0x96, 0xDD, 0xD6, 0xEA, 0x46, 0x41, 0x50, 0x69, 0x11, 0x4F, 0xB6, 0x9B, 0xFC, 0x75, 0x1F, 0xD0,
                               0xC9, 0x4E, 0xAD, 0xD9, 0xAE, 0x1E, 0xF1, 0x30, 0xBB, 0xD4, 0xB0, 0x55, 0x8F, 0x14, 0xE8, 0xC4 };
}
