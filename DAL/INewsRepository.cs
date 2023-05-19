using DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface INewsRepository
    {
        Task<IEnumerable<NewsDTO>> GetAllNewsAsync();
        Task<NewsDTO> AddNews(NewsDTO item);
        Task<NewsDTO> GetNewsByIdAsync(string id);
        Task<NewsDTO> LikeNewsByIdAsync(string id, bool like);
        Task<NewsDTO> DislikeNewsByIdAsync(string id, bool dislike);
        Task DeleteAllAsync();
        Task DeleteByTitleAsync(string title);
    }
}
