namespace Example.Server.Models;

class User
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string[] Roles { get; set; }

    public User(string name, string password, params string[] roles)
    {
        Name = name;
        Password = password;
        Roles = roles;
    }
}
