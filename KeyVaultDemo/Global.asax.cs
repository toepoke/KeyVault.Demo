using System.Web.Http;

namespace KeyVaultDemo {
	public class WebApiApplication : System.Web.HttpApplication {
		protected void Application_Start() {
			GlobalConfiguration.Configure(WebApiConfig.Register);
			//FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
		}
	}
}
