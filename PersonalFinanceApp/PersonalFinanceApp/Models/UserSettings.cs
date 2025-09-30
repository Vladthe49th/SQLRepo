using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalFinanceApp.Models
{
    public class UserSettings
    {
        public int Id { get; set; }
        public string Currency { get; set; } = "UAH";
        public bool ShowBalanceInStartup { get; set; } = true;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
