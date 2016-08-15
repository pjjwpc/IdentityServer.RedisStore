using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Host.EntityFrameworkCore {
    public class AppUser : IdentityUser<Guid> {
        public AppUser() { }

        public AppUser(string userName) : this() {
            UserName = userName;
        }
    }
}
