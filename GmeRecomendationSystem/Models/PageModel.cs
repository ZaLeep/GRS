namespace GmeRecomendationSystem.Models
{
    public class PageModel
    {
        public PageModel(int id, int size)
        {
            Items = new List<ItemModel>();
            Id = id;
            PageSize = size;
        }
        public int Id { get; set; }
        public int PageSize { get; set; }
        public List<ItemModel> Items { get; set; }
    }
}
