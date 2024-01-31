using Assignment2.Models;
using Assignment2.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Assignment2.Model
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;

        public AuthDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Membership> Registers { get; set; }
		public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("AuthConnectionString");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Membership>()
                .Property(e => e.CreditCardNo)
                .HasConversion(
                    v => Encrypt(v),   // Function to encrypt
                    v => Decrypt(v));  // Function to decrypt
        }

        private string Encrypt(string input)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateSecureKey();
                aesAlg.IV = GenerateRandomIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                    }
                    return Convert.ToBase64String(aesAlg.IV.Concat(msEncrypt.ToArray()).ToArray());
                }
            }
        }

        private string Decrypt(string input)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GenerateSecureKey();
                aesAlg.IV = Encoding.UTF8.GetBytes(input.Substring(0, 16));  // Assuming IV is stored in the first 16 bytes


                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(input.Substring(16))))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        private byte[] GenerateSecureKey(int keySizeInBytes = 32)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[keySizeInBytes];
                rng.GetBytes(key);
                return key;
            }
        }

        private byte[] GenerateRandomIV()
        {
            // Generate a random initialization vector (IV) for each encryption
            byte[] iv = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }
            return iv;
        }
    }
}
