using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ViewLocalizationSample.Controllers
{
    public class PageController : ApiController
    {
        [HttpGet]
        [Authorize]
        public IHttpActionResult Index()
        {
            Encoding encoding = null;
            var result = "<h1>Protected content...</h1><br /><a href='/applogout'>Logout</a>";

            if (Request.Headers.AcceptLanguage != null)
            {
                var language = Request.Headers.AcceptLanguage.Select(p => p.Value).FirstOrDefault();
                switch (language)
                {
                    case "pt-BR":
                        encoding = Encoding.Default;
                        result = "<h1>Conteúdo protegido...</h1><br /><a href='/applogout'>Sair</a>";
                        break;
                }
            }

            return new HtmlActionResult(result, encoding);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("applogout")]
        public IHttpActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();

            return Redirect(new Uri("/", UriKind.Relative));
            //return Redirect(new Uri("/core/logout", UriKind.Relative));
        }
    }

    public class HtmlActionResult : IHttpActionResult
    {
        private readonly string _content;
        private readonly Encoding _encoding;

        public HtmlActionResult(string content, Encoding encoding = null)
        {
            _content = content;
            _encoding = encoding ?? Encoding.UTF8;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(_content, _encoding) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
        }
    }
}