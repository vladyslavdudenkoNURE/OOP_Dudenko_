using System;
using System.Collections.Generic;

namespace MelomanApp
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Country { get; set; }
        public string ActiveYears { get; set; }
        public string Description { get; set; }

        public override string ToString() => Name;
    }

    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public string Duration { get; set; }
        public int Year { get; set; }

        public override string ToString() => Title;
    }

    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public int Year { get; set; }
        public string Label { get; set; }
        public List<int> SongIds { get; set; } = new List<int>();

        public override string ToString() => Title;
    }
}
