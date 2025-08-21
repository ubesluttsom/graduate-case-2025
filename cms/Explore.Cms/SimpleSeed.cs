using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

public static class SimpleSeed
{
    public static async Task SeedDatabase(String clientConnectionString, String databaseName)
    {
        var client = new MongoClient(clientConnectionString);
        var db = client.GetDatabase(databaseName);
        
        Console.WriteLine("Starting database seeding...");

        // Clear entire database for clean slate
        await client.DropDatabaseAsync(databaseName);
        db = client.GetDatabase(databaseName);

        // Use fixed seed for consistent data
        var random = new Random(42);
        
        // Clear existing data
        await db.DropCollectionAsync("rooms");
        await db.DropCollectionAsync("guests");  
        await db.DropCollectionAsync("events");
        await db.DropCollectionAsync("transactions");
        
        // Create rooms (101-115)
        var rooms = Enumerable.Range(101, 15)
            .Select(num => new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"roomNumber", num},
                {"guestIds", new BsonArray()},
                {"transactionIds", new BsonArray()}
            })
            .ToArray();
        await db.GetCollection<BsonDocument>("rooms").InsertManyAsync(rooms);
        Console.WriteLine($"Created {rooms.Length} rooms");
        
        // Create guests with room assignments
        var guests = new[]
        {
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Erik"}, {"lastName", "Nordahl"}, {"email", "erik.nordahl@email.com"}, {"roomId", rooms[0]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Astrid"}, {"lastName", "Bergström"}, {"email", "astrid.bergstrom@email.com"}, {"roomId", rooms[0]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Magnus"}, {"lastName", "Johansen"}, {"email", "magnus.johansen@email.com"}, {"roomId", rooms[1]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Ingrid"}, {"lastName", "Larsen"}, {"email", "ingrid.larsen@email.com"}, {"roomId", rooms[2]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Olaf"}, {"lastName", "Svendsen"}, {"email", "olaf.svendsen@email.com"}, {"roomId", rooms[2]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Freya"}, {"lastName", "Hansen"}, {"email", "freya.hansen@email.com"}, {"roomId", rooms[3]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Thor"}, {"lastName", "Andersen"}, {"email", "thor.andersen@email.com"}, {"roomId", rooms[4]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Saga"}, {"lastName", "Karlsson"}, {"email", "saga.karlsson@email.com"}, {"roomId", rooms[4]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Bjørn"}, {"lastName", "Eriksson"}, {"email", "bjorn.eriksson@email.com"}, {"roomId", rooms[5]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Luna"}, {"lastName", "Petersen"}, {"email", "luna.petersen@email.com"}, {"roomId", rooms[6]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Akira"}, {"lastName", "Tanaka"}, {"email", "akira.tanaka@email.com"}, {"roomId", rooms[7]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Sarah"}, {"lastName", "Mitchell"}, {"email", "sarah.mitchell@email.com"}, {"roomId", rooms[8]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "James"}, {"lastName", "Wilson"}, {"email", "james.wilson@email.com"}, {"roomId", rooms[8]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Maria"}, {"lastName", "Rodriguez"}, {"email", "maria.rodriguez@email.com"}, {"roomId", rooms[9]["_id"]} },
            new BsonDocument { {"_id", BsonBinaryData.Create(Guid.NewGuid())}, {"firstName", "Chen"}, {"lastName", "Wei"}, {"email", "chen.wei@email.com"}, {"roomId", rooms[10]["_id"]} }
        };
        await db.GetCollection<BsonDocument>("guests").InsertManyAsync(guests);
        Console.WriteLine($"Created {guests.Length} guests");
        
        // Create Arctic events
        var events = new[]
        {
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "RIB Safari Adventure"}, 
                {"description", "Experience the Arctic waters in our high-speed rigid inflatable boats. Navigate through icebergs while searching for seals, walruses, and Arctic foxes along the dramatic coastline of Svalbard."}, 
                {"date", DateTime.Now.AddDays(2)}, 
                {"availableSpots", 12},
                {"guestIds", new BsonArray()}
            },
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "Husky Dog Sledding Expedition"}, 
                {"description", "Mush through the pristine Arctic wilderness with our team of Greenland huskies. Learn traditional sledding techniques while traversing frozen fjords and snow-covered tundra."}, 
                {"date", DateTime.Now.AddDays(3)}, 
                {"availableSpots", 8},
                {"guestIds", new BsonArray()}
            },
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "Snowmobile Arctic Explorer"}, 
                {"description", "Roar across the frozen landscape on powerful snowmobiles. Visit remote glaciers, abandoned Soviet mining settlements, and enjoy panoramic views of the Arctic Ocean."}, 
                {"date", DateTime.Now.AddDays(4)}, 
                {"availableSpots", 10},
                {"guestIds", new BsonArray()}
            },
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "Whale Safari & Marine Life"}, 
                {"description", "Join our marine biologist on a zodiac expedition to spot magnificent Arctic whales including beluga, narwhal, and bowhead whales. Learn about Arctic marine ecosystems."}, 
                {"date", DateTime.Now.AddDays(5)}, 
                {"availableSpots", 15},
                {"guestIds", new BsonArray()}
            },
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "Antarctica Base Camp Experience"}, 
                {"description", "Spend a night under the Antarctic stars in our heated expedition tents. Experience 24-hour daylight, glacier calving sounds, and the ultimate polar wilderness camping."}, 
                {"date", DateTime.Now.AddDays(6)}, 
                {"availableSpots", 6},
                {"guestIds", new BsonArray()}
            },
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "Arctic Ocean Kayaking"}, 
                {"description", "Paddle silently through mirror-calm Arctic waters in our specially designed cold-water kayaks. Get up close to icebergs and potentially curious seals in their natural habitat."}, 
                {"date", DateTime.Now.AddDays(7)}, 
                {"availableSpots", 8},
                {"guestIds", new BsonArray()}
            },
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "Snowshoe Trekking Adventure"}, 
                {"description", "Strap on traditional snowshoes and trek across the Arctic tundra. Visit ancient Inuit archaeological sites and learn survival techniques used by polar explorers."}, 
                {"date", DateTime.Now.AddDays(8)}, 
                {"availableSpots", 12},
                {"guestIds", new BsonArray()}
            },
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "Svalbard Hiking Expedition"}, 
                {"description", "Embark on a challenging 8-hour guided hike through Svalbard's dramatic mountain ranges. Encounter Arctic wildlife, visit research stations, and witness stunning glacier formations."}, 
                {"date", DateTime.Now.AddDays(9)}, 
                {"availableSpots", 10},
                {"guestIds", new BsonArray()}
            },
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "Polar Bear Photography Workshop"}, 
                {"description", "Join our wildlife photographer for an exclusive workshop on capturing Arctic wildlife. Learn specialized techniques for photographing polar bears, Arctic foxes, and seabirds."}, 
                {"date", DateTime.Now.AddDays(10)}, 
                {"availableSpots", 6},
                {"guestIds", new BsonArray()}
            },
            new BsonDocument { 
                {"_id", BsonBinaryData.Create(Guid.NewGuid())}, 
                {"name", "Northern Lights & Astronomy"}, 
                {"description", "Experience the magic of Aurora Borealis during our midnight expedition. Our astronomer will guide you through Arctic constellations while you witness nature's light show."}, 
                {"date", DateTime.Now.AddDays(15)}, 
                {"availableSpots", 20},
                {"guestIds", new BsonArray()}
            }
        };
        await db.GetCollection<BsonDocument>("events").InsertManyAsync(events);
        Console.WriteLine($"Created {events.Length} events");
        
        // Assign guests to events (first 6 events only)
        var eventsCollection = db.GetCollection<BsonDocument>("events");
        
        for (int i = 0; i < 6; i++)
        {
            var eventDoc = events[i];
            var availableSpots = eventDoc["availableSpots"].AsInt32;
            var guestsToAssign = guests.OrderBy(x => random.Next()).Take(random.Next(2, Math.Min(availableSpots, 6)));
            
            var guestIds = new BsonArray();
            foreach (var guest in guestsToAssign)
            {
                guestIds.Add(guest["_id"]);
            }
            
            await eventsCollection.UpdateOneAsync(
                Builders<BsonDocument>.Filter.Eq("_id", eventDoc["_id"]),
                Builders<BsonDocument>.Update.Set("guestIds", guestIds)
            );
        }
        Console.WriteLine("Assigned guests to events");
        
        // Create transactions
        var transactionDescriptions = new[]
        {
            "Arctic gear rental",
            "Spa treatment - Hot stone massage",
            "Premium dining - Reindeer steak dinner",
            "Bar tab - Arctic cocktails",
            "Souvenir shop - Polar expedition gear",
            "Photography service - Professional Arctic photos",
            "Extra bedding and pillows",
            "Room service - Champagne and chocolates",
            "Laundry service",
            "Wi-Fi premium package",
            "Expedition gear cleaning",
            "Medical consultation"
        };
        
        var allTransactions = new List<BsonDocument>();
        
        foreach (var guest in guests)
        {
            var numberOfTransactions = random.Next(1, 4);
            
            for (int i = 0; i < numberOfTransactions; i++)
            {
                var transaction = new BsonDocument
                {
                    {"_id", BsonBinaryData.Create(Guid.NewGuid())},
                    {"amount", Math.Round((decimal)(random.NextDouble() * 500 + 50), 2)}, // Between $50-$550
                    {"description", transactionDescriptions[random.Next(transactionDescriptions.Length)]},
                    {"guestId", guest["_id"]},
                    {"roomId", guest["roomId"]},
                    {"transactionDate", DateTime.Now.AddDays(-random.Next(0, 7))} // Random date within last week
                };
                
                allTransactions.Add(transaction);
            }
        }
        
        await db.GetCollection<BsonDocument>("transactions").InsertManyAsync(allTransactions);
        Console.WriteLine($"Created {allTransactions.Count} transactions");
        
        Console.WriteLine("Database seeding completed!");
        Console.WriteLine("\nCollections created:");
        Console.WriteLine($"- rooms: {rooms.Length} documents");
        Console.WriteLine($"- guests: {guests.Length} documents");
        Console.WriteLine($"- events: {events.Length} documents");
        Console.WriteLine($"- transactions: {allTransactions.Count} documents");
    }
}
