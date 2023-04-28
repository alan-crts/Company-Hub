using CompanyHub.Context;
using CompanyHub.Models;
using CompanyHub.Services;

namespace CompanyHub.Data;

public class DataSeeder
{
    private readonly MainContext _myContext;
    private readonly IHashService _hashService;

    public DataSeeder(MainContext myContext, IHashService hashService)
    {
        _myContext = myContext;
        _hashService = hashService;
    }

    public void SeedData()
    {
        _myContext.Employee.RemoveRange(_myContext.Employee);
        _myContext.Service.RemoveRange(_myContext.Service);
        _myContext.Site.RemoveRange(_myContext.Site);

        _myContext.SaveChanges();

        _myContext.Site.Add(new Site
        {
            City = "Paris",
            Description = "Siège administratif",
        });

        _myContext.Site.Add(new Site
        {
            City = "Nantes",
            Description = "Site de production",
        });

        _myContext.Site.Add(new Site
        {
            City = "Toulouse",
            Description = "Site de production",
        });

        _myContext.Site.Add(new Site
        {
            City = "Nice",
            Description = "Site de production",
        });

        _myContext.Site.Add(new Site
        {
            City = "Lille",
            Description = "Site de production",
        });

        _myContext.SaveChanges();
        
        _myContext.Service.Add(new Service
        {
            Name = "Direction"
        });
        
        _myContext.Service.Add(new Service
        {
            Name = "Comptabilité"
        });
        
        _myContext.Service.Add(new Service
        {
            Name = "Ressources Humaines"
        });
        
        _myContext.Service.Add(new Service
        {
            Name = "Informatique"
        });
        
        _myContext.Service.Add(new Service
        {
            Name = "Production"
        });
        
        _myContext.Service.Add(new Service
        {
            Name = "Commercial"
        });

        _myContext.SaveChanges();

        _myContext.Employee.Add(new Employee
        {
            FirstName = "Alan",
            LastName = "Courtois",
            Email = "alancrts27@gmail.com",
            Password = _hashService.GetHash("alan"),
            MobilePhone = "06" + Faker.RandomNumber.Next(10000000, 99999999),
            LandlinePhone = "02" + Faker.RandomNumber.Next(10000000, 99999999),
            IsAdmin = true,
            SiteId = _myContext.Site.ToList()[Faker.RandomNumber.Next(0, _myContext.Site.Count())].Id,
            ServiceId = _myContext.Service.ToList()[Faker.RandomNumber.Next(0, _myContext.Service.Count())].Id
        });

        for (int i = 0; i < 1000; i++)
        {
            _myContext.Employee.Add(new Employee
            {
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Email = Faker.Internet.Email(),
                Password = _hashService.GetHash("user"),
                MobilePhone = "06" + Faker.RandomNumber.Next(10000000, 99999999),
                LandlinePhone = "02" + Faker.RandomNumber.Next(10000000, 99999999),
                SiteId = _myContext.Site.ToList()[Faker.RandomNumber.Next(0, _myContext.Site.Count())].Id,
                ServiceId = _myContext.Service.ToList()[Faker.RandomNumber.Next(0, _myContext.Service.Count())].Id
            });
        }
        
        _myContext.SaveChanges();
    }
}