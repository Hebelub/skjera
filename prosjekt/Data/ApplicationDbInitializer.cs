using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using prosjekt.Models;

namespace prosjekt.Data;

public class ApplicationDbInitializer
{
    // This is seeded with time, so that you can replicate the mistake by seeing when the mistake occured
    private static readonly Random s_random = new(DateTime.Now.Hour * 60 + DateTime.Now.Minute);

    private static ApplicationUser AddUser(UserManager<ApplicationUser> um, string name, string email)
    {
        var user = new ApplicationUser
            { NickName = name, UserName = email, Email = email, EmailConfirmed = true };
        um.CreateAsync(user, "Password1.").Wait();
        return user;
    }
    
    private static List<string> s_wordList = new()
    {
        "valley", "position", "welcome", "steam", "charm", "descent", "switch", "us", "cycle", "compromise",
        "situation", "likely", "variation", "pressure", "vat", "manner", "brilliance", "rifle", "draw", "function",
        "remember", "grudge", "examination", "age", "discriminate", "salvation", "tin", "pier", "division", "left",
        "umbrella", "straight", "on", "era", "delete", "pleasant", "grind", "fire", "turkey", "dictionary"
    };

    private static string GetRandomWord(bool capitalizeFirst = false)
    {
        var randomWord = s_wordList[s_random.Next(s_wordList.Count)];
        if (capitalizeFirst && !string.IsNullOrEmpty(randomWord))
        {
            randomWord = randomWord[0].ToString().ToUpper() + randomWord.Substring(1);
        }

        return randomWord;
    }

    private static string GetRandomEmail(bool capitalizeFirst = false)
    {
        List<string> mailEndingList = new()
        {
            "gmail.com", "uia.no", "hotmail.com", "outlook.com", "skjera.no"
        };
        return GetRandomWord() + "@" + mailEndingList[s_random.Next(mailEndingList.Count)];
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
        var startTime = DateTime.Now.Date + TimeSpan.FromHours((float)s_random.Next(-4000, 4000) / 4);

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
            TimeCreated = startTime - TimeSpan.FromMinutes(s_random.Next(60 * 24 * 15))
        };

