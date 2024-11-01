using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avtorizaciya_gostinica.ApplicationData
{
    internal class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; } 
        public string Role { get; set; } // "Администратор" или "Пользователь"
        public bool IsBlocked { get; set; }
        public int LoginAttempts { get; set; }
    }
}
