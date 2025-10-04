using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Assignment3
{
    public class RequestValidator
    {
        public Response ValidateRequest(Request request)
        {
            var list = new List<string>();
            ValidateMethod(request, ref list);
            ValidatePath(request, ref list);
            ValidateDate(request, ref list);
            ValidateBody(request, ref list);

            var response = new Response();
            response.Status = list.Count > 0 ? "4 " + string.Join(", ", list) : "1 Ok"; // Append Bad Request status code if there are errors
            return response;
        }

        #region Method Validation
        private void ValidateMethod(Request request, ref List<string> errorList)
        {
            var methodMissing = string.IsNullOrWhiteSpace(request.Method);
            var methodMissingMessage = "missing method";
            if (methodMissing) errorList.Add(methodMissingMessage);

            var validMethods = new List<string>()
            {
                "create",
                "read",
                "update",
                "delete",
                "echo"
            };
            var methodInvalid = !validMethods.Contains(request.Method);
            var methodInvalidMessage = "illegal method";
            if (methodInvalid) errorList.Add(methodInvalidMessage);
        }


        #endregion

        #region Path Validation
        private void ValidatePath(Request request, ref List<string> errorList)
        {
            if (string.IsNullOrWhiteSpace(request.Path)) { errorList.Add("missing path"); return; }
            if (!(new UrlParser()).ParseUrl(request.Path)) errorList.Add("illegal path");
        }
        #endregion

        #region Date Validation
        private void ValidateDate(Request request, ref List<string> errorList)
        {
            if (string.IsNullOrWhiteSpace(request.Date))
            {
                errorList.Add("missing date");
                return;
            }

            var parseSuccess = long.TryParse(request.Date, out long _);
            if (!parseSuccess) errorList.Add("illegal date");
        }
        #endregion

        #region Body Validation
        private void ValidateBody(Request request, ref List<string> errorList)
        {
            var requiresBody = new List<string>() { "update", "create", "echo" };
            if (!requiresBody.Contains(request.Method)) return;

            if (string.IsNullOrWhiteSpace(request.Body))
            {
                errorList.Add("missing body");
                return;
            }

            try
            {
                var parse = JsonSerializer.Deserialize<object>(request.Body);
            }
            catch (Exception ex)
            {
                errorList.Add("illegal body");
            }
        }


        #endregion
    }
}
