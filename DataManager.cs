using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MelomanApp
{
    public class DataManager
    {
        private readonly string _artistsFile;
        private readonly string _songsFile;
        private readonly string _albumsFile;

        public List<Artist> Artists { get; private set; } = new List<Artist>();
        public List<Song> Songs { get; private set; } = new List<Song>();
        public List<Album> Albums { get; private set; } = new List<Album>();

        public DataManager(string dataFolder = "data")
        {
            if (!Directory.Exists(dataFolder))
                Directory.CreateDirectory(dataFolder);

            _artistsFile = Path.Combine(dataFolder, "artists.json");
            _songsFile   = Path.Combine(dataFolder, "songs.json");
            _albumsFile  = Path.Combine(dataFolder, "albums.json");

            LoadAll();
        }

        //Load

        public void LoadAll()
        {
            Artists = LoadFile<List<Artist>>(_artistsFile) ?? new List<Artist>();
            Songs   = LoadFile<List<Song>>(_songsFile)     ?? new List<Song>();
            Albums  = LoadFile<List<Album>>(_albumsFile)   ?? new List<Album>();
        }

        private T LoadFile<T>(string path) where T : class
        {
            if (!File.Exists(path)) return null;
            try
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch { return null; }
        }

        //Save

        public void SaveArtists() => SaveFile(_artistsFile, Artists);
        public void SaveSongs()   => SaveFile(_songsFile, Songs);
        public void SaveAlbums()  => SaveFile(_albumsFile, Albums);

        private void SaveFile<T>(string path, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        //ID helpers

        public int NextArtistId() => Artists.Any() ? Artists.Max(a => a.Id) + 1 : 1;
        public int NextSongId()   => Songs.Any()   ? Songs.Max(s => s.Id) + 1   : 1;
        public int NextAlbumId()  => Albums.Any()  ? Albums.Max(a => a.Id) + 1  : 1;

        //Queries

        public List<Song> GetSongsByArtist(int artistId) =>
            Songs.Where(s => s.ArtistId == artistId).ToList();

        public List<Album> GetAlbumsContainingSong(int songId) =>
            Albums.Where(a => a.SongIds.Contains(songId)).ToList();

        public string GetArtistName(int artistId) =>
            Artists.FirstOrDefault(a => a.Id == artistId)?.Name ?? "—";

        public List<Song> GetAlbumSongs(Album album) =>
            album.SongIds
                 .Select(id => Songs.FirstOrDefault(s => s.Id == id))
                 .Where(s => s != null)
                 .ToList();

        //Statistics

        public int AlbumCountForArtist(int artistId) =>
            Albums.Count(a => a.ArtistId == artistId);

        public int SongCountForArtist(int artistId) =>
            Songs.Count(s => s.ArtistId == artistId);
    }
}
