using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3
{
    public class UrlParser
    {
        public bool HasId { get; set; }
        public int Id { get; set; }
        public string Path { get; set; }
        public bool ParseUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var segments = url.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // Must start with api and have at least 2 segments (api + resource)
            if (segments.Length < 2 || segments[0] != "api")
                return false;

            Path = "/" + string.Join('/', segments.Take(2));

            if (segments.Length == 3)
            {
                if (int.TryParse(segments[2], out int id))
                {
                    HasId = true;
                    Id = id;
                }
                else
                {
                    return false; // invalid id
                }
            }
            else
            {
                HasId = false;
            }

            return true;
        }
    }
}
