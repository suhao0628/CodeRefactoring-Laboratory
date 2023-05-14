namespace Delivery.Api.Model.Dto
{
    public class DishPagedListDto
    {
        public List<DishDto> Dishes { get; set; }

        public PageInfoModel Pagination { get; set; }
    }
}
