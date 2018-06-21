namespace IntegrationTests
{
	using System;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using Microsoft.Extensions.DependencyInjection;
	using MongoDB.Driver;
	using NUnit.Framework;

	public class UserIntegrationTestsBase : AssertionHelper
	{
		protected MongoDatabase Database;
		protected MongoCollection<IdentityUser> Users;
		protected MongoCollection<IdentityRole> Roles;

		// note: for now we'll have interfaces to both the new and old apis for MongoDB, that way we don't have to update all the tests at once and risk introducing bugs
		protected IMongoDatabase DatabaseNewApi;
		protected IServiceProvider ServiceProvider;

		private const string IdentityTesting = "identity-testing";

		private string ConnectionString
		{
			get
			{
				var connectionBuilder = new MongoUrlBuilder
				{
					Username = IdentityTesting,
					Password = "test",
					Server = new MongoServerAddress("localhost", 27017),
					DatabaseName = IdentityTesting
				};

				return connectionBuilder.ToString();
			}
		}

		[SetUp]
		public void BeforeEachTest()
		{
			var client = new MongoClient(ConnectionString);

			// todo move away from GetServer which could be deprecated at some point
			Database = client.GetServer().GetDatabase(IdentityTesting);
			Users = Database.GetCollection<IdentityUser>("users");
			Roles = Database.GetCollection<IdentityRole>("roles");

			DatabaseNewApi = client.GetDatabase(IdentityTesting);

			Database.DropCollection("users");
			Database.DropCollection("roles");

			ServiceProvider = CreateServiceProvider<IdentityUser, IdentityRole>();
		}

		protected UserManager<IdentityUser> GetUserManager()
			=> ServiceProvider.GetService<UserManager<IdentityUser>>();

		protected RoleManager<IdentityRole> GetRoleManager()
			=> ServiceProvider.GetService<RoleManager<IdentityRole>>();

		protected IServiceProvider CreateServiceProvider<TUser, TRole>(Action<IdentityOptions> optionsProvider = null)
			where TUser : IdentityUser
			where TRole : IdentityRole
		{
			var services = new ServiceCollection();
			optionsProvider = optionsProvider ?? (options => { });
			services.AddIdentity<TUser, TRole>(optionsProvider)
				.AddDefaultTokenProviders()
				.RegisterMongoStores<TUser, TRole>(ConnectionString);

			services.AddLogging();

			return services.BuildServiceProvider();
		}
	}
}