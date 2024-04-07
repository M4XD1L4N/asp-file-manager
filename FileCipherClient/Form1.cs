using System.Collections.Specialized;
using System.Text.Json;
using System.Text;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

namespace FileCipherClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            updateMainPanel();
        }



        private void uploadCB_CheckedChanged(object sender, EventArgs e)
        {
            if (uploadCB.Checked)
            {
                downloadCB.Checked = false;
                updateMainPanel();
            }
        }

        private void downloadCB_CheckedChanged(object sender, EventArgs e)
        {
            if (downloadCB.Checked)
            {
                uploadCB.Checked = false;
                updateMainPanel();
            }
        }

        private void updateMainPanel()
        {
            if (!uploadCB.Checked && !downloadCB.Checked)
                return;

            if (uploadCB.Checked)
            {
                infoLbl.Text = "Upload Mode";

            }
            else
            {
                infoLbl.Text = "Download Mode";
                startBtn.Enabled = false;
            }
        }

        static double BytesToMb(long value)
        {
            return ((double)value) / 1024 / 1024;
        }

        private void openPathBtn_Click(object sender, EventArgs e)
        {
            if (uploadCB.Checked)
            {
                openFileDialog.InitialDirectory = filePath.Text;
                openFileDialog.Filter = "All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath.Text = openFileDialog.FileName;
                    var info = new FileInfo(openFileDialog.FileName);
                    fileInfo.Text = $"fileName: {info.Name} \n"
                                  + $"fileSize: {BytesToMb(info.Length)} Mb\n"
                                  + $"creationTime: {info.CreationTime} \n"
                                  + $"last updated time: {info.LastWriteTime} \n";
                }
                startBtn.Enabled = true;
            }
            else if (downloadCB.Checked)
            {
                folderBrowserDialog1.SelectedPath = filePath.Text;
                folderBrowserDialog1.InitialDirectory = filePath.Text;
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    filePath.Text = folderBrowserDialog1.SelectedPath;
                }
            }
        }

        private static readonly string serverName = "https://localhost:7157";
        private static readonly HttpClient client = new HttpClient();
        private static readonly int dataLimit = 1 * 1024 * 1024;
        private void startBtn_ClickAsync(object sender, EventArgs e)
        {
            if (uploadCB.Checked)
            {
                _ = startSend();
            }
            else
            {
                _ = startDownload();
            }
        }

        int totalChunks = 0;
        int completedChunks = 0;

        private async Task startDownload()
        {
            mainPB.Value = 0;
            completedChunks = 0;
            int bytesRead;
            while (completedChunks < totalChunks)
            {
                var chunk = await GetChunk(completedChunks);
                var decryptChunk = DecodeBuffer(chunk);
                StoreData(decryptChunk);
            }
            mainPB.Value = 100;
        }

        string fileName = "1.txt";
        private void StoreData(string chunk)
        {
            BinaryWriter binWriter = new BinaryWriter(System.IO.File.Open(Path.Combine(folderBrowserDialog1.SelectedPath, fileName), FileMode.Append));
            //$todo Decrypt chunk with priKey
            var originData =Convert.FromHexString(chunk);
            binWriter.Write(originData);
            binWriter.Close();
        }

        private async Task<string> GetChunk(int chunkId)
        {
            string result = null;
            using StringContent jsonContent = new(
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    FileId = DownloadFileId,
                    Pos = chunkId,
                }),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(serverName + "/api/Download/get-chunk", jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var definition = new { Data  = new {   Id = 0, FileId =0, Pos = 0, Data = "" }, Status = 0 };
            var parsedData = JsonConvert.DeserializeAnonymousType(responseString, definition);
            if (parsedData.Status == 200 && parsedData.Data.Pos == chunkId)
            {
                result = parsedData.Data.Data;
                completedChunks++;
                mainPB.Value = completedChunks * 100 / totalChunks;
            }

            return result;
        }

        private async Task startSend()
        {
            totalChunks = 0;
            completedChunks = 0;
            var info = new FileInfo(filePath.Text);
            using StringContent jsonContent = new(
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    FileName = info.Name,
                    CipherType = 0,
                    Size = info.Length
                }),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(serverName + "/api/Load/start-send-file", jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var definition = new { FileId = 0, Status = 0, PubKey = "" };
            var parsedData = JsonConvert.DeserializeAnonymousType(responseString, definition);
            if (parsedData.Status == 200)
            {
                downloadPriKey = parsedData.PubKey;
                //$todo: you got public hex key in parsed data
                totalChunks = (int)Math.Ceiling((float)info.Length / dataLimit);
                for (int i = 0; i < totalChunks; ++i)
                {
                    var buffer = ReadFile(info, i);
                    var encodedBuffer = EncodeBuffer(buffer);
                    //await 
                    SendChunk(parsedData.FileId, i, encodedBuffer);
                }
            }
        }

        private string EncodeBuffer(string buffer)
        {
            var definition = new { AES = "", IV = "" };
            var parsedData = JsonConvert.DeserializeAnonymousType(downloadPriKey, definition);
            var encryptedBuffer = CryptoChannel.AES.EncryptStringToBytes_Aes(buffer,
                                                         Convert.FromHexString(parsedData.AES),
                                                         Convert.FromHexString(parsedData.IV));
            return Convert.ToHexString(encryptedBuffer);
        }

        private string DecodeBuffer(string buffer)
        {
            var definition = new { AES = "", IV = "" };
            var parsedData = JsonConvert.DeserializeAnonymousType(downloadPriKey, definition);
            var decryptedBuffer = CryptoChannel.AES.DecryptStringFromBytes_Aes(Convert.FromHexString(buffer),
                                                         Convert.FromHexString(parsedData.AES),
                                                         Convert.FromHexString(parsedData.IV));
            return decryptedBuffer;
        }

        private async Task SendChunk(int fileId, int chunkPos, string buffer)
        {
            using StringContent jsonContent = new(
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    ChunkPos = chunkPos,
                    FileId = fileId,
                    Data = buffer
                }),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(serverName + "/api/Load/send-chunk", jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var definition = new { Status = 0 };
            var parsedData = JsonConvert.DeserializeAnonymousType(responseString, definition);
            if (parsedData.Status == 200)
            {
                completedChunks++;
                mainPB.Value = (100 * completedChunks) / totalChunks;
                if (completedChunks == totalChunks)
                {
                    mainPB.Value = 100;
                    SendFileFinished(fileId);
                }
            }
        }

        private async Task SendFileFinished(int fileId)
        {
            using StringContent jsonContent = new(
            System.Text.Json.JsonSerializer.Serialize(new
            {
                FileId = fileId,
            }),
            Encoding.UTF8,
            "application/json");

            var response = await client.PostAsync(serverName + "/api/Load/end-send-file", jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var definition = new { Status = 0, AccessKey = "" };
            var parsedData = JsonConvert.DeserializeAnonymousType(responseString, definition);
            if (parsedData.Status == 200)
            {
                accessTokenTB.Text = parsedData.AccessKey;
            }
        }


        public static string ReadFile(FileInfo fileInfo, int chunkId)
        {
            int remainderBuffer = (int)fileInfo.Length - chunkId * dataLimit;
            int resultSize = remainderBuffer > dataLimit ? dataLimit : remainderBuffer;
            byte[] buffer = new byte[resultSize];
            FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            System.IO.BinaryReader binReader = new System.IO.BinaryReader(fileStream, Encoding.ASCII);
            try
            {
                fileStream.Seek(chunkId * dataLimit, SeekOrigin.Begin);
                binReader.Read(buffer, 0, resultSize);
            }
            finally
            {
                binReader.Close();
                fileStream.Close();
            }

            return Convert.ToHexString(buffer, 0, resultSize);
        }

        private void accessTokenTB_TextChanged(object sender, EventArgs e)
        {
            startBtn.Enabled = false;
            if (accessTokenTB.Text.Length < 64)
            {
                return;
            }
            _ = CheckDownload(accessTokenTB.Text);

        }

        int DownloadFileId = 0;
        string downloadPriKey;

        private async Task CheckDownload(string _accessKey)
        {
            using StringContent jsonContent = new(
            System.Text.Json.JsonSerializer.Serialize(new
            {
                accessKey = _accessKey,
            }),
            Encoding.UTF8,
            "application/json");
            

            var response = await client.PostAsync(serverName + "/api/Download/send-access-key", jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var definition = new { data = new { id = 0, name = "", size = 0 }, Status = 0, numberOfChunks = 0, privateKey = "" };
            var parsedData = JsonConvert.DeserializeAnonymousType(responseString, definition);
            if (parsedData.Status == 200)
            {
                DownloadFileId = parsedData.data.id;
                totalChunks = parsedData.numberOfChunks;
                startBtn.Enabled = true;
                fileName = parsedData.data.name;
                downloadPriKey = parsedData.privateKey;
                fileInfo.Text = $"SaveTo: {folderBrowserDialog1.SelectedPath + "/" + parsedData.data.name} \n"
                              + $"fileName: {parsedData.data.name} \n"
                              + $"fileSize: {BytesToMb(parsedData.data.size)} Mb\n"
                              + $"numberOfChunks: {parsedData.numberOfChunks} \n";
            }
        }

    }
}