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
        public string Id { get; set; }
        public string Path { get; set; }
        public bool ParseUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            var split = url.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            if (split.Length < 2 || split.Length > 3) return false;

            if (split.Length == 2)
            {
                HasId = false;
            }
            else
            {
                var last = split[^1];
                HasId = int.TryParse(last, out int tempId);
                if (HasId)
                {
                    Id = tempId.ToString();
                }
            }

            Path = "/" + string.Join('/', split[0], split[1]);
            return true;
        }
    }
}