        return randomEvent;
    }

    private static Comment CreateRandomComment(List<EventModel> events, ApplicationUser user)
    {
        var randomEvent = events[s_random.Next(events.Count)];

        // Add comments to random Events
        var comment = new Comment
        {
            PostTime = randomEvent.TimeCreated + TimeSpan.FromMinutes(s_random.Next(60 * 24 * 15)),
            EditTime = null,
            Text = GetRandomSentence(s_random.Next(1, 50)),
            EventModelId = randomEvent.Id,
            EventModel = events[s_random.Next(events.Count)],
            PostedBy = user
        };

        return comment;
    }

    private static void AddRandomUser(UserManager<ApplicationUser> um)
    {
        AddUser(um, GetRandomWord(true), GetRandomEmail());
    }

    private static List<OrganizationModel> CreateNOrganizations(int numOrganizations)
    {
        var organizations = new List<OrganizationModel>();
        for (var i = 0; i < numOrganizations; ++i)
        {
            organizations.Add(new OrganizationModel
            {
                Name = GetRandomWord(true)
                       + (s_random.Next(3) == 0 ? " " + GetRandomWord(true) : "")
                       + (s_random.Next(6) == 0 ? " Grimstad" : ""),
                Description = GetRandomSentence(s_random.Next(300) + 5)
            });
        }

        return organizations;
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

        for (var i = 0; i < 24; ++i)
        {
            AddRandomUser(um);
        }

        // Add Admin User
        var admin = AddUser(um, "Admin", "admin@uia.no");
        um.AddToRoleAsync(admin, "Admin");

        // Create Organizations
        var organizations = new List<OrganizationModel>
        {
            new(
                "Studentlaget Grimstad",
                "Norges Kristelige Student- og Skoleungdomslag, forkortet NKSS og ofte bare kalt Laget, er en kristen ungdomsorganisasjon som driver arbeid på skoler og studiesteder i hele Norge. Den ble stiftet 12. mars 1924 av blant andre Carl Fredrik Wisløff og Ole Hallesby. Foranledningen var den lange og pågående kirkestriden."),
            new(
                "Beta",
                "Velkommen til BETA! BETA er linjeforeningen for datastudentene ved UiA, og drives av studenter ved disse studiene. BETA's formål er å gi disse studentene veiledning i studiesituasjonen, arrangere kurs som utfyller fagtilbudet ved UiA, fremme kontakten med næringslivet og bidra med sosiale aktiviteter."
            ),
            new(
                "Tekna",
                "Har du, eller studerer du til, en master i naturvitenskap eller teknologi, kan du bli medlem i Tekna. Vi er Norges største fagforening for deg med mastergrad, og tilbyr blant annet juridisk bistand, faglige tilbud og nettverk over hele landet, samt en av landets beste bank- og forsikringsavtaler."
            ),
            new(
                "Bluebox Grimstad",
                "Velkommen til studentenes storstue i Grimstad! Bluebox er drevet av studenter, for studenter. Her arrangeres konserter, standup, live podcaster, fester, sosiale samlinger, quizkvelder og mye mer. Bluebox har et bredt tilbud gjennom semesteret.  arrangerer konserter, stand up, live podcaster, fester, sosiale samlinger, Bluebox Quiz og mye mer. Ofte i samarbeid med linjeforeninger, studentorganisasjoner og studentakiviteter. Huset driftes av studentene som organiserer alle medlemmene komitéer med hver sine ansvarsområder. Her er det lav terskel for å engasjere seg og få venner på tvers av studieretningene i Grimstad. Bluebox åpnet dørene i august 2010, og har i 10 år vært fast stoppested for de største norske artistene på turne. Følg med på programmet for dette semesteret, og ikke minst: Engasjer deg på huset!"
            ),
            new(
                "Touchpoint",
                "Touchpoint er et kristent arbeid i regi av Misjonskirken Norge og UNG. Arbeidet retter seg spesielt mot studenter og unge voksne fra 18 år og oppover. Vi startet i Kristiansand høsten 2013, og senere har Touchpoint kommet til Jæren, Tromsø og Aust-Agder. Touchpoint ønsker å være et godt tilbud som menigheter kan bruke for å nå unge voksne. Touchpoint sin visjon er å «nå mennesker som er borte fra Gud og menighet, og gjøre dem til etterfølgere av Jesus."
            ),
            new(
                "Tørr Å Si Det",
                "Torsdag 29 september kl.19.00 blir første offisielle samling for menn i regi av «Tør Å Si Det». På programmet står artisten og foredragsholder Jan-Erik Mæland «GinoBless» som opplevde omsorgssvikt og er det man kaller et løvetannbarn. I tillegg til å drive eget plateselskap har Gino en lang fartstid bak seg som artist under navnet GinoBless. Han er mest kjent for låten «Elsket» som gjorde seg sterkt bemerket på Sørlandet i artikkelen «Får hardbarkede kriminelle til å gråte» som var blant topp 5 mest leste saker på Fædrelandsvennen. Gino kan vise til flere gjesteopptredener hos riksdekkende medier som blant annet NRK og God morgen Norge på TV2 for å fronte snakk om mobbing, i regi av Blå Kors. Med sitt engasjement og formidlingsevne ønsker han å spre tro, håp og kjærlighet hvor han deler sin historie om hvordan det var å vokse opp uten en far, en mor som slet og hvilke utfordringer han fikk grunnet oppveksten helt inn i voksen alder. Videre deler Gino sin reise om hvordan summen av alle små valg har ført han dit hvor han er i dag. En ærlig og åpen prat om livets opp og nedturer og hvorfor «Tør Å Si Det» er blitt en hjertesak. Det blir biljard, varm mat og drikke som serveres, samt deilige toner fremført av Norges egen “Ed Sheeran”, artisten Daniel Hillestad. Velkommen til et fellesskap hvor målet er å inspirere oss menn til mer åpenhet om livets utfordringer. Sammen kan vi motivere hverandre til vekst. Vi sees! Sted: Lokalet er i 3 etasje på Fokussenteret som ligger rett ved E18 i Grimstad. "
            )
        };
        organizations.AddRange(CreateNOrganizations(8));
        db.OrganizationModels.AddRange(organizations);


        // Create Events
        var events = new List<EventModel>();

        for (var i = 0; i < 100; ++i)
        {
            events.Add(CreateRandomEvent(organizations.ToList()));
        }

        db.AddRange(events);

        // Add access rights to user
        var accessRights = new[]
        {
            new UserOrganizationRelation(user, organizations[1], AccessRight.FullAccess)
        };
        db.UserOrganization.AddRange(accessRights);


        // Create event models
        var comments = new List<Comment>();

        foreach (var u in um.Users.ToList())
        {
            var numComments = s_random.Next(8);
            for (var i = 0; i < numComments; ++i)
            {
                comments.Add(CreateRandomComment(events, u));
            }
        }

        // Add User Relations
        var userEventRelations = new List<UserEventRelation>();
        var userOrganizationRelations = new List<UserOrganizationRelation>();
        foreach (var u in um.Users.ToList())
        {
            foreach (var ev in events)
            {
                if (s_random.Next(6) == 0)
                {
                    userEventRelations.Add(
                        new UserEventRelation(ev, u)
                        {
                            EventId = ev.Id,
                            IsAttending = true
                        }
                    );
                }
            }

            foreach (var o in organizations)
            {
                if (s_random.Next(4) == 0)
                {
                    userOrganizationRelations.Add(
                        new UserOrganizationRelation(u, o)
                        {
                            UserId = u.Id,
                            OrganizationId = o.Id,
                            IsFollowing = true,
                            AccessRight = AccessRight.NoAccess
                        }
                    );
                }
            }
        }

        db.UserEventRelations.AddRange(userEventRelations);
        db.UserOrganization.AddRange(userOrganizationRelations);

        db.Comments.AddRange(comments);

        // Save changes to the database
        db.SaveChanges();
    }
}