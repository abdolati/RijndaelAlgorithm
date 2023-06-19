using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RijndaelAlgorithm
{
    public partial class RijndaelAlgorithm : Form
    {
        private byte[] originalFileData;
        private byte[] encryptedFileData;
        private byte[] decryptedFileData;

        private byte[] encryptionKey;
        private byte[] encryptionIV;

        public RijndaelAlgorithm()
        {
            InitializeComponent();
        }
        Stopwatch stopwatch = new Stopwatch();

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
            //TimeSpan elapsedTime = TimeSpan.Zero;
            //DateTime start = DateTime.Now;

            if (originalFileData == null)
            {
                MessageBox.Show("Please import a file first.");
                return;
            }
            stopwatch.Start();
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
           
            stopwatch.Stop();

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Encrypted files (.Reg)|.Reg"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                File.WriteAllBytes(fileName, encryptedFileData);
                TxtPathfilEncrypt.Text = fileName;
            }
         
            TxtEncryptTime.Text =$" {stopwatch.Elapsed.TotalSeconds}";        
        }

        private void btndecrypt_Click(object sender, EventArgs e)
        {
       
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
            stopwatch.Stop();
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


            TxtDecryptedFileTime.Text = $" {stopwatch.Elapsed.TotalSeconds}";

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            TxPathFile.Clear();
            TxtDecryptedFileTime.Clear();
            TxtPathfilEncrypt.Clear();
            TxtEncryptTime.Clear();
            TxtPathFileDecrypte.Clear();        
        }
    }
}