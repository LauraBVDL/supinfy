using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using Supinfy.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using System.IO;

namespace Supinfy.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Connection");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Register()
        {
            User user;
            if (CheckMail(Request.Form["regMail"]))
            {
                user = new User(Request.Form["regUsername"], this.hashPassword(Request.Form["regPassword"]), Request.Form["regName"],
                    Request.Form["regFirstname"], Request.Form["regMail"]);
                ViewData["Username"] = user.username;
                return View("Register");
            } else return null;
        }

        public ActionResult Login()
        {
            PlaylistModel plStarter;
            SoundModel sndStarter;
            User user;
            String password;
            user = new User(Request.Form["logUsername"]);
            if (user != null)
            {
                password = this.hashPassword(Request.Form["logPassword"]);
                if (password.Equals(user.password))
                {
                    ViewData["User"] = user;
                    plStarter = PlaylistModel.List(user.role, user.id);
                    sndStarter = SoundModel.List();
                    return View("Login", Tuple.Create(user, plStarter, sndStarter));
                } else user = null;
            }
            if (user == null) Response.Write("Denied Access");
            return null;
        }

        public ActionResult Uploading()
        {
            CloudBlockBlob block;
            CloudBlobContainer container;
            HttpPostedFileBase file;
            Stream source;
            String targetFileName, filename, title;
            title = Request.Form["title"];
            file = Request.Files["fileToUpload"];
            filename = file.FileName;
            source = file.InputStream;
            targetFileName = filename.Substring(filename.LastIndexOf('\\'));
            container = this.GetCloudBlobContainer();
            container.CreateIfNotExists();
            block = container.GetBlockBlobReference(targetFileName);
            block.UploadFromStream(source);
            Response.Write("APPEL OK " + filename);
            new SoundModel(title, targetFileName);
            return null;
        }

        public void AddToList()
        {
            String add, to;
            int i;
            add = Request.Form["add"]; to = Request.Form["to"];
            i = PlaylistModel.Add(add, to);
            Response.Write((i > 0) ?"OK":"ERROR");
        }

        public void CreateList()
        {
            PlaylistModel plModel;
            String playlist, resp, userId;
            playlist = Request.Form["playlist"];
            userId = Request.Form["user"];
            plModel = new PlaylistModel(int.Parse(userId), playlist);
            if (plModel != null) resp = "<div>" + playlist + "<input type=\"hidden\" value=\"" + plModel.userId + "\"/></div>";
            else resp = "ERROR";
            Response.Write(resp);
        }

        public void GetSounds()
        {
            PlaylistModel plModel;
            String playlist, chaine;
            int i;
            playlist = Request.Form["playlist"];
            plModel = new PlaylistModel(playlist);
            for (i = 0, chaine = ""; i < plModel.idSong.Length; i++)
            {
                if (i > 0) chaine += ",";
                chaine += SoundModel.GetFileName(plModel.idSong[i]);
            }
            Response.Write(chaine);
        }

        public void Increase()
        {
            String result;
            result = SoundModel.Increment(Request.Form["playtime"]);
            Response.Write(result);
        }

        //Les fichiers sont tous stockés sur ce cloud
        //Les accès anonymes sont autorisés
        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("supinfy_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("test-blob-container");
            return container;
        }

        public bool CheckMail(String mail)
        {
            String[] part, subpart;
            bool validity;
            validity = false;
            try
            {
                part = mail.Split('@');
                if (part.Length == 2)
                {
                    if (part[0].Length > 1)
                    {
                        part[1] = part[1].Replace('.', ',');
                        subpart = part[1].Split(',');
                        if (subpart.Length == 2) validity = (subpart[1].Length <= 3) && (subpart[1].Length > 1) && (subpart[0].Length > 1);
                    }
                }
            } catch (Exception e) { Console.Write("Invalid address"); }
            return validity;
        }

        public String hashPassword(String password)
        {
            String hashstring;
            byte[] hash, bytes;
            hashstring = null;
            using (SHA256 sha = SHA256.Create()) {
                bytes = Encoding.ASCII.GetBytes(password);
                hash = sha.ComputeHash(bytes);
                hashstring = Encoding.ASCII.GetString(hash);
                hashstring = hashstring.Replace('\'', 'a');
            }
            return hashstring;
        }
    }
}