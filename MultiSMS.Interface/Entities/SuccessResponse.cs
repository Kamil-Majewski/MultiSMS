namespace MultiSMS.Interface.Entities
{
    public class SuccessResponse
    {
        public bool Success { get; set; }
        public int Queued { get; set; }
        public int Unsent { get; set; }
        public Item[] Items { get; set; } = default!;
    }

    public class Item
    {
        public string Id { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Queued { get; set; } = default!;
        public int Parts { get; set; }
        public string Text { get; set; } = default!;
        public int Stat_id { get; set; } = default!;
    }
}
