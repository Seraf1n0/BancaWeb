namespace APIBanca.Models
{

    public class GetListCardMovement
    {
        public List<GetCardMovement> items { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }

    }

}