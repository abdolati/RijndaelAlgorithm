using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RijndaelAlgorithm
{
    public partial class Form1 : Form
    {
        private byte[] originalFileData;
        private byte[] encryptedFileData;
        private byte[] decryptedFileData;

        private byte[] encryptionKey;
        private byte[] encryptionIV;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnimportFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                originalFileData = File.ReadAllBytes(fileName);
                TxPathFile.Text = fileName;
            }
        }

        private void btnencrypt_Click(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = TimeSpan.Zero;
            DateTime start = DateTime.Now;

            if (originalFileData == null)
            {
                MessageBox.Show("Please import a file first.");
                return;
            }

            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.GenerateKey();
            rijndael.GenerateIV();

            encryptionKey = rijndael.Key;
            encryptionIV = rijndael.IV;

            ICryptoTransform encryptor = rijndael.CreateEncryptor(encryptionKey, encryptionIV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(originalFileData, 0, originalFileData.Length);
                    csEncrypt.FlushFinalBlock();
                    encryptedFileData = msEncrypt.ToArray();
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Encrypted files (.enc)|.enc"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                File.WriteAllBytes(fileName, encryptedFileData);
                TxtPathfilEncrypt.Text = fileName;
            }

            TimeSpan stopTime = DateTime.Now.Subtract(start);
            elapsedTime += stopTime;
            TxtEncryptTime.Text = elapsedTime.ToString();
            //MessageBox.Show("File encryption complete. Time taken: " + elapsedTime.ToString());
        }

        private void btndecrypt_Click(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = TimeSpan.Zero;
            DateTime start = DateTime.Now;


            if (encryptedFileData == null || encryptionKey == null || encryptionIV == null)
            {
                MessageBox.Show("Please encrypt a file first.");
                return;
            }

            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Key = encryptionKey;
            rijndael.IV = encryptionIV;

            ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

            using (MemoryStream msDecrypt = new MemoryStream())
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                {
                    csDecrypt.Write(encryptedFileData, 0, encryptedFileData.Length);
                    csDecrypt.FlushFinalBlock();
                    decryptedFileData = msDecrypt.ToArray();
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Original File Extension |" + Path.GetExtension(TxPathFile.Text)
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                File.WriteAllBytes(fileName, decryptedFileData);
                TxtPathFileDecrypte.Text = fileName;
            }

            TimeSpan stopTime = DateTime.Now.Subtract(start);
            elapsedTime += stopTime;
            TxtDecryptedFileTime.Text = elapsedTime.ToString();

            //MessageBox.Show("File decryption complete. Time taken: " + elapsedTime.ToString());
        }
    }
}
