using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubInfo
{
    internal class GitUser
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Avatar_url { get; set; }
        public DateTime Created_at { get; set; }
    }
}
