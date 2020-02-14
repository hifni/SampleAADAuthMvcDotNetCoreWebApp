using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SampleAADAuthMvcDotNetCoreWebApp.Services;
using Graph = Microsoft.Graph;
using Microsoft.Identity.Web;
using System.IO;
using SampleAADAuthMvcDotNetCoreWebApp.Infrastructure;

namespace SampleAADAuthMvcDotNetCoreWebApp.Controllers
{
    public class UserProfileController : Controller
    {
        readonly ITokenAcquisition tokenAcquisition;
        readonly WebOptions webOptions;

        public async Task<IActionResult> Index()
        {
            // Initialize the GraphServiceClient. 
            Graph::GraphServiceClient graphClient = GetGraphServiceClient(new[] { Constants.ScopeUserRead });

            var me = await graphClient.Me.Request().GetAsync();
            ViewData["Me"] = me;

            try
            {
                // Get user photo
                using (var photoStream = await graphClient.Me.Photo.Content.Request().GetAsync())
                {
                    byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                    ViewData["Photo"] = Convert.ToBase64String(photoByte);
                }
            }
            catch (System.Exception)
            {
                ViewData["Photo"] = null;
            }

            return View();
        }

        private Graph::GraphServiceClient GetGraphServiceClient(string[] scopes)
        {
            return GraphServiceClientFactory.GetAuthenticatedGraphClient(async () =>
            {
                string result = await tokenAcquisition.GetAccessTokenOnBehalfOfUserAsync(scopes);
                return result;
            }, "https://graph.microsoft.com/beta");
        }
    }
}