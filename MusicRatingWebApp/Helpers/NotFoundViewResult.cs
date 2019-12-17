using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MusicRatingWebApp.Helpers
{
    public class NotFoundViewResult : ViewResult
    {
        public NotFoundViewResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int) HttpStatusCode.NotFound;
        }
    }
}
