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
    public class SharedAPIMethods
    {
        public static Permission CreatePermission(string type, string role)
        {
            Permission permission = new Permission();
            permission.Type = type;
            permission.Role = role;
            return permission;
        }
        public static Permission CreatePublicPermission()
        {
            Permission publicPermission = CreatePermission("anyone", "reader");
            publicPermission.AllowFileDiscovery = true;
            return publicPermission;
        }
        public static Permission MakeFilePublic(string fileId, DriveService service)
        {
            var request = service.Permissions.List(fileId);
            var res = service.Permissions.List(fileId).Execute();
            var hasReadPermission = res.Permissions.Any(p => p.Role == "reader");
            if (!hasReadPermission)
            {
                var publicPermission = CreatePublicPermission();
                var result = service.Permissions.Create(publicPermission, fileId).Execute();
                return result;
            }
            return null;
        }

        public static UserCredential GetUserCredential(string[] scopes)
        {
            string serverPath = AppDomain.CurrentDomain.BaseDirectory;
            using (var stream = new FileStream(serverPath + "/AuctionGoogleDrive/client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string CredentialPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                CredentialPath = Path.Combine(CredentialPath, "driveApiCredentials", "drive-credentials.json");
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "User",
                  CancellationToken.None,
                  new FileDataStore(CredentialPath, true)).Result;
            }
        }
        public static DriveService GetDriveService(UserCredential credential, string ApplicationName)
        {
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            }
                );

        }
    }
}