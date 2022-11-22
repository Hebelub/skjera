using Microsoft.AspNetCore.Identity;
using prosjekt.Models;

namespace prosjekt.Data;

public class ApplicationDbInitializer
{
    public static void Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um,
        RoleManager<IdentityRole> rm)
    {
        
        // Recreate DB during development
        db.Database.EnsureDeleted();

        db.Database.EnsureCreated();

        // Create roles
        var adminRole = new IdentityRole("Admin");
        rm.CreateAsync(adminRole).Wait();

        // Create Standard User
        var user = new ApplicationUser
            { UserName = "user@uia.no", Email = "user@uia.no", EmailConfirmed = true };
        um.CreateAsync(user, "Password1.").Wait();

        // Add Admin User
        var admin = new ApplicationUser
            { UserName = "admin@uia.no", Email = "admin@uia.no", EmailConfirmed = true };
        um.CreateAsync(admin, "Password1.").Wait();
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
            new EventModel(organizations.ElementAt(0), DateTime.Parse("01/01/2001"), DateTime.Parse("01/01/2011"), "Event 01", 
                "Description 1 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod.",
                "Info 1 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
                ),
            new EventModel(organizations.ElementAt(1), DateTime.Parse("02/02/2002"), DateTime.Parse("01/01/2022"), "Event 02", 
                "Description 2 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod.",
                "Info 2 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(2), DateTime.Parse("03/03/2003"), DateTime.Parse("01/01/2033"), "Event 03", 
                "Description 3 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod.",
                "Info 3 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(2), DateTime.Parse("04/04/2004"), null, "Event 04", 
                "Description 4 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod.",
                "Info 4 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(1), DateTime.Parse("05/05/2005"), null, "Event 05", 
                "Description 5 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod.",
                "Info 5 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(0), DateTime.Parse("06/06/2006"), null, "Event 06", 
                "Description 6 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod.",
                "Info 6 - Proin rutrum vel velit id consequat. Quisque eu viverra nulla. Sed eu diam sed eros posuere accumsan. Sed dapibus ornare nibh vel volutpat. Nunc sed purus at nisi faucibus porta ut in est. Sed laoreet dui non ex congue tristique. Morbi ullamcorper quis nunc a maximus. Nunc magna tortor, eleifend sed vestibulum vitae, elementum a libero. Nullam faucibus sem eu consectetur congue. Aliquam ante quam, ultricies a semper eget, placerat a risus. Morbi libero nunc, rutrum vitae dui sit amet, mattis iaculis eros. Aenean eget lacinia erat. "
            ),
            new EventModel(organizations.ElementAt(0), DateTime.Now, null, "Test event",
                "Description testing",
                "Info testing"),
            new EventModel(organizations.ElementAt(0), DateTime.Now, null, "Test event 2",
            "Description testing 2",
            "Info testing 2 " +
            "This information is only so that håvard can show his group that the user is able to scroll in this div so that long text does not ruin the partial view"),
            new EventModel(organizations.ElementAt(0), DateTime.Now, null, "Software engineering exam", 
                "Teller 70% av karakteren Teller 70% av karakteren", "Teller 70% av karakteren")
        };
        
        events[6].StartTime = DateTime.Now;
        events[6].EndTime = DateTime.Now + TimeSpan.FromHours(7);
        events[6].Location = "Camp Nou";
        events[7].StartTime = DateTime.Now;
        events[7].EndTime = DateTime.Now + TimeSpan.FromDays(2);
        events[7].Location = "Mount Everest";
        db.EventModels.AddRange(events);

        //Barnabas: 
        var accessRights = new[]
        {
            new UserOrganization(user, organizations[1], AccessRight.FullAccess)
        };
        db.UserOrganization.AddRange(accessRights);


        // Finally save changes to the database
        db.SaveChanges();
    }
}