using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using File = Google.Apis.Drive.v3.Data.File;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.Diagnostics;
using Google.Apis.Drive.v3.Data;

namespace SOOS_Auction.AuctionGoogleDrive
{
    public class ApiMethods
    {
        static ApiMethods()
        {
            auctionService = SharedAPIMethods.GetDriveService(SharedAPIMethods.GetUserCredential(scopes),ApplicationName);
        }
        static string FolderMime = "application/vnd.google-apps.folder";

        static string[] scopes = new string[] { DriveService.Scope.Drive,
                                    DriveService.Scope.DriveAppdata,
                                    DriveService.Scope.DriveFile,
                                    DriveService.Scope.DriveMetadataReadonly,
                                    DriveService.Scope.DriveReadonly,
                                    DriveService.Scope.DriveScripts };
        private static string ApplicationName = "SoosAuction";
        private static DriveService auctionService;

        public static IList<File> GetFiles()
        {
            IList<File> files = auctionService.Files.List().Execute().Files;

            foreach (var file in files)
            {
                Debug.WriteLine(file.Id);
            }

            return auctionService.Files.List().Execute().Files;
        }
        public static string CreateFolder()
        {
            var fileMetadata = new File()
            {
                Name = "Invoices",
                MimeType = FolderMime
            };
            var request = auctionService.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();
            return file.Id;
        }
        public static string Upload(string filename, string path, string contentType, string folderId="null")
        {
            var fileMetaData = new File();
            fileMetaData.Name = filename;
            if (folderId != "null") fileMetaData.Parents = new List<string>() { folderId};
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(path, FileMode.Open))
            {
                request = auctionService.Files.Create(fileMetaData, stream, contentType);
                request.Upload();
            }
            var file = request.ResponseBody;
            return file.Id; 
        }
        
    }
}