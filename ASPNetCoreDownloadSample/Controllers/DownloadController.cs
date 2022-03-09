using System.IO.Compression;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace ASPNetCoreDownloadSample.Controllers
{
    public class DownloadController : Controller
    {
        public async Task<IActionResult> Index()
        {
            // Azure Storage にあるZIPファイル
            var zipFileName = "XamarinMacAppPipelineTest.app.zip";
            // ZIP ファイル内の Mac アプリファイル名
            var sourceAppFileName = "XamarinMacAppPipelineTest.app";
            // 変更後の Mac アプリファイル名
            var newAppFileName = "XamarinMacAppPipelineTest-1234567890.app";

            // Azure Storage から ZIP ファイルを取得する
            var connectionString = Settings.Instance.StorageConnectionString;
            var blobContainer = new BlobContainerClient(connectionString, "uploads");
            var blobClient = blobContainer.GetBlobClient(zipFileName);
            var result = await blobClient.DownloadStreamingAsync();

            // 取得したZIPファイルの Stream から名前を変更した ZIP を取得する
            var bytes = ChangeZipFolderName(result.Value.Content, sourceAppFileName,newAppFileName);

            // ZIP ファイルとしてダウンロードできる形で返す
            return this.File(bytes, "application/octet-stream", zipFileName);
        }

        byte[] ChangeZipFolderName(Stream sourceStream, string sourceAppFileName, string newAppFileName)
        {
            // ZIP ファイルをオープンする
            using var sourceArchive = new ZipArchive(sourceStream, ZipArchiveMode.Read);

            // 新しいZIPを作成する
            using var memoryStream = new MemoryStream();
            using var newArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);
            foreach (var entry in sourceArchive.Entries)
            {
                // ディレクトリ名を新しいアプリ名に変更する
                // var newEntryName = entry.FullName.Replace(sourceAppFileName, newAppFileName);
                var newEntryName = entry.FullName;

                var newEntry = newArchive.CreateEntry(newEntryName);
                Console.WriteLine($"Entry:{newEntryName}");

                using (var archiveEntry = entry.Open())
                using (var newArchiveEntry = newEntry.Open())
                {
                    archiveEntry.CopyTo(newArchiveEntry);
                }
            }
            newArchive.Dispose();  // これがないと ZIP の最後部分が書き込まれない

            return memoryStream.ToArray();
        }
    }
}
