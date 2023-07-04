using System.Linq.Expressions;
using MagicVilla_Web.Models;

namespace MagicVilla_Web.Repository.IRepository
{
	public interface IVillaRepository :IRepository<Villa>
	{
		
		Task<Villa> UpdateAsync(Villa entity);
		
	}
}
