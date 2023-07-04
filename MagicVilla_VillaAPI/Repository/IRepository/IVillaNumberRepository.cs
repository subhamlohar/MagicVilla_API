using System.Linq.Expressions;
using MagicVilla_Web.Models;

namespace MagicVilla_Web.Repository.IRepository
{
	public interface IVillaNumberRepository : IRepository<VillaNumber>
	{
		
		Task<VillaNumber> UpdateAsync(VillaNumber entity);
		
	}
}
