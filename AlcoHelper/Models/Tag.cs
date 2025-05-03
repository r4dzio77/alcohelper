namespace AlcoHelper.Models
{
    public class Tag //Służy do opisywania alkoholu cechami (np. owocowy, słodki)
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<AlcoholTag>? AlcoholTags { get; set; }
    }

}
