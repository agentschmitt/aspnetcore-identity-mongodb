using System.Collections.Generic;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Identity.MongoDB
{
	using global::MongoDB.Bson;
	using global::MongoDB.Bson.Serialization.Attributes;

	public class IdentityRole
	{
		public IdentityRole()
		{
			Id = ObjectId.GenerateNewId().ToString();
			Claims = new List<IdentityRoleClaim>();
		}

		public IdentityRole(string roleName) : this()
		{
			Name = roleName;
		}

		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string Name { get; set; }

		public string NormalizedName { get; set; }

		[BsonIgnoreIfNull]
		public virtual List<IdentityRoleClaim> Claims { get; set; }

		public virtual void AddClaim(Claim claim)
		{
			Claims.Add(new IdentityRoleClaim(claim));
		}

		public virtual void RemoveClaim(Claim claim)
		{
			Claims.RemoveAll(c => c.Type == claim.Type && c.Value == claim.Value);
		}

		public override string ToString() => Name;
	}
}