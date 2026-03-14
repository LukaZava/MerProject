using MERPROJ.Models;
using Microsoft.EntityFrameworkCore;

namespace MERPROJ.Data
{
    public class SeederService
    {
        public static async Task SeedAsync(MerDbContext context, IWebHostEnvironment env)
        {
            if (await context.Customers.AnyAsync())
            {
                return;
            }

            var seedFolder = Path.Combine(env.ContentRootPath, "Data");

            var firstNames = LoadLines(Path.Combine(seedFolder, "firstnames.txt"));
            var lastNames = LoadLines(Path.Combine(seedFolder, "lastnames.txt"));
            var cities = LoadLines(Path.Combine(seedFolder, "cities.txt"));
            var countries = LoadLines(Path.Combine(seedFolder, "countries.txt"));

            if (firstNames.Count == 0 || lastNames.Count == 0 || cities.Count == 0 || countries.Count == 0)
            {
                throw new InvalidOperationException("Seed data files are missing or empty.");
            }

            const int totalRecords = 100_000;
            const int batchSize = 5_000;

            var customers = new List<Customer>(batchSize);
            var baseDate = DateTime.UtcNow.AddYears(-2);

            context.ChangeTracker.AutoDetectChangesEnabled = false;

            try
            {
                for (int i = 1; i <= totalRecords; i++)
                {
                    var firstName = firstNames[(i - 1) % firstNames.Count];
                    var lastName = lastNames[(i - 1) / firstNames.Count % lastNames.Count];
                    var city = cities[(i - 1) / (firstNames.Count * lastNames.Count) % cities.Count];
                    var country = countries[(i - 1) / (firstNames.Count * lastNames.Count * cities.Count) % countries.Count];

                    var createdAt = baseDate.AddMinutes(i);

                    customers.Add(new Customer
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = $"{firstName.ToLower()}.{lastName.ToLower()}.{i}@example.com",
                        Phone = $"+38591{i % 1000000:D6}",
                        City = city,
                        Country = country,
                        IsActive = i % 10 != 0,
                        CreatedAt = createdAt,
                        LastModifiedAt = i % 7 == 0 ? createdAt.AddDays(i % 30 + 1) : null
                    });

                    if (customers.Count == batchSize)
                    {
                        await context.Customers.AddRangeAsync(customers);
                        await context.SaveChangesAsync();
                        customers.Clear();
                    }
                }

                if (customers.Count > 0)
                {
                    await context.Customers.AddRangeAsync(customers);
                    await context.SaveChangesAsync();
                    customers.Clear();
                }
            }
            finally
            {
                context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        private static List<string> LoadLines(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Seed file not found: {path}");
            }

            return File.ReadAllLines(path)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
