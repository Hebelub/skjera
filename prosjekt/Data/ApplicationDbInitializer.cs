using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using prosjekt.Models;

namespace prosjekt.Data;

public class ApplicationDbInitializer
{
    private static Random s_random = new();
    
    private static ApplicationUser AddUser(UserManager<ApplicationUser> um, string name, string email)
    {
        var user = new ApplicationUser
            { NickName = name, UserName = email, Email = email, EmailConfirmed = true };
        um.CreateAsync(user, "Password1.").Wait();
        return user;
    }

    private static string GetRandomWord(bool capitalizeFirst=false)
    {
        List<string> wordList = new()
        {
            "valley", "position", "welcome", "steam", "charm", "descent", "switch", "us", "cycle", "compromise",
            "situation", "likely", "variation", "pressure", "vat", "manner", "brilliance", "rifle", "draw", "function",
            "remember", "grudge", "examination", "age", "discriminate", "salvation", "tin", "pier", "division", "left",
            "umbrella", "straight", "on", "era", "delete", "pleasant", "grind", "fire", "turkey", "dictionary"
        };

        var randomWord = wordList[s_random.Next(wordList.Count)];
        if (capitalizeFirst && !string.IsNullOrEmpty(randomWord))
        {
            randomWord = randomWord[0].ToString().ToUpper() + randomWord.Substring(1);
        }

        return randomWord;
    }

    private static string GetRandomSentence(int numWords)
    {
        if (numWords == 0) 
            return "";
        var s = GetRandomWord(true);
        for (var i = 0; i < numWords; ++i)
            s += " " + GetRandomWord();
        s += ".";
        return s;
    }

    private static EventModel CreateRandomEvent(List<OrganizationModel> organizations)
    {
        // Get Random Organization
        var randomOrganization = organizations[s_random.Next(organizations.Count)];

        // Start Time
        var startTime = DateTime.Now.Date + TimeSpan.FromHours((float)s_random.Next(-5000, 5000) / 4);
        
        // Complete the Random Event
        var randomEvent = new EventModel
        {
            Title = GetRandomWord(true),
            Info = GetRandomSentence(s_random.Next(30) + 5),
            Organizer = randomOrganization,
            OrganizerId = randomOrganization.Id,
            StartTime = startTime,
            Duration = s_random.Next(2) == 0
                ? TimeSpan.Zero
                : TimeSpan.FromHours((float)s_random.Next(40) / 4),
            Location = GetRandomWord(true) + " " + s_random.Next(240),
            TimeCreated = startTime - TimeSpan.FromMinutes(s_random.Next(60*24*15))
        };
        
        return randomEvent;
    }

    private static Comment CreateRandomComment(List<EventModel> events, ApplicationUser user)
    {
        var randomEvent = events[s_random.Next(events.Count)];
        
        // Add comments to random Events
        var comment = new Comment
        {
            PostTime = randomEvent.TimeCreated + TimeSpan.FromMinutes(s_random.Next(60*24*15)),
            EditTime = null,
            Text = GetRandomSentence(s_random.Next(1, 50)),
            EventModelId = randomEvent.Id,
            EventModel = events[s_random.Next(events.Count)],
            PostedBy = user
        };

        return comment;
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
        var events = new List<EventModel>();
        
        for (var i = 0; i < 90; ++i)
        {
            events.Add(CreateRandomEvent(organizations.ToList()));
        }
        
        db.AddRange(events);

        // Add access rights to user
        var accessRights = new[]
        {
            new UserOrganization(user, organizations[1], AccessRight.FullAccess)
        };
        db.UserOrganization.AddRange(accessRights);
        
        
        // Create event models
        var comments = new List<Comment>();

        var rnd = new Random();
        foreach (var u in um.Users.ToList())
        {
            var numComments = rnd.Next(64);
            for (var i = 0; i < numComments; ++i)
            {
                comments.Add(CreateRandomComment(events, u));
            }
        }

        db.Comments.AddRange(comments);

        // Save changes to the database
        db.SaveChanges();
    }
}