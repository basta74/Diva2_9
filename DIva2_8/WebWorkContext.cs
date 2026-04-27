using Diva2.Core;
using Diva2.Core.Main;
using Diva2.Core.Main.Users;
using Microsoft.AspNetCore.Identity;

public class WebWorkContext : IWorkContext
{
    private readonly UserManager<User8> userManager;
    private readonly IHttpContextAccessor httpContextAccesso;
    private User8 cachedUser;

    public WebWorkContext(IHttpContextAccessor httpContextAccesso)
    {

        this.httpContextAccesso = httpContextAccesso;
    }

    public User8 CurrrentUser
    {


        get
        {
            User8 user = new User8();
            /*
              if (cachedUser != null)
                  return cachedUser;
              User8 user = null;
              if (user == null)
              {
                  //try to get registered user
                  user = userManager.GetUserAsync(httpContextAccesso.HttpContext.User).Result;
              }
              /**/
            cachedUser = user;
            return cachedUser;
        }
        set
        {
            cachedUser = value;
        }
    }
    public Pobocka CurentPobocka { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string CurrentDomain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}