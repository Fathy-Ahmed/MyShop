using myshop.Entities.Repositories;
using myshop.DataAccess.Data;
using myshop.Entities.Models;
namespace myshop.DataAccess.Implementaion
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly AppDbContext context;

        public ApplicationUserRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }
       
    }








}
