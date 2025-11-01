using System;
using System.Text;
using System.Security.Cryptography;

public static class HashProtect
{
    public static string HashSecretData(string datoAEncriptar)
    {
        byte[] datoBytes = Encoding.UTF8.GetBytes(datoAEncriptar);
        byte[] hashBytes = SHA256.HashData(datoBytes);

        return Convert.ToBase64String(hashBytes);
    }
}
