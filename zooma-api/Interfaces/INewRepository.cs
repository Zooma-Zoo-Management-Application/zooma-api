using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface INewRepository
    {
        List<News> GetAllNews();   
        News GetNewById(int id); 
        User GetNewsByStaffId(short id);
        List<News> GetPinNews();
        List<News> GetUnpinNews();
    }
}
