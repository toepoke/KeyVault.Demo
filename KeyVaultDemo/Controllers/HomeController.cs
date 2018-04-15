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

		// localhost:port/api/home
		public async Task<IHttpActionResult> Get() {
			AuthenticationCallback keyVaultCallback = new AuthenticationCallback(GetAccessTokenAsync);
			KeyVaultClient keyVaultClient = new KeyVaultClient(keyVaultCallback);

			string usernameSecretId = WebConfigurationManager.AppSettings["UsernameSecretIdentifier"];
			string passwordSecretId = WebConfigurationManager.AppSettings["PasswordSecretIdentifier"];

			SecretBundle keyValueDbPassphraseSecret = await keyVaultClient.GetSecretAsync(usernameSecretId);
			SecretBundle keyValueAdminPasswordSecret = await keyVaultClient.GetSecretAsync(passwordSecretId);

			return Ok(new {
				DbPassphrase = keyValueDbPassphraseSecret.Value,
				AdminPassword = keyValueAdminPasswordSecret.Value
			});
		}


		public static async Task<string> GetAccessTokenAsync(string authority, string resource, string scope) {
			var clientId = WebConfigurationManager.AppSettings["ClientID"];
			var clientSecret = WebConfigurationManager.AppSettings["ClientSecret"];

			var context = new AuthenticationContext(authority);
			ClientCredential credential = new ClientCredential(clientId, clientSecret);
			AuthenticationResult result = await context.AcquireTokenAsync(resource, credential);

			if (result == null)
				throw new InvalidOperationException("Failed to obtain the JWT token");

			return result.AccessToken;
		}


	} // HomeController



} // Controllers
