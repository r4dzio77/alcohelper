namespace AlcoHelper.Models
{
    public class AlcoholTag //tabela pośrednicząca miedzy alkoholem a tagami
    {
        public int AlcoholId { get; set; }
        public int TagId { get; set; }

        public Alcohol? Alcohol { get; set; }
        public Tag? Tag { get; set; }
    }
}
