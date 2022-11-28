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
                "Studentlaget Grimstad",
                "Vi er en kristen studenorganisasjon som ofte samles i bibel og bønn. I tillegg har vi mange andre gøye aktiviteter, så følg med!"),
            new OrganizationModel(
                "Beta",
                "Description 2 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod."
            ),
            new OrganizationModel(
                "Bedriftpresentasjoner",
                "Description 3 - Vivamus commodo eros eget nunc porttitor lacinia. Aenean erat mi, semper at facilisis quis, feugiat quis ligula. Morbi leo magna, semper ut varius eget, accumsan quis dui. In hac habitasse platea dictumst. Cras feugiat aliquet fringilla. Sed nec vestibulum leo. Nullam sit amet metus viverra erat placerat pretium. Quisque fermentum auctor massa a euismod."
            )
        };

        db.OrganizationModels.AddRange(organizations);

        // Create Events
        var events = new[]
        {
            new EventModel(organizations.ElementAt(0), DateTime.Parse("11/04/2022 01:01:01"),
                TimeSpan.Parse("0:01:00"), DateTime.Parse("01/01/2001"), DateTime.Parse("01/01/2011"),
                "Grilling",
                "Det blir grilling med studentlaget i Grimstad. Kom og bli kjent med oss!",
                "Dømmesmoen"
            ),
            new EventModel(organizations.ElementAt(1), DateTime.Parse("11/09/2022 02:02:02"),
                TimeSpan.Parse("0:02:00"), DateTime.Parse("02/02/2002"), DateTime.Parse("01/01/2022"),
                "Camping",
                "Hei vi skal campe, så bli med på dette! Det blir felleskjøring fra UiA grimstad",
                "Preikestolen"
            ),
            new EventModel(organizations.ElementAt(2), DateTime.Parse("11/01/2022 03:03:03"),
                TimeSpan.Parse("0:03:00"), DateTime.Parse("03/03/2003"), DateTime.Parse("01/01/2033"),
                "Blåtur",
                "Vi skal på blåtur, så bli med på dette! Det vil si at dere ikke vet hva som skjer før dere kommer!",
                "Langtvekkistan"
            ),
            new EventModel(organizations.ElementAt(2), DateTime.Parse("11/30/2022"), null, DateTime.Parse("04/04/2004"),
                null, "Sofagaming",
                "Bli med å game med oss a. Pizza til guttaboisa!",
                "Tønnevoldsgate 26"
            ),
            new EventModel(organizations.ElementAt(1), DateTime.Parse("11/08/2022"), null, DateTime.Parse("05/05/2005"),
                null, "Skihopping",
                "Hvis du aldrig har prøvd å hoppe i ski. Så bli med nå da vel! :)",
                "Hoppbakken i Grimstad"
            ),
            new EventModel(organizations.ElementAt(0), DateTime.Parse("11/08/2022"), null, DateTime.Parse("06/06/2006"),
                null, "Kaiakpadling",
                "Dette kommer til å bli heftig altså. Gjør deg klar til å bli våt og kald!",
                "Stranda"
            ),
            new EventModel(organizations.ElementAt(0), DateTime.Parse("11/28/2022"), null, DateTime.Parse("06/06/2006"),
            null, "Snøballkrig",
            "Vi skal ha snøballkrig! Kom og vær med! Det blir kakao og vafler!",
            "UiA Grimstad"
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