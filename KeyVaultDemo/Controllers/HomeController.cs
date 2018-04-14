using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using static Microsoft.Azure.KeyVault.KeyVaultClient;

namespace KeyVaultDemo.Controllers {

	// localhost:port/api/home
	[RoutePrefix("api/")]
	public class HomeController : ApiController {
		// GET api
		public async Task<IHttpActionResult> Get() {
			AuthenticationCallback keyVaultCallback = new AuthenticationCallback(GetAccessTokenAsync);
			KeyVaultClient keyVaultClient = new KeyVaultClient(keyVaultCallback);

			string dbConnectionSecretId = WebConfigurationManager.AppSettings["DbConnectionSecretIdentifier"];
			string adminPasswordSecretId = WebConfigurationManager.AppSettings["AdminPasswordSecretIdentifier"];

			SecretBundle keyValueDbPassphraseSecret = await keyVaultClient.GetSecretAsync(dbConnectionSecretId);
			SecretBundle keyValueAdminPasswordSecret = await keyVaultClient.GetSecretAsync(adminPasswordSecretId);

			return Ok(new {
				DbPassphrase = keyValueDbPassphraseSecret.Value,
				AdminPassword = keyValueAdminPasswordSecret.Value
			});
		}


		public static async Task<string> GetAccessTokenAsync(string authority, string resource, string scope) {
			var clientId = WebConfigurationManager.AppSettings["ClientID"];
			var clientSecret = WebConfigurationManager.AppSettings["ClientSecret"];

			var context = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
			ClientCredential credential = new ClientCredential(clientId, clientSecret);
			AuthenticationResult result = await context.AcquireTokenAsync(resource, credential);

			if (result == null)
				throw new InvalidOperationException("Failed to obtain the JWT token");

			return result.AccessToken;
		}


	} // HomeController



} // Controllers
