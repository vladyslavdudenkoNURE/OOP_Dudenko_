using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MelomanApp
{
    public class MainForm : Form
    {
        private DataManager _dm;

        private TabControl tabMain;
        private TabPage tabArtists, tabSongs, tabAlbums, tabSearch;

        //Artists tab
        private DataGridView dgvArtists;
        private Button btnAddArtist, btnEditArtist, btnDeleteArtist;

        //Songs tab
        private DataGridView dgvSongs;
        private Button btnAddSong, btnEditSong, btnDeleteSong;

        //Albums tab
        private DataGridView dgvAlbums;
        private Button btnAddAlbum, btnEditAlbum, btnDeleteAlbum, btnViewAlbum;

        //Search tab
        private GroupBox grpByArtist, grpBySong;
        private ComboBox cmbArtistSearch;
        private ListBox lstSongsByArtist;
        private ComboBox cmbSongSearch;
        private ListBox lstAlbumsBySong;
        private Label lblSongsCount, lblAlbumsCount;

        public MainForm()
        {
            _dm = new DataManager();
            InitializeComponent();
            LoadSampleDataIfEmpty();
            RefreshAll();
        }

        //UI Init

        private void InitializeComponent()
        {
            Text = "Довідник меломана";
            Size = new Size(950, 620);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9f);

            tabMain = new TabControl { Dock = DockStyle.Fill };

            tabArtists = new TabPage("🎸 Виконавці");
            tabSongs   = new TabPage("🎵 Пісні");
            tabAlbums  = new TabPage("💿 Альбоми");
            tabSearch  = new TabPage("🔍 Пошук");

            tabMain.TabPages.AddRange(new[] { tabArtists, tabSongs, tabAlbums, tabSearch });

            BuildArtistsTab();
            BuildSongsTab();
            BuildAlbumsTab();
            BuildSearchTab();

            Controls.Add(tabMain);
        }

        private void BuildArtistsTab()
        {
            dgvArtists = BuildGrid();
            dgvArtists.Columns.Add("Id",          "ID");
            dgvArtists.Columns.Add("Name",        "Назва/Ім'я");
            dgvArtists.Columns.Add("Genre",       "Жанр");
            dgvArtists.Columns.Add("Country",     "Країна");
            dgvArtists.Columns.Add("ActiveYears", "Роки активності");
            dgvArtists.Columns.Add("Description", "Опис");
            dgvArtists.Columns["Id"].Width = 40;
            dgvArtists.Columns["Description"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            var panel = BuildButtonPanel(
                out btnAddArtist,    "Додати",
                out btnEditArtist,   "Редагувати",
                out btnDeleteArtist, "Видалити");

            btnAddArtist.Click    += (s, e) => OpenArtistDialog(null);
            btnEditArtist.Click   += (s, e) => EditSelectedArtist();
            btnDeleteArtist.Click += (s, e) => DeleteSelectedArtist();
            dgvArtists.DoubleClick += (s, e) => EditSelectedArtist();

            tabArtists.Controls.Add(dgvArtists);
            tabArtists.Controls.Add(panel);
        }

        private void BuildSongsTab()
        {
            dgvSongs = BuildGrid();
            dgvSongs.Columns.Add("Id",       "ID");
            dgvSongs.Columns.Add("Title",    "Назва");
            dgvSongs.Columns.Add("Artist",   "Виконавець");
            dgvSongs.Columns.Add("Duration", "Тривалість");
            dgvSongs.Columns.Add("Year",     "Рік");
            dgvSongs.Columns["Id"].Width    = 40;
            dgvSongs.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            var panel = BuildButtonPanel(
                out btnAddSong,    "Додати",
                out btnEditSong,   "Редагувати",
                out btnDeleteSong, "Видалити");

            btnAddSong.Click    += (s, e) => OpenSongDialog(null);
            btnEditSong.Click   += (s, e) => EditSelectedSong();
            btnDeleteSong.Click += (s, e) => DeleteSelectedSong();
            dgvSongs.DoubleClick += (s, e) => EditSelectedSong();

            tabSongs.Controls.Add(dgvSongs);
            tabSongs.Controls.Add(panel);
        }

        private void BuildAlbumsTab()
        {
            dgvAlbums = BuildGrid();
            dgvAlbums.Columns.Add("Id",     "ID");
            dgvAlbums.Columns.Add("Title",  "Назва альбому");
            dgvAlbums.Columns.Add("Artist", "Виконавець");
            dgvAlbums.Columns.Add("Year",   "Рік");
            dgvAlbums.Columns.Add("Label",  "Лейбл");
            dgvAlbums.Columns.Add("Tracks", "Треків");
            dgvAlbums.Columns["Id"].Width    = 40;
            dgvAlbums.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            var panel = BuildButtonPanel4(
                out btnAddAlbum,    "Додати",
                out btnEditAlbum,   "Редагувати",
                out btnDeleteAlbum, "Видалити",
                out btnViewAlbum,   "Переглянути треки");

            btnAddAlbum.Click    += (s, e) => OpenAlbumDialog(null);
            btnEditAlbum.Click   += (s, e) => EditSelectedAlbum();
            btnDeleteAlbum.Click += (s, e) => DeleteSelectedAlbum();
            btnViewAlbum.Click   += (s, e) => ViewAlbumTracks();
            dgvAlbums.DoubleClick += (s, e) => ViewAlbumTracks();

            tabAlbums.Controls.Add(dgvAlbums);
            tabAlbums.Controls.Add(panel);
        }

        private void BuildSearchTab()
        {
            //By artist
            grpByArtist = new GroupBox
            {
                Text = "Всі пісні виконавця",
                Location = new Point(10, 10),
                Size = new Size(440, 520),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            cmbArtistSearch = new ComboBox
            {
                Location = new Point(10, 28),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };
            var btnFindSongs = new Button
            {
                Text = "Знайти",
                Location = new Point(318, 26),
                Size = new Size(100, 27)
            };
            lblSongsCount = new Label
            {
                Location = new Point(10, 58),
                Size = new Size(420, 20),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.Gray
            };
            lstSongsByArtist = new ListBox
            {
                Location = new Point(10, 80),
                Size = new Size(418, 420),
                Font = new Font("Segoe UI", 9f)
            };

            btnFindSongs.Click += (s, e) => SearchSongsByArtist();

            grpByArtist.Controls.AddRange(new Control[]
                { cmbArtistSearch, btnFindSongs, lblSongsCount, lstSongsByArtist });

            //By song
            grpBySong = new GroupBox
            {
                Text = "Альбоми, де зустрічається пісня",
                Location = new Point(460, 10),
                Size = new Size(440, 520),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };

            cmbSongSearch = new ComboBox
            {
                Location = new Point(10, 28),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f)
            };
            var btnFindAlbums = new Button
            {
                Text = "Знайти",
                Location = new Point(318, 26),
                Size = new Size(100, 27)
            };
            lblAlbumsCount = new Label
            {
                Location = new Point(10, 58),
                Size = new Size(420, 20),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.Gray
            };
            lstAlbumsBySong = new ListBox
            {
                Location = new Point(10, 80),
                Size = new Size(418, 420),
                Font = new Font("Segoe UI", 9f)
            };

            btnFindAlbums.Click += (s, e) => SearchAlbumsBySong();

            grpBySong.Controls.AddRange(new Control[]
                { cmbSongSearch, btnFindAlbums, lblAlbumsCount, lstAlbumsBySong });

            tabSearch.Controls.Add(grpByArtist);
            tabSearch.Controls.Add(grpBySong);
        }

        //Grid helper

        private DataGridView BuildGrid()
        {
            var dgv = new DataGridView
            {
                Location = new Point(0, 0),
                Size = new Size(930, 510),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f)
            };
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            return dgv;
        }

        private Panel BuildButtonPanel(
            out Button b1, string t1,
            out Button b2, string t2,
            out Button b3, string t3)
        {
            var panel = new Panel { Dock = DockStyle.Bottom, Height = 45 };
            b1 = new Button { Text = t1, Location = new Point(5, 8),   Size = new Size(100, 30) };
            b2 = new Button { Text = t2, Location = new Point(115, 8), Size = new Size(110, 30) };
            b3 = new Button { Text = t3, Location = new Point(235, 8), Size = new Size(100, 30) };
            panel.Controls.AddRange(new Control[] { b1, b2, b3 });
            return panel;
        }

        private Panel BuildButtonPanel4(
            out Button b1, string t1,
            out Button b2, string t2,
            out Button b3, string t3,
            out Button b4, string t4)
        {
            var panel = new Panel { Dock = DockStyle.Bottom, Height = 45 };
            b1 = new Button { Text = t1, Location = new Point(5,   8), Size = new Size(100, 30) };
            b2 = new Button { Text = t2, Location = new Point(115, 8), Size = new Size(110, 30) };
            b3 = new Button { Text = t3, Location = new Point(235, 8), Size = new Size(100, 30) };
            b4 = new Button { Text = t4, Location = new Point(345, 8), Size = new Size(160, 30) };
            panel.Controls.AddRange(new Control[] { b1, b2, b3, b4 });
            return panel;
        }

        //Refresh

        private void RefreshAll()
        {
            RefreshArtists();
            RefreshSongs();
            RefreshAlbums();
            RefreshSearchCombos();
        }

        private void RefreshArtists()
        {
            dgvArtists.Rows.Clear();
            foreach (var a in _dm.Artists)
                dgvArtists.Rows.Add(a.Id, a.Name, a.Genre, a.Country, a.ActiveYears, a.Description);
        }

        private void RefreshSongs()
        {
            dgvSongs.Rows.Clear();
            foreach (var s in _dm.Songs)
                dgvSongs.Rows.Add(s.Id, s.Title, _dm.GetArtistName(s.ArtistId), s.Duration, s.Year);
        }

        private void RefreshAlbums()
        {
            dgvAlbums.Rows.Clear();
            foreach (var a in _dm.Albums)
                dgvAlbums.Rows.Add(a.Id, a.Title, _dm.GetArtistName(a.ArtistId), a.Year, a.Label, a.SongIds.Count);
        }

        private void RefreshSearchCombos()
        {
            cmbArtistSearch.Items.Clear();
            foreach (var a in _dm.Artists) cmbArtistSearch.Items.Add(a);
            if (cmbArtistSearch.Items.Count > 0) cmbArtistSearch.SelectedIndex = 0;

            cmbSongSearch.Items.Clear();
            foreach (var s in _dm.Songs) cmbSongSearch.Items.Add(s);
            if (cmbSongSearch.Items.Count > 0) cmbSongSearch.SelectedIndex = 0;
        }

        //Artist CRUD

        private void OpenArtistDialog(Artist existing)
        {
            using var dlg = new ArtistDialog(existing);
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var a = dlg.Result;
            if (existing == null) { a.Id = _dm.NextArtistId(); _dm.Artists.Add(a); }
            else { var idx = _dm.Artists.IndexOf(existing); _dm.Artists[idx] = a; }

            _dm.SaveArtists();
            RefreshAll();
        }

        private void EditSelectedArtist()
        {
            var sel = GetSelectedItem(dgvArtists);
            if (sel < 0) return;
            int id = (int)dgvArtists.Rows[sel].Cells["Id"].Value;
            OpenArtistDialog(_dm.Artists.Find(a => a.Id == id));
        }

        private void DeleteSelectedArtist()
        {
            var sel = GetSelectedItem(dgvArtists);
            if (sel < 0) return;
            if (MessageBox.Show("Видалити виконавця?", "Підтвердження",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            int id = (int)dgvArtists.Rows[sel].Cells["Id"].Value;
            _dm.Artists.RemoveAll(a => a.Id == id);
            _dm.SaveArtists();
            RefreshAll();
        }

        //Song CRUD

        private void OpenSongDialog(Song existing)
        {
            using var dlg = new SongDialog(existing, _dm.Artists);
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var s = dlg.Result;
            if (existing == null) { s.Id = _dm.NextSongId(); _dm.Songs.Add(s); }
            else { var idx = _dm.Songs.IndexOf(existing); _dm.Songs[idx] = s; }

            _dm.SaveSongs();
            RefreshAll();
        }

        private void EditSelectedSong()
        {
            var sel = GetSelectedItem(dgvSongs);
            if (sel < 0) return;
            int id = (int)dgvSongs.Rows[sel].Cells["Id"].Value;
            OpenSongDialog(_dm.Songs.Find(s => s.Id == id));
        }

        private void DeleteSelectedSong()
        {
            var sel = GetSelectedItem(dgvSongs);
            if (sel < 0) return;
            if (MessageBox.Show("Видалити пісню?", "Підтвердження",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            int id = (int)dgvSongs.Rows[sel].Cells["Id"].Value;
            _dm.Songs.RemoveAll(s => s.Id == id);
            _dm.Albums.ForEach(a => a.SongIds.Remove(id));
            _dm.SaveSongs(); _dm.SaveAlbums();
            RefreshAll();
        }

        //Album CRUD

        private void OpenAlbumDialog(Album existing)
        {
            using var dlg = new AlbumDialog(existing, _dm.Artists, _dm.Songs);
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var a = dlg.Result;
            if (existing == null) { a.Id = _dm.NextAlbumId(); _dm.Albums.Add(a); }
            else { var idx = _dm.Albums.IndexOf(existing); _dm.Albums[idx] = a; }

            _dm.SaveAlbums();
            RefreshAll();
        }

        private void EditSelectedAlbum()
        {
            var sel = GetSelectedItem(dgvAlbums);
            if (sel < 0) return;
            int id = (int)dgvAlbums.Rows[sel].Cells["Id"].Value;
            OpenAlbumDialog(_dm.Albums.Find(a => a.Id == id));
        }

        private void DeleteSelectedAlbum()
        {
            var sel = GetSelectedItem(dgvAlbums);
            if (sel < 0) return;
            if (MessageBox.Show("Видалити альбом?", "Підтвердження",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            int id = (int)dgvAlbums.Rows[sel].Cells["Id"].Value;
            _dm.Albums.RemoveAll(a => a.Id == id);
            _dm.SaveAlbums();
            RefreshAll();
        }

        private void ViewAlbumTracks()
        {
            var sel = GetSelectedItem(dgvAlbums);
            if (sel < 0) return;
            int id = (int)dgvAlbums.Rows[sel].Cells["Id"].Value;
            var album = _dm.Albums.Find(a => a.Id == id);
            using var dlg = new AlbumTracksDialog(album, _dm);
            dlg.ShowDialog();
        }

        //Search

        private void SearchSongsByArtist()
        {
            lstSongsByArtist.Items.Clear();
            if (!(cmbArtistSearch.SelectedItem is Artist artist)) return;

            var songs = _dm.GetSongsByArtist(artist.Id);
            lblSongsCount.Text = $"Знайдено пісень: {songs.Count}";

            foreach (var s in songs)
            {
                string albums = string.Join(", ", _dm.GetAlbumsContainingSong(s.Id)
                                                      .ConvertAll(a => a.Title));
                lstSongsByArtist.Items.Add($"{s.Title}  [{s.Duration}]  {s.Year}" +
                    (string.IsNullOrEmpty(albums) ? "" : $"  |  Альбоми: {albums}"));
            }
        }

        private void SearchAlbumsBySong()
        {
            lstAlbumsBySong.Items.Clear();
            if (!(cmbSongSearch.SelectedItem is Song song)) return;

            var albums = _dm.GetAlbumsContainingSong(song.Id);
            lblAlbumsCount.Text = $"Знайдено альбомів: {albums.Count}";

            foreach (var a in albums)
                lstAlbumsBySong.Items.Add($"{a.Title}  ({a.Year})  [{_dm.GetArtistName(a.ArtistId)}]  —  {a.Label}");
        }

        //Helpers

        private int GetSelectedItem(DataGridView dgv)
        {
            if (dgv.SelectedRows.Count == 0)
            { MessageBox.Show("Виберіть рядок!", "Увага"); return -1; }
            return dgv.SelectedRows[0].Index;
        }

        //Sample data

        private void LoadSampleDataIfEmpty()
        {
            if (_dm.Artists.Count > 0) return;

            _dm.Artists.AddRange(new[]
            {
                new Artist { Id=1, Name="The Beatles",   Genre="Rock",    Country="Великобританія", ActiveYears="1960–1970", Description="Легендарний рок-гурт" },
                new Artist { Id=2, Name="Queen",         Genre="Rock",    Country="Великобританія", ActiveYears="1970–…",    Description="Один із найкращих гуртів світу" },
                new Artist { Id=3, Name="Океан Ельзи",   Genre="Рок",     Country="Україна",         ActiveYears="1994–…",    Description="Провідний український рок-гурт" },
            });

            _dm.Songs.AddRange(new[]
            {
                new Song { Id=1,  Title="Let It Be",         ArtistId=1, Duration="3:50", Year=1970 },
                new Song { Id=2,  Title="Hey Jude",          ArtistId=1, Duration="7:11", Year=1968 },
                new Song { Id=3,  Title="Come Together",     ArtistId=1, Duration="4:17", Year=1969 },
                new Song { Id=4,  Title="Bohemian Rhapsody", ArtistId=2, Duration="5:55", Year=1975 },
                new Song { Id=5,  Title="We Will Rock You",  ArtistId=2, Duration="2:01", Year=1977 },
                new Song { Id=6,  Title="Don't Stop Me Now", ArtistId=2, Duration="3:29", Year=1978 },
                new Song { Id=7,  Title="Без бою",           ArtistId=3, Duration="4:10", Year=1998 },
                new Song { Id=8,  Title="Така як ти",        ArtistId=3, Duration="3:55", Year=2000 },
                new Song { Id=9,  Title="Квіти",             ArtistId=3, Duration="3:40", Year=2005 },
            });

            _dm.Albums.AddRange(new[]
            {
                new Album { Id=1, Title="Let It Be",       ArtistId=1, Year=1970, Label="Apple Records",  SongIds=new System.Collections.Generic.List<int>{1,2,3} },
                new Album { Id=2, Title="A Night at the Opera", ArtistId=2, Year=1975, Label="EMI",        SongIds=new System.Collections.Generic.List<int>{4} },
                new Album { Id=3, Title="News of the World",    ArtistId=2, Year=1977, Label="EMI",        SongIds=new System.Collections.Generic.List<int>{5,6} },
                new Album { Id=4, Title="Там де нас нема",      ArtistId=3, Year=1998, Label="WEA Ukraine",SongIds=new System.Collections.Generic.List<int>{7,8} },
                new Album { Id=5, Title="Суперсиметрія",        ArtistId=3, Year=2005, Label="Navigator",  SongIds=new System.Collections.Generic.List<int>{9} },
                new Album { Id=6, Title="Greatest Hits",        ArtistId=1, Year=2000, Label="Apple Records", SongIds=new System.Collections.Generic.List<int>{1,2} },
            });

            _dm.SaveArtists(); _dm.SaveSongs(); _dm.SaveAlbums();
        }
    }
}
