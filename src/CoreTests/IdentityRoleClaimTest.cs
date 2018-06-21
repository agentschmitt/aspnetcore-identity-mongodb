namespace Tests
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using Microsoft.AspNetCore.Identity.MongoDB;
	using NUnit.Framework;

	[TestFixture]
	public class IdentityRoleClaimTests : AssertionHelper
	{
		[Test]
		public void Create_FromClaim_SetsTypeAndValue()
		{
			var claim = new Claim("type", "value");

			var roleClaim = new IdentityRoleClaim(claim);

			Expect(roleClaim.Type, Is.EqualTo("type"));
			Expect(roleClaim.Value, Is.EqualTo("value"));
		}

		[Test]
		public void ToSecurityClaim_SetsTypeAndValue()
		{
			var roleClaim = new IdentityRoleClaim { Type = "t", Value = "v"};

			var claim = roleClaim.ToSecurityClaim();

			Expect(claim.Type, Is.EqualTo("t"));
			Expect(claim.Value, Is.EqualTo("v"));
		}

		[Test]
		public void AddClaim()
		{
			var role = new IdentityRole();
			var newClaim = new Claim("newType", "newValue");

			role.AddClaim(newClaim);

			Expect(role.Claims.First().Type, Is.EqualTo("newType"));
			Expect(role.Claims.First().Value, Is.EqualTo("newValue"));
		}

		[Test]
		public void RemoveClaim()
		{
			var oldClaim = new Claim("newType", "newValue");
			var role = new IdentityRole
			{
				Claims = new List<IdentityRoleClaim> { new IdentityRoleClaim(oldClaim) }
			};			

			role.RemoveClaim(oldClaim);

			Expect(role.Claims, Has.No.Member(oldClaim));
		}
	}
}