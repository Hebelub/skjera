using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using prosjekt.Models;

namespace prosjekt.Data;

public class ApplicationDbInitializer
{
    private static ApplicationUser AddUser(UserManager<ApplicationUser> um, string name, string email)
    {
        var user = new ApplicationUser
            { NickName = name, UserName = email, Email = email, EmailConfirmed = true };
        um.CreateAsync(user, "Password1.").Wait();
        return user;
    }
    
    public static void Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um,
        RoleManager<IdentityRole> rm)
    {
        // Recreate DB during development
        db.Database.EnsureDeleted();

        db.Database.EnsureCreated();

        // Create roles
        var adminRole = new IdentityRole("Admin");
        rm.CreateAsync(adminRole).Wait();

        // Create Standard Users

        var user = AddUser(um, "user", "user@uia.no");
        AddUser(um, "Gabriel Løsnesløkken", "gabriell@uia.no");
        AddUser(um, "Teklit Amanuel", "teklit@uia.no");
        AddUser(um, "Gabriel Løsnesløkken", "gabriell@uia.no");
        AddUser(um, "Aziz", "aziz@uia.no");
        AddUser(um, "Amanuel Hayele Tsegay", "aman3hda@gmail.com");
        AddUser(um, "Barnabas Mulu Boka", "barnabas@gmail.com");
        AddUser(um, "Håvard Berge", "hovardb@uia.no");

        // Add Admin User
        var admin = AddUser(um, "Admin", "admin@uia.no");
        um.AddToRoleAsync(admin, "Admin");


        // Create Organizations
        var organizations = new[]
        {
            new OrganizationModel(
                "Organization 1",
                "Description 1 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod."),
            new OrganizationModel(
                "Organization 2",
                "Description 2 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod."
            ),
            new OrganizationModel(
                "Organization 3",
                "Description 3 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod."
            )
        };

        db.OrganizationModels.AddRange(organizations);

        // Create Events
        var events = new[]
        {
            new EventModel(organizations.ElementAt(0), DateTime.Parse("01/01/2001 01:01:01"),
                TimeSpan.Parse("0:01:00"), DateTime.Parse("01/01/2001"), DateTime.Parse("01/01/2011"),
                "Event 01",
                "Info 1 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(1), DateTime.Parse("01/02/2001 02:02:02"),
                TimeSpan.Parse("0:02:00"), DateTime.Parse("02/02/2002"), DateTime.Parse("01/01/2022"),
                "Event 02",
                "Info 2 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(2), DateTime.Parse("01/03/2001 03:03:03"),
                TimeSpan.Parse("0:03:00"), DateTime.Parse("03/03/2003"), DateTime.Parse("01/01/2033"),
                "Event 03",
                "Info 3 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(2), DateTime.Parse("01/04/2001"), null, DateTime.Parse("04/04/2004"),
                null, "Event 04",
                "Info 4 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(1), DateTime.Parse("01/05/2001"), null, DateTime.Parse("05/05/2005"),
                null, "Event 05",
                "Info 5 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(0), DateTime.Parse("01/06/2001"), null, DateTime.Parse("06/06/2006"),
                null, "Event 06",
                "Info 6 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            )
        };

        db.EventModels.AddRange(events);

        // Add access rights to user
        var accessRights = new[]
        {
            new UserOrganization(user, organizations[1], AccessRight.FullAccess)
        };
        db.UserOrganization.AddRange(accessRights);


        // Create comments
        var comments = new[]
        {
            new Comment(events[0], user, "comment 1"),
            new Comment(events[1], user, "comment 2"),
            new Comment(events[1], user, "comment 3"),
            new Comment(events[2], user, "comment 4"),
            new Comment(events[2], user, "comment 5"),
            new Comment(events[2], user, "comment 6"),
            new Comment(events[3], user, "comment 7"),
            new Comment(events[3], user, "comment 8"),
            new Comment(events[3], user, "comment 9"),
            new Comment(events[3], user, "comment 10"),
        };
        db.Comments.AddRange(comments);


        // Save changes to the database
        db.SaveChanges();
    }
}