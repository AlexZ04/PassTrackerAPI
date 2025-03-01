﻿namespace PassTrackerAPI.Data.Entities
{
    public class RequestDB
    {
        public Guid Id { get; set; }
        public UserDb User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public TypeRequestDB TypeRequest { get; set; }
        public StatusRequestDB StatusRequest { get; set; }
        public byte[]? Photo { get; set; }

    }
}
