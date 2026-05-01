
using Diva2.Core.Main.Domains;
using Diva2.Core.Main.Users;
using Diva2.Data.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

public class ApplicationDbContext : IdentityDbContext<User8, Role8, int, IdentityUserClaim<int>, UserRoles8, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public string SubDomain { get; private set; } = "";


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainService domainService) : base(options)
    {
        if (domainService == null)
        {
            throw new ArgumentNullException(nameof(domainService));
        }

        SubDomain domain = domainService.Domain;

        if (domain == null)
        {
            // throw new NullReferenceException("Subdomain is null");
        }

        SubDomain = domain.name;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var typesToRegisterAll = Assembly.GetExecutingAssembly().GetTypes().Where(type => !String.IsNullOrEmpty(type.Namespace));
        var typesToRegister2 = typesToRegisterAll.Where(x => x.GetInterfaces().Any(y => y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));
        foreach (var type in typesToRegister2)
        {
            dynamic configurationInstance = Activator.CreateInstance(type);
            modelBuilder.ApplyConfiguration(configurationInstance);
        }

        modelBuilder.Entity<IdentityUserClaim<int>>(b =>
        {
            b.HasKey(uc => uc.Id);
            b.ToTable("spin_userclaim");
        });

        modelBuilder.Entity<IdentityRoleClaim<int>>(b =>
        {
            b.HasKey(rc => rc.Id);
            b.ToTable("spin_roleclaim");
        });

        modelBuilder.Entity<IdentityUserLogin<int>>(b =>
        {
            b.ToTable("spin_userlogin");
        });

        modelBuilder.Entity<IdentityUserToken<int>>(b =>
        {
            b.ToTable("spin_usertoken");
        }); /**/
    }

    public new DbSet<TEntity> Set<TEntity>() where TEntity : class
    {
        return base.Set<TEntity>();
    }

    public string GenerateCreateScript()
    {
        return Database.GenerateCreateScript();
    }

}

